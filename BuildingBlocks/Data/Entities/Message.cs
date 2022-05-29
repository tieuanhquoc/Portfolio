using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public string CreatorName => Creator?.Username;
        [JsonIgnore]
        public virtual User Creator { get; set; }
        [JsonIgnore]
        public virtual Room Room { get; set; }
    }

    public enum MessageStatus
    {
        [Display(Name = "Đã gửi")] Sent = 1,
        [Display(Name = "Đã xem")] Seen = 2
    }
}