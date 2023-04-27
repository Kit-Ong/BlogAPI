using System.Threading.Tasks;

namespace BlogAPI.Authentication
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }
}