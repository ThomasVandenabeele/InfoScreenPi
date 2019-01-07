using System;

namespace InfoScreenPi.Entities
{
    public class RssFeed : Entity, IEntityBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Boolean Active { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public DateTime PublicationDate { get; set; }
        public Background StandardBackground { get ; set; }
    }
}
