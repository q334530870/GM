using System.Configuration;

namespace iDai360.Scheduler.Core.Configuration
{
    public class TaskCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Task();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Task) element).Name;
        }
    }
}
