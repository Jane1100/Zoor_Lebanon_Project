using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class State
    {
        public State()
        {
            Cities = new HashSet<City>();
        }

        public int StateId { get; set; }
        public string? State1 { get; set; }
        public int? CountryId { get; set; }

        public virtual Country? Country { get; set; }
        public virtual ICollection<City> Cities { get; set; }
    }
}
