using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class Reward
    {
        public int RewardsId { get; set; }
        public int? UserId { get; set; }
        public int? PointsEarned { get; set; }
        public int? PointsSpent { get; set; }
        public int? CurrentBalance { get; set; }
        public DateOnly? TransactionDate { get; set; }

        public virtual User? User { get; set; }
    }
}
