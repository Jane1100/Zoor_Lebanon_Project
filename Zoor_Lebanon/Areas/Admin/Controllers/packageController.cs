﻿using Microsoft.AspNetCore.Mvc;
      

        // GET: Add Package
        [HttpGet]

        // POST: Add Package with Location
        [HttpPost]
            // Validate dates: Ensure start_date is today or later
            if (model.Package.StartDate.HasValue && model.Package.StartDate.Value < DateOnly.FromDateTime(DateTime.Today))
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











        // GET: Manage Packages
        /* [HttpGet]

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
                return RedirectToAction(nameof(Widgets));
            }

            return View(package);
        }


        [HttpGet]
        public async Task<IActionResult> editpackages(int id)
        {
            var package = await _context.Packages
                .Include(p => p.PackageType)
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package == null)
            {
                return NotFound();
            }

            var states = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
            var cities = await _context.Locations.Where(l => l.State == package.Location.State).Select(l => l.City).Distinct().ToListAsync();

            var viewModel = new PackageViewModel
            {
                Package = package,
                PackageTypes = await _context.PackageTypes.ToListAsync(),
                States = states,
                Cities = cities // Ensure cities are loaded based on the package's current state
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package != null)
            {
                _context.Packages.Remove(package);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Package deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Package not found.";
            }

            return RedirectToAction(nameof(Widgets));
        }




        [HttpPost]
        public async Task<IActionResult> editpackages(int id, PackageViewModel model, string state, string city)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Package not found.";
                return RedirectToAction(nameof(Widgets));
            }

            // Date validation
            if (model.Package.StartDate.HasValue && model.Package.StartDate.Value < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("Package.StartDate", "Start date cannot be in the past.");
            }

            if (model.Package.EndDate.HasValue && model.Package.EndDate.Value <= model.Package.StartDate.Value)
            {
                ModelState.AddModelError("Package.EndDate", "End date must be after the start date.");
            }

            // Fetch LocationId based on selected State and City
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.State == state && l.City == city);
            if (location == null)
            {
                ModelState.AddModelError("", "Invalid Location selection.");
            }

            if (!ModelState.IsValid)
            {
                model.PackageTypes = await _context.PackageTypes.ToListAsync();
                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
                return View(model);
            }

            // Update the package details
            package.PackageName = model.Package.PackageName;
            package.Description = model.Package.Description;
            package.UnitPrice = model.Package.UnitPrice;
            package.TotalSpots = model.Package.TotalSpots;
            package.StartDate = model.Package.StartDate;
            package.EndDate = model.Package.EndDate;
            package.LocationId = location.LocationId;
            package.PackageTypeId = model.Package.PackageTypeId;


            _context.Packages.Update(package);
            await _context.SaveChangesAsync();

            return RedirectToAction("widgets", "package", new { area = "admin" });
        }


        // GET: Package Details
        // GET: Package Details
        /*    [HttpGet]
            public async Task<IActionResult> pdetails(int id)
            {
                var package = await _context.Packages
                    .Include(p => p.Location)
                    .Include(p => p.PackageType)
                    .FirstOrDefaultAsync(p => p.PackageId == id);

                if (package == null)
                {
                    TempData["ErrorMessage"] = "Package not found.";
                    return RedirectToAction(nameof(Widgets));
                }

                return View(package);
            }
    */


        // POST: Delete Package
      /*  [HttpPost]