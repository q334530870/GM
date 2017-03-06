using System.Configuration;

namespace iDai360.Scheduler.Core.Configuration
{
    public class SchedulerConfig : ConfigurationSection
    {
        [ConfigurationProperty("tasks")]
        public TaskCollection Tasks
        {
            get
            {
                return this["tasks"] as TaskCollection;
            }
        }
    }
}
