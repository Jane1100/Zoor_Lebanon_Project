using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class Coupon
    {
        public Coupon()
        {
            UserCoupons = new HashSet<UserCoupon>();
        }

        public int CouponId { get; set; }
        public string? CouponCode { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public int? PointsCost { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<UserCoupon> UserCoupons { get; set; }
    }
}
