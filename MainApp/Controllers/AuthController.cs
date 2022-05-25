using System.Security.Claims;
using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using MainApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Controllers;

public class AuthController : Controller
{
    private readonly DatabaseContext _databaseContext;

    [ActivatorUtilitiesConstructor]
    public AuthController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpGet]
    public async Task<IActionResult> Login(ResponseMessage responseMessage = null)
    {
        if (User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) != null)
            await HttpContext.SignOutAsync();

        if (responseMessage != null && responseMessage.Status != ResponseStatus.None)
        {
            ViewData["ResponseMessage"] = responseMessage.Message;
            ViewData["ResponseStatus"] = responseMessage.Status.ToString();
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        if (ModelState.IsValid)
        {
            await HttpContext.SignOutAsync();
            var user = await _databaseContext.Users.FirstOrDefaultAsync(x =>
                x.Username == loginModel.UserName &&
                x.Status == UserStatus.Enabled
            );

            if (user == null)
                return RedirectToAction(
                    "Login",
                    "Auth",
                    ResponseMessage.Error("Tên đăng nhập hoặc mật khẩu không chính xác")
                );

            var passwordHash = new PasswordHasher<User>();
            if (passwordHash.VerifyHashedPassword(new User(), user.Password, loginModel.Password) ==
                PasswordVerificationResult.Failed)
            {
                return RedirectToAction(
                    "Login",
                    "Auth",
                    ResponseMessage.Error("Tên đăng nhập hoặc mật khẩu không chính xác")
                );
            }


            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Id", user.Id.ToString()),
                        new Claim("Name", user.Username),
                        new Claim("Email", user.Email),
                        new Claim("Phone", user.Phone),
                        new Claim("Avatar", user.Avatar),
                        new Claim("Role", user.Role.ToString()),
                        new Claim("About", user.AboutMe),
                        new Claim("Address", user.Adress),
                        new Claim(ClaimTypes.Role, user.Role.ToString()),
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
            );

            await HttpContext.SignInAsync(principal);
            if (user.Role == UserRole.Member)
                return RedirectToAction("Index", "Home");
            else if (user.Role == UserRole.Admin)
            {
                return RedirectToAction("Index", "Home", new {area = "Admin"});
            }
            else
            {
                return View();
            }
        }

        return View(loginModel);
    }

    [HttpGet]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }
}