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
    public async Task<IActionResult> Create(UserSkillModel userSkillModel)
    {
        if (ModelState.IsValid)
        {
            var images = await UploadedFile(userSkillModel.Images);

            var userSkill = new UserSkill
            {
                Project = images,
                Information = userSkillModel.Information,
                Skill = userSkillModel.Skill,
                PercentSkill = userSkillModel.PercentSkill,
                Time = userSkillModel.Time,
                ShortTitle = userSkillModel.Time,
                TitleProject = userSkillModel.TitleProject,
                UserId = User.GetId(),
                CreatedAt = DateTime.Now
            };

            await _databaseContext.AddAsync(userSkill);
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction("Index", "UserSkills", new {area = "Member"});
        }

        return View("~/Areas/Member/Views/UserSkills/Create.cshtml", userSkillModel);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userSkill = await _databaseContext.UserSkills.FindAsync(id);
        if (userSkill == null)
        {
            return NotFound();
        }

        return View("~/Areas/Member/Views/UserSkills/Edit.cshtml", new UserSkillModel
        {
            Id = userSkill.Id,
            Information = userSkill.Information,
            Skill = userSkill.Skill,
            PercentSkill = userSkill.PercentSkill,
            Project = userSkill.Project,
            Time = userSkill.Time,
            TitleProject = userSkill.TitleProject,
            ShortTitle = userSkill.ShortTitle,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserSkillModel userSkillModel)
    {
        if (id != userSkillModel.Id)
        {
            return NotFound();
        }

        var userSkill = await _databaseContext.UserSkills.FindAsync(id);
        if (userSkill == null)
            return NotFound();

        if (ModelState.IsValid)
        {
            var images = await UploadedFile(userSkillModel.Images);

            userSkill.Information = userSkillModel.Information;
            userSkill.Skill = userSkillModel.Skill;
            userSkill.PercentSkill = userSkillModel.PercentSkill;
            userSkill.Time = userSkillModel.Time;
            userSkill.ShortTitle = userSkillModel.ShortTitle;
            userSkill.TitleProject = userSkillModel.TitleProject;
            userSkill.Project = string.IsNullOrEmpty(images) ? userSkill.Project : images;

            _databaseContext.UserSkills.Update(userSkill);
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View("~/Areas/Member/Views/UserSkills/Edit.cshtml", new UserSkillModel
        {
            Id = userSkill.Id,
            Information = userSkill.Information,
            Skill = userSkill.Skill,
            PercentSkill = userSkill.PercentSkill,
            Project = userSkill.Project,
            Time = userSkill.Time,
            TitleProject = userSkill.TitleProject,
            ShortTitle = userSkill.ShortTitle,
        });
    }


    // GET: UserSkills/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var userSkill = await _databaseContext.UserSkills.FirstOrDefaultAsync(m =>
            m.Id == id &&
            m.UserId == User.GetId()
        );
        if (userSkill == null)
            return NotFound();
        _databaseContext.UserSkills.Remove(userSkill);
        await _databaseContext.SaveChangesAsync();
        return View("~/Areas/Member/Views/UserSkills/Index.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> IndexSearch()
    {
        string username = HttpContext.Request.Form["username"];
        var users = await _databaseContext.Users.Where(x => x.Username.ToLower().Equals(username.ToLower()))
            .ToListAsync();
        var userIds = users.Select(x => x.Id).ToList();
        var userSkills = await _databaseContext.UserSkills.Where(x => userIds.Contains(x.UserId)).ToListAsync();
        return View("~/Areas/Member/Views/UserSkills/IndexSearch.cshtml", userSkills);
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