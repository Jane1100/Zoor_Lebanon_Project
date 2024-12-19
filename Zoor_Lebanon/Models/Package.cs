using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class Package
    {
        public Package()
        {
            ActivitySchedules = new HashSet<ActivitySchedule>();
            Bookings = new HashSet<Booking>();
            Reviews = new HashSet<Review>();
        }

        public int PackageId { get; set; }
        public string? PackageName { get; set; }
        public string? Description { get; set; }
        public int? LocationId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? AvailableSpots { get; set; }
        public int? TotalSpots { get; set; }
        public int? PackageTypeId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal? AverageDuration { get; set; }

        public virtual Location? Location { get; set; }
        public virtual PackageType? PackageType { get; set; }
        public virtual ICollection<ActivitySchedule> ActivitySchedules { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public string Status { get; set; }
    }
}
