using BuildingBlocks.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;

[AllowAnonymous]
public class ApiBaseController : Controller
{
    protected long CurrentUserId => User.GetId();
}