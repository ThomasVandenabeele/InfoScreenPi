using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfoScreenPi.Entities
{
    public class CustomItem : Item, IExpiring, IStatic
    {
        //[ForeignKey("BackgroundId")]
        public int BackgroundId {get; set;}
        public Background Background { get; set; }
        [Column("Content")]
        public string Content { get; set; }
        [Column("Archieved")]
        public bool Archieved { get; set; }
        [Column("ExpireDateTime")]
        public DateTime ExpireDateTime { get; set; }
    }
}
