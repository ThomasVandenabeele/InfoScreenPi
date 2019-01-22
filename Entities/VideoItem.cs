using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfoScreenPi.Entities
{
    public class VideoItem : Item, IExpiring
    {
      public string URL { get; set; }
      [Column("Archieved")]
      public bool Archieved { get; set; }
      [Column("ExpireDateTime")]
      public DateTime ExpireDateTime { get; set; }
    }
}
