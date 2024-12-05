namespace Zoor_Lebanon.Models.TourismReportViewModel
{
    public class TourismReportViewModel
    {
        public int VisitorCount { get; set; }
        public decimal VisitorGrowthRate { get; set; }
        public List<PackageSummary> TopPackages { get; set; } // Use PackageSummary for simplicity
        public decimal TotalSales { get; set; }
        public decimal SalesGrowthRate { get; set; }
        public ChartData VisitorChartData { get; set; }
        public ChartData SalesChartData { get; set; }
    }

    public class PackageSummary
    {
        public string PackageName { get; set; }
        public decimal UnitPrice { get; set; }
        public int BookingsCount { get; set; }
    }

    public class ChartData
    {
        public List<string> Dates { get; set; }
        public List<int> Values { get; set; }
    }


}
