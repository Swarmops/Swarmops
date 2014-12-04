using System;

namespace Swarmops.Logic.DashboardTasks
{
    public class TaskBase
    {
        public TaskBase(int identity, string description, DateTime opened, DateTime expires)
        {
            Identity = identity;
            Description = description;
            Opened = opened;
            Expires = expires;
        }

        public int Identity { get; protected set; }
        public string Description { get; protected set; }
        public DateTime Opened { get; protected set; }
        public DateTime Expires { get; protected set; }
    }
}