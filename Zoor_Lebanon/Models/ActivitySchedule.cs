namespace Zoor_Lebanon.Models
{
    public partial class ActivitySchedule
    {
        public int ActivityId { get; set; }
        public string? Description { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public int? PackageId { get; set; }

        public virtual Package? Package { get; set; }
    }
}
