using System.Security.Claims;

namespace BuildingBlocks.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue("Id");
        return int.TryParse(value, out var id) ? id : 0;
    }

    public static string GetName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("Name") ?? string.Empty;
    }

    public static string GetAvatar(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("Avatar");
    }

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("Email");
    }

    public static string GetPhone(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("Phone");
    }

    public static string GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("Role");
    }

    public static string GetAbout(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("About");
    }

    public static string GetAddress(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("Address");
    }
}