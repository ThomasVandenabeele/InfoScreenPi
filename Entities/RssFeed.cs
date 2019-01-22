using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfoScreenPi.Entities
{
    public class RssFeed : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Boolean Active { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public DateTime PublicationDate { get; set; }
        public Background StandardBackground { get ; set; }
        /*[ForeignKey("RssFeedId")]
        public ICollection<RSSItem> RSSItems {get; set;}*/
    }
}
