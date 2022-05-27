using BuildingBlocks.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Areas.Member.Controllers;

[Area("Member")]
[Authorize(Roles = "Member")]
public class ApiBaseController : Controller
{
    protected long CurrentUserId => User.GetId();
}