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
        ViewData["alluser"] = await _databaseContext.Users.Where(x=>x.Role == UserRole.Member).ToListAsync();
        ViewData["Comment"] = await _databaseContext.Comments.Include(c => c.Post).Include(c => c.User).ToListAsync();
        return View("~/Areas/Member/Views/Post/Index.cshtml", posts);
    }
    
    public IActionResult Create()
    {
        return View("~/Areas/Member/Views/Post/Create.cshtml");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostModel postModel)
    {
        var user = HttpContext.Session.GetString("user");
        if (ModelState.IsValid)
        {
            var images = await UploadedFile(postModel.Images);
            postModel.Title = images;
            
            var post= new Post
            {
                
            }
            post.Posttile = uniqueFileName;
            post.Status = 1;
            post.UserId = Convert.ToInt32(user);
            post.Totallike = 0;
            _databaseContext.Add(postModel);
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
        return View(post);
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