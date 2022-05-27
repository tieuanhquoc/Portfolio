using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Admin.Controllers;

public class UsersController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;

    public UsersController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _databaseContext.Users.Where(x => x.Role != UserRole.Admin).ToListAsync());
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> UpdateStatus(int userId)
    {
        var user = await _databaseContext.Users.FindAsync(userId);
        if (user == null)
            return RedirectToAction("Index");
        user.Status = user.Status == UserStatus.Disabled ? UserStatus.Enabled : UserStatus.Disabled;
        _databaseContext.Users.Update(user);
        await _databaseContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}