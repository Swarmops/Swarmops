namespace Swarmops.Logic.Swarm
{
    public class DashboardTodo
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string JavaScript { get; set; }
        public TodoUrgency Urgency { get; set; }

        public string OnClick
        {
            get
            {
                if (string.IsNullOrEmpty (this.JavaScript))
                {
                    return "document.location='" + this.Url + "'; return false;";
                }

                return this.JavaScript;
            }
        }
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