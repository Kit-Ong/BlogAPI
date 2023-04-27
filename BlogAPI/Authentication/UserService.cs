using System;
using System.Threading.Tasks;

namespace BlogAPI.Authentication
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<User> Authenticate(string username, string password)
        {
            if (username != _configuration["loginUsername"] || password != _configuration["loginPassword"])
            {
                return Task.FromResult<User>(null);
            }

            var user = new User
            {
                Username = username,
                Id = Guid.NewGuid().ToString("N")
            };

            return Task.FromResult(user);
        }
    }
}