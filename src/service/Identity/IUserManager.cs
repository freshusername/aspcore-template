using api.models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace service.Identity
{
    public interface IUserManager
    {
        public Task<User> GetUserAsync(ClaimsPrincipal principal);
    }
}
