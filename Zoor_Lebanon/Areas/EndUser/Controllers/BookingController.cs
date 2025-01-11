using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models;

namespace Zoor_Lebanon_Booking_Platform.Areas.EndUser.Controllers
{
    [Area("EndUser")]
    public class BookingController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public BookingController(zoor_lebanonContext context)
        {
            _context = context;
        }

        // Booking Form View
        public IActionResult BookingForm(int packageId)
        {
            var package = _context.Packages.FirstOrDefault(p => p.PackageId == packageId);

            // Skip redirection if booking is already successful
            if (package.AvailableSpots <= 0 && TempData["SuccessMessage"] == null)
            {
                TempData["ErrorMessage"] = "This package is unavailable or sold out.";
                return RedirectToAction("Packages", "Home");
            }

            // Convert DateOnly to DateTime for range calculation
            var startDate = package.StartDate.Value.ToDateTime(new TimeOnly(0, 0));
            var endDate = package.EndDate.Value.ToDateTime(new TimeOnly(0, 0));

            // Generate available dates
            var availableDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset))
                .Select(date => date.ToString("yyyy-MM-dd")) // Format as string if needed in the view
                .ToList();

            // Populate booking form with package details
            ViewBag.Package = package;
            ViewBag.AvailableDates = availableDates;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBooking(int packageId, DateTime travelDate, int quantity, string participantDetails, string? discountCode, decimal totalPrice)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to book a package.";
                return RedirectToAction("BookingForm", new { packageId });
            }

            var package = _context.Packages.FirstOrDefault(p => p.PackageId == packageId);
            if (package == null)
            {
                TempData["ErrorMessage"] = "The package does not exist.";
                return RedirectToAction("BookingForm", new { packageId });
            }

            if (package.AvailableSpots < quantity)
            {
                TempData["ErrorMessage"] = "Not enough spots available for this package.";
                return RedirectToAction("BookingForm", new { packageId });
            }

            decimal recalculatedTotalPrice = package.UnitPrice.GetValueOrDefault(0) * quantity;

            UserCoupon? userCoupon = null;
            if (!string.IsNullOrEmpty(discountCode))
            {
                userCoupon = await _context.UserCoupons
                    .Include(uc => uc.Coupon)
                    .FirstOrDefaultAsync(uc =>
                        uc.UserId == userId &&
                        uc.Coupon.CouponCode == discountCode &&
                        uc.Coupon.IsActive == true);

                if (userCoupon == null || userCoupon.IsRedeemed == true)
                {
                    TempData["ErrorMessage"] = "Invalid or already redeemed discount code.";
                    return RedirectToAction("BookingForm", new { packageId });
                }

                recalculatedTotalPrice -= recalculatedTotalPrice * (userCoupon.Coupon.DiscountValue ?? 0);
            }

            totalPrice = recalculatedTotalPrice;

            var booking = new Booking
            {
                BookingDate = DateTime.Now,
                UserId = userId,
                PackageId = packageId,
                TravelDate = DateOnly.FromDateTime(travelDate),
                Quantity = quantity,
                ParticipantDetails = participantDetails,
                PaymentStatus = "paid",
                TotalPrice = totalPrice,
                CancellationStatus = false,
                DiscountCode = discountCode
            };

            package.AvailableSpots -= quantity;
            _context.Bookings.Add(booking);

            if (userCoupon != null)
            {
                userCoupon.IsRedeemed = true; // Mark coupon as redeemed
            }

            await _context.SaveChangesAsync();

            AddPoints(userId.Value, package.Points);

            TempData["SuccessMessage"] = "Your booking was successful! You have a 24h margin for cancellation.";
            return RedirectToAction("BookingForm", new { packageId });
        }

        private void AddPoints(int userId, int points)
        {
            // Fetch the latest reward to use the updated balance
            int currentBalance = GetCurrentUserPoints(userId);

            // Add points to the current balance
            _context.Rewards.Add(new Reward
            {
                UserId = userId,
                PointsEarned = points,
                PointsSpent = 0, // No points are spent on this action
                CurrentBalance = currentBalance + points, // Add earned points to the current balance
                TransactionDate = DateOnly.FromDateTime(DateTime.Now)
            });

            _context.SaveChanges();
        }

        private int GetCurrentUserPoints(int userId)
        {
            // Fetch the most recent reward entry for the user
            var latestReward = _context.Rewards
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RewardsId)
                .FirstOrDefault();

            // If no rewards, return 0; else return the current balance
            return latestReward?.CurrentBalance ?? 0;
        }

        [HttpPost]
        public async Task<IActionResult> ValidateDiscountCode([FromBody] DiscountValidationRequest request)
        {
            if (request == null)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "You must be logged in to redeem a discount." });
            }

            var userCoupon = await _context.UserCoupons
                .Include(uc => uc.Coupon)
                .FirstOrDefaultAsync(uc =>
                    uc.UserId == userId &&
                    uc.Coupon.CouponCode == request.DiscountCode &&
                    uc.Coupon.IsActive == true);

            if (userCoupon == null || userCoupon.IsRedeemed == true)
            {
                return Json(new { success = false, message = "Invalid or already redeemed discount code." });
            }

            var package = await _context.Packages.FirstOrDefaultAsync(p => p.PackageId == request.PackageId);
            if (package == null)
            {
                return Json(new { success = false, message = "Package not found." });
            }

            decimal unitPrice = package.UnitPrice ?? 0;
            decimal totalPrice = unitPrice * request.Quantity;
            totalPrice -= totalPrice * (userCoupon.Coupon.DiscountValue ?? 0);

            return Json(new { success = true, totalPrice });
        }


        public class DiscountValidationRequest
        {
            public string DiscountCode { get; set; }
            public int PackageId { get; set; }
            public int Quantity { get; set; }
        }

        public IActionResult BookingConfirmation()
        {
            return View();
        }

        /*private void SendConfirmationEmail(int userId, Booking booking)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return;

            string subject = "Booking Confirmation";
            string message = $"Hello {user.Firstname},\n\n" +
                             $"Your booking for {booking.Package.PackageName} on {booking.TravelDate} has been confirmed.\n" +
                             $"Details:\n" +
                             $"- Package: {booking.Package.PackageName}\n" +
                             $"- Travel Date: {booking.TravelDate}\n" +
                             $"- Quantity: {booking.Quantity}\n" +
                             $"- Total Price: ${booking.TotalPrice}\n\n" +
                             "You have 24 hours to cancel your booking if needed.\n\n" +
                             "Thank you for booking with Zoor Lebanon.";

            EmailService.SendEmail(user.Email, subject, message);
        }*/


        [HttpPost]
        public IActionResult CancelBooking(int bookingId, string reason)
        {
            var booking = _context.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => new
                {
                    b.BookingId,
                    b.CancellationStatus,
                    b.PackageId,
                    b.Quantity
                })
                .FirstOrDefault();

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found.";
                return RedirectToAction("MyBookings");
            }

            // Update CancellationStatus
            _context.Bookings.First(b => b.BookingId == booking.BookingId).CancellationStatus = true;

            // Refund the spots
            var package = _context.Packages.FirstOrDefault(p => p.PackageId == booking.PackageId);
            if (package != null)
            {
                package.AvailableSpots += booking.Quantity;
            }

            _context.SaveChanges();

            // Set success message
            TempData["SuccessMessage"] = "Your booking has been successfully canceled.";

            // Redirect back to MyBookings
            return RedirectToAction("MyBookings");
        }


        public IActionResult MyBookings()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    BookingId = b.BookingId,
                    TravelDate = b.TravelDate,
                    Quantity = b.Quantity,
                    TotalPrice = b.TotalPrice ?? 0,
                    PaymentStatus = b.PaymentStatus,
                    CancellationStatus = b.CancellationStatus ?? false, // Default to false if null
                    PackageName = b.Package.PackageName,
                    BookingDate = b.BookingDate
                })
                .ToList();

            // Perform the CanCancel calculation in-memory
            var bookingViewModel = bookings.Select(b => new
            {
                b.BookingId,
                b.TravelDate,
                b.Quantity,
                b.TotalPrice,
                b.PaymentStatus,
                b.CancellationStatus,
                b.PackageName,
                CanCancel = b.BookingDate.HasValue && (DateTime.Now - b.BookingDate.Value).TotalHours <= 24
            }).ToList();

            ViewBag.Bookings = bookingViewModel;
            return View();
        }



        public IActionResult PackageDetails(int packageId)
        {
            var package = _context.Packages
                .Include(p => p.ActivitySchedules)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User) // To display reviewer details
                .FirstOrDefault(p => p.PackageId == packageId);

            if (package == null)
            {
                TempData["ErrorMessage"] = "Package not found.";
                return RedirectToAction("Packages", "Home", new { area = "EndUser" });
            }

            return View(package);
        }




    }
}
