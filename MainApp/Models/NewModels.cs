using BuildingBlocks.Data.Entities;

namespace MainApp.Models;

public class NewModel : New
{
    public List<IFormFile> Images { get; set; }
}