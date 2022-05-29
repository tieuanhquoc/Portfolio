using BuildingBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Member.Controllers;

public class NewsController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;

    [ActivatorUtilitiesConstructor]
    public NewsController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var news = await _databaseContext.News.ToListAsync();
        return View(news);
    }
}