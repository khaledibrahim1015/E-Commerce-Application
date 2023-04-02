using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using E_Project_.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace E_Project_.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        //
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        // act as ViewModel 
        public class InputModel
        {//
            [Required(ErrorMessage ="Full Name is Required ")]
            [Display(Name = "Full Name")]
            public string FullName { set; get; }
            [Required(ErrorMessage = "Phone Number is Required ")]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { set; get; }




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
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        // On get Method will check if two Role Exist Or Not If not Exist THEN cRAETE THEM 
        public async Task OnGetAsync(string returnUrl = null)
        {
            // Execcute only Fisrt Time
            if(! await _roleManager.RoleExistsAsync(WC.AdminRole))
            {
              await   _roleManager.CreateAsync(new IdentityRole(WC.AdminRole));
               await _roleManager.CreateAsync(new IdentityRole(WC.CustomerRole));

            }

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //create user 
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email ,FullName=Input.FullName,PhoneNumber=Input.PhoneNumber};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // I create user sucess

                    // Assign user to role admin  in first to create admin user 

                    await _userManager.AddToRoleAsync(user, WC.AdminRole);


                    // then

                    //if(User.IsInRole(WC.AdminRole))
                    //{
                    //    await _userManager.AddToRoleAsync(user, WC.AdminRole);
                    //}
                    //else
                    //{
                    //    await _userManager.AddToRoleAsync(user, WC.CustomerRole);
                    //}


                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // when an admin create admin account the newaccount that create logged in => we have to change behaviour 
                        if(!User.IsInRole(WC.AdminRole) )// that mean the user has already  looged in
                         {
                            // // if the user in not in admin role then sign in user 
                            // in this case we dont want to sign in the user 
                            await _signInManager.SignInAsync(user, isPersistent: false);

                        }
                        else
                        {
                            // if user that logged in an admin user and try to create an admin user => 
                            return RedirectToAction("Index");
                        }
                         
                      
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
