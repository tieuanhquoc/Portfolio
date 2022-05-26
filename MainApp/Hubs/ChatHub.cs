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

        var messages = await _databaseContext.Messages
            .Where(x =>
                x.RoomId == room.Id
            )
            .Include(x => x.Creator)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();


        await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.RoomId.ToString());
        _connections[Context.ConnectionId] = userConnection;
        await Clients.Group(userConnection.RoomId.ToString()).SendAsync("Messages", messages);
    }

    public async Task SendMessage(string message)
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

            await _databaseContext.Messages.AddAsync(new Message
            {
                Content = message,
                RoomId = userConnection.RoomId,
                CreatorId = userConnection.CreatorId,
                Status = MessageStatus.Sent,
                CreatedAt = DateTime.Now
            });
            await _databaseContext.SaveChangesAsync();

            var messages = await _databaseContext.Messages
                .Where(x =>
                    x.RoomId == room.Id
                )
                .Include(x => x.Creator)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            // await Clients.Group(userConnection.RoomId.ToString()).SendAsync("ReceiveMessage", userConnection.CreatorId, message);
            await Clients.Group(userConnection.RoomId.ToString()).SendAsync("Messages", messages);
        }
    }
}