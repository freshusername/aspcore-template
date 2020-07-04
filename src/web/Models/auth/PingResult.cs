using System.Collections.Generic;

namespace web.Models.auth
{
    public class PingResult
    {
        public string Id { get; }

        public string UserName { get; }

        public bool IsAuthenticated { get; }

        public List<string> LoginProviders { get; }

        public PingResult(string id, string userName, bool isAuthenticated, List<string> loginProviders)
        {
            Id = id;
            UserName = userName;
            IsAuthenticated = isAuthenticated;
            LoginProviders = loginProviders;
        }
    }
}
