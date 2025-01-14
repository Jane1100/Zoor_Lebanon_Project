namespace Zoor_Lebanon.Models
{
    public partial class Country
    {
        public Country()
        {
            States = new HashSet<State>();
        }

        public int CountryId { get; set; }
        public string? Country1 { get; set; }

        public virtual ICollection<State> States { get; set; }
    }
}
