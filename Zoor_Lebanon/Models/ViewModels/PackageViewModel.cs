using Zoor_Lebanon.Models;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models.ViewModels
{
    public class PackageViewModel
    {
        public Package Package { get; set; } // For Package Details
        public List<ActivitySchedule> ActivitySchedules { get; set; } = new List<ActivitySchedule>();

        public List<PackageType> PackageTypes { get; set; } // Dropdown for Package Types
        public int LocationId { get; set; }
        public List<string> States { get; set; } // Dropdown for States
        public List<string> Cities { get; set; } // Dropdown for Cities

        
    }
}
