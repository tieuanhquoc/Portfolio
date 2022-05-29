using System.Text.Json.Serialization;

namespace BuildingBlocks.Data.Entities;

public class Room
{
    public int Id { get; set; }
    public int CreatorId { get; set; }
    public int MemberId { get; set; }

    [JsonIgnore]
    public User Creator { get; set; }
    [JsonIgnore]
    public User Member { get; set; }

    public virtual ICollection<Message> Messages { get; set; }
}