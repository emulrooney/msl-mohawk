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
                var emailIdPattern = "^[0-9]+\\@mohawkcollege.ca$";
                var emailIdValidated = Regex.Match(username, emailIdPattern);
                var idPattern = "^[0-9]+$";
                var idValidated = Regex.Match(username, idPattern);
                var namePattern = "^[a-z]+.[a-z0-9]+$";
                var nameValidated = Regex.Match(username, namePattern);

                var accounts = await _userManager.Users.ToListAsync();

                if (emailIdValidated.Success)
                {
                    int ID = int.Parse(username.Replace("@mohawkcollege.ca", ""));
                    var account = accounts.Where(a => a.StudentId == ID).FirstOrDefault();
                    if (account != null) { username = account.Email; }
                }
                else if (idValidated.Success) 
                {
                    int ID = int.Parse(username);
                    var account = accounts.Where(a => a.StudentId == ID).FirstOrDefault();
                    if (account != null) { username = account.Email; }
                }
                else if (nameValidated.Success)
                {
                    username += "@mohawkcollege.ca";
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
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
