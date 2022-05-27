using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using BuildingBlocks.Extensions;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Admin.Controllers;

public class PostsController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;

    [ActivatorUtilitiesConstructor]
    public PostsController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _databaseContext.Posts.Include(p => p.User).ToListAsync();
        return View(posts);
    }


    [HttpGet("{postId:int}"), ActionName("Delete")]
    public async Task<IActionResult> Delete(int postId)
    {
        var post = await _databaseContext.Posts.FirstOrDefaultAsync(m => m.Id == postId);
        if (post == null)
        {
            return RedirectToAction(nameof(Index));
        }

        _databaseContext.Posts.Remove(post);
        await _databaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}