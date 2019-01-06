using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public class UserRepository : EntityBaseRepository<User>, IUserRepository
    {
        IRoleRepository _roleReposistory;
        InfoScreenContext _context;
        public UserRepository(InfoScreenContext context, IRoleRepository roleReposistory)
            : base(context)
        {
            _roleReposistory = roleReposistory;
            _context = context;
        }

        public User GetSingleByUsername(string username)
        {
            return this.GetSingle(x => x.Username == username);
        }

        public IEnumerable<Role> GetUserRoles(string username)
        {
            //List<Role> _roles = null;

            User user = this.GetSingle(u => u.Username == username, u => u.UserRoles);
            if(user != null) return user.UserRoles.Select(ur => _context.Roles.FirstOrDefault(r => r.Id == ur.RoleId)).AsEnumerable();
            else return null;
            /*if(user != null)
            {
                _roles = new List<Role>();
                foreach (var _userRole in _user.UserRoles)
                    _roles.Add(_roleReposistory.GetSingle(_userRole.RoleId));


            }

            return _roles;*/
        }
    }
}
