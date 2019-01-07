using System;

namespace InfoScreenPi.Entities
{
    public class Item : Entity, IEntityBase
    {
        public ItemKind Soort { get; set; }
        public RssFeed RssFeed { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Background Background { get; set; }
        public Boolean Active { get; set; }
        public Boolean Archieved { get; set; }
        public DateTime ExpireDateTime { get; set; }
    }
}
