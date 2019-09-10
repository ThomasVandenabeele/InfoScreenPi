namespace InfoScreenPi.Entities
{
  public interface IStatic : IItem
  {
    int BackgroundId {get; set;}
    Background Background { get; set; }
    string Content { get; set; }
  }
}
