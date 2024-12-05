using Zoor_Lebanon.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zoor_Lebanon.Models.ViewModels
{
    public class PackageViewModel
    {
        public Package Package { get; set; } // For Package Details
        public List<PackageType> PackageTypes { get; set; } // Dropdown for Package Types
        public List<Location> Locations { get; set; } // Dropdown for Locations
        public List<string?> States { get; set; } // Dropdown for States
        public List<string?> Cities { get; set; } // Dropdown for Cities
  
      
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total spots must be greater than 0.")]
        public int TotalSpots { get; set; }
    
        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [CustomValidation(typeof(PackageViewModel), "ValidateStartDate")]
        public DateTime StartDate { get; set; }
       
        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public static ValidationResult? ValidateStartDate(DateTime startDate, ValidationContext context)
        {
            return startDate >= DateTime.Today
                ? ValidationResult.Success
                : new ValidationResult("Start date cannot be in the past.");
        }
    }
}
