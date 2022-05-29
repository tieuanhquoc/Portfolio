using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using BuildingBlocks.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Member.Controllers
{
    public class CommentsController : ApiBaseController
    {
        private readonly DatabaseContext _databaseContext;

        [ActivatorUtilitiesConstructor]
        public CommentsController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _databaseContext.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        [HttpPost("{postId:int}")]
        public async Task<ActionResult> Create(int postId)
        {
            var post = await _databaseContext.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                return new NotFoundResult();
            string content = HttpContext.Request.Form["content"];
            var comment = new Comment
            {
                PostId = postId,
                Content = content,
                Status = CommentStatus.Enabled,
                UserId = User.GetId(),
                CreatedAt = DateTime.Now,
            };

            await _databaseContext.Comments.AddAsync(comment);
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction("Index", "Posts", new {area = "Member"});
        }


        // // GET: Comments/Delete/5
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

            return RedirectToAction("Index", "Posts", new {area = "Member"});
        }

        //
        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _databaseContext.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();
            _databaseContext.Comments.Remove(comment);
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction("Index", "Posts", new {area = "Member"});
        }
    }
}