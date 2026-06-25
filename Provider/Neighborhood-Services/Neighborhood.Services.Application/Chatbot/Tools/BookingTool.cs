using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Bookings.Commands.CreateBookingCommands;
using Neighborhood.Services.Application.Bookings.Queries.GetTechnicianAvailableSlots;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Application.Shared;
using System.ComponentModel;
using System.Globalization;

namespace Neighborhood.Services.Application.Chatbot.Tools
{
    // The chatbot's only WRITE tool: it actually places a Direct booking, closing the loop the
    // agent already walks the user through (recommend/find technician -> check availability -> book).
    // It wraps the EXISTING CreateBookingCommand unchanged — region resolution, the per-slot lock,
    // working-hours/overlap validation and escrow timing all stay in that handler. We only add two
    // chatbot-specific guards on top:
    //
    //   1. The booking is created as PENDING with FinalPrice = 0 (the command's default). The
    //      `confirmed` parameter here is ONLY the chatbot's user-confirmation gate — it does NOT
    //      mean the booking is confirmed/priced. The technician still reviews and sends a quote.
    //   2. Before booking we RE-CHECK the technician's live free slots and require the chosen time
    //      to be one of them, so we never book a time the model carried over from an earlier turn
    //      that has since been taken (or was never actually free).
    //
    // Built per-request because it carries this request's coords + whether the caller is logged in.
    public class BookingTool
    {
        private readonly IMediator _mediator;
        private readonly IVectorMemory _memory;
        private readonly IGeocodingService _geocodingService;
        private readonly IProblemTypeRepository _problemTypeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger _logger;
        private readonly double? _latitude;
        private readonly double? _longitude;
        // The authenticated user's id (null = guest). A customer record for it is what makes the
        // user eligible to book — checked lazily only when create_booking is actually called.
        private readonly string? _currentUserId;

        // A booking pins one EXACT problem type (drives category + price range), so a wrong match is
        // worse than asking again — keep this STRICT (same threshold the pricing tool uses). This
        // only applies to the FALLBACK classification; a problemTypeId the model carries from an
        // earlier recommend_technician/estimate_price result is reused directly (no re-classify).
        private const float ClassifierConfidenceThreshold = 0.5f;

        public BookingTool(
            IMediator mediator,
            IVectorMemory memory,
            IGeocodingService geocodingService,
            IProblemTypeRepository problemTypeRepository,
            ICustomerRepository customerRepository,
            ILogger logger,
            double? latitude,
            double? longitude,
            string? currentUserId)
        {
            _mediator = mediator;
            _memory = memory;
            _geocodingService = geocodingService;
            _problemTypeRepository = problemTypeRepository;
            _customerRepository = customerRepository;
            _logger = logger;
            _latitude = latitude;
            _longitude = longitude;
            _currentUserId = currentUserId;
        }

        [KernelFunction("create_booking")]
        [Description("Place a Direct booking request for the logged-in customer with a chosen " +
            "technician. The booking is created as PENDING — the technician then reviews it and " +
            "sends a price quote; this does NOT confirm a price or charge anything yet. " +
            "ALWAYS call this FIRST with confirmed=false to get a summary, show it to the user, and " +
            "only after the user explicitly agrees call it again with confirmed=true. Requires the " +
            "technician's id (from find_technicians/recommend_technician) and a time the user picked " +
            "from check_availability.")]
        public async Task<string> CreateBooking(
            [Description("The technician's numeric id, as returned by find_technicians or recommend_technician.")]
            int technicianId,
            [Description("Short description of the problem/service, e.g. 'leaking tap', 'AC not cooling'.")]
            string serviceDescription,
            [Description("The chosen start time in 'YYYY-MM-DD HH:mm' (24h) format, e.g. '2026-07-02 14:00'. " +
                "Must be one of the free start-times returned by check_availability.")]
            string scheduledAt,
            [Description("The customer's street address for the visit. Leave empty on the first " +
                "(confirmed=false) call — a suggested address is returned for the user to confirm or correct. " +
                "On the confirmed=true call, pass the address the user accepted or corrected.")]
            string? address = null,
            [Description("Set to true ONLY after the user has seen the summary and explicitly agreed " +
                "to book. Defaults to false, which returns a summary WITHOUT booking.")]
            bool confirmed = false,
            [Description("The problem type id from an earlier recommend_technician or estimate_price " +
                "result (their output includes 'matched service #<id>'). Pass it to reuse the service " +
                "already identified — this skips re-classifying the description. Leave 0 if unknown.")]
            int problemTypeId = 0)
        {
            _logger.LogInformation(
                "BookingTool: create_booking CALLED — tech={Tech} desc='{Desc}' problemTypeId={Ptid} at='{At}' confirmed={Confirmed} userId={UserId}",
                technicianId, serviceDescription, problemTypeId, scheduledAt, confirmed, _currentUserId ?? "(guest)");

            // 1. WRITE actions are for logged-in CUSTOMERS only.
            if (string.IsNullOrWhiteSpace(_currentUserId))
                return "NOT_LOGGED_IN: The user must be logged in to book. Ask them to log in first, " +
                       "then try again.";

            // Only a CUSTOMER account can place a booking. We detect this by whether the signed-in
            // user has a customer record — technicians/staff don't, so block them cleanly here
            // (otherwise it fails deeper with a confusing "customer not found" error).
            var customer = await _customerRepository.GetByUserIdAsync(_currentUserId);
            if (customer is null)
                return "ONLY_CUSTOMERS: Bookings can only be placed from a CUSTOMER account. If the " +
                       "user is signed in as a technician or staff member, tell them they need a " +
                       "customer account to book a service.";

            // 2. A booking needs a real location point. We require shared GPS coords so the booking
            //    has accurate Location — and we can reverse-geocode them into a suggested address.
            if (!_latitude.HasValue || !_longitude.HasValue)
                return "NO_LOCATION: We don't have the user's location. Ask them to tap 'share location' " +
                       "first so we can book with an accurate address, then try again.";

            // 3. Determine the problem type. PREFER one the model carried from an earlier
            //    recommend_technician/estimate_price result — the service was already established
            //    when we picked the technician, so don't make the user describe it again. Only if no
            //    (valid) id was passed do we fall back to classifying the free text.
            int resolvedProblemTypeId;
            if (problemTypeId > 0 && await _problemTypeRepository.GetByIdAsync(problemTypeId) is not null)
            {
                resolvedProblemTypeId = problemTypeId;
            }
            else
            {
                var hits = await _memory.SearchDetailedAsync("problem-types", serviceDescription, topK: 1);
                var top = hits.FirstOrDefault();
                if (top is null
                    || top.Score < ClassifierConfidenceThreshold
                    || !top.Fields.TryGetValue("problemTypeId", out var idStr)
                    || !int.TryParse(idStr, out resolvedProblemTypeId))
                {
                    _logger.LogInformation("BookingTool: NO_MATCH — topScore={Score}", top?.Score);
                    return "NO_MATCH: Could not confidently identify the service from that description. " +
                           "Ask the user to describe the problem more specifically before booking. " +
                           "(If you already recommended a technician for this problem, pass the " +
                           "matched problemTypeId so re-classification is skipped.)";
                }
            }

            // 4. Parse the chosen time.
            if (!DateTime.TryParseExact(scheduledAt, new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-ddTHH:mm" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var when))
                return "INVALID_TIME: Could not read the time. Provide it as 'YYYY-MM-DD HH:mm', " +
                       "e.g. '2026-07-02 14:00'.";

            // 5. RE-CHECK live availability — never trust a time the model carried from an earlier
            //    turn. The chosen start must be one of the technician's currently-free slots.
            var freeSlots = (await _mediator.Send(new GetTechnicianAvailableSlotsQuery
            {
                TechnicianId = technicianId,
                Date = when.Date
            })).ToList();

            if (!freeSlots.Any(s => s == when))
            {
                _logger.LogInformation(
                    "BookingTool: chosen time {When} not in {Count} live free slot(s)", when, freeSlots.Count);
                if (freeSlots.Count == 0)
                    return $"SLOT_TAKEN: {when:yyyy-MM-dd HH:mm} is not available and the technician has " +
                           "no free start-times that day. Ask the user to pick another day.";
                var times = string.Join(", ", freeSlots.Select(s => s.ToString("HH:mm")));
                return $"SLOT_TAKEN: {when:yyyy-MM-dd HH:mm} is no longer free. The technician's free " +
                       $"start-times on {when:yyyy-MM-dd} are: {times}. Ask the user to choose one of these.";
            }

            // 6. Suggest an address from the user's coords (reverse-geocode) for them to confirm or
            //    correct. Fail-open: if geocoding is down we just leave the suggestion blank.
            string suggestedAddress = address ?? string.Empty;
            if (string.IsNullOrWhiteSpace(suggestedAddress))
            {
                try
                {
                    var geo = await _geocodingService.GetAddressAsync(_latitude.Value, _longitude.Value);
                    suggestedAddress = geo?.FormattedAddress ?? string.Empty;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "BookingTool: reverse-geocode failed for ({Lat},{Lng})",
                        _latitude, _longitude);
                }
            }

            // 7. Confirmation gate. Until the user explicitly agrees, return a summary and DO NOT book.
            if (!confirmed)
            {
                var addressLine = string.IsNullOrWhiteSpace(suggestedAddress)
                    ? "address: (ask the user for their street address)"
                    : $"suggested address: \"{suggestedAddress}\" (ask the user to confirm or correct it)";
                return "NEEDS_CONFIRMATION: Do NOT claim anything is booked yet. Show the user this " +
                       "summary and ask them to confirm:\n" +
                       $"- technician id: {technicianId}\n" +
                       $"- service: {serviceDescription}\n" +
                       $"- when: {when:yyyy-MM-dd HH:mm}\n" +
                       $"- {addressLine}\n" +
                       "Tell the user the technician will review the request and send a price quote " +
                       "(nothing is charged now). When the user agrees, call create_booking again with " +
                       "confirmed=true and the final address.";
            }

            // Confirmed — but still need an address to attach to the booking.
            if (string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(suggestedAddress))
                return "NEED_ADDRESS: Ask the user for their street address, then call create_booking " +
                       "again with confirmed=true and that address.";
            var finalAddress = string.IsNullOrWhiteSpace(address) ? suggestedAddress : address;

            // 8. Place the booking via the EXISTING command (resolves the customer from the authed
            //    user itself, creates it PENDING with FinalPrice = 0, holds no escrow — the tech
            //    quotes later). Surface domain validation as readable lines for the model to relay.
            try
            {
                var bookingId = await _mediator.Send(new CreateBookingCommand
                {
                    TechnicianId = technicianId,
                    ProblemTypeId = resolvedProblemTypeId,
                    Description = serviceDescription,
                    Address = finalAddress,
                    Latitude = _latitude.Value,
                    Longitude = _longitude.Value,
                    ScheduledAt = when
                });

                _logger.LogInformation("BookingTool: created booking #{Id}", bookingId);
                return $"BOOKED: request #{bookingId} created for {when:yyyy-MM-dd HH:mm}. It is PENDING — " +
                       "tell the user the technician will review it and send a price quote; nothing is " +
                       "charged yet.";
            }
            catch (UnauthorizedException)
            {
                return "NOT_LOGGED_IN: The user must be logged in to book. Ask them to log in first.";
            }
            catch (ConflictException ex)
            {
                return $"SLOT_TAKEN: {ex.Message} Ask the user to choose another time.";
            }
            catch (ValidationException ex)
            {
                return $"CANNOT_BOOK: {ex.Message} Relay this to the user and suggest a fix.";
            }
            catch (NotFoundException ex)
            {
                return $"NOT_FOUND: {ex.Message} Ask the user to re-check the technician or service.";
            }
            catch (Exception ex)
            {
                // Safety net — never surface a raw 500 to the chat. Relay a generic failure.
                _logger.LogError(ex, "BookingTool: create_booking unexpected failure");
                return "CANNOT_BOOK: Something went wrong placing the booking. Ask the user to try " +
                       "again shortly or use the booking page.";
            }
        }
    }
}
