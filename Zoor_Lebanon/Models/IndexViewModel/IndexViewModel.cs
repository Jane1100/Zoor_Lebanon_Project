using System.Collections.Generic;

namespace Zoor_Lebanon.Models.IndexViewModel
{
    public class IndexViewModel
    {
        public List<dynamic> VisitorChartData { get; set; }
        public List<dynamic> SalesChartData { get; set; }
        public Dictionary<string, int> TouristByCityCount { get; set; }
        public double SalesGrowthRate { get; set; }  // Adding SalesGrowthRate
    }
}
