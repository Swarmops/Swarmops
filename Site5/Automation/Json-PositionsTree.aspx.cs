using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Common.Enums;
using Swarmops.Common.Generics;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_PositionsTree : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            // If level if system-wide, execute this block. Also, move the data to the real place, please

            JsonPositions rootPositions = new JsonPositions();

            Tree<Position> systemPositions = Positions.ForSystem().Tree;

            Response.Output.WriteLine(RecursePositionTree (systemPositions.RootNodes));

            Response.End();
        }

        private string RecursePositionTree (List<TreeNode<Position>> positionNodes)
        {
            List<string> elements = new List<string>();
            string reportsTo = string.Empty;
            if (positionNodes.Count > 0 && positionNodes[0].Data.ParentIdentity != 0)
            {
                
            }

            foreach (TreeNode<Position> positionNode in positionNodes)
            {
                Position position = positionNode.Data;
                string localizedPositionName = position.Localized (positionNode.Data.MaxCount != 1);
                PositionAssignments assignments = position.Assignments;

                string expires = string.Empty;

                string assignedName = Resources.Global.Swarm_Vacant + " <a href='#' class='LocalAssignPerson'>" + Resources.Global.Swarm_AssignPerson + "</a>" ;
                if (assignments.Count > 0)
                {
                    assignedName = assignments[0].Person.Canonical;
                }

                string element = string.Format("\"id\":\"{0}\",\"positionTitle\":\"{1}\",\"assignedName\":\"{2}\",\"expires\":\"{3}\",\"minMax\":\"{4} / {5}\"", 
                    position.Identity, JsonSanitize (localizedPositionName), JsonSanitize (assignedName), JsonSanitize (expires), position.MinCount, position.MaxCount == 0? @"&infin;" : position.MaxCount.ToString());

                // TODO: Add all assignments after the first one right here

                if (position.Children.Count > 0)
                {
                    element += ",\"state\":\"open\",\"children\":" + RecursePositionTree(positionNode.Children);
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