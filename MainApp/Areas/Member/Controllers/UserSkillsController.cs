using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using BuildingBlocks.Extensions;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Member.Controllers;

public class UserSkillsController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    [ActivatorUtilitiesConstructor]
    public UserSkillsController(DatabaseContext databaseContext, IWebHostEnvironment webHostEnvironment)
    {
        _databaseContext = databaseContext;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetId();
        var userSkills = await _databaseContext.UserSkills
            .Where(x => x.UserId == userId)
            .Include(x => x.User).ToListAsync();
        return View("~/Areas/Member/Views/UserSkills/Index.cshtml", userSkills);
    }

    public async Task<IActionResult> Details()
    {
        var userId = User.GetId();
        var userSkills = await _databaseContext.UserSkills
            .Where(x => x.UserId == userId)
            .Include(x => x.User).ToListAsync();
        return View("~/Areas/Member/Views/UserSkills/Details.cshtml", userSkills);
    }

    public IActionResult Create()
    {
        return View("~/Areas/Member/Views/UserSkills/Create.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserSkillCreate skillCreate)
    {
        if (ModelState.IsValid)
        {
            var images = await UploadedFile(skillCreate.Images);

            var userSkill = new UserSkill
            {
                Project = images,
                Information = skillCreate.Information,
                Skill = skillCreate.Skill,
                PercentSkill = skillCreate.PercentSkill,
                Time = skillCreate.Time,
                ShortTitle = skillCreate.Time,
                TitleProject = skillCreate.TitleProject,
                UserId = User.GetId(),
                CreatedAt = DateTime.Now
            };

            await _databaseContext.AddAsync(userSkill);
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction("Index", "UserSkills", new {area = "Member"});
        }

        return View(skillCreate);
    }

    private async Task<string> UploadedFile(List<IFormFile> files)
    {
        var fileNames = new List<string>();
        if (files == null || files.Count == 0)
            return string.Join(",", fileNames);
        foreach (var formFile in files)
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