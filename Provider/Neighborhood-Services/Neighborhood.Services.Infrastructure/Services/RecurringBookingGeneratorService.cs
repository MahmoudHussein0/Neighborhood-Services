using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Escrows.Commands.CreateEscrow;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.RecurringBookings;

namespace Neighborhood.Services.Infrastructure.Services
{
    public class RecurringBookingGeneratorService
    {
        IRecurringBookingRepository _recurringBookingRepository;
        IBookingRepository _bookingRepository;
        IWalletRepository _walletRepository;
        IMediator _mediator;
        INotificationService _notificationService;
        IUnitOfWork _unitOfWork;
        ILogger<RecurringBookingGeneratorService> _logger;
        public RecurringBookingGeneratorService(
            IRecurringBookingRepository recurringBookingRepository,
            IBookingRepository bookingRepository,
            IWalletRepository walletRepository,
            IMediator mediator,
            INotificationService notificationService,
            IUnitOfWork unitOfWork,
            ILogger<RecurringBookingGeneratorService> logger)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _bookingRepository = bookingRepository;
            _walletRepository = walletRepository;
            _mediator = mediator;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task GenerateBookings()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var dueRecurringBookings = await _recurringBookingRepository.GetDueRecurringBookingsAsync(today);
            foreach (var recurring in dueRecurringBookings)
            {
                var occurrenceDates = new List<DateOnly>();
                for(int i = 0; i <7; i++)
                {
                    var candidate = today.AddDays(i);
                    var isOccurence = recurring.Pattern switch
                    {
                        RecurringPattern.Daily=>true,
                        RecurringPattern.Weekly=>candidate.DayOfWeek==recurring.DayOfWeek,
                        RecurringPattern.Monthly=>candidate.Day==recurring.DayOfMonth,
                       _ =>false
                    };
                    if (isOccurence)
                    {
                        occurrenceDates.Add(candidate);
                    }
                }
                foreach (var date in occurrenceDates)
                {
                    var start = date.ToDateTime(recurring.TimeOfDay);
                    var end = start.AddMinutes(recurring.DurationMinutes);

                    // No approved price yet (technician hasn't set it / customer hasn't approved)
                    // → nothing to charge, so don't generate an occurrence.
                    if (recurring.AgreedPrice is null or <= 0)
                        continue;

                    var price = recurring.AgreedPrice.Value;

                    // Skip if already generated for this occurrence
                    var alreadyExists = await _bookingRepository
                        .GetByConditionAsync(b =>
                            b.RecurringBookingId == recurring.Id &&
                            b.ScheduledAt == start &&
                            !b.IsDeleted);

                    if (alreadyExists.Any())
                        continue;

                    // Skip if slot is taken by another booking
                    var hasOverlap = await _bookingRepository
                        .HasOverlappingConfirmedBookingAsync(recurring.TechnicianId, start, end);

                    if (hasOverlap)
                        continue;

                    // The customer must be able to fund this occurrence up front, exactly like a
                    // normal booking holds escrow at accept-time. If the wallet can't cover it,
                    // skip the occurrence and nudge the customer to top up — never create an
                    // unpaid Confirmed job.
                    var customerUserId = recurring.Customer.ApplicationUserId;
                    var wallet = await _walletRepository.GetByUserIdAsync(customerUserId);
                    if (wallet is null || wallet.Balance < price)
                    {
                        await NotifyTopUp(customerUserId, date, price);
                        continue;
                    }

                    var booking = new Booking
                    {
                        CustomerId = recurring.CustomerId,
                        TechnicianId = recurring.TechnicianId,
                        ProblemTypeId = recurring.ProblemTypeId,
                        RecurringBookingId = recurring.Id,
                        BookingType = BookingType.Recurring,
                        Description = "Auto-generated recurring booking",
                        Address = recurring.Address,
                        ScheduledAt = start,
                        DurationMinutes = recurring.DurationMinutes,
                        EstimatedPrice = price,
                        FinalPrice = price,
                        Status = BookingStatus.Confirmed,
                        Location = recurring.Location,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Persist first so the escrow can reference the new booking's Id.
                    await _bookingRepository.AddAsync(booking);
                    await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                    // Hold the customer's payment in escrow at the agreed price. The pre-check above
                    // covers the common no-funds case; this try/catch guards the rare race where the
                    // balance changed in between, so we never leave a Confirmed booking without a hold.
                    try
                    {
                        await _mediator.Send(new CreateEscrowCommand
                        {
                            BookingId = booking.Id,
                            WalletId = wallet.Id,
                            Amount = price
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Escrow hold failed for recurring booking {RecurringId} occurrence {Date}; rolling back generated booking {BookingId}.",
                            recurring.Id, date, booking.Id);

                        booking.IsDeleted = true;
                        await _bookingRepository.UpdateAsync(booking);
                        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                        await NotifyTopUp(customerUserId, date, price);
                    }
                }
            }

        }

        private async Task NotifyTopUp(string customerUserId, DateOnly date, decimal price)
        {
            if (string.IsNullOrEmpty(customerUserId))
                return;

            try
            {
                await _notificationService.SendNotificationToUser(
                    customerUserId,
                    $"Your recurring booking on {date:yyyy-MM-dd} couldn't be scheduled. Please top up your wallet to cover EGP {price:0.##}.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Top-up notification failed for user {UserId}.", customerUserId);
            }
        }
    }
}
