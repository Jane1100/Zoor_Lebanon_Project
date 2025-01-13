﻿using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models;


namespace Zoor_Lebanon.Areas.admin.Controllers


        /*  public IActionResult Index2()
          {
              // Fetch packages with related data, you might want to adjust the query based on your needs
              var packages = _context.Packages
                  .Select(p => new
                  {
                      PackageId = p.PackageId,
                      PackageName = p.PackageName,
                      Status = p.Status, // Assuming Status is a calculated field or stored directly in the Package table
                      Popularity = p.Bookings.Count() // Example metric: counting bookings as popularity
                  })
                  .ToList();

              // Convert the anonymous type to a view model if necessary or pass directly
              var viewModel = new PackagesViewModel
              {
                  Packages = packages
              };

              return View(viewModel);
          }

  */
        // GET: Add Package

        // POST: Add Package with Location
        /*[HttpPost]
            // Validate dates: Ensure start_date is today or later
            /*if (model.Package.StartDate.HasValue && model.Package.StartDate.Value < (DateTime.Today))
            {
                ModelState.AddModelError("Package.StartDate", "Start date cannot be in the past.");
                model.PackageTypes = await _context.PackageTypes.ToListAsync();
                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
                return View(model);
            }

            // Ensure end_date is after start_date
            if (model.Package.EndDate.HasValue && model.Package.StartDate.HasValue &&
                model.Package.EndDate.Value <= model.Package.StartDate.Value)
            {
                ModelState.AddModelError("Package.EndDate", "End date must be after the start date.");
                model.PackageTypes = await _context.PackageTypes.ToListAsync();
                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
                return View(model);
            }

            // Fetch LocationId based on selected State and City
            var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.State == state && l.City == city);

            // Create Package Object
            var package = new Package
                PackageTypeId = model.Package.PackageTypeId,// Foreign key for PackageType
                 Status = "Active"
            };
            return RedirectToAction("Index2", "Home", new { area = "admin" });


        [HttpGet]
            var states = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
            _logger.LogInformation("States fetched: " + (states.Count.ToString()));
            var viewModel = new PackageViewModelAdmin
            {
                Package = new Package(),
                PackageTypes = await _context.PackageTypes.ToListAsync(),
                States = states
            };


        [HttpPost]
        public async Task<IActionResult> AddPackage(PackageViewModelAdmin model, string state, string city)
        {
            Console.WriteLine($"Received Package Name: {model.Package.PackageName}");
            _logger.LogInformation($"Received Start Date: {model.Package.StartDate}");
            _logger.LogInformation($"Received End Date: {model.Package.EndDate}");
            Console.WriteLine($"Received State: {state}");
            Console.WriteLine($"Received City: {city}");
           
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.State == state && l.City == city);

            if (location == null)
            {
                ModelState.AddModelError("", "Invalid location selection.");
                return View(model);
            }

            model.Package.LocationId = location.LocationId;  // Ensure the location ID is set

            _context.Packages.Add(model.Package);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index2", "Home", new { area = "admin" });
        }

        // GET: Fetch Cities by State (Ajax)
        [HttpGet]

        // GET: Fetch LocationId (Ajax)
        [HttpGet]

        // GET: View Widgets (Packages)
        [HttpGet]
        public async Task<IActionResult> Widgets()
        {
            var packages = await _context.Packages
                .Include(p => p.Location)
                .Include(p => p.PackageType)
                .Where(p => p.Status == "Active") // Add this line to filter by Active status
                .OrderByDescending(p => p.PackageId)
                .ToListAsync();

            return View(packages); // Ensure the view file is named "Widgets.cshtml"
        }
        // GET: Search Package
        [HttpGet]
        }

        // GET: Package Details
        [HttpGet]
        public async Task<IActionResult> pdetails(int id)
        {
            var package = await _context.Packages
                .Include(p => p.Location)
                .Include(p => p.PackageType)
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package == null)
            {
                TempData["ErrorMessage"] = "Package not found.";
                return RedirectToAction(nameof(editpackages));
            }

            return View(package);
        }
/*
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
        }*/

        [HttpGet("~/Admin/package/EditPackages/{packageId}")]
        public async Task<IActionResult> EditPackages(int? packageId)
        {
            if (packageId == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.Location)
                .Include(p => p.PackageType)
                .FirstOrDefaultAsync(p => p.PackageId == packageId.Value);

            if (package == null)
            {
                return NotFound();
            }

            var viewModel = new PackageViewModelAdmin
            {
                Package = package,
                PackageTypes = await _context.PackageTypes.ToListAsync(),
                States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync(),
                Cities = await _context.Locations.Where(l => l.State == package.Location.State).Select(l => l.City).Distinct().ToListAsync()
            };

            return View(viewModel);
        }




        [HttpPost]
        public async Task<IActionResult> editpackages(int id, PackageViewModelAdmin model)
        {
            // Find the package in the database
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Package not found.";
                return RedirectToAction(nameof(editpackages));
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

            return RedirectToAction(nameof(editpackages));
        }



        [HttpPost]
        public async Task<IActionResult> DeletePackage(int packageId)
        {
            var package = await _context.Packages.FindAsync(packageId);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Package not found.";
                return RedirectToAction(nameof(Widgets));
            }

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Package deleted successfully.";
            return RedirectToAction(nameof(Widgets));
        }



      


        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.PackageId == id);
        }



        // GET: Index showing packages
        public async Task<IActionResult> Index2()
        {
            var packages = await _context.Packages
                .Select(p => new PackageDetail
                {
                    PackageId = p.PackageId,
                    PackageName = p.PackageName,
                    Status = p.Status,
                    Popularity = p.Bookings.Count
                }).ToListAsync();

            var viewModel = new PackagesViewModel
            {
                Packages = packages
            };

            return View(viewModel);
        }






    }