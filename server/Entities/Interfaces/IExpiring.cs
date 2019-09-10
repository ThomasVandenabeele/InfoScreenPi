using System;

namespace InfoScreenPi.Entities
{
  public interface IExpiring : IItem
  {
    bool Archieved { get; set; }
    DateTime ExpireDateTime { get; set; }
  }
}
