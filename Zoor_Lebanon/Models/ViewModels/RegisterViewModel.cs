using System.ComponentModel.DataAnnotations;

namespace Zoor_Lebanon.Models.ViewModels
{
    public class RegisterViewModel
    {
        public User? User { get; set; } // For user details like name, email, password, etc.
        public List<Preference>? Preferences { get; set; } // List of all available preferences.
        public List<Country>? Countries { get; set; } // List of countries for the dropdown.
        public List<State>? States { get; set; } // List of states for the dropdown.
        public List<City>? Cities { get; set; } // List of cities for the dropdown.

        [Required]
        public int? CityId { get; set; } // To capture the selected city ID.

        [Required]
        public int? StateId { get; set; } // To capture the selected state ID.

        [Required]
        public int? CountryId { get; set; } // To capture the selected country ID.

        [Required]
        public List<int>? SelectedPreferences { get; set; } // To capture selected preferences.

        public string? CountryCode { get; set; } // To store the selected country code for phone numbers.
    }
}
