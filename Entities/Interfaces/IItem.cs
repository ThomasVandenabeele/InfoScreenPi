namespace InfoScreenPi.Entities
{
  public interface IItem
  {
    string Title { get; set; }
    bool Active { get; set; }
    int DisplayTime { get; set; }
  }
}
