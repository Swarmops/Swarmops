using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.DashboardTasks
{
    public class TaskVolunteer : TaskBase
    {
        public TaskVolunteer (Volunteer volunteer)
            : base (
                volunteer.Identity, "Volunteer " + volunteer.Name, volunteer.OpenedDateTime,
                volunteer.OpenedDateTime.AddDays (21))
        {
            // empty ctor
        }
    }
}