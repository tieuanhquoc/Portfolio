using BuildingBlocks.Data;
using BuildingBlocks.Extensions;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Areas.Member.Controllers;

public class ChatBoxController : ApiBaseController
{
    private readonly DatabaseContext _databaseContext;

    [ActivatorUtilitiesConstructor]
    public ChatBoxController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<IActionResult> Index(int? userId)
    {
        var chatBoxModel = new ChatBoxModel
        {
            Users = await _databaseContext.Users.Where(x => x.Id != User.GetId()).ToListAsync(),
            Rooms = await _databaseContext.Rooms.Where(x =>
                    x.CreatorId == User.GetId() ||
                    x.MemberId == User.GetId()
                )
                .Include(x => x.Messages)
                .ToListAsync()
        };
        if (userId != null)
        {
            var user = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
                chatBoxModel.User = user;
            var currentUserId = User.GetId();
            var room = await _databaseContext.Rooms.Where(x =>
                    (x.CreatorId == currentUserId &&
                     x.MemberId == userId.Value) ||
                    (x.CreatorId == userId.Value &&
                     x.MemberId == currentUserId)
                )
                .FirstOrDefaultAsync();

            if (room != null)
            {
                var messages = await _databaseContext.Messages
                    .Where(x => x.RoomId == room.Id)
                    .Include(x => x.Creator)
                    .ToListAsync();
                chatBoxModel.Messages = messages;
            }
        }


        return View(chatBoxModel);
    }
}