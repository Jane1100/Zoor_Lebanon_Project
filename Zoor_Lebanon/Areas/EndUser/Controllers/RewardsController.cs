using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models;

namespace Zoor_Lebanon.Areas.EndUser.Controllers
{
    [Area("EndUser")]
    public class RewardsController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public RewardsController(zoor_lebanonContext context)
        {
            _context = context;
        }

        public IActionResult Rewards()
        {
            ViewData["ShowCarousel"] = false;
            var rewards = _context.Coupons.Where(c => c.IsActive == true).ToList();
            return View(rewards);
        }

        [HttpPost]
        public async Task<IActionResult> Redeem(int couponId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Json(new { success = false, message = "Please log in to redeem coupons." });
            }

            // Get the most recent reward record for the user
            var latestReward = _context.Rewards
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RewardsId)  // Getting the latest reward based on RewardsId
                .FirstOrDefault();

            if (latestReward == null)
            {
                return Json(new { success = false, message = "No rewards found for your account." });
            }

            var coupon = await _context.Coupons.FindAsync(couponId);

            if (coupon == null)
            {
                return Json(new { success = false, message = "Invalid coupon." });
            }

            // Check if the user has already redeemed this coupon
            var alreadyRedeemed = _context.UserCoupons
                .Any(uc => uc.UserId == userId && uc.CouponId == couponId && uc.IsRedeemed == true);

            if (alreadyRedeemed)
            {
                return Json(new { success = false, message = "You have already redeemed this reward." });
            }

            if (latestReward.CurrentBalance < coupon.PointsCost)
            {
                return Json(new { success = false, message = "Insufficient points to redeem this coupon." });
            }

            // Deduct points from the user's current balance in the Reward table
            latestReward.CurrentBalance -= coupon.PointsCost;

            // Mark the coupon as redeemed
            _context.UserCoupons.Add(new UserCoupon
            {
                UserId = userId.Value,
                CouponId = coupon.CouponId,
                UnlockDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsRedeemed = true
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Coupon redeemed successfully!" });
        }






    }
}
