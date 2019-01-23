namespace InfoScreenPi.Entities
{
        public class Device : Entity
        {
            public string Name { get; set; }
            public string IP { get; set; }
            public string Hostname { get; set; }
            public string OperateString { get; set; }
            public string CronString { get; set; }
            public string CronCommand { get; set; }
            public string ScreenOnCommand { get; set; }
            public string ScreenOffCommand { get; set; }
        }
}