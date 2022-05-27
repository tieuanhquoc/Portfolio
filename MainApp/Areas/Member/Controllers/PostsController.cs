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
        return View(posts);
    }

    public IActionResult Create()
    {
        return View();
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

        return View(postModel);
    }

    public async Task<IActionResult> Like(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var post = await _databaseContext.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        post.Like += 1;
        _databaseContext.Posts.Update(post);
        await _databaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var comments = _databaseContext.Comments.Where(c => c.PostId == id);
        _databaseContext.RemoveRange(comments);
        
        var post = await _databaseContext.Posts
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (post == null)
        {
            return RedirectToAction(nameof(Index));
        }

        _databaseContext.Posts.Remove(post);
        await _databaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
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