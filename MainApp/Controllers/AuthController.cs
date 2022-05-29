using System.Security.Claims;
using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using MainApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly DatabaseContext _databaseContext;
    private readonly IWebHostEnvironment _webHostEnvironment;


    [ActivatorUtilitiesConstructor]
    public AuthController(DatabaseContext databaseContext, IWebHostEnvironment webHostEnvironment)
    {
        _databaseContext = databaseContext;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> AccessDenied()
    {
        if (User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) != null)
            await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
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
                        new Claim("Email", user.Email ?? string.Empty),
                        new Claim("Phone", user.Phone ?? string.Empty),
                        new Claim("Avatar", user.Avatar ?? string.Empty),
                        new Claim("Role", user.Role.ToString()),
                        new Claim("About", user.AboutMe ?? string.Empty),
                        new Claim("Address", user.Address ?? string.Empty),
                        new Claim(ClaimTypes.Role, user.Role.ToString()),
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
            );

            await HttpContext.SignInAsync(principal);
            if (user.Role == UserRole.Member)
                return RedirectToAction("Index", "Home", new {area = "Member"});
            else if (user.Role == UserRole.Admin)
            {
                return RedirectToAction("Index", "Users", new {area = "Admin"});
            }
            else
            {
                return View();
            }
        }

        return View(loginModel);
    }


    public IActionResult Register(ResponseMessage responseMessage = null)
    {
        if (responseMessage != null && responseMessage.Status != ResponseStatus.None)
        {
            ViewData["ResponseMessage"] = responseMessage.Message;
            ViewData["ResponseStatus"] = responseMessage.Status.ToString();
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserModels userModels)
    {
        if (ModelState.IsValid)
        {
            if (await _databaseContext.Users.AnyAsync(x => x.Username.Trim() == userModels.Username.Trim()))
                return RedirectToAction(
                    "Register",
                    "Auth",
                    ResponseMessage.Error("Tên đăng nhập đã tồn tại")
                );

            var image = await UploadedFile(userModels.Images);
            userModels.Avatar = image;
            userModels.Username = userModels.Username.Trim();
            var passwordHash = new PasswordHasher<User>();
            userModels.Password = passwordHash.HashPassword(new User(), userModels.Password);
            userModels.Role = UserRole.Member;
            userModels.CreatedAt = DateTime.Now;
            if (!string.IsNullOrEmpty(userModels.Email) && userModels.Email.Contains("IT"))
            {
                userModels.Status = UserStatus.Enabled;
            }
            else
            {
                userModels.Status = UserStatus.Waiting;
            }

            _databaseContext.Add(userModels);
            await _databaseContext.SaveChangesAsync();
            if (userModels.Status == UserStatus.Enabled)
                return RedirectToAction(
                    "Login",
                    "Auth",
                    ResponseMessage.Success("Đăng ký thành công")
                );
            else
            {
                return RedirectToAction(
                    "Login",
                    "Auth",
                    ResponseMessage.Success("Tài khoản của bạn đang được đuyệt vui lòng chờ...")
                );
            }
        }

        return View(userModels);
    }

    [HttpGet]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }


    private async Task<string> UploadedFile(List<IFormFile> files)
    {
        var fileNames = new List<string>();
        if (files == null || files.Count == 0)
            return string.Join(",", fileNames);
        foreach (var formFile in files.Take(1))
        {
            if (formFile.Length > 0)
            {
                // full path to file in temp location
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var fileExtension = Path.GetExtension(formFile.FileName);
                var filename = $"{Guid.NewGuid():N}{fileExtension}";
                var fileNameWithPath = Path.Combine(filePath, filename);
                await using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                await formFile.CopyToAsync(stream);
                fileNames.Add(filename);
            }
        }

        return string.Join(",", fileNames);
    }
}