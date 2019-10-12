namespace StealNews.Core.Settings
{
    public class BackgroundWorkersConfiguration
    {
        public int BackgroundNewsGeneratorTimeOutSec{ get; set; }
        public int TimeOfStartingWorkersHoursUtc { get; set; }
        public int TimeOfEndingWorkersHoursUtc { get; set; }
    }
}
