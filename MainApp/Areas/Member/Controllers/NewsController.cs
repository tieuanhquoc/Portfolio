using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using BuildingBlocks.Helpers;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUglify;

namespace MainApp.Areas.Member.Controllers;

public class NewsController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    [ActivatorUtilitiesConstructor]
    public NewsController(DatabaseContext databaseContext, IWebHostEnvironment webHostEnvironment)
    {
        _databaseContext = databaseContext;
        _webHostEnvironment = webHostEnvironment;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var news = await _databaseContext.News.ToListAsync();
        return View("~/Areas/Member/Views/News/Index.cshtml", news);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("~/Areas/Member/Views/News/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewModel newModel)
    {
        if (!Uri.IsWellFormedUriString(newModel.Source, UriKind.Absolute))
            return NotFound();

        var client = new HttpClient();
        var html = await client.GetStringAsync(newModel.Source);

        var result = Uglify.HtmlToText(html);

        var images = await UploadedFile(newModel.Images);
        var newEntity = new New
        {
            Title = newModel.Title,
            Content = result.Code,
            Images = images,
            Source = newModel.Source
        };
        await _databaseContext.News.AddAsync(newEntity);
        await _databaseContext.SaveChangesAsync();

        return View("~/Areas/Member/Views/News/Create.cshtml", newModel);
    }

    [HttpPost]
    public async Task<IActionResult> Crawl(NewModel newModel)
    {
        if (!Uri.IsWellFormedUriString(newModel.Source, UriKind.Absolute))
            return NotFound();

        var client = new HttpClient();
        var html = await client.GetStringAsync(newModel.Source);
        html.RemoveHtml();
        return View("~/Areas/Member/Views/News/Create.cshtml", new NewModel
        {
            Content = html
        });
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