using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IDataService _data;
        private readonly IEncryptionService _encryptionService;

        public MembershipService(IDataService dataService, IEncryptionService encryptionService)
        {
            _data = dataService;
            _encryptionService = encryptionService;
        }

        public MembershipContext ValidateUser(string username, string password)
        {
            var membershipCtx = new MembershipContext();

            var user = _data.GetSingleByUsername(username);
            if (user != null && isUserValid(user, password))
            {
                var userRoles = GetUserRoles(user.Username);
                membershipCtx.User = user;

                var identity = new GenericIdentity(user.Username);
                membershipCtx.Principal = new GenericPrincipal(
                    identity,
                    userRoles.Select(x => x.Name).ToArray());
            }

            return membershipCtx;
        }

        public User CreateUser(string username, string email, string password, int[] roles)
        {
            if (_data.GetSingleByUsername(username) != null) throw new Exception("Username is already in use");

            var passwordSalt = _encryptionService.CreateSalt();

            var user = new User()
            {
                Username = username,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = _encryptionService.EncryptPassword(password, passwordSalt),
                DateCreated = DateTime.Now
            };
            _data.Add(user);
            _data.Commit();

            if (roles != null || roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    addUserToRole(user, role);
                }
            }

            _data.Commit();
            return user;
        }

        public User GetUser(int userId)
        {
            return _data.GetSingle<User>(userId);
        }


        public List<Role> GetUserRoles(string username)
        {
            List<Role> _result = new List<Role>();

            var existingUser = _data.GetSingleByUsername(username);

            if (existingUser != null)
            {
                foreach (var userRole in existingUser.UserRoles)
                {
                    _result.Add(userRole.Role);
                }
            }
            return _result.Distinct().ToList();
        }


        private void addUserToRole(User user, int roleId)
        {
            var role = _data.GetSingle<Role>(roleId);
            if (role == null)
                throw new Exception("Role doesn't exist.");

            var userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };
            _data.Add(userRole);
            _data.Commit();
        }

        private bool isPasswordValid(User user, string password)
        {
            return string.Equals(_encryptionService.EncryptPassword(password, user.Salt), user.HashedPassword);
        }

        private bool isUserValid(User user, string password)
        {
            if (isPasswordValid(user, password))
            {
                return !user.IsLocked;
            }
            return false;
        }

    }
}
