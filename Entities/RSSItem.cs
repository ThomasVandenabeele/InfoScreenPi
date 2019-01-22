using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfoScreenPi.Entities
{
    public class RSSItem : Item, IStatic
    {
        //public string Source { get; set; }
        [ForeignKey("RssFeedId")]
        public RssFeed RssFeed { get; set; }
        //public int BackgroundId {get; set;}
        //[ForeignKey("BackgroundId")]
        public int BackgroundId {get; set;}
        public Background Background { get; set; }
        [Column("Content")]
        public string Content { get; set; }
    }
}
