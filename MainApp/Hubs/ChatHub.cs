using BuildingBlocks.Data;
using BuildingBlocks.Data.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Hubs;

public class ChatHub : Hub
{
    private readonly IDictionary<string, UserConnection> _connections;
    private readonly DatabaseContext _databaseContext;

    public ChatHub(IDictionary<string, UserConnection> connections, DatabaseContext databaseContext)
    {
        _connections = connections;
        _databaseContext = databaseContext;
    }

    public async Task JoinRoom(UserConnection userConnection)
    {
        if (userConnection.CreatorId != 0 && userConnection.MemberId != 0)
        {
            var room = await _databaseContext.Rooms.Where(x =>
                    (x.CreatorId == userConnection.CreatorId &&
                     x.MemberId == userConnection.MemberId) ||
                    (x.CreatorId == userConnection.MemberId &&
                     x.MemberId == userConnection.CreatorId)
                )
                .FirstOrDefaultAsync();

            if (room == null)
            {
                room = new Room
                {
                    CreatorId = userConnection.CreatorId,
                    MemberId = userConnection.MemberId
                };
                await _databaseContext.Rooms.AddAsync(room);
                await _databaseContext.SaveChangesAsync();
            }

            userConnection.RoomId = room.Id;

            var messages = await _databaseContext.Messages.IgnoreAutoIncludes()
                .Where(x =>
                    x.RoomId == room.Id
                )
                .Include(x => x.Creator)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();


            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.RoomId.ToString());
            _connections[Context.ConnectionId] = userConnection;
            await Clients.Group(userConnection.RoomId.ToString()).SendAsync("ReceiveMessages", messages);
        }
    }

    public async Task SendMessage(string content)
    {
        if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
        {
            var room = await _databaseContext.Rooms.Where(x =>
                    (x.CreatorId == userConnection.CreatorId &&
                     x.MemberId == userConnection.MemberId) ||
                    (x.CreatorId == userConnection.MemberId &&
                     x.MemberId == userConnection.CreatorId)
                )
                .FirstOrDefaultAsync();


            if (room == null)
            {
                room = new Room
                {
                    CreatorId = userConnection.CreatorId,
                    MemberId = userConnection.MemberId
                };
                await _databaseContext.Rooms.AddAsync(room);
                await _databaseContext.SaveChangesAsync();
            }

            var message = new Message
            {
                Content = content,
                RoomId = room.Id,
                CreatorId = userConnection.CreatorId,
                Status = MessageStatus.Sent,
                CreatedAt = DateTime.Now
            };

            await _databaseContext.Messages.AddAsync(message);
            await _databaseContext.SaveChangesAsync();

            message = await _databaseContext.Messages
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x =>
                    x.Id == message.Id
                );

            // await Clients.Group(userConnection.RoomId.ToString()).SendAsync("ReceiveMessage", userConnection.CreatorId, message);
            await Clients.Group(userConnection.RoomId.ToString()).SendAsync("ReceiveMessage", message);
        }
    }
}