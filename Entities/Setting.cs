namespace InfoScreenPi.Entities
{
    public class Setting : IEntityBase
    {
        public int Id { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}