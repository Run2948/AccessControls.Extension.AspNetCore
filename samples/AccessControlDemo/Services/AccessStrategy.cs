using AccessControls.Extension.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlDemo.Services
{
    public class ResourceAccessStrategy : IResourceAccessStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PermissionContext _permissionContext;

        public ResourceAccessStrategy(IHttpContextAccessor httpContextAccessor, PermissionContext permissionContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionContext = permissionContext;
        }

        public bool IsCanAccess(string accessKey)
        {
            if (!_permissionContext.Allowed(accessKey))
            {
                return false;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext.User.Identity.IsAuthenticated;
        }

        public IActionResult DisallowedCommonResult => new ContentResult
        {
            Content = "You have no access",
            ContentType = "text/html",
            StatusCode = 403
        };

        public IActionResult DisallowedAjaxResult => new JsonResult(new { Data = "You have no access", Code = 403 });
    }

    public class ControlAccessStrategy : IControlAccessStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PermissionContext _permissionContext;

        public ControlAccessStrategy(IHttpContextAccessor httpContextAccessor, PermissionContext permissionContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionContext = permissionContext;
        }

        public bool IsControlCanAccess(string accessKey)
        {
            if (!_permissionContext.Allowed(accessKey))
            {
                return false;
            }

            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
