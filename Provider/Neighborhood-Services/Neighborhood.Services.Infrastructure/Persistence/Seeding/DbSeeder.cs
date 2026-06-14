using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.RecurringBookings;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Domain.TechnicianCategories;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Domain.SupportTickets;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Domain.TechniciansPricing;
using Neighborhood.Services.Domain.TechniciansAvailability;
using Neighborhood.Services.Domain.Wallets;
using Neighborhood.Services.Domain.PromoCodes;
using Neighborhood.Services.Domain.Transactions;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Domain.Invoices;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neighborhood.Services.Infrastructure.Persistence.Seeding
{
    // Dev/test data seeder. Runs on startup; inserts a minimal coherent dataset
    // so the API can be exercised end-to-end. Seeds reference data + accounts +
    // the booking domain (Ahmed's). Other domains are each owner's to seed.
    public static class DbSeeder
    {
        private const string DefaultPassword = "Pass@123";

        public static async Task SeedAsync(IServiceProvider services, IWebHostEnvironment environment, ILogger logger = null)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await context.Database.MigrateAsync();

            if (await context.Categories.AnyAsync())
                return;

            var problemTypes = await SeedReferenceDataAsync(context, environment, logger);
            var (customers, technicians) = await SeedAccountsAsync(context, userManager);
            await SeedTechnicianCategoriesAsync(context, technicians);
            await SeedTechnicianPricingAsync(context, technicians, problemTypes);

            // 🎯 إضافة الـ Staff والـ Admin الافتراضي في الترتيب الصحيح
            await SeedStaffAccountsAsync(context, userManager);

            await SeedPromoCodesAsync(context);
            await SeedBookingDomainAsync(context, customers, technicians, problemTypes);

            await SeedTransactionsAndEscrowsAsync(context);

            await SeedSupportTicketsAsync(context, customers);

            await context.SaveChangesAsync();
        }

        // ---------- 1. Categories + Problem types ----------
        private static async Task<List<ProblemType>> SeedReferenceDataAsync(ApplicationDbContext context, IWebHostEnvironment environment, ILogger logger = null)
        {
            try
            {
                if (!context.Categories.Any())
                {
                    var categoriesPath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seeding", "Categories.json");
                    if (File.Exists(categoriesPath))
                    {
                        var categoriesDate = await File.ReadAllTextAsync(categoriesPath);
                        var categories = JsonSerializer.Deserialize<List<Category>>(categoriesDate);

                        if (categories != null && categories.Count > 0)
                        {
                            foreach (var category in categories)
                                await context.AddAsync(category);
                            await context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        logger?.LogWarning($"Categories.json file not found at {categoriesPath}. Skipping categories seed.");
                    }
                }

                var problemTypesPath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seeding", "ProblemTypes.json");
                var problemTypes = new List<ProblemType>();

                if (File.Exists(problemTypesPath))
                {
                    var ProblemTypeDate = await File.ReadAllTextAsync(problemTypesPath);
                    problemTypes = JsonSerializer.Deserialize<List<ProblemType>>(ProblemTypeDate);

                    if (!context.ProblemTypes.Any())
                    {
                        if (problemTypes != null && problemTypes.Count > 0)
                        {
                            foreach (var problemType in problemTypes)
                                await context.AddAsync(problemType);
                            await context.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    logger?.LogWarning($"ProblemTypes.json file not found at {problemTypesPath}. Skipping problem types seed.");
                }

                return problemTypes ?? new List<ProblemType>();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error seeding reference data. Continuing with empty reference data.");
                return new List<ProblemType>();
            }
        }

        // ---------- 2. Core Accounts Seeding (Customers & Technicians) ----------
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

        // ---------- 3. Staff & Admin Seeding ----------
        private static async Task SeedStaffAccountsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // الأدمن الأساسي معاه صلاحية واحدة وهي FullAccess
            await CreateStaffUserAsync(context, userManager, "admin@test.com", "System Administrator", StaffRole.Admin, PermissionType.FullAccess);

            // الـ Moderator هنا أخد صلاحيتين مع بعض (ManageUsers و ManagePromos) وتقدري تزودي تالتة ورابعة كمان 🎯
            await CreateStaffUserAsync(context, userManager, "moderator@test.com", "Content Moderator", StaffRole.Admin,
                PermissionType.ManageUsers,
                PermissionType.ManagePromos);

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
            if (customers == null || customers.Count < 2 || technicians == null || technicians.Count < 2)
            {
                customers = await context.Customers.Take(2).ToListAsync();
                technicians = await context.Technicians.Take(2).ToListAsync();

                if (customers.Count < 2 || technicians.Count < 2) return;
            }

            if (problemTypes == null || problemTypes.Count == 0)
            {
                problemTypes = await context.ProblemTypes.ToListAsync();
                if (problemTypes.Count == 0) return;
            }

            var c1 = customers[0];
            var c2 = customers[1];
            var t1 = technicians[0];
            var t2 = technicians[1];

            var leak = problemTypes.FirstOrDefault() ?? new ProblemType();
            var wiring = problemTypes.Count > 2 ? problemTypes[2] : (problemTypes.Skip(1).FirstOrDefault() ?? leak);

            var hasExistingRequests = await context.ServiceRequests.AnyAsync();
            if (hasExistingRequests) return;

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
            await context.SaveChangesAsync();

            context.Offers.AddRange(
                new Offer { ServiceRequestId = sr1.Id, TechnicianId = t1.Id, Price = 240, EstimatedDuration = 120, Message = "I can handle this today.", ScheduledAt = DateTime.UtcNow.AddDays(2), Status = OfferStatus.Pending, CreatedAt = DateTime.UtcNow },
                new Offer { ServiceRequestId = sr1.Id, TechnicianId = t2.Id, Price = 280, EstimatedDuration = 90, Message = "Available tomorrow morning.", ScheduledAt = DateTime.UtcNow.AddDays(2).AddHours(2), Status = OfferStatus.Pending, CreatedAt = DateTime.UtcNow }
            );

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

            await context.SaveChangesAsync();
        }

        // ---------- 5. Helper Methods For Creating Entities ----------

        private static async Task<Customer> CreateCustomerAsync(
            ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            string email, string fullName, int age, double lat, double lng)
        {
            var user = await CreateUserAsync(userManager, email, fullName, age, ApplicationUserRole.Customer, lat, lng);

            var existingCustomer = await context.Customers
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (existingCustomer != null)
                return existingCustomer;

            var customer = new Customer
            {
                ApplicationUserId = user.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Customers.Add(customer);

            var walletExists = await context.Wallets.AnyAsync(w => w.UserId == user.Id);
            if (!walletExists)
            {
                context.Wallets.Add(new Wallet
                {
                    UserId = user.Id,
                    Balance = 5000m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
            return customer;
        }

        private static async Task CreateStaffUserAsync(
       ApplicationDbContext context, UserManager<ApplicationUser> userManager,
       string email, string fullName, StaffRole domainRole, params PermissionType[] permissions)
        {
            // 1. إنشاء أو جلب الـ Identity User
            var user = await CreateUserAsync(userManager, email, fullName, 25, ApplicationUserRole.Staff, 31.2001, 29.9187);

            // 2. فحص هل الموظف موجود في جدول الـ Staff قبل كدا
            var existingStaff = await context.Staffs.Where(s => s.UserId == user.Id).FirstOrDefaultAsync();
            // الأفضل نستخدم AnyAsync عشان الأداء، أو نعمل تشيك بالإيميل
            if (await context.Staffs.AnyAsync(s => s.UserId == user.Id)) return;

            // 3. إنشاء كائن الـ Staff (من غير ما نعمل SaveChanges)
            var staffMember = new Staff
            {
                UserId = user.Id,
                Role = domainRole,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Staffs.AddAsync(staffMember);

            // 4. إضافة الصلاحيات بربطها بكائن الـ Staff نفسه (Navigation Property) 🎯
            if (permissions != null && permissions.Length > 0)
            {
                foreach (var permission in permissions)
                {
                    var staffPermission = new StaffPermission
                    {
                        // StaffId = staffMember.Id, 👈 شيلنا السطر ده عشان ميعملش إيرور (بيكون بـ 0 لسه)
                        Staff = staffMember, // 🎯 ربطنا الكائن بالكامل هنا، الـ EF هيهندل الـ ID تلقائياً أثناء الحفظ
                        Permission = permission
                    };
                    await context.StaffPermissions.AddAsync(staffPermission);
                }
            }
            // ❌ شيلنا الـ SaveChangesAsync اللي كانت هنا تماماً
        }
        
        private static async Task<Technician> CreateTechnicianAsync(
            ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            string email, string fullName, int age, string nationalId, double lat, double lng,
            decimal rating = 4.5m, bool isAvailable = true,
            TechnicianVerificationStatus verificationStatus = TechnicianVerificationStatus.Approved)
        {
            var user = await CreateUserAsync(userManager, email, fullName, age, ApplicationUserRole.Technician, lat, lng);

            var existingTechnician = await context.Technicians
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id || t.NationalId == nationalId);

            if (existingTechnician != null)
            {
                var hasWallet = await context.Wallets.AnyAsync(w => w.UserId == user.Id);
                if (!hasWallet)
                {
                    context.Wallets.Add(new Wallet
                    {
                        UserId = user.Id,
                        Balance = 1000m,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    await context.SaveChangesAsync();
                }
                return existingTechnician;
            }

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
            await context.SaveChangesAsync();

            var hasAvailability = await context.TechnicianAvailabilities.AnyAsync(a => a.TechnicianId == technician.Id);
            if (!hasAvailability)
            {
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
            }

            var walletExists = await context.Wallets.AnyAsync(w => w.UserId == user.Id);
            if (!walletExists)
            {
                context.Wallets.Add(new Wallet
                {
                    UserId = user.Id,
                    Balance = 1000m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
            return technician;
        }

        private static async Task<ApplicationUser> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email, string fullName, int age, ApplicationUserRole role, double lat, double lng)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return existingUser;

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
    
        // ---------- 6. PromoCode Seeding ----------
        private static async Task SeedPromoCodesAsync(ApplicationDbContext context)
        {
            if (await context.PromoCodes.AnyAsync())
                return;

            var now = DateTime.UtcNow;
            var promoCodes = new List<PromoCode>
            {
                new PromoCode
                {
                    Code = "WELCOME10",
                    DiscountPercentage = 10m,
                    MaxUses = 100,
                    UsedCount = 0,
                    ExpiresAt = now.AddMonths(6),
                    IsActive = true,
                    CreatedAt = now
                },
                new PromoCode
                {
                    Code = "SUMMER25",
                    DiscountPercentage = 25m,
                    MaxUses = 50,
                    UsedCount = 0,
                    ExpiresAt = now.AddMonths(3),
                    IsActive = true,
                    CreatedAt = now
                },
                new PromoCode
                {
                    Code = "WINTER50",
                    DiscountPercentage = 50m,
                    MaxUses = 20,
                    UsedCount = 0,
                    ExpiresAt = now.AddMonths(1),
                    IsActive = true,
                    CreatedAt = now
                }
            };

            await context.PromoCodes.AddRangeAsync(promoCodes);
            await context.SaveChangesAsync();
        }

    // ---------- 7. Support Tickets Seeding ----------
private static async Task SeedSupportTicketsAsync(
    ApplicationDbContext context, List<Customer> customers)
        {
            if (await context.SupportTickets.AnyAsync())
                return;

            if (customers == null || customers.Count == 0)
                customers = await context.Customers.Take(2).ToListAsync();

            if (customers.Count == 0) return;

            var c1 = customers[0];
            var c2 = customers.Count > 1 ? customers[1] : customers[0];

            // جيب booking موجود عشان نربطه بالتيكت
            var booking = await context.Bookings.FirstOrDefaultAsync();

            var tickets = new List<SupportTicket>
    {
        new SupportTicket
        {
            UserId = c1.ApplicationUserId,
            BookingId = booking?.Id,
            SenderName= "Sara Customer",
            SenderEmail= "sara@gmail.com",
            Subject = "الفني لم يصلح المشكلة",
            Description = "الراجل خد مني الفلوس حق تصليح الغسالة وبعدين لقيت الغسالة عطلانة تاني",
            Status = SupportTicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        },
        new SupportTicket
        {
            UserId = c2.ApplicationUserId,
            SenderName= "Omar guste",
            SenderEmail= "omar@gmail.com",
            Subject = "مشكلة في السحب من المحفظة",
            Description = "حاولت مرارًا وتكرارًا السحب من المحفظة ولم أستطع، برجاء حل مشكلتي",
            Status = SupportTicketStatus.Open,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = false
        }
    };

            await context.SupportTickets.AddRangeAsync(tickets);
            await context.SaveChangesAsync();
        }

        // ---------- 8. Transactions, Escrows & Invoices Seeding ----------
        private static async Task SeedTransactionsAndEscrowsAsync(ApplicationDbContext context)
        {
            if (await context.Transactions.AnyAsync())
                return;

            var now = DateTime.UtcNow;

            // Grab the wallets we need
            var wallets = await context.Wallets.ToListAsync();
            var customers = await context.Customers.ToListAsync();
            var technicians = await context.Technicians.ToListAsync();
            var bookings = await context.Bookings.OrderBy(b => b.Id).ToListAsync();

            if (wallets.Count == 0 || customers.Count == 0 || technicians.Count == 0)
                return;

            var c1 = customers[0];
            var c2 = customers.Count > 1 ? customers[1] : customers[0];
            var t1 = technicians[0];
            var t2 = technicians.Count > 1 ? technicians[1] : technicians[0];

            var c1Wallet = wallets.FirstOrDefault(w => w.UserId == c1.ApplicationUserId);
            var c2Wallet = wallets.FirstOrDefault(w => w.UserId == c2.ApplicationUserId);
            var t1Wallet = wallets.FirstOrDefault(w => w.UserId == t1.ApplicationUserId);
            var t2Wallet = wallets.FirstOrDefault(w => w.UserId == t2.ApplicationUserId);

            if (c1Wallet == null || t1Wallet == null) return;

            // ── Customer 1: Top-Up history (so transaction list has rows) ──────────
            var tx1 = new Transaction { ToWalletId = c1Wallet.Id, Amount = 1000m, Fee = 0, Currency = "EGP", Type = TransactionType.TopUp, Status = TransactionStatus.Completed, CreatedAt = now.AddDays(-10) };
            var tx2 = new Transaction { ToWalletId = c1Wallet.Id, Amount = 2000m, Fee = 0, Currency = "EGP", Type = TransactionType.TopUp, Status = TransactionStatus.Completed, CreatedAt = now.AddDays(-7) };
            var tx3 = new Transaction { ToWalletId = c1Wallet.Id, Amount = 500m,  Fee = 0, Currency = "EGP", Type = TransactionType.TopUp, Status = TransactionStatus.Completed, CreatedAt = now.AddDays(-3) };

            // ── Customer 2: Top-Up history ─────────────────────────────────────────
            Transaction? tx4 = null;
            if (c2Wallet != null)
                tx4 = new Transaction { ToWalletId = c2Wallet.Id, Amount = 3000m, Fee = 0, Currency = "EGP", Type = TransactionType.TopUp, Status = TransactionStatus.Completed, CreatedAt = now.AddDays(-5) };

            // ── Technician 1: Earnings (transfer from escrow) ──────────────────────
            var tx5 = new Transaction { ToWalletId = t1Wallet.Id, Amount = 200m, Fee = 0, Currency = "EGP", Type = TransactionType.Transfer, Status = TransactionStatus.Completed, CreatedAt = now.AddDays(-2) };

            // ── Technician 1: Withdrawal request (Pending) ────────────────────────
            var tx6 = new Transaction { FromWalletId = t1Wallet.Id, Amount = 300m, Fee = 0, Currency = "EGP", Type = TransactionType.Withdrawal, Status = TransactionStatus.Pending, CreatedAt = now.AddDays(-1) };

            // ── Technician 2: Earnings ─────────────────────────────────────────────
            Transaction? tx7 = null;
            if (t2Wallet != null)
                tx7 = new Transaction { ToWalletId = t2Wallet.Id, Amount = 350m, Fee = 0, Currency = "EGP", Type = TransactionType.Transfer, Status = TransactionStatus.Completed, CreatedAt = now.AddDays(-3) };

            // Persist all transactions
            var txList = new List<Transaction> { tx1, tx2, tx3, tx5, tx6 };
            if (tx4 != null) txList.Add(tx4);
            if (tx7 != null) txList.Add(tx7);

            await context.Transactions.AddRangeAsync(txList);
            await context.SaveChangesAsync();

            // ── Escrow: Pending booking (Booking 1) → Held escrow ─────────────────
            var pendingBooking = bookings.FirstOrDefault(b => b.Status == BookingStatus.Pending);
            if (pendingBooking != null && !await context.Escrows.AnyAsync(e => e.BookingId == pendingBooking.Id))
            {
                var bookingPaymentTx = new Transaction
                {
                    FromWalletId = c1Wallet.Id,
                    Amount = 200m, Fee = 0, Currency = "EGP",
                    Type = TransactionType.BookingPayment,
                    Status = TransactionStatus.Completed,
                    CreatedAt = now.AddDays(-1)
                };
                await context.Transactions.AddAsync(bookingPaymentTx);
                await context.SaveChangesAsync();

                await context.Escrows.AddAsync(new Escrow
                {
                    BookingId = pendingBooking.Id,
                    WalletId = c1Wallet.Id,
                    Amount = 200m,
                    Status = EscrowStatus.Held,
                    HeldAt = now.AddDays(-1)
                });
                await context.SaveChangesAsync();
            }

            // ── Escrow: Completed booking (Booking 2) → Released escrow + Invoice ─
            var completedBooking = bookings.FirstOrDefault(b => b.Status == BookingStatus.Completed);
            if (completedBooking != null && !await context.Escrows.AnyAsync(e => e.BookingId == completedBooking.Id))
            {
                var releaseTx = new Transaction
                {
                    FromWalletId = c2Wallet?.Id,
                    ToWalletId = t2Wallet?.Id,
                    Amount = 350m, Fee = 0, Currency = "EGP",
                    Type = TransactionType.Transfer,
                    Status = TransactionStatus.Completed,
                    CreatedAt = now.AddDays(-3)
                };
                await context.Transactions.AddAsync(releaseTx);
                await context.SaveChangesAsync();

                var escrow = new Escrow
                {
                    BookingId = completedBooking.Id,
                    WalletId = c2Wallet?.Id ?? c1Wallet.Id,
                    Amount = 350m,
                    Status = EscrowStatus.Released,
                    HeldAt = now.AddDays(-4),
                    ReleasedAt = now.AddDays(-3)
                };
                await context.Escrows.AddAsync(escrow);
                await context.SaveChangesAsync();

                // ── Invoice for the completed booking ──────────────────────────────
                if (!await context.Invoices.AnyAsync(i => i.BookingId == completedBooking.Id))
                {
                    await context.Invoices.AddAsync(new Invoice
                    {
                        BookingId = completedBooking.Id,
                        TransactionId = releaseTx.Id,
                        CustomerId = completedBooking.CustomerId,
                        TechnicianId = completedBooking.TechnicianId,
                        Amount = 350m,
                        Tax = 0,
                        TotalAmount = 350m,
                        Status = InvoiceStatus.Paid,
                        IssuedAt = now.AddDays(-3),
                        PaidAt = now.AddDays(-3)
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}