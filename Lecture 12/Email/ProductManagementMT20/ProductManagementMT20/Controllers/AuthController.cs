using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagementMT20.Models.Entities;
using ProductManagementMT20.Models.VM;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProductManagementMT20.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Generate email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Create the confirmation link
                    var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, Request.Scheme);

                    // Send the email (you need to implement this)
                    SendConfirmationEmail(user.Email, confirmationLink);

                    // Redirect to a page informing the user to check their email
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine($"Error: {error.Code} - {error.Description}"); // Logs errors in the console
                }
            }
            return View(model);
        }
        private async Task SendConfirmationEmail(string email, string confirmationLink)
        {
            var apiKey = "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"; // Replace with your actual SendGrid API key
            var client = new SendGridClient(apiKey);

            // Use an email address from your custom domain
            var from = new EmailAddress("ekaterine_tchonishvili@mziuri.ge", "ProductManagement_EmailService");
            var to = new EmailAddress(email);
            var subject = "Confirm your email";
            var plainTextContent = $"Please confirm your email by clicking this link: {confirmationLink}";
            var htmlContent = $"<a href='{confirmationLink}'>Confirm Email</a>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
            Console.WriteLine($"SendGrid Response Status: {response.StatusCode}");
            if (!response.StatusCode.ToString().StartsWith("2"))
            {
                Console.WriteLine($"SendGrid Response Body: {await response.Body.ReadAsStringAsync()}");
            }
        }



        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var model = new ConfirmedViewModel();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                model.IsSuccess = false;
                model.Message = "Invalid confirmation request.";
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                model.IsSuccess = false;
                model.Message = "User not found.";
                return View(model);
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                model.IsSuccess = true;
                model.Message = "Your email has been successfully confirmed!";
            }
            else
            {
                model.IsSuccess = false;
                model.Message = "Email confirmation failed. The token may be invalid or expired.";
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your account is locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Email confirmation required.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }

                // Log the result for debugging
                Console.WriteLine($"Login result: {result}");
            }

            return View(model);
        }
    }
}
