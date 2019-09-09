
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoScreenPi.Entities;

namespace InfoScreenPi.ViewModels
{
   
    public class AccountViewModel
    {
        public bool SignedIn { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public bool IsLocked { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLogin { get; set; }

        public AccountViewModel(User user)
        {
            this.Id = user.Id;  
            this.SignedIn = true;
            this.Email = user.Email;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Username = user.Username;
            this.IsLocked = user.IsLocked;
            this.DateCreated = user.DateCreated;
            this.LastLogin = user.LastLogin;
        }

    }
}
