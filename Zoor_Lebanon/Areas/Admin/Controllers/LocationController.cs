using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Zoor_Lebanon.Models;

namespace Zoor_Lebanon.Areas.Admin.Controllers
{
    [Area("Admin")] // Ensure the controller is part of the "Admin" area
    public class LocationController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public LocationController(zoor_lebanonContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
         {
            var location = await _context.Locations
                .Include(l => l.Packages)
                .ToListAsync();

            return View(location);
         }
        public IActionResult ManageLocation()
        {
            var locations = _context.Locations
          .Include(l => l.Packages) // Include related packages
          .Select(l => new
          {
              l.City,
              l.State,
              IsActive = l.Packages.Any(), // Determine if the location is active based on packages
              Packages = l.Packages.Select(p => new { p.PackageId })
          })
          .OrderByDescending(l => l.IsActive) // Order active locations first
          .ToList();

            ViewData["LocationsJson"] = JsonConvert.SerializeObject(locations);
            return View();
        }



        // GET: Admin/Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Locations/Create
        // POST: Admin/Locations/Create (Form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationId,State,City")] Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // POST: Admin/Locations/Create (AJAX call)
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Location location)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingLocation = await _context.Locations.FindAsync(location.LocationId);
                    if (existingLocation == null)
                    {
                        return NotFound();
                    }

                    // Update the location fields
                    existingLocation.City = location.City;
                    existingLocation.State = location.State;

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Location updated successfully!" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Error updating location.", error = ex.Message });
                }
            }
            return BadRequest(new { message = "Invalid data." });
        }


        // GET: Admin/Locations/Edit/5
        /*   public async Task<IActionResult> Edit(int? id)
           {
               if (id == null)
               {
                   return NotFound();
               }

               var location = await _context.Locations.FindAsync(id);
               if (location == null)
               {
                   return NotFound();
               }
               return View(location);
           }*/
        // POST: Admin/Locations/Create (AJAX call)
        [HttpPost]
        public async Task<IActionResult> addnew([FromBody] Location location)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Locations.Add(location);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Location added successfully!" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Error saving location.", error = ex.Message });
                }
            }
            return BadRequest(new { message = "Invalid data." });
        }

        // POST: Admin/Locations/Edit/5
      /*  [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationId,State,City")] Location location)
        {
            if (id != location.LocationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.LocationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }*/
     

        private bool LocationExists(int locationId)
        {
            throw new NotImplementedException();
        }

        // GET: Admin/Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.LocationId == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Admin/Locations/Delete/5
    /*   [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }   

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationId == id);
        }*/
    }
}
