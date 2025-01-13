namespace Zoor_Lebanon.Models
{
    public partial class User
    {
        public User()
        {
            Bookings = new HashSet<Booking>();
            Reviews = new HashSet<Review>();
            Rewards = new HashSet<Reward>();
            TourOperators = new HashSet<TourOperator>();
            UserCoupons = new HashSet<UserCoupon>();
            UserPreferences = new HashSet<UserPreference>();
        }

        public int UserId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public DateTime? Dob { get; set; }
        public int? CityId { get; set; }
        /// <summary>
        /// User email with global validation
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Global phone number validation
        /// </summary>
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }

        public string? Salt { get; set; }
        public int? RoleId { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual City? City { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Reward> Rewards { get; set; }
        public virtual ICollection<TourOperator> TourOperators { get; set; }
        public virtual ICollection<UserCoupon> UserCoupons { get; set; }
        public virtual ICollection<UserPreference> UserPreferences { get; set; }
    }
}
