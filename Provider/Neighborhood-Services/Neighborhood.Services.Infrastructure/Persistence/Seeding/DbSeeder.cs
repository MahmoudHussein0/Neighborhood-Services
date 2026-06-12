using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.RecurringBookings;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Domain.TechnicianCategories;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Domain.TechniciansPricing;
using Neighborhood.Services.Domain.TechniciansAvailability;
using Neighborhood.Services.Domain.Wallets;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using NetTopologySuite.Geometries;
using System.Text.Json;

namespace Neighborhood.Services.Infrastructure.Persistence.Seeding
{
    // Dev/test data seeder. Runs on startup; inserts a minimal coherent dataset
    // so the API can be exercised end-to-end. Seeds reference data + accounts +
    // the booking domain (Ahmed's). Other domains are each owner's to seed.
    public static class DbSeeder
    {
        private const string DefaultPassword = "Pass@123";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await context.Database.MigrateAsync();

            if (await context.Categories.AnyAsync())
                return;

            var problemTypes = await SeedReferenceDataAsync(context);

            var (customers, technicians) = await SeedAccountsAsync(context, userManager);
            await SeedTechnicianCategoriesAsync(context, technicians);
            await SeedTechnicianPricingAsync(context, technicians, problemTypes);
            await SeedBookingDomainAsync(context, customers, technicians, problemTypes);

            await context.SaveChangesAsync();
        }

        // ---------- Categories + Problem types ----------
        private static async Task<List<ProblemType>> SeedReferenceDataAsync(ApplicationDbContext context)
        {
            //var plumbing = new Category { Name = "Plumbing", Icon = "🔧", CreatedAt = DateTime.UtcNow };
            //var electrical = new Category { Name = "Electrical", Icon = "💡", CreatedAt = DateTime.UtcNow };
            //var cleaning = new Category { Name = "Cleaning", Icon = "🧹", CreatedAt = DateTime.UtcNow };
            //context.Categories.AddRange(plumbing, electrical, cleaning);
            //await context.SaveChangesAsync();

            //var problemTypes = new List<ProblemType>
            //{
            //    new() { Name = "Leak Repair", Description = "Fix a leaking pipe or faucet", MinPrice = 100, MaxPrice = 300, CategoryId = plumbing.Id, CreatedAt = DateTime.UtcNow },
            //    new() { Name = "Pipe Installation", Description = "Install new piping", MinPrice = 200, MaxPrice = 500, CategoryId = plumbing.Id, CreatedAt = DateTime.UtcNow },
            //    new() { Name = "Wiring", Description = "Electrical wiring work", MinPrice = 150, MaxPrice = 400, CategoryId = electrical.Id, CreatedAt = DateTime.UtcNow },
            //    new() { Name = "Deep Clean", Description = "Full apartment deep cleaning", MinPrice = 80, MaxPrice = 200, CategoryId = cleaning.Id, CreatedAt = DateTime.UtcNow }
            //};
            //context.ProblemTypes.AddRange(problemTypes);
            //await context.SaveChangesAsync();

            //return problemTypes;


            if (!context.Categories.Any())
            {
                var categoriesDate = await File.ReadAllTextAsync("../Neighborhood.Services.Infrastructure/Persistence/Seeding/Categories.json");
                var categories = JsonSerializer.Deserialize<List<Category>>(categoriesDate);

                if (categories.Count > 0)
                {
                    foreach (var category in categories)
                        await context.AddAsync(category);
                    await context.SaveChangesAsync();
                }
            }


            var ProblemTypeDate = await File.ReadAllTextAsync("../Neighborhood.Services.Infrastructure/Persistence/Seeding/ProblemTypes.json");
            var problemTypes = JsonSerializer.Deserialize<List<ProblemType>>(ProblemTypeDate);
            if (!context.ProblemTypes.Any())
            {
                if (problemTypes.Count > 0)
                {
                    foreach (var problemType in problemTypes)
                        await context.AddAsync(problemType);
                    await context.SaveChangesAsync();
                }
            }

            return problemTypes;
        }








        // ---------- Accounts ----------
        private static async Task<(List<Customer> customers, List<Technician> technicians)> SeedAccountsAsync(
            ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var customers = new List<Customer>
            {
                await CreateCustomerAsync(context, userManager, "customer1@test.com", "Sara Customer", 30, 31.2001, 29.9187),
                await CreateCustomerAsync(context, userManager, "customer2@test.com", "Omar Customer", 27, 31.2100, 29.9250),
                await CreateCustomerAsync(context, userManager, "customer3@test.com", "Heba Customer", 33, 31.2200, 29.9350),
                await CreateCustomerAsync(context, userManager, "customer4@test.com", "Youssef Customer", 25, 31.1950, 29.9100)
            };

            // Varied technicians (rating / availability / verification) so the Find Technician
            // badges, sorting and filters all have something to show.
            var technicians = new List<Technician>
            {
                await CreateTechnicianAsync(context, userManager, "tech1@test.com", "Ali Technician", 35, "29801011200123", 31.2050, 29.9200, 4.5m, true, TechnicianVerificationStatus.Approved),
                await CreateTechnicianAsync(context, userManager, "tech2@test.com", "Mona Technician", 40, "29505052500456", 31.2150, 29.9300, 4.8m, true, TechnicianVerificationStatus.Approved),
                await CreateTechnicianAsync(context, userManager, "tech3@test.com", "Khaled Technician", 38, "29002021200789", 31.2080, 29.9220, 4.2m, true, TechnicianVerificationStatus.Approved),
                await CreateTechnicianAsync(context, userManager, "tech4@test.com", "Nour Technician", 29, "29404041400321", 31.2250, 29.9400, 3.9m, false, TechnicianVerificationStatus.Approved),
                await CreateTechnicianAsync(context, userManager, "tech5@test.com", "Hassan Technician", 45, "27803031300654", 31.1900, 29.9050, 4.6m, true, TechnicianVerificationStatus.Pending),
                await CreateTechnicianAsync(context, userManager, "tech6@test.com", "Layla Technician", 31, "29606061600987", 31.2300, 29.9450, 5.0m, true, TechnicianVerificationStatus.Approved)
            };

            await context.SaveChangesAsync();
            return (customers, technicians);
        }

        // ---------- Technician ↔ Category assignments ----------
        private static async Task SeedTechnicianCategoriesAsync(ApplicationDbContext context, List<Technician> technicians)
        {
            var categories = await context.Categories.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
            if (categories.Count == 0)
                return;

            // Give each technician a rotating slice of categories so the data is varied (some
            // overlap, some differ) — enough to exercise the category filter + per-tech booking.
            var perTech = Math.Min(3, categories.Count);
            for (var i = 0; i < technicians.Count; i++)
            {
                for (var j = 0; j < perTech; j++)
                {
                    var category = categories[(i + j) % categories.Count];
                    context.TechnicianCategories.Add(new TechnicianCategory
                    {
                        TechnicianId = technicians[i].Id,
                        CategoryId = category.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        // ---------- Technician ↔ ProblemType pricing ----------
        // Every technician gets a price band for every problem type. The booking Quote
        // flow REQUIRES a pricing row (the tech can only quote within [MinPrice, MaxPrice],
        // and the customer's Book Now modal shows that range), so without this the Direct
        // flow can't be exercised. Each tech's band sits inside the problem type's own
        // market range, nudged a little per technician so the ranges vary (higher-index
        // technicians price slightly higher) — useful for the "this tech vs market" hint.
        private static async Task SeedTechnicianPricingAsync(
            ApplicationDbContext context, List<Technician> technicians, List<ProblemType> problemTypes)
        {
            if (await context.TechnicianPricings.AnyAsync())
                return;

            var now = DateTime.UtcNow;
            var techCount = technicians.Count;

            for (var t = 0; t < techCount; t++)
            {
                foreach (var pt in problemTypes)
                {
                    var range = pt.MaxPrice - pt.MinPrice;

                    // Raise the floor a bit for higher-index techs, and lower the ceiling a
                    // bit for lower-index techs — both rounded to the nearest 10 EGP.
                    var min = pt.MinPrice + RoundTo10(range * t / 25m);
                    var max = pt.MaxPrice - RoundTo10(range * (techCount - 1 - t) / 33m);
                    if (max <= min) max = min + 10m; // safety for very narrow ranges

                    context.TechnicianPricings.Add(new TechnicianPricing
                    {
                        TechnicianId = technicians[t].Id,
                        ProblemTypeId = pt.Id,
                        MinPrice = min,
                        MaxPrice = max,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static decimal RoundTo10(decimal value) => Math.Round(value / 10m) * 10m;

        // ---------- Booking domain (service requests, offers, bookings, recurring) ----------
        private static async Task SeedBookingDomainAsync(
            ApplicationDbContext context, List<Customer> customers, List<Technician> technicians, List<ProblemType> problemTypes)
        {
            var c1 = customers[0];
            var c2 = customers[1];
            var t1 = technicians[0];
            var t2 = technicians[1];
            var leak = problemTypes[0];
            var wiring = problemTypes[2];

            // Open service requests (near seeded coords so the geo query returns them)
            var sr1 = new ServiceRequest
            {
                Description = "Kitchen sink leaking under the cabinet",
                Address = "12 Nile St, Alexandria",
                Budget = 250,
                Status = ServiceRequestStatus.Open,
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Location = new Point(29.9187, 31.2001) { SRID = 4326 },
                CustomerId = c1.Id,
                CategoryId = leak.CategoryId,
                ProblemTypeId = leak.Id
            };
            var sr2 = new ServiceRequest
            {
                Description = "Living room outlet sparks when used",
                Address = "5 Corniche Rd, Alexandria",
                Budget = 300,
                Status = ServiceRequestStatus.Open,
                ScheduledAt = DateTime.UtcNow.AddDays(3),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Location = new Point(29.9250, 31.2100) { SRID = 4326 },
                CustomerId = c2.Id,
                CategoryId = wiring.CategoryId,
                ProblemTypeId = wiring.Id
            };
            context.ServiceRequests.AddRange(sr1, sr2);
            await context.SaveChangesAsync(); // need sr Ids

            // Two pending offers on sr1 (one per technician)
            context.Offers.AddRange(
                new Offer { ServiceRequestId = sr1.Id, TechnicianId = t1.Id, Price = 240, EstimatedDuration = 120, Message = "I can handle this today.", ScheduledAt = DateTime.UtcNow.AddDays(2), Status = OfferStatus.Pending, CreatedAt = DateTime.UtcNow },
                new Offer { ServiceRequestId = sr1.Id, TechnicianId = t2.Id, Price = 280, EstimatedDuration = 90, Message = "Available tomorrow morning.", ScheduledAt = DateTime.UtcNow.AddDays(2).AddHours(2), Status = OfferStatus.Pending, CreatedAt = DateTime.UtcNow }
            );

            // Direct bookings (read fixtures — these did NOT go through the escrow/payment flow)
            context.Bookings.AddRange(
                new Booking
                {
                    BookingType = BookingType.Direct,
                    Description = "Bathroom faucet drip",
                    Address = "12 Nile St, Alexandria",
                    ScheduledAt = DateTime.UtcNow.AddDays(1),
                    EstimatedPrice = 200,
                    FinalPrice = 0,
                    Status = BookingStatus.Pending,
                    Location = new Point(29.9187, 31.2001) { SRID = 4326 },
                    CustomerId = c1.Id,
                    TechnicianId = t1.Id,
                    ProblemTypeId = leak.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Booking
                {
                    BookingType = BookingType.Direct,
                    Description = "Replaced kitchen pipe (past job)",
                    Address = "5 Corniche Rd, Alexandria",
                    ScheduledAt = DateTime.UtcNow.AddDays(-3),
                    DurationMinutes = 90,
                    EstimatedPrice = 350,
                    FinalPrice = 350,
                    Status = BookingStatus.Completed,
                    ClientConfirmed = true,
                    ConfirmedAt = DateTime.UtcNow.AddDays(-2),
                    Location = new Point(29.9250, 31.2100) { SRID = 4326 },
                    CustomerId = c2.Id,
                    TechnicianId = t2.Id,
                    ProblemTypeId = wiring.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                }
            );

            // One active recurring booking
            context.RecurringBookings.Add(new RecurringBooking
            {
                Address = "12 Nile St, Alexandria",
                Location = new Point(29.9187, 31.2001) { SRID = 4326 },
                Pattern = RecurringPattern.Weekly,
                DayOfWeek = DayOfWeek.Monday,
                TimeOfDay = new TimeOnly(10, 0),
                DurationMinutes = 60,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Status = RecurringBookingStatus.Active,
                AgreedPrice = 150,
                CreatedAt = DateTime.UtcNow,
                CustomerId = c1.Id,
                TechnicianId = t1.Id,
                ProblemTypeId = leak.Id
            });
        }

        private static async Task<Customer> CreateCustomerAsync(
            ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            string email, string fullName, int age, double lat, double lng)
        {
            var user = await CreateUserAsync(userManager, email, fullName, age, ApplicationUserRole.Customer, lat, lng);

            var customer = new Customer
            {
                ApplicationUserId = user.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Customers.Add(customer);

            context.Wallets.Add(new Wallet
            {
                UserId = user.Id,
                Balance = 5000m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync(); // need customer.Id for booking-domain seeding
            return customer;
        }

        private static async Task<Technician> CreateTechnicianAsync(
            ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            string email, string fullName, int age, string nationalId, double lat, double lng,
            decimal rating = 4.5m, bool isAvailable = true,
            TechnicianVerificationStatus verificationStatus = TechnicianVerificationStatus.Approved)
        {
            var user = await CreateUserAsync(userManager, email, fullName, age, ApplicationUserRole.Technician, lat, lng);

            var technician = new Technician
            {
                ApplicationUserId = user.Id,
                NationalId = nationalId,
                Experience = "5 years of professional experience",
                Rating = rating,
                MaxTravelDistance = 20000,
                VerificationStatus = verificationStatus,
                IsAvailable = isAvailable,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Technicians.Add(technician);
            await context.SaveChangesAsync(); // need technician.Id

            for (var day = DayOfWeek.Sunday; day <= DayOfWeek.Thursday; day++)
            {
                context.TechnicianAvailabilities.Add(new TechnicianAvailability
                {
                    TechnicianId = technician.Id,
                    DayOfWeek = day,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0)
                });
            }

            context.Wallets.Add(new Wallet
            {
                UserId = user.Id,
                Balance = 1000m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
            return technician;
        }

        private static async Task<ApplicationUser> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email, string fullName, int age, ApplicationUserRole role, double lat, double lng)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                Age = age,
                ApplicationUserRole = role,
                Location = new Point(lng, lat) { SRID = 4326 },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, DefaultPassword);
            if (!result.Succeeded)
                throw new Exception($"Failed to seed user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return user;
        }
    }
}
