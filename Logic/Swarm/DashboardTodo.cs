namespace Swarmops.Logic.Swarm
{
    public class DashboardTodo
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public TodoUrgency Urgency { get; set; }
    }

    public enum TodoUrgency
    {
        Unknown = 0,
        Normal,
        Yellow,
        Orange,
        Red
    }
}