using System.Linq;
using Zoor_Lebanon.Models.IndexViewModel;
using Zoor_Lebanon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models.TourismReportViewModel;
using ChartData = Zoor_Lebanon.Models.IndexViewModel.ChartData; // Alias for ChartData
using Zoor_Lebanon.Models.Helper;

[Area("Admin")]
public class IndexController : Controller
{
    private readonly zoor_lebanonContext _context;

    public IndexController(zoor_lebanonContext context)
    {
        _context = context;
    }

 
    public IActionResult Index3()
    {
        var model = new IndexViewModel
        {
            BookingPackageData = _context.Bookings
                .Join(_context.Packages,
                      booking => booking.PackageId,
                      package => package.PackageId,
                      (booking, package) => new { booking, package })
                .GroupBy(x => x.package.PackageName)
                .Select(g => new ChartData
                {
                    Label = g.Key,
                    Value = g.Count()
                })
                .ToList(),

            TopPackages = _context.Packages
                .OrderByDescending(p => p.Bookings.Count)
                .Take(5)
                .Select(p => new ChartData
                {
                    Label = p.PackageName,
                    Value = p.Bookings.Count
                })
                .ToList(),

        

            ActiveLocations = _context.Locations
                .OrderByDescending(l => l.Packages.Sum(p => p.Bookings.Count))
                .Take(5)
                .Select(l => new ChartData
                {
                    Label = l.City,
                    Value = l.Packages.Sum(p => p.Bookings.Count)
                })
                .ToList(),

            UserDemographics = _context.Users
    .Include(u => u.City) // Include the City navigation property
    .ThenInclude(c => c.State) // Then include the State navigation property from City
    .Where(u => u.City != null && u.City.State != null && u.City.State.Country != null) // Make sure all navigation properties are not null
    .GroupBy(u => u.City.State.Country.Country1) // Assuming Country1 is the country name in the Country entity
    .Select(g => new ChartData
    {
        Label = g.Key,
        Value = g.Count()
    })
    .ToList(),


            AverageBookingValues = _context.Bookings
                .GroupBy(b => 1)
                .Select(g => new ChartData
                {
                    Label = "Average Booking Value",
                    Value = (int)g.Average(b => b.TotalPrice)
                })
                .ToList(),
            TopTourOperators = _context.TourOperators
    .Where(t => t.User != null) // Ensure that TourOperator has an associated User
    .Select(t => new
    {
        TourOperator = t,
        BookingCount = _context.Bookings.Count(b => b.UserId == t.UserId) // Assuming Bookings are linked to User
    })
    .OrderByDescending(t => t.BookingCount)
    .Take(5)
    .Select(t => new ChartData
    {
        Label = t.TourOperator.CompanyName,
        Value = t.BookingCount
    })
    .ToList()

    };

        return View(model);
    }




}
