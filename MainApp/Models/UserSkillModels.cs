
namespace MainApp.Models;

public class UserSkillCreate
{
    public string Information { get; set; }
    public string Skill { get; set; }
    public double PercentSkill { get; set; }
    public string Project { get; set; }
    public string Time { get; set; }
    public string TitleProject { get; set; }
    public string ShortTitle { get; set; }
    public List<IFormFile> Images { get; set; }
}