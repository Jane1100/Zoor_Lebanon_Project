//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Zoor_Lebanon.Models;
//using Zoor_Lebanon.Models.ViewModels;


//namespace Zoor_Lebanon.Areas.TourOperator.Controllers
//{
//    [Area("TourOperator")]
//    public class PackageController : Controller
//    {
//        private readonly zoor_lebanonContext _context;

//        public PackageController(zoor_lebanonContext context)
//        {
//            _context = context;
//        }
//        [HttpGet]
//        public async Task<IActionResult> addpackage()
//        {
//            var viewModel = new PackageViewModel
//            {
//                Package = new Package(),
//                PackageTypes = await _context.PackageTypes.ToListAsync(),
//                States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync()
//            };
//            return View(viewModel); // Correctly passing a PackageViewModel
//        }


//        [HttpPost]
//        public async Task<IActionResult> addpackageWithLocation(PackageViewModel model, string State, string City)
//        {

//            // Fetch LocationId
//            var location = await _context.Locations
//             .FirstOrDefaultAsync(l => l.State == State && l.City == City);
//            if (location == null)
//            {
//                ModelState.AddModelError("", "Invalid Location selection.");
//                model.PackageTypes = await _context.PackageTypes.ToListAsync();
//                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
//                return View(model);
//            }


//            // Validate Start Date and End Date
//            if (model.Package.StartDate < DateTime.Now)
//            {
//                ModelState.AddModelError("Package.StartDate", "Start date cannot be in the past.");
//            }

//            if (model.Package.EndDate > DateTime.Now.AddYears(1))
//            {
//                ModelState.AddModelError("Package.EndDate", "End date cannot exceed one year from today.");
//            }

//            if (model.Package.EndDate < model.Package.StartDate)
//            {
//                ModelState.AddModelError("Package.EndDate", "End date cannot be before the start date.");
//            }

//            // Validate Start and End Dates
//            if (model.Package.StartDate < DateTime.Now || model.Package.EndDate < model.Package.StartDate)
//            {
//                ModelState.AddModelError("Package.StartDate", "Start date must be today or later.");
//                ModelState.AddModelError("Package.EndDate", "End date must be after start date.");
//                model.PackageTypes = await _context.PackageTypes.ToListAsync();
//                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
//                return View(model); // Return the view with the same model
//            }




//            // Save the package
//            var package = new Package
//            {
//                PackageName = model.Package.PackageName,
//                Description = model.Package.Description,
//                UnitPrice = model.Package.UnitPrice,
//                TotalSpots = model.Package.TotalSpots,
//                AverageDuration = model.Package.AverageDuration,
//                StartDate = model.Package.StartDate,
//                EndDate = model.Package.EndDate,
//                LocationId = location.LocationId,
//                PackageTypeId = model.Package.PackageTypeId,
//                Status = "Pending"
//            };


//            _context.Packages.Add(package);
//            await _context.SaveChangesAsync(); // Generates the PackageId

//            if (model.ActivitySchedules != null)
//            {
//                foreach (var activity in model.ActivitySchedules)
//                {
//                    var activitySchedule = new ActivitySchedule
//                    {
//                        Description = activity.Description,
//                        FromTime = activity.FromTime,
//                        ToTime = activity.ToTime,
//                        PackageId = package.PackageId // Now PackageId exists
//                    };
//                    _context.ActivitySchedules.Add(activitySchedule);
//                }
//                await _context.SaveChangesAsync(); // Save activities
//            }



//            TempData["SuccessMessage"] = "Package and activities successfully added!";
//            return RedirectToAction("ManagePackages");
//        }




//        [HttpPost]
//        public async Task<IActionResult> addpackage(Package package)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(package); // Return with validation errors
//            }

//            // Save the package to the database
//            _context.Packages.Add(package);
//            await _context.SaveChangesAsync();
//            //return RedirectToAction("AddActivitySchedule", new { packageId = package.PackageId });

//            // Redirect to the Widgets page or a relevant page after successful creation
//            return RedirectToAction("ManagePackages");
//        }




//        [HttpGet]
//        public async Task<IActionResult> GetCitiesByState(string state)
//        {
//            if (string.IsNullOrEmpty(state))
//            {
//                return Json(new List<string>()); // Return an empty list if state is null
//            }

//            var cities = await _context.Locations
//                .Where(l => l.State == state) // Ensure this matches your state name
//                .Select(l => l.City)
//                .Distinct()
//                .ToListAsync();

//            return Json(cities);
//        }

//        public async Task<IActionResult> widgets()
//        {
//            var packages = await _context.Packages
//                .Include(p => p.Location)
//                .Include(p => p.PackageType)
//                .OrderByDescending(p => p.PackageId)
//                .ToListAsync();

//            return View(packages);
//        }


//        [HttpGet]
//        public async Task<IActionResult> ManagePackages()
//        {
//            var packages = await _context.Packages
//                .Include(p => p.Location)
//                .Include(p => p.PackageType)
//                .OrderByDescending(p => p.PackageId)
//                .ToListAsync();


//            return View(packages);
//        }

//        // Route for Packages Overview
//        [HttpGet]
//        public IActionResult PackagesOverview()
//        {
//            return View();
//        }


//        [HttpGet]
//        public async Task<IActionResult> editpackages(int id)
//        {
//            // Fetch the package by ID
//            var package = await _context.Packages
//                .Include(p => p.PackageType)
//                .Include(p => p.Location)
//                .FirstOrDefaultAsync(p => p.PackageId == id);

//            if (package == null)
//            {
//                return NotFound();
//            }

//            // Prepare the view model with the package and dropdown lists
//            var model = new PackageViewModel
//            {
//                Package = package,
//                PackageTypes = await _context.PackageTypes.ToListAsync(),
//                States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync()
//            };

//            return View(model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> editpackages(int id, PackageViewModel model, string State, string City)
//        {
//            // Find the package in the database
//            var package = await _context.Packages.FindAsync(id);
//            if (package == null)
//            {
//                TempData["ErrorMessage"] = "Package not found.";
//                return RedirectToAction(nameof(ManagePackages));
//            }

//            // Fetch LocationId based on the selected State and City
//            var location = await _context.Locations
//                .FirstOrDefaultAsync(l => l.State == State && l.City == City);

//            if (location == null)
//            {
//                ModelState.AddModelError("", "Invalid Location selection.");
//                model.PackageTypes = await _context.PackageTypes.ToListAsync();
//                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
//                return View(model);
//            }

//            // Check if values have changed
//            bool isUpdated = false;

//            if (package.PackageName != model.Package.PackageName)
//            {
//                package.PackageName = model.Package.PackageName;
//                isUpdated = true;
//            }
//            if (package.Description != model.Package.Description)
//            {
//                package.Description = model.Package.Description;
//                isUpdated = true;
//            }
//            if (package.UnitPrice != model.Package.UnitPrice)
//            {
//                package.UnitPrice = model.Package.UnitPrice;
//                isUpdated = true;
//            }
//            if (package.TotalSpots != model.Package.TotalSpots)
//            {
//                package.TotalSpots = model.Package.TotalSpots;
//                isUpdated = true;
//            }
//            if (package.StartDate != model.Package.StartDate)
//            {
//                package.StartDate = model.Package.StartDate;
//                isUpdated = true;
//            }
//            if (package.EndDate != model.Package.EndDate)
//            {
//                package.EndDate = model.Package.EndDate;
//                isUpdated = true;
//            }
//            if (package.PackageTypeId != model.Package.PackageTypeId)
//            {
//                package.PackageTypeId = model.Package.PackageTypeId;
//                isUpdated = true;
//            }
//            if (package.LocationId != location.LocationId)
//            {
//                package.LocationId = location.LocationId;
//                isUpdated = true;
//            }

//            // Save changes only if there are updates
//            if (isUpdated)
//            {
//                try
//                {
//                    _context.Packages.Update(package);
//                    await _context.SaveChangesAsync();

//                    TempData["SuccessMessage"] = "Package updated successfully.";
//                }
//                catch (Exception ex)
//                {
//                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
//                }
//            }
//            else
//            {
//                TempData["InfoMessage"] = "No changes were made.";
//            }

//            return RedirectToAction(nameof(ManagePackages));
//        }



//        //private bool PackageExists(int id)
//        //{
//        //    return _context.Packages.Any(e => e.PackageId == id);
//        //}


//        [HttpPost]
//        public async Task<IActionResult> DeletePackage(int id)
//        {
//            var package = await _context.Packages
//                .Include(p => p.Bookings) // Assuming "Bookings" is the navigation property for related bookings
//                .FirstOrDefaultAsync(p => p.PackageId == id);

//            if (package != null)
//            {
//                if (package.Bookings != null && package.Bookings.Any())
//                {
//                    // Option 1: Prevent deletion
//                    TempData["ErrorMessage"] = "Package cannot be deleted as it has associated bookings.";
//                    return RedirectToAction(nameof(ManagePackages));

//                    // Option 2: Uncomment below to cascade delete bookings
//                    // _context.Bookings.RemoveRange(package.Bookings);
//                }

//                _context.Packages.Remove(package);
//                await _context.SaveChangesAsync();

//                TempData["SuccessMessage"] = "Package deleted successfully.";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = "Package not found.";
//            }

//            return RedirectToAction(nameof(ManagePackages));
//        }




//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.ViewModels;


namespace Zoor_Lebanon.Areas.TourOperator.Controllers
{
    [Area("TourOperator")]
    public class PackageController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public PackageController(zoor_lebanonContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> addpackage()
        {
            var viewModel = new PackageViewModel
            {
                Package = new Package(),
                PackageTypes = await _context.PackageTypes.ToListAsync(),
                States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync()
            };
            return View(viewModel); // Pass the correct view model
        }

        [HttpPost]
        public async Task<IActionResult> addpackageWithLocation(PackageViewModel model, string State, string City)
        {
            // Fetch LocationId based on selected State and City
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.State == State && l.City == City);

            if (location == null)
            {
                ModelState.AddModelError("", "Invalid Location selection.");
                model.PackageTypes = await _context.PackageTypes.ToListAsync();
                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
                return View(model);
            }

            // Validate Start Date and End Date
            if (model.Package.StartDate < DateTime.Now)
            {
                ModelState.AddModelError("Package.StartDate", "Start date cannot be in the past.");
            }

            if (model.Package.EndDate > DateTime.Now.AddYears(1))
            {
                ModelState.AddModelError("Package.EndDate", "End date cannot exceed one year from today.");
            }

            if (model.Package.EndDate < model.Package.StartDate)
            {
                ModelState.AddModelError("Package.EndDate", "End date cannot be before the start date.");
            }

            // Return the view if there are validation errors
            if (!ModelState.IsValid)
            {
                model.PackageTypes = await _context.PackageTypes.ToListAsync();
                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
                return View(model);
            }

            // Create and save the package if validation succeeds
            var package = new Package
            {
                PackageName = model.Package.PackageName,
                Description = model.Package.Description,
                UnitPrice = model.Package.UnitPrice,
                TotalSpots = model.Package.TotalSpots,
                AverageDuration = model.Package.AverageDuration,
                StartDate = model.Package.StartDate,
                EndDate = model.Package.EndDate,
                LocationId = location.LocationId, // Assign the fetched LocationId
                PackageTypeId = model.Package.PackageTypeId, // Assign Package Type
                Status = "Pending" // Set status to Pending for admin review
            };

            _context.Packages.Add(package); // Save the package in the database
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your package has been created successfully.";
            return RedirectToAction("ManagePackages");
        }

        //[HttpPost]
        //public async Task<IActionResult> addpackage(Package package)
        //{



        //    _context.Packages.Add(package);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("ManagePackages");
        //}
        public async Task<IActionResult> AddPackage(PackageViewModel model, string state, string city)
        {
            // Fetch the location based on state and city from the user input
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.State == state && l.City == city);

            // Validate location existence
            if (location == null)
            {
                // Handle the error, perhaps by adding an error to the model state and returning a view
                Console.WriteLine("error");
            }

            // Set package location
            model.Package.LocationId = location.LocationId;



            // Add and save the package
            _context.Packages.Add(model.Package);
            await _context.SaveChangesAsync();

            // Optionally, redirect or return a success response
            return RedirectToAction("widgets");
        }


        [HttpGet]
        public async Task<IActionResult> GetCitiesByState(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                return Json(new List<string>()); // Return an empty list if state is null
            }

            var cities = await _context.Locations
                .Where(l => l.State == state) // Ensure this matches your state name
                .Select(l => l.City)
                .Distinct()
                .ToListAsync();

            return Json(cities);
        }

        public async Task<IActionResult> widgets()
        {
            var packages = await _context.Packages
                .Include(p => p.Location)
                .Include(p => p.PackageType)
                .OrderByDescending(p => p.PackageId)
                .ToListAsync();

            return View(packages);
        }

       

        [HttpGet]
        public async Task<IActionResult> ManagePackages()
        {
            var packages = await _context.Packages
                .Include(p => p.Location)
                .Include(p => p.PackageType)
                .OrderByDescending(p => p.PackageId) // Sorting packages by ID or other criteria
                .ToListAsync();

            return View(packages);
        }

        // Route for Packages Overview
        [HttpGet]
        public IActionResult PackagesOverview()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> editpackages(int id)
        {
            // Fetch the package by ID
            var package = await _context.Packages
                .Include(p => p.PackageType)
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package == null)
            {
                return NotFound();
            }

            // Prepare the view model with the package and dropdown lists
            var model = new PackageViewModel
            {
                Package = package,
                PackageTypes = await _context.PackageTypes.ToListAsync(),
                States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync()
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> editpackages(int id, PackageViewModel model)
        {
            // Find the package in the database
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Package not found.";
                return RedirectToAction(nameof(ManagePackages));
            }

            // Check if values have changed
            bool isUpdated = false;

            if (package.PackageName != model.Package.PackageName)
            {
                package.PackageName = model.Package.PackageName;
                isUpdated = true;
            }
            if (package.Description != model.Package.Description)
            {
                package.Description = model.Package.Description;
                isUpdated = true;
            }
            if (package.UnitPrice != model.Package.UnitPrice)
            {
                package.UnitPrice = model.Package.UnitPrice;
                isUpdated = true;
            }
            if (package.TotalSpots != model.Package.TotalSpots)
            {
                package.TotalSpots = model.Package.TotalSpots;
                isUpdated = true;
            }
            if (package.StartDate != model.Package.StartDate)
            {
                package.StartDate = model.Package.StartDate;
                isUpdated = true;
            }
            if (package.EndDate != model.Package.EndDate)
            {
                package.EndDate = model.Package.EndDate;
                isUpdated = true;
            }
            if (package.PackageTypeId != model.Package.PackageTypeId)
            {
                package.PackageTypeId = model.Package.PackageTypeId;
                isUpdated = true;
            }

            // Save changes only if there are updates
            if (isUpdated)
            {
                try
                {
                    _context.Packages.Update(package);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Package updated successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                TempData["InfoMessage"] = "No changes were made.";
            }

            return RedirectToAction(nameof(ManagePackages));
        }


        //private bool PackageExists(int id)
        //{
        //    return _context.Packages.Any(e => e.PackageId == id);
        //}


        [HttpPost]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var package = await _context.Packages
                .Include(p => p.Bookings) // Assuming "Bookings" is the navigation property for related bookings
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package != null)
            {
                if (package.Bookings != null && package.Bookings.Any())
                {
                    // Option 1: Prevent deletion
                    TempData["ErrorMessage"] = "Package cannot be deleted as it has associated bookings.";
                    return RedirectToAction(nameof(ManagePackages));

                    // Option 2: Uncomment below to cascade delete bookings
                    // _context.Bookings.RemoveRange(package.Bookings);
                }

                _context.Packages.Remove(package);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Package deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Package not found.";
            }

            return RedirectToAction(nameof(ManagePackages));
        }




    }
}

