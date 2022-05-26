namespace BuildingBlocks.Data.Entities;

public class Room
{
    public int Id { get; set; }
    public int CreatorId { get; set; }
    public int MemberId { get; set; }

    public User Creator { get; set; }
    public User Member { get; set; }

    public virtual ICollection<Message> Messages { get; set; }
}