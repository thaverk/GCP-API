using Microsoft.AspNetCore.Mvc;
using PhasePlayWeb.Controllers;

namespace PhasePlayWeb.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(UsersController.ConfirmEmail),
                controller: "Users",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(UsersController.ResetPassword),
                controller: "Users",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}
