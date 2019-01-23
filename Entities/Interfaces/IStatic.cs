namespace InfoScreenPi.Entities
{
  public interface IStatic : IItem
  {
    Background Background { get; set; }
    string Content { get; set; }
  }
}
