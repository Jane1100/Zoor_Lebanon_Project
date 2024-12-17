using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.Helper;

namespace Zoor_Lebanon.Areas.Tourist.Controllers
{
    [Area("Tourist")]
    public class BookingController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public BookingController(zoor_lebanonContext context)
        {
            _context = context;
        }

        // Booking Form View
      /*  public IActionResult BookingForm(int packageId)
        {
            var package = _context.Packages.FirstOrDefault(p => p.PackageId == packageId);

            if (package == null || package.AvailableSpots <= 0)
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
        public IActionResult SubmitBooking(int packageId, DateTime travelDate, int quantity, string participantDetails)
        {
            // Check user authentication
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var package = _context.Packages.FirstOrDefault(p => p.PackageId == packageId);
            if (package == null || package.AvailableSpots < quantity)
            {
                TempData["ErrorMessage"] = "Not enough spots available for this package.";
                return RedirectToAction("Packages", "Home");
            }

            // Calculate total price
            decimal totalPrice = (package.UnitPrice ?? 0) * quantity;

            // Create a new booking
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
                CancellationStatus = false
            };

            // Update package spots
            package.AvailableSpots -= quantity;

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Your booking has been successful. A confirmation email has been sent.";

            // Send confirmation email (email logic to be added in Step 7)
            SendConfirmationEmail(userId.Value, booking);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult BookingConfirmation()
        {
            return View();
        }

        private void SendConfirmationEmail(int userId, Booking booking)
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
        }


        [HttpPost]
        public IActionResult CancelBooking(int bookingId, string reason)
        {
            var booking = _context.Bookings
            .Where(b => b.BookingId == bookingId)
            .Select(b => new
            {
                b.BookingId,
                b.CancellationStatus, // This should map correctly after the above fixes
                b.PackageId,
                b.Quantity
            })
            .FirstOrDefault();

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found.";
                return RedirectToAction("MyBookings");
            }

            // Update CancellationStatus as boolean
            _context.Bookings.First(b => b.BookingId == booking.BookingId).CancellationStatus = true;


            // Refund the spots
            var package = _context.Packages.FirstOrDefault(p => p.PackageId == booking.PackageId);
            if (package != null)
            {
                package.AvailableSpots += booking.Quantity;
            }

            _context.SaveChanges();

            // Send cancellation email
            *//*var user = _context.Users.FirstOrDefault(u => u.UserId == booking.UserId);
            if (user != null)
            {
                string subject = "Booking Cancellation Confirmation";
                string message = $"Hello {user.Firstname},\n\n" +
                                 $"Your booking for {booking.Package.PackageName} has been canceled.\n" +
                                 $"Reason: {reason}\n\n" +
                                 "If this was a mistake, feel free to book again.\n\n" +
                                 "Thank you for choosing Zoor Lebanon.";
                EmailService.SendEmail(user.Email, subject, message);
            }*//*

            TempData["SuccessMessage"] = "Your booking has been successfully canceled.";
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
                    PackageName = b.Package.PackageName
                })
                .ToList();

            ViewBag.Bookings = bookings;
            return View();
        }
*/
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