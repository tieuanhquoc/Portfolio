using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Data.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        //
        public int RoomId { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
        public virtual Room Room { get; set; }
    }

    public enum MessageStatus
    {
        [Display(Name = "Đã gửi")] Sent = 1,
        [Display(Name = "Đã xem")] Seen = 2
    }
}