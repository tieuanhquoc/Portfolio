using BuildingBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Admin.Controllers;

public class CommentsController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;

    [ActivatorUtilitiesConstructor]
    public CommentsController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<IActionResult> Index()
    {
        var comments = await _databaseContext.Comments
            .Include(c => c.Post)
            .Include(c => c.User)
            .ToListAsync();
        return View(comments);
    }

    [Route("Delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var comment = await _databaseContext.Comments.FindAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        _databaseContext.Comments.Remove(comment);
        await _databaseContext.SaveChangesAsync();

        return RedirectToAction("Index", "Posts", new {area = "Admin"});
    }
}