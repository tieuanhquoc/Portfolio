using BuildingBlocks.Data.Entities;

namespace MainApp.Models;

public class NewsModel : News
{
    public List<IFormFile> Images { get; set; }
}