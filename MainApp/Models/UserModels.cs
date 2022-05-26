using BuildingBlocks.Data.Entities;

namespace MainApp.Models;

public class UserModels : User
{
    public List<IFormFile> Images { get; set; }
}