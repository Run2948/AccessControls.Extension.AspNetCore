using System.Collections.Generic;

namespace AccessControlDemo.Services
{
    public class AccessKeyResolver
    {
        private readonly Dictionary<string, string> _accessKeys = new Dictionary<string, string>()
        {
            { "/Home/Privacy", "Privacy" },
            { "/Account/Logout", "Logout"}
        };

        public string GetAccessKey(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return _accessKeys.ContainsKey(path) ? _accessKeys[path] : null;
        }
    }
}
