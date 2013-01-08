using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Tasks
{
    public class TaskVolunteer: TaskBase
    {
        public TaskVolunteer(Volunteer volunteer)
            : base(
                volunteer.Identity, "Volunteer " + volunteer.Name, volunteer.OpenedDateTime,
                volunteer.OpenedDateTime.AddDays(21))
        {
            // empty ctor
        }
    }
}
