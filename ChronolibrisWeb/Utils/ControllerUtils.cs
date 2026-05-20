using System.Security.Claims;

namespace ChronolibrisWeb.Utils
{
    public static class ControllerUtils
    {
        public static bool TryGetUserId(ClaimsPrincipal User, out long userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return long.TryParse(claim?.Value, out userId);
        }

        public static bool TryGetRole(ClaimsPrincipal User, out string role)
        {
            var claim = User.FindFirst(ClaimTypes.Role);
            if (claim == null)
            {
                role = "";
                return false;
            }
            role = claim.Value;
            return true;
        }
    }
}
