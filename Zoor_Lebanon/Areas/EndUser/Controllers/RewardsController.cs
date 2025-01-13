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
                TempData["ErrorMessage"] = "Please log in to redeem coupons.";
                return RedirectToAction("Rewards");
            }

            var latestReward = _context.Rewards
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RewardsId)
                .FirstOrDefault();

            if (latestReward == null)
            {
                TempData["ErrorMessage"] = "No rewards found for your account.";
                return RedirectToAction("Rewards");
            }

            var coupon = await _context.Coupons.FindAsync(couponId);
            if (coupon == null)
            {
                TempData["ErrorMessage"] = "Invalid coupon.";
                return RedirectToAction("Rewards");
            }

            var alreadyRedeemed = _context.UserCoupons
                .Any(uc => uc.UserId == userId && uc.CouponId == couponId && uc.IsRedeemed == true);

            if (alreadyRedeemed)
            {
                TempData["ErrorMessage"] = "You have already redeemed this reward.";
                return RedirectToAction("Rewards");
            }

            if (latestReward.CurrentBalance < coupon.PointsCost)
            {
                TempData["ErrorMessage"] = "Insufficient points to redeem this coupon.";
                return RedirectToAction("Rewards");
            }

            // Deduct points and redeem
            latestReward.CurrentBalance -= coupon.PointsCost;

            _context.UserCoupons.Add(new UserCoupon
            {
                UserId = userId.Value,
                CouponId = coupon.CouponId,
                UnlockDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsRedeemed = true
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Coupon redeemed successfully!";
            return RedirectToAction("Rewards");
        }


    }
}
