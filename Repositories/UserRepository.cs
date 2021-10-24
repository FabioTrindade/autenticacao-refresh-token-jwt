using RefreshTokenAuth.Models;
using System.Collections.Generic;
using System.Linq;

namespace RefreshTokenAuth.Repositories
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            var users = new List<User>
            {
                new User { Id = 1, UserName = "batman", Password = "batman", Role = "manager" },
                new User { Id = 2, UserName = "robin", Password = "robin", Role = "employee" }
            };

            return users.Where(w => w.UserName.ToLower() == username.ToLower() && w.Password == password).FirstOrDefault();
        }
    }
}
