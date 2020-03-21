using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSL_APP.Data;
using MSL_APP.Models;

namespace MSL_APP.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        // Use to read the eligible table in order to let student to register an account
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            // declare the class variable for the database context and set it in the constructor
            ApplicationDbContext context) : base() 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Home/Student");
            if (ModelState.IsValid)
            {
                bool eligibleUser = false;
                string firstName = "";
                string lastName = "";
                int studentId = 0;
                //int eligibleId = 0;

                // Check user's email is in the eligible list or not
                var eligibleStudent = await _context.EligibleStudent.ToListAsync();
                foreach (EligibleStudent student in eligibleStudent) {
                    if (student.StudentEmail == Input.Email) {
                        eligibleUser = true;
                        firstName = student.FirstName;
                        lastName = student.LastName;
                        studentId = student.StudentID;
                        //eligibleId = student.Id;
                        break;
                    }
                }

                if (eligibleUser)
                {
                    var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, FirstName = firstName, LastName = lastName, StudentId = studentId, ActiveStatus="Actived", Role="Student" };

                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        //Assign the new user to the student role
                        var addRole = await _userManager.AddToRoleAsync(user, "Student");

                        if (addRole.Succeeded) 
                        {
                            // Write log for get key action
                            Logs newLog = new Logs()
                            {
                                StudentId = studentId,
                                StudentEmail = Input.Email,
                                Action = "Register",
                                Product = "",
                                ProductKey = "",
                            };
                            _context.Add(newLog);
                            _context.SaveChanges();

                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { userId = user.Id, code = code },
                                protocol: Request.Scheme);

                            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        foreach (var error in addRole.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else {
                    ModelState.AddModelError(string.Empty, "Sorry, your e-mail address is not eligible to create an account. Please contact the administrator for permission.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
