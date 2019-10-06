namespace StealNews.Core.Settings
{
    public class BackgroundWorkersConfiguration
    {
        public int BackgroundNewsGeneratorTimeOutSec{ get; set; }
        public int TimeOfStartingWorkersHours { get; set; }
        public int TimeOfEndingWorkersHours { get; set; }
    }
}
