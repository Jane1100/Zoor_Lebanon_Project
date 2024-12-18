using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models.RegisterViewModel
{
    public class RegisterViewModel
    {
        // User Details
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public DateTime? Dob { get; set; } // DateTime is easier to work with in forms
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; } // Plain password entered by the user
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        // public User? User { get; set; }
        // Preferences

        public List<Preference> Preferences { get; set; } = new List<Preference>();
        public List<int> SelectedPreferences { get; set; } = new List<int>();
        // Dropdown Data
        public List<Country>? Countries { get; set; } // List of available countries
        public List<State>? States { get; set; } // List of states for selected country
        public List<City>? Cities { get; set; } // List of cities for selected state
     

     /*       [Required]
            public int? CityId { get; set; } // To capture the selected city ID.

            [Required]
            public List<int>? SelectedPreferences { get; set; } // To capture selected preferences.*/
    
        public string? CountryCode { get; set; } // To store the selected country code for phone numbers.
        public User User { get;  set; }
    }
}
