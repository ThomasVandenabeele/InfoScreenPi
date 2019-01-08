using System;

namespace InfoScreenPi.Entities
{
    public class Error : Entity
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
