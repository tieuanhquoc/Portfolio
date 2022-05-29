using System.Text;
using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using BuildingBlocks.Helpers;
using HtmlAgilityPack;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Admin.Controllers;

public class NewsController : ApiBaseController
{
    private readonly DatabaseContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    [ActivatorUtilitiesConstructor]
    public NewsController(DatabaseContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var news = await _context.News.ToListAsync();
        return View(news);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost("Create")]
    public async Task<IActionResult> Create(NewsModel newsModel)
    {
        if (!string.IsNullOrEmpty(Request.Form["Crawl"].ToString()))
        {
            if (!Uri.IsWellFormedUriString(newsModel.Source, UriKind.Absolute))
                return View();

            var fullPath =
                new Uri("http://newspaper-demo.herokuapp.com/articles/show?url_to_clean=" + newsModel.Source);
            var htmlWeb = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8 //Set UTF8 để hiển thị tiếng Việt
            };

            var document = await htmlWeb.LoadFromWebAsync(fullPath.ToString());
            var title = document.DocumentNode.SelectSingleNode("/html/body/section/div/div/table/tbody/tr[1]/td[2]");
            var content = document.DocumentNode.SelectSingleNode("/html/body/section/div/div/table/tbody/tr[3]/td[2]");


            return View(new NewsModel
            {
                Content = content.InnerText.RemoveHtml(),
                Title = title.InnerText.RemoveHtml(),
                Source = newsModel.Source
            });
        }
        else
        {
            var images = await UploadedFile(newsModel.Images);
            var newEntity = new News
            {
                Title = newsModel.Title,
                Content = newsModel.Content,
                Images = images,
                Source = newsModel.Source
            };
            await _context.News.AddAsync(newEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }

    [HttpGet("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        Console.WriteLine("sdfffffffffffffff");
        var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
        if (news == null)
        {
            return RedirectToAction(nameof(Index));
        }

        _context.News.Remove(news);
        await _context.SaveChangesAsync();
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