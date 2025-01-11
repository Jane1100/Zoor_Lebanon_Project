namespace Zoor_Lebanon.Models.ViewModels
{
    public class RewardViewModel
    {
        public int CouponId { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountValue { get; set; } // Represents the percentage discount (e.g., 10% off)
        public int PointsCost { get; set; }
    }
}
