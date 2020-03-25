using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MSL_APP.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Security.Claims;
using MSL_APP.Utility;
using SendGrid;

namespace MSL_APP.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Mohawk ID / Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Check user's input is either email, name, ID, or ID + email suffix
                var username = Input.Email.Trim().ToLower();
                var emailPattern = "^[a-z]+.[a-z0-9]+\\@mohawkcollege.ca$";
                var emailValidated = Regex.Match(username, emailPattern);
                var emailIdPattern = "^[0-9]+\\@mohawkcollege.ca$";
                var emailIdValidated = Regex.Match(username, emailIdPattern);
                var idPattern = "^[0-9]+$";
                var idValidated = Regex.Match(username, idPattern);
                var namePattern = "^[a-z]+.[a-z0-9]+$";
                var nameValidated = Regex.Match(username, namePattern);

                var accounts = await _userManager.Users.ToListAsync();
                // Username input is full email address. ex. firstname.lastname@mohawkcollege.ca
                if (emailValidated.Success)
                {
                    var account = accounts.Where(a => a.Email == username).FirstOrDefault();
                    if (account == null)
                    {
                        ModelState.AddModelError(string.Empty, "Account does not exists. Please register a new account.");
                        return Page();
                    }
                }
                // Username input is ID + mohawk suffix. ex. 000101010@mohawkcollege.ca
                else if (emailIdValidated.Success)
                {
                    int ID = int.Parse(username.Replace("@mohawkcollege.ca", ""));
                    var account = accounts.Where(a => a.StudentId == ID).FirstOrDefault();
                    if (account != null) { username = account.Email; }
                    else {
                        ModelState.AddModelError(string.Empty, "Account does not exists. Please register a new account.");
                        return Page();
                    }
                }
                // username input is ID. ex. 000101010
                else if (idValidated.Success)
                {
                    int ID = int.Parse(username);
                    var account = accounts.Where(a => a.StudentId == ID).FirstOrDefault();
                    if (account != null) { username = account.Email; }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Account does not exist. Please register a new account.");
                        return Page();
                    }
                }
                // username input is firstname.lastname
                else if (nameValidated.Success)
                {
                    username += "@mohawkcollege.ca";
                    var account = accounts.Where(a => a.Email == username).FirstOrDefault();
                    if (account != null) { username = account.Email; }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Account does not exist. Please register a new account.");
                        return Page();
                    }
                }
                // for customized admin accounts that already exists in the database
                else if (accounts.Any(a => a.Email.ToLower() == username)) 
                {
                    // do nothing
                }
                // Invalid username format
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username format. Please use Mohawk ID or Email to login.");
                    return Page();
                }

              
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(username, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    // check the user is banned or not
                    var accountList = await _userManager.Users.ToListAsync();
                    foreach (var user in accountList)
                    {
                        if (user.Email.ToLower() == username.ToLower())
                        {
                            //Check if email is confirmed. If not, don't let them in but prompt
                            //to send a new email.
                            if (!user.EmailConfirmed)
                            {
                                ViewData["UserEmail"] = user.Email;
                                ViewData["EmailUnconfirmed"] = true;
                                await _signInManager.SignOutAsync();
                                return Page();
                            }



                            if (user.ActiveStatus == "Disabled")
                            {
                                await _signInManager.SignOutAsync();
                                _logger.LogInformation("User logged out.");
                                ModelState.AddModelError(string.Empty, "sorry, your account has been locked. please contact the administrator for permission.");
                                return Page();
                            }

                            else if (user.ActiveStatus == "Actived") {
                                _logger.LogInformation("User logged in.");

                                var roles = await _userManager.GetRolesAsync(user);
                                foreach (var role in roles)
                                {
                                    if (role == "Admin")
                                    {
                                        returnUrl = Url.Content("~/Home/Admin");
                                        break;
                                    }
                                    else if (role == "Student")
                                    {
                                        returnUrl = Url.Content("~/Home/Student");
                                        break;
                                    }
                                }
                                return LocalRedirect(returnUrl);
                            }
                        }
                    }
                    return Page();
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Wrong password. Try again or click Forgot password to reset it.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

    }
}
