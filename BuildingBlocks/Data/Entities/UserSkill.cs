namespace BuildingBlocks.Data.Entities
{
    public class UserSkill
    {
        public int Id { get; set; }
        public string Information { get; set; }
        public string Skill { get; set; }
        public double PercentSkill { get; set; }
        public string Project { get; set; }
        public string Time { get; set; }
        public string TitleProject { get; set; }

        public string ShortTitle { get; set; }
        public DateTime CreatedAt { get; set; }

        //
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}