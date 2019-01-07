namespace InfoScreenPi.Entities
{
    public class UserRole : Entity, IEntityBase
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
