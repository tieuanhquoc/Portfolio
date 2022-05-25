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
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }

    public enum MessageStatus
    {
        [Display(Name = "Đã gửi")] Sent = 1,
        [Display(Name = "Đã xem")] Seen = 2
    }
}