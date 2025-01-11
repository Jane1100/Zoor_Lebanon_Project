namespace Zoor_Lebanon.Models
{
    public partial class Booking
    {
        public Booking()
        {
            Payments = new HashSet<Payment>();
        }

        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public int? UserId { get; set; }
        public int? PackageId { get; set; }
        public DateOnly? TravelDate { get; set; }
        public int Quantity { get; set; } // Number of participants
        public string? ParticipantDetails { get; set; } // JSON or string for extra participant info

        public string? PaymentStatus { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? CancellationStatus { get; set; }
        public string? DiscountCode { get; set; }  // Store the applied discount code

        public virtual Package? Package { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
