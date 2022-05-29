using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public CommentStatus Status { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }

    public enum CommentStatus
    {
        Enabled = 1,
        Block = 2
    }
}