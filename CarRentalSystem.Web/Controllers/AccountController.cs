using AutoMapper;
using CarRentalSystem.Application.Contracts.Account;
using CarRentalSystem.Application.Features.Account.Commands.ExternalLogin;
using CarRentalSystem.Application.Features.Account.Commands.RegisterCustomer;
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
                return RedirectToAction("Login", new { error = $"Error from provider: {remoteError}" });

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

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

            return result ? RedirectToAction("CompleteProfile") : RedirectToAction("Login", new { error = "External login failed." });
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

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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

    }
}
