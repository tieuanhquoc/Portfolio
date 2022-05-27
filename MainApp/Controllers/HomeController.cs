using System.Diagnostics;
using MainApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;


public class HomeController : ApiBaseController
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home", new { area = "Member" });
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return RedirectToAction("Index", "Home", new { area = "Member" });
    }
}