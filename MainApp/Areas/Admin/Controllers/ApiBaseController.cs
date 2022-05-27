using BuildingBlocks.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
[Route("[area]/[Controller]")]
public class ApiBaseController : Controller
{
    protected long CurrentUserId => User.GetId();
}