namespace Zoor_Lebanon.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public string? Firstname { get; set; }

        public string? Lastname { get; set; }
        public string? Email { get; set; }

        public string? Dob { get; set; } // Display as a string for simplicity


        public string? CityName { get; set; } // Name of the city


        public int? CityId { get; set; } // Add CityId for updates

        public int PointsBalance { get; set; } // Total points balance for the user
        public List<RedeemedReward> RedeemedRewards { get; set; } = new();
    }

    public class RedeemedReward
    {
        public string? Description { get; set; } // Description of the reward
        public string? UnlockDate { get; set; } // When the reward was redeemed
    }
}
