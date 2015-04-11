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
            _customCookieClass = "LocalPosition" + Request["Cookie"]; // may be null and that's ok

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
                string assignedName = string.Format("<a positionId='{3}' positionName='{4}' class='{1} LocalAssignPerson'>{2}</a> {0}", Resources.Controls.Swarm.Positions_Vacant, _customCookieClass, Resources.Controls.Swarm.Positions_AssignPerson, position.Identity, JavascriptEscape(position.Localized()));

                if (assignments.Count > 0)
                {
                    assignedName = assignments[0].Person.Canonical;
                    int expireYear = assignments[0].ExpiresDateTimeUtc.Year;
                    if (expireYear > 2000 && expireYear < 3000) // as in, "is defined"
                    {
                        expires = assignments[0].ExpiresDateTimeUtc.ToString("yyyy-MMM-dd");
                    }
                }

                string element = string.Format("\"id\":\"{0}-1\",\"positionTitle\":\"{1}\",\"assignedName\":\"{2}\",\"expires\":\"{3}\",\"minMax\":\"{4} / {5}\",\"iconType\":\"{6}\"", 
                    position.Identity, JsonSanitize (localizedPositionName), JsonSanitize (assignedName), JsonSanitize (expires), position.MinCount, 
                    position.MaxCount == 0? @"&infin;" : position.MaxCount.ToString(CultureInfo.InvariantCulture),
                    position.MaxCount == 1? "singular" : "plural");

                // TODO: Add all assignments after the first one right here

                int assignmentCount = 1;

                while (assignmentCount < assignments.Count)
                {
                    // add more lines to match the number of assignments for this position

                    elements.Add ("{" + element + "}");

                    expires = string.Empty;
                    int expireYear = assignments[assignmentCount].ExpiresDateTimeUtc.Year;
                    if (expireYear > 2000 && expireYear < 3000) // as in, "is defined"
                    {
                        expires = assignments[assignmentCount].ExpiresDateTimeUtc.ToString ("yyyy-MMM-dd");
                    }
                    element =
                        String.Format (
                            "\"id\":\"{0}-{1}\",\"iconType\":\"hidden\",\"positionTitle\":\"&nbsp;\",\"assignedName\":\"{2}\",\"expires\":\"{3}\"",
                            position.Identity, assignmentCount+1, assignments[assignmentCount].Person.Canonical, expires);

                    assignmentCount++;
                }

                if (assignmentCount < position.MaxCount || (position.MaxCount == 0 && position.Assignments.Count > 0))
                {
                    // finally, if the assigned count is less than max count, add a "assign another person" link

                    elements.Add ("{" + element + "}");
                    string addPerson =
                        string.Format (
                            "<a positionId='{1}' positionName='{2}' class='{3} LocalAssignPerson'>{0}</a>",
                            Resources.Controls.Swarm.Positions_AssignMorePerson, position.Identity,
                            JavascriptEscape (position.Localized()), _customCookieClass);

                    element =
                        String.Format(
                        "\"id\":\"{0}-0\",\"iconType\":\"hidden\",\"positionTitle\":\"&nbsp;\",\"assignedName\":\"{1}\"",
                            position.Identity, addPerson);

                }

                if (position.Children.Count > 0)  // This should only trigger when position.MaxCount is also 1, or a very weird UI will result
                {
                    element += ",\"state\":\"open\",\"children\":" + RecursePositionTree(positionNode.Children);
                }

                elements.Add("{" + element + "}");
            }

            return "[" + String.Join(",", elements.ToArray()) + "]";
        }

        private string _customCookieClass;

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