using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSL_APP.Data;
using MSL_APP.Models;

namespace MSL_APP.Controllers
{   
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager; 

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> RoleManager)
        {
            _userManager = userManager;
            _roleManager = RoleManager;
        }

        //Seed the database with users, roles and assign users to roles
        public async Task<IActionResult> SeedUserData()
        {
            //Variable to hold the status of our identity operations
            IdentityResult result;

            //Create 2 new roles (Customer, Admin)
            result = await _roleManager.CreateAsync(new IdentityRole("Student"));
            if (!result.Succeeded)
                return View("Error", new ErrorViewModel { RequestId = "Failed to add Student role" });

            result = await _roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!result.Succeeded)
                return View("Error", new ErrorViewModel { RequestId = "Failed to add Admin role" });


            //Create a list of customers
            List<ApplicationUser> CustomersList = new List<ApplicationUser>();

            //Sample bank clients
            CustomersList.Add(new ApplicationUser
            {
                Email = "studentTest1@email.com",
                UserName = "studentTest1@email.com"
            });

            foreach (ApplicationUser cust in CustomersList)
            {
                //Create the new user
                result = await _userManager.CreateAsync(cust, "Mohawk1!");
                if (!result.Succeeded)
                    return View("Error", new ErrorViewModel { RequestId = "Failed to add new user" });
                //Assign the new user to the customer role
                result = await _userManager.AddToRoleAsync(cust, "Student");
                if (!result.Succeeded)
                    return View("Error", new ErrorViewModel { RequestId = "Failed to assign customer role" });

            }

            //Create a list of admins
            List<ApplicationUser> AdminsList = new List<ApplicationUser>();

            //Sample bank admins
            AdminsList.Add(new ApplicationUser
            {
                Email = "Admin@email.com",
                UserName = "Admin@email.com"
            });


            foreach (ApplicationUser adm in AdminsList)
            {
                //Create the new user
                result = await _userManager.CreateAsync(adm, "Mohawk1!");
                if (!result.Succeeded)
                    return View("Error", new ErrorViewModel { RequestId = "Failed to add new admin user" });
                //Assign the new user to the customer role
                result = await _userManager.AddToRoleAsync(adm, "Admin");
                if (!result.Succeeded)
                    return View("Error", new ErrorViewModel { RequestId = "Failed to assign admin role" });

            }



            //If we are here, everything executed according to plan, so we will show a success message
            return Content("Users setup completed");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}