using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class TourOperator
    {
        public int OperatorId { get; set; }
        public int? UserId { get; set; }
        public string? CompanyName { get; set; }
        /// <summary>
        /// Lebanon phone number starting with +961
        /// </summary>
        public string? BusinessPhone { get; set; }

        public virtual User? User { get; set; }
    }
}
