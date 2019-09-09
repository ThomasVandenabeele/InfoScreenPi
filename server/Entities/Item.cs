namespace InfoScreenPi.Entities
{
    public abstract class Item : Entity, IItem
    {
        public string Title { get; set; }
        public bool Active { get; set; }
        public int DisplayTime { get; set; }
    }
}
