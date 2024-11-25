using System;
using System.Collections.Generic;

namespace Zoor_Lebanon.Models
{
    public partial class PackageType
    {
        public PackageType()
        {
            Packages = new HashSet<Package>();
        }

        public int PackageTypeId { get; set; }
        public string? PackageType1 { get; set; }

        public virtual ICollection<Package> Packages { get; set; }
    }
}
