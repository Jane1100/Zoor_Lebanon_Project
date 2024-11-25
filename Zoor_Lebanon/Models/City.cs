using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class City
    {
        public City()
        {
            Users = new HashSet<User>();
        }

        public int CityId { get; set; }
        public string? City1 { get; set; }
        public int? StateId { get; set; }

        public virtual State? State { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
