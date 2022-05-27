using System.Security.Claims;
using BuildingBlocks.Data;
using BuildingBlocks.Extensions;
using MainApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Areas.Member.Controllers;

public class UsersController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    [ActivatorUtilitiesConstructor]
    public UsersController(DatabaseContext databaseContext, IWebHostEnvironment webHostEnvironment)
    {
        _databaseContext = databaseContext;
        _webHostEnvironment = webHostEnvironment;
    }


    public async Task<IActionResult> Edit()
    {
        var user = await _databaseContext.Users.FindAsync(User.GetId());
        if (user == null)
        {
            return NotFound();
        }

        return View(new UserModels
        {
            Id = user.Id,
            Username = user.Username,
            Avatar = user.Avatar,
            Email = user.Email,
            Phone = user.Phone,
            AboutMe = user.AboutMe,
            Address = user.Address,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserModels userModels)
    {
        if (id != User.GetId())
            return NotFound();

        if (ModelState.IsValid)
        {
            var user = await _databaseContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var images = await UploadedFile(userModels.Images);
            user.Email = userModels.Email;
            user.Phone = userModels.Phone;
            user.AboutMe = userModels.AboutMe;
            user.Address = userModels.Address;
            user.Avatar = string.IsNullOrEmpty(images) ? user.Avatar : images;
            userModels.Avatar = user.Avatar;


            await HttpContext.SignOutAsync();
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
                        new Claim("Address", user.Address),
                        new Claim(ClaimTypes.Role, user.Role.ToString()),
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
            );

            await HttpContext.SignInAsync(principal);

            _databaseContext.Users.Update(user);
            await _databaseContext.SaveChangesAsync();

            return View(userModels);
        }

        return View(userModels);
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