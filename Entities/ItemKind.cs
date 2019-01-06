namespace InfoScreenPi.Entities
{
    public class ItemKind : IEntityBase
    {
        public int Id {get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
    }
}
