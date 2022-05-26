using BuildingBlocks.Data.Entities;

namespace MainApp.Models;

public class PostModel : Post
{
    public List<IFormFile> Images { get; set; }
}