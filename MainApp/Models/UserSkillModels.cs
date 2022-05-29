
using BuildingBlocks.Data.Entities;

namespace MainApp.Models;

public class UserSkillModel : UserSkill
{
    public List<IFormFile> Images { get; set; }
}

public class FindUserSkillModel
{
    public List<UserSkill> UserSkills { get; set; }
    public User User { get; set; }
}