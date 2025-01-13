using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Zoor_Lebanon.Models;
namespace Zoor_Lebanon.Models.ViewModels
{

    public class PackageViewModelAdmin
    {
        public Package Package { get; set; } // For Package Details
        public List<PackageType> PackageTypes { get; set; } // Dropdown for Package Types
        public List<Location> Locations { get; set; } // Dropdown for Locations
        public List<string?> States { get; set; } // Dropdown for States
        public List<string?> Cities { get; set; } // Dropdown for Cities


        public string State { get; set; } // Add this
        public string City { get; set; }  // Add this

       
        // Define a simple package detail class
        public class PackageDetail
        {
            public int PackageId { get; set; }
            public string PackageName { get; set; }
            public string Status { get; set; }
            public int Popularity { get; set; }
        }
    }

}