namespace StealNews.Core.Settings
{
    public class BackgroundWorkerConfiguration
    {
        public int BackgroundNewsGeneratorTimeOutSec{ get; set; }
        public int TimeOfStartingWorkersHoursUtc { get; set; }
        public int TimeOfEndingWorkersHoursUtc { get; set; }
    }
}
