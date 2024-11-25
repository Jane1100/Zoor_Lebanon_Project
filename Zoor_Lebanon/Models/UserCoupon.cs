using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class UserCoupon
    {
        public int UserId { get; set; }
        public int CouponId { get; set; }
        public DateOnly? UnlockDate { get; set; }
        public bool? IsRedeemed { get; set; }

        public virtual Coupon Coupon { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
