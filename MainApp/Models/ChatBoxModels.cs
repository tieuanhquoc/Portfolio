using BuildingBlocks.Data.Entities;

namespace MainApp.Models;

public class ChatBoxModel
{
    public List<User> Users { get; set; } = new();
    public User User { get; set; }
    public List<Room> Rooms { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}