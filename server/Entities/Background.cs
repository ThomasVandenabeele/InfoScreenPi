using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfoScreenPi.Entities
{
    public class Background : Entity
    {
        public string Url { get; set; }
        //public RSSItem RSSItem { get; set; }
        //public ICollection<CustomItem> CustomItems { get; set; }
        //[NotMapped]
        /*public ICollection<Item> Items
        {
          get
          {
            return (ICollections<Item>)((IEnumerable<Item>)RSSItems.AsEnumerable()).Concat(CustomItems.AsEnumerable());
          }
        }*/
    }
}
//images/backgrounds/47f6ee6c-c04b-4b38-beb3-8abab9583766-1944587.jpg
