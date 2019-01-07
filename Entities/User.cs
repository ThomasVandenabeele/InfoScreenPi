using System;
using System.Collections.Generic;

namespace InfoScreenPi.Entities
{
    public class User : Entity, IEntityBase
    {
        public User()
        {
            UserRoles = new List<UserRole>();
        }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public bool IsLocked { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLogin { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
