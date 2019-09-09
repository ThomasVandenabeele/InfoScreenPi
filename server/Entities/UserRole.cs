namespace InfoScreenPi.Entities
{
    public class UserRole : Entity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
