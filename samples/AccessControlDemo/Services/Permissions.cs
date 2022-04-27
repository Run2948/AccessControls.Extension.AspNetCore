namespace AccessControlDemo.Services
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
    }

    public class PermissionContext
    {
        public PermissionContext()
        {
            Permissions = new List<Permission>()
            {
                new Permission() { Id = Guid.NewGuid(), Code = "account:logout", Url = "/Account/Logout"},
                new Permission() { Id = Guid.NewGuid(), Code = "home:index", Url = "/Home/Index"},
                new Permission() { Id = Guid.NewGuid(), Code = "home:seen", Url = "#"},
                new Permission() { Id = Guid.NewGuid(), Code = "home:privacy", Url = "/Home/Privacy"},
            };

            UserPermissions = Permissions.Take(2).ToList();
        }

        public List<Permission> Permissions { get; set; }
        public List<Permission> UserPermissions { get; set; }
    }

    public static class PermissionHelper
    {
        public static bool Allowed(this PermissionContext context, string accessKey)
        {
            return context.Permissions.Any(key => string.Equals(key.Code, accessKey, StringComparison.OrdinalIgnoreCase)) && context.UserPermissions.Any(key => string.Equals(key.Code, accessKey, StringComparison.OrdinalIgnoreCase));
        }
    }
}
