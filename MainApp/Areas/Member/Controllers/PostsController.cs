using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using BuildingBlocks.Extensions;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Member.Controllers;

public class PostsController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    [ActivatorUtilitiesConstructor]
    public PostsController(DatabaseContext databaseContext, IWebHostEnvironment webHostEnvironment)
    {
        _databaseContext = databaseContext;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _databaseContext.Posts.Include(p => p.User).ToListAsync();
        var userId = User.GetId();
        var userDetails = await _databaseContext.Users.FindAsync(userId);
        ViewData["User"] = userDetails;
        ViewData["alluser"] = await _databaseContext.Users.Where(x => x.Role == UserRole.Member).ToListAsync();
        ViewData["Comment"] = await _databaseContext.Comments.Include(c => c.Post).Include(c => c.User).ToListAsync();
        return View("~/Areas/Member/Views/Posts/Index.cshtml", posts);
    }

    public IActionResult Create()
    {
        return View("~/Areas/Member/Views/Posts/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostModel postModel)
    {
        var userId = User.GetId();
        if (ModelState.IsValid)
        {
            var images = await UploadedFile(postModel.Images);
            var post = new Post
            {
                Note = postModel.Note,
                Title = postModel.Title,
                Like = 0,
                Status = PostStatus.Enabled,
                CreatedAt = DateTime.Now,
                UrlImages = images,
                UserId = userId
            };
            

            await _databaseContext.Posts.AddAsync(post);
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View("~/Areas/Member/Views/Posts/Create.cshtml", postModel);
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