namespace Zoor_Lebanon.Models.UserProfileViewModel
{
    public class UserProfileViewModel
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? Dob { get; set; } // Display as a string for simplicity
        public string? CityName { get; set; } // Name of the city
        public int? CityId { get; set; } // Add CityId for updates

    }


}
