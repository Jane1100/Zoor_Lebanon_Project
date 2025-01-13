namespace Zoor_Lebanon.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public string? ReviewDescription { get; set; }
        public int? UserId { get; set; }
        public int? PackageId { get; set; }
        public int? Rating { get; set; }
        public string? PublishedStatus { get; set; }

        public virtual Package? Package { get; set; }
        public virtual User? User { get; set; }
    }
}
