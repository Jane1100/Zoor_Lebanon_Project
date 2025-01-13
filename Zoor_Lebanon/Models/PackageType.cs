using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zoor_Lebanon.Models
{
    public partial class PackageType
    {
        public PackageType()
        {
            Packages = new HashSet<Package>();
        }

        public int PackageTypeId { get; set; }

        [Column("package_type")]
        public string? PackageType1 { get; set; }

        public virtual ICollection<Package> Packages { get; set; }
    }
}
