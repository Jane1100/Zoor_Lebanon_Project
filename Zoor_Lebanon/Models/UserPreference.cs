using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class UserPreference
    {
        public int UserPreferenceId { get; set; }
        public int? UserId { get; set; }
        public int? PreferenceId { get; set; }

        public virtual Preference? Preference { get; set; }
        public virtual User? User { get; set; }
    }
}
