using System.Collections.Generic;

namespace Zoor_Lebanon.Models.IndexViewModel
{
    public class ChartData
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }

    public class IndexViewModel
    {
        public List<ChartData> BookingPackageData { get; set; }
        public List<ChartData> TopPackages { get; set; }
        public List<ChartData> ActiveLocations { get; set; }
        public List<ChartData> UserDemographics { get; set; }
        public List<ChartData> AverageBookingValues { get; set; }
        public List<ChartData> TopTourOperators { get; set; }
        public object VisitorDemographics { get; internal set; }
    }


}
