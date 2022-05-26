
using BuildingBlocks.Data.Entities;

namespace MainApp.Models;

public class UserSkillModel : UserSkill
{
    public List<IFormFile> Images { get; set; }
}