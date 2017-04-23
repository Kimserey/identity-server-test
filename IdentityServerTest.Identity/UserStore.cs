using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerTest.Identity
{
    public class UserAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime PasswordTimestamp { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<ValueTuple<string, string>> Claims { get; set; }
    }

    public interface IUserStore
    {
        UserAccount GetUser(Guid userId);
        void CreateUser(string name, string email, string password, bool enabled, IEnumerable<ValueTuple<string, string>> claims);
        bool VerifyUserPassword(string username, string password);
    }

    public class UserStore : IUserStore
    {
        // test in memory
        private List<UserAccount> _accounts = new List<UserAccount>();
        private ICryptography _cryptography;

        public UserStore(ICryptography cryptography)
        {
            _cryptography = cryptography;
        }

        public void CreateUser(string name, string email, string password, bool enabled, IEnumerable<(string, string)> claims)
        {
            _accounts.Add(new UserAccount
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Claims = claims,
                Email = email,
                Enabled = enabled,
                Name = name,
                Password = password,
                PasswordTimestamp = DateTime.UtcNow
            });
        }

        public UserAccount GetUser(Guid id)
        {
           return _accounts.SingleOrDefault(i => i.Id == id);
        }

        public bool VerifyUserPassword(string username, string password)
        {
            var userAccount = _accounts.FirstOrDefault(u => u.Name == username);
            if (userAccount == null)
            {
                return false;
            }
            else
            {
                return _cryptography.Verify(userAccount.Password, password);
            }
        }
    }
}
