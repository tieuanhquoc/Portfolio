using BuildingBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Member.Controllers
{
    public class CommentsController : Controller
    {
        private readonly DatabaseContext _databaseContext;

        public CommentsController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var comments = await _databaseContext.Comments.Include(c => c.Post).Include(c => c.User).ToListAsync();
            return View("~/Areas/Member/Views/Comment/Index.cshtml", comments);
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

            return View("~/Areas/Member/Views/Comment/Details.cshtml", comment);
        }

        // [HttpPost]
        // public ActionResult Create(int? id)
        // {
        //     var user = HttpContext.Session.GetString("user");
        //     Comment comment = new Comment();
        //     String cmt = HttpContext.Request.Form["cmt"];
        //     comment.PostId = id;
        //     comment.Comment1 = cmt;
        //     comment.Status = 1;
        //     comment.UserId = Convert.ToInt32(user);
        //     _context.Add(comment);
        //     _context.SaveChanges();
        //     return RedirectToAction("Index", "Posts");
        //
        // }
        //
        //
        // // GET: Comments/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var comment = await _context.Comments.FindAsync(id);
        //     if (comment == null)
        //     {
        //         return NotFound();
        //     }
        //     ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comment.PostId);
        //     ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", comment.UserId);
        //     return View(comment);
        // }
        //
        // // POST: Comments/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("Id,Comment1,UserId,PostId,Status")] Comment comment)
        // {
        //     if (id != comment.Id)
        //     {
        //         return NotFound();
        //     }
        //
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(comment);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!CommentExists(comment.Id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comment.PostId);
        //     ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", comment.UserId);
        //     return View(comment);
        // }
        //
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
        // // POST: Comments/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var comment = await _context.Comments.FindAsync(id);
        //     _context.Comments.Remove(comment);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }
        //
        // private bool CommentExists(int id)
        // {
        //     return _context.Comments.Any(e => e.Id == id);
        // }
    }
}