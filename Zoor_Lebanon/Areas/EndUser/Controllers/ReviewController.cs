using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Zoor_Lebanon.Areas.EndUser.Models;
using Zoor_Lebanon.Models;

namespace Zoor_Lebanon.Areas.EndUser.Controllers
{
    [Area("EndUser")]
    public class ReviewController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public ReviewController(zoor_lebanonContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(int packageId, string reviewText, int rating)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to submit a review.";
                return RedirectToAction("PackageDetails", "Booking", new { area = "EndUser", packageId });
            }

            var booking = _context.Bookings.FirstOrDefault(b =>
                b.PackageId == packageId &&
                b.UserId == userId &&
                (b.CancellationStatus == false || b.CancellationStatus == null) &&
                b.TravelDate != null && b.TravelDate.Value < DateOnly.FromDateTime(DateTime.Now));

            if (booking == null)
            {
                TempData["ErrorMessage"] = "You cannot submit a review for this package. Either you did not book it, or cancelled it, or did not complete the tour yet.";
                return RedirectToAction("PackageDetails", "Booking", new { area = "EndUser", packageId });
            }

            var reviewData = new { review = reviewText };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(reviewData), Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("http://localhost:5000/process_review", jsonContent);
                var resultString = await response.Content.ReadAsStringAsync();

                // Deserialize response into a typed object
                var reviewResult = JsonConvert.DeserializeObject<ReviewResponse>(resultString);

                if (reviewResult?.Status == "rejected")
                {
                    TempData["ErrorMessage"] = $"Your review has been rejected. Reason: {reviewResult.Reason}";
                    return RedirectToAction("PackageDetails", "Booking", new { area = "EndUser", packageId });
                }
            }

            var review = new Review
            {
                PackageId = packageId,
                UserId = userId,
                ReviewDescription = reviewText,
                Rating = rating,
                PublishedStatus = "published"
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your review has been submitted successfully. Thank you for your feedback!";
            return RedirectToAction("PackageDetails", "Booking", new { area = "EndUser", packageId });
        }



    }
}
