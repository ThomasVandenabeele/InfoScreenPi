using System;

namespace InfoScreenPi.Entities
{
    public class Error : Entity, IEntityBase
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
