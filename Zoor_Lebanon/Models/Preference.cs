using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class Preference
    {
        public Preference()
        {
            UserPreferences = new HashSet<UserPreference>();
        }

        public int PreferenceId { get; set; }
        public string? PreferenceDescription { get; set; }

        public virtual ICollection<UserPreference> UserPreferences { get; set; }
    }
}
