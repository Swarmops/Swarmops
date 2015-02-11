using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_PersonRoleTreeData : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            // If level if system-wide, execute this block. Also, move the data to the real place, please

            JsonPositions rootPositions = new JsonPositions();

            Organization sandbox = Organization.FromIdentity (1); // Used as sysadmin basis

            string[] writeAdminIds = sandbox.Parameters.TemporaryAccessListWrite.Split (',');
            string[] readAdminIds = sandbox.Parameters.TemporaryAccessListRead.Split(',');

            JsonPosition root = new JsonPosition();
            root.Id = "Root"; // heh. Sysadmin has id "root". Heh.
            root.LocalizedTitle = Position.Localized (PositionTitle.System_AdministratorMain);
            root.AssignedName = Person.FromIdentity (Int32.Parse (writeAdminIds[0])).Canonical;

            root.Children.Add(new JsonPosition { Id = "AsstRW", LocalizedTitle = Position.Localized(PositionTitle.System_AdministratorAssistantRW, true), AssignedName = Resources.Global.Swarm_Vacant });
            root.Children.Add(new JsonPosition { Id = "AsstRO", LocalizedTitle = Position.Localized(PositionTitle.System_AdministratorAssistantRO, true), AssignedName = Resources.Global.Swarm_Vacant });

            JsonPositions positions = new JsonPositions();
            positions.Add (root);

            Response.Output.WriteLine(RecursePositionTree (positions));

            Response.End();
        }

        private string RecursePositionTree (JsonPositions positions)
        {
            List<string> elements = new List<string>();

            foreach (JsonPosition position in positions)
            {
                string element = string.Format("\"id\":\"{0}\",\"positionTitle\":\"{1}\",\"assignedName\":\"{2}\"", 
                    JsonSanitize (position.Id), JsonSanitize (position.LocalizedTitle), JsonSanitize (position.AssignedName));

                if (position.Children.Count > 0)
                {
                    element += ",\"state\":\"open\",\"children\":" + RecursePositionTree(position.Children);
                }

                elements.Add("{" + element + "}");
            }

            return "[" + String.Join(",", elements.ToArray()) + "]";
        }

        private class JsonPosition
        {
            public JsonPosition()
            {
                Children = new JsonPositions();
            }

            public string Id { get; set; }
            public string LocalizedTitle { get; set; }
            public string AssignedName { get; set; }

            public JsonPositions Children { get; set; }
        }

        private class JsonPositions : List<JsonPosition>
        {
            // just a typedef
        }
    }
}