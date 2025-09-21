using AutoMapper;
using CarRentalSystem.Application.Contracts.Account;
using CarRentalSystem.Application.Contracts.OTP;
using CarRentalSystem.Application.Features.Account.Commands.ExternalLogin;
using CarRentalSystem.Application.Features.Account.Commands.RegisterCustomer;
using CarRentalSystem.Application.Features.Account.Commands.RegisterCustomerWithOTP;
using CarRentalSystem.Application.Features.OTP.Commands.SendRegistrationOTP;
using CarRentalSystem.Application.Features.OTP.Commands.VerifyRegistrationOTP;
using CarRentalSystem.Infrastructure.Identity;
using CarRentalSystem.Web.ViewModels.Account;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalSystem.Web.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IMediator mediator, IMapper mapper, SignInManager<ApplicationUser> signInManager)
        {
            _mediator = mediator;
            _mapper = mapper;
            _signInManager = signInManager;
        }
        [HttpPost("RegisterCustomerAjax")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCustomerAjax([FromForm] RegisterCustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { success = false, errors });
            }

            var command = _mapper.Map<RegisterCustomerCommand>(model);

            var result = await _mediator.Send(command);

            if (result)
            {
                
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var createdUser = await _signInManager.UserManager.FindByEmailAsync(model.Email);
                if (createdUser != null)
                {
                    await _signInManager.SignInAsync(createdUser, isPersistent: false);
                }

                return Ok(new { success = true });
            }

            return BadRequest(new { success = false, errors = new[] { "Registration failed (User may already exist or password is weak)." } });
        }
        [HttpGet("ExternalLogin")]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
                return RedirectToAction("Index", "Home", new { error = $"Error from provider: {remoteError}" });

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Index", "Home", new { error = "External login failed. Please try again." });

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            var externalUser = new ExternalUserModel
            {
                Email = email,
                FullName = name,
                ProviderKey = info.ProviderKey,
                LoginProvider = info.LoginProvider
            };

            var command = new ExternalLoginCommand { User = externalUser };
            var result = await _mediator.Send(command);

            return result ? RedirectToAction("Index", "Home") : RedirectToAction("Index", "Home", new { error = "External login failed. Please try again." });
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Try to find the user first
            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Check if user has a password
            var hasPassword = await _signInManager.UserManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                ModelState.AddModelError(string.Empty, "No password set for this account. Please set a password first.");
                return View(model);
            }

            // Try password sign in
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (user != null && await _signInManager.UserManager.IsInRoleAsync(user, "Admin"))
                {
                    return LocalRedirect(returnUrl ?? Url.Action("Index", "Admin"));
                }
                else
                {
                    return LocalRedirect(returnUrl ?? Url.Action("Index", "Dashboard"));
                }
            }

            // Debug information
            var debugInfo = $"User found: {user.Email}, EmailConfirmed: {user.EmailConfirmed}, HasPassword: {hasPassword}";
            ModelState.AddModelError(string.Empty, $"Invalid login attempt. Debug: {debugInfo}, Result: {result}");
            return View(model);
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Admin/Login")]
        public IActionResult AdminLogin(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Admin/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
                if (user != null && await _signInManager.UserManager.IsInRoleAsync(user, "Admin"))
                {
                    return LocalRedirect(returnUrl ?? Url.Action("Index", "Admin"));
                }
                else
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Access denied. Admin privileges required.");
                    return View(model);
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost("SendRegistrationOTP")]
        public async Task<IActionResult> SendRegistrationOTP([FromBody] OTPRequest request)
        {
            try
            {
                // Log the incoming request for debugging
                Console.WriteLine($"OTP Request received - Email: {request?.Email}, Purpose: {request?.Purpose}");

                if (request == null)
                {
                    Console.WriteLine("Request is null");
                    return BadRequest(new { success = false, message = "Request body is null" });
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    Console.WriteLine("Email is null or empty");
                    return BadRequest(new { success = false, message = "Email is required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    Console.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                    return BadRequest(new { success = false, message = "Invalid request", errors = errors });
                }

                var command = new SendRegistrationOTPCommand
                {
                    Email = request.Email
                };

                Console.WriteLine($"Sending OTP command for email: {request.Email}");
                var result = await _mediator.Send(command);
                Console.WriteLine($"OTP result: Success={result.Success}, Message={result.Message}");
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SendRegistrationOTP: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("VerifyRegistrationOTP")]
        public async Task<IActionResult> VerifyRegistrationOTP([FromBody] OTPVerificationRequest request)
        {
            try
            {
                Console.WriteLine($"OTP Verification Request received - Email: {request?.Email}, Code: {request?.Code}, Purpose: {request?.Purpose}");

                if (request == null)
                {
                    Console.WriteLine("Verification request is null");
                    return BadRequest(new { success = false, message = "Request body is null" });
                }

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
                {
                    Console.WriteLine("Email or Code is null or empty");
                    return BadRequest(new { success = false, message = "Email and Code are required" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    Console.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                    return BadRequest(new { success = false, message = "Invalid request", errors = errors });
                }

                var command = new VerifyRegistrationOTPCommand
                {
                    Email = request.Email,
                    Code = request.Code,
                    Purpose = request.Purpose ?? "Registration"
                };

                Console.WriteLine($"Sending OTP verification command for email: {request.Email}");
                var result = await _mediator.Send(command);
                Console.WriteLine($"OTP verification result: {result}");
                
                return Ok(new { success = result, message = result ? "OTP verified successfully" : "Invalid OTP" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in VerifyRegistrationOTP: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("RegisterCustomerWithOTPAjax")]
        public async Task<IActionResult> RegisterCustomerWithOTPAjax([FromForm] RegisterCustomerWithOTPViewModel model)
        {
            try
            {
                Console.WriteLine($"Registration Request received - Email: {model?.Email}, FullName: {model?.FullName}");

                if (model == null)
                {
                    Console.WriteLine("Registration model is null");
                    return BadRequest(new { success = false, errors = new[] { "Registration data is required" } });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    Console.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                    return BadRequest(new { success = false, errors });
                }

                Console.WriteLine($"Mapping registration command for email: {model.Email}");
                var command = _mapper.Map<RegisterCustomerWithOTPCommand>(model);

                Console.WriteLine($"Sending registration command for email: {model.Email}");
                var result = await _mediator.Send(command);
                Console.WriteLine($"Registration result: {result}");

                if (result)
                {
                    Console.WriteLine($"Registration successful for email: {model.Email}");
                    var createdUser = await _signInManager.UserManager.FindByEmailAsync(model.Email);
                    if (createdUser != null)
                    {
                        Console.WriteLine($"Signing in user: {model.Email}");
                        await _signInManager.SignInAsync(createdUser, isPersistent: false);
                    }

                    return Ok(new { success = true });
                }

                Console.WriteLine($"Registration failed for email: {model.Email}");
                return BadRequest(new { success = false, errors = new[] { "Registration failed. Please check your OTP code and try again." } });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RegisterCustomerWithOTPAjax: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { success = false, errors = new[] { $"Registration failed: {ex.Message}" } });
            }
        }

    }
}
