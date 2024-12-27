using System.Linq;
using Zoor_Lebanon.Models.IndexViewModel;
using Zoor_Lebanon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class DashboardController : Controller
{
    private readonly zoor_lebanonContext _context;

    public DashboardController(zoor_lebanonContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var visitorChartData = _context.Visitors
            .GroupBy(v => v.VisitDate.Date)
            .Select(group => new
            {
                Date = group.Key.ToString("MM/dd/yyyy"),
                Count = group.Count()
            })
            .OrderBy(x => x.Date)
            .ToList();

        var salesChartData = _context.Sales
            .GroupBy(s => s.SaleDate.Month)
            .Select(group => new
            {
                Month = group.Key,
                TotalSales = group.Sum(s => s.Amount)
            })
            .OrderBy(x => x.Month)
            .ToList();

        var model = new IndexViewModel
        {
            VisitorChartData = visitorChartData,
            SalesChartData = salesChartData,
            TouristByCityCount = _context.Users
                .Where(u => u.CityId.HasValue)
                .GroupBy(u => u.City.City1)
                .ToDictionary(g => g.Key, g => g.Count()),
            SalesGrowthRate = CalculateGrowthRate(salesChartData)  // Calculate and assign SalesGrowthRate
        };

        return View(model);
    }

    private double CalculateGrowthRate(List<dynamic> data)
    {
        // Adjust this calculation based on your specific needs
        if (data.Count >= 2)
        {
            var last = data.Last().TotalSales;
            var first = data.First().TotalSales;
            if (first != 0)
                return ((double)last - first) / first * 100;  // Return percentage growth
        }
        return 0;
    }
}
