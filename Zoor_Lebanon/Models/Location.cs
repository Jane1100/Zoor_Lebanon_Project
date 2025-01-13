namespace Zoor_Lebanon.Models
{
    public partial class Location
    {
        public Location()
        {
            Packages = new HashSet<Package>();
        }

        public int LocationId { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }

        public virtual ICollection<Package> Packages { get; set; }
    }
}
