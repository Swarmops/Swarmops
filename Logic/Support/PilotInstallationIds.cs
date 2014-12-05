namespace Swarmops.Logic.Support
{
    public class PilotInstallationIds
    {
        public static string PiratePartySE
        {
            get { return "d7588903-5fd0-40cf-a5b1-9af7a722cb6e"; }
        }

        public static string DevelopmentSandbox
        {
            get { return "a8187d16-d913-4ddc-a3d5-214f7c14aac5"; }
        }

        public static string SwarmopsLive
        {
            get { return "db8f4ade-07f2-4f4f-a1c7-c9da91f93ea9"; }
        }

        public static bool IsPilot (string installationId)
        {
            string thisInstallationId = Persistence.Key["SwarmopsInstallationId"];
            if (installationId == thisInstallationId)
            {
                return true;
            }

            return false;
        }
    }
}