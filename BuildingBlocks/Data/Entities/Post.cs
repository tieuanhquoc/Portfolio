using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public string Title { get; set; }
        public int Like { get; set; }
        public string UrlImages { get; set; }
        public PostStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        //
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }

    public enum PostStatus
    {
        Enabled = 1,
        Disabled = 2
    }
}