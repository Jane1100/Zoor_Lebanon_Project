using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.ViewModels;

namespace Zoor_Lebanon.Controllers
{
    [Area("Admin")]
    public class OperatorController : Controller
    {
        private readonly zoor_lebanonContext context;

        public OperatorController(zoor_lebanonContext context)
        {
            this.context = context;
        }



        [HttpGet]
        public  ActionResult Addtr()
        {
            return View(new TourOperatorViewModel());
        }

        // POST: Admin/Operator/Addtr
        [HttpPost]
        
        public async Task<IActionResult>  Addtr(TourOperatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tourOperator = new TourOperator
                    {
                        CompanyName = model.CompanyName,
                        BusinessPhone = model.BusinessPhone,
                        // Set other properties as needed
                    };

                    // Ensure these lines are active and correctly configured
                    context.TourOperators.Add(tourOperator);
                    context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tour Operator added successfully!";
                    return RedirectToAction("Operators"); // Redirect to the operators list with success message
                }
                catch (Exception ex)
                {
                    // Log the error details here
                    ModelState.AddModelError("", "Failed to add the tour operator. Error: " + ex.Message);
                }
            }

            // If model state is not valid, or an exception was thrown, return to the form with the current model to display validation errors
            return View(model);
        }


        public IActionResult Operators()
        {
            // Fetch TourOperators with their related User and City
            var operators = context.TourOperators
                .Include(o => o.User) // Include User relationship
                .ThenInclude(u => u.City) // Include City relationship
                .Select(o => new
                {
                    o.OperatorId,
                    o.CompanyName,
                    o.BusinessPhone,
                    Firstname = o.User.Firstname,
                    Lastname = o.User.Lastname,
                    CityName = o.User.City.City1,

                })
                .ToList();

            return View(operators);
        }
     

  

        [HttpGet("Admin/Operator/Delete/{operatorId}")]
        public async Task<IActionResult> Delete(int operatorId)
        {
            var tourOperator = await context.TourOperators
                                             .Include(op => op.User) // Include User if you need to display some confirmation or log
                                             .FirstOrDefaultAsync(op => op.OperatorId == operatorId);
            if (tourOperator == null)
            {
                TempData["ErrorMessage"] = "Tour Operator not found.";
                return RedirectToAction("Operators");
            }

            try
            {
                context.TourOperators.Remove(tourOperator);
                await context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tour Operator deleted successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception details here to understand what went wrong
                TempData["ErrorMessage"] = $"Error deleting tour operator: {ex.Message}";
            }

            return RedirectToAction("Operators");
        }

     

        [HttpGet("Admin/Operator/ProfileU/{operatorId}")]
        public async Task<IActionResult> ProfileU(int? operatorId)
        {
            if (!operatorId.HasValue)
            {
                TempData["ErrorMessage"] = "Operator identification is required.";
                return RedirectToAction("Tourists");
            }

            var tourOperator = await context.TourOperators
                                             .Include(op => op.User)
                                             .FirstOrDefaultAsync(op => op.OperatorId == operatorId.Value);

            if (tourOperator == null)
            {
                TempData["ErrorMessage"] = "Tour Operator not found.";
                return RedirectToAction("Tourists");
            }

            return View(tourOperator);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileU(TourOperator model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation Error.";
                return View(model);
            }

            var tourOperator = await context.TourOperators
                                             .Include(op => op.User)
                                             .FirstOrDefaultAsync(op => op.OperatorId == model.OperatorId);

            if (tourOperator == null)
            {
                TempData["ErrorMessage"] = "Tour Operator not found.";
                return RedirectToAction("Tourists");
            }

            // Update Tour Operator details
            tourOperator.CompanyName = model.CompanyName;
            tourOperator.BusinessPhone = model.BusinessPhone;

            // Update User details if present and ensure changes are tracked
          
            try
            {
                context.Update(tourOperator);
                await context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tour Operator profile updated successfully!";
                return RedirectToAction("Tourists", "User", new { area = "Admin" }); // Redirect to the Tourists action in the User controller
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating profile: {ex.Message}";
                return View(model);
            }
        }


    }
}
/*     [HttpGet("Admin/Operator/ProfileU/{operatorId}")]
           public async Task<IActionResult> ProfileU(int operatorId)
           {
               var tourOperator = await _context.TourOperators
                                                .Include(o => o.User)
                                                .FirstOrDefaultAsync(o => o.OperatorId == operatorId);

               if (tourOperator == null)
               {
                   return NotFound("Tour Operator not found");
               }

               return View(tourOperator);
           }*/

/*  [HttpPost]
      public async Task<IActionResult> UpdateProfile(User model)
      {
          if (model == null || model.UserId <= 0)
          {
              TempData["ErrorMessage"] = "Invalid user data.";
              return RedirectToAction("ProfileU", new { userId = model.UserId });
          }

          var user = await _context.Users.FindAsync(model.UserId);

          if (user == null)
          {
              TempData["ErrorMessage"] = "User not found.";
              return RedirectToAction("ProfileU", new { userId = model.UserId });
          }

          try
          {
              // Check if values are updated
              if (user.Firstname != model.Firstname ||
                  user.Lastname != model.Lastname ||
                  user.Email != model.Email ||
                  user.PhoneNumber != model.PhoneNumber ||
                  user.Dob != model.Dob)
              {
                  user.Firstname = model.Firstname;
                  user.Lastname = model.Lastname;
                  user.Email = model.Email;
                  user.PhoneNumber = model.PhoneNumber;
                  user.Dob = model.Dob;

                  _context.Users.Update(user);
                  await _context.SaveChangesAsync();

                  TempData["SuccessMessage"] = "Profile updated successfully.";
              }
              else
              {
                  TempData["InfoMessage"] = "No changes detected.";
              }
          }
          catch (Exception ex)
          {
              TempData["ErrorMessage"] = $"Error updating profile: {ex.Message}";
          }

          return RedirectToAction("ProfileU", new { userId = model.UserId });
      }
*/



/* [HttpGet]
 public async Task<IActionResult> UpcomingBookings()
 {
     var today = DateOnly.FromDateTime(DateTime.Now);

     var bookings = await _context.Bookings
         .Include(b => b.Package)
         .ToListAsync();

     // Separate into Upcoming and Previous Packages
     var allPackages = bookings
         .GroupBy(b => new { b.PackageId, b.Package.PackageName })
         .Select(group => new
         {
             PackageId = group.Key.PackageId,
             PackageName = group.Key.PackageName,
             BookingCount = group.Count(),
             EarliestTravelDate = group.Where(b => b.TravelDate >= today)
                                       .Min(b => b.TravelDate),
             LatestTravelDate = group.Where(b => b.TravelDate < today)
                                     .Max(b => b.TravelDate)
         })
         .ToList();

     // Pass data directly as a model
     return View(allPackages);
 }
*/


/*   public IActionResult Dashboard()
   {
       var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

       return View();
   }
*/


