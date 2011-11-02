using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Governance;

public partial class Pages_v4_Organization_VoteMotionAmendments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Motions motions = Motions.ForMeeting(Meeting.FromIdentity(1));

            this.RepeaterMotions.DataSource = motions;
            this.RepeaterMotions.DataBind();
        }

        motionDesignations = new Dictionary<int, string>();
    }

    protected void RepeaterMotions_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (motionDesignations == null)
        {
            motionDesignations = new Dictionary<int, string>();
        }

        RepeaterItem item = e.Item;
        if ((item.ItemType == ListItemType.Item) ||
            (item.ItemType == ListItemType.AlternatingItem))
        {
            Repeater repeaterAmendments = (Repeater)item.FindControl("RepeaterAmendments");
            Motion motion = (Motion)item.DataItem;
            motionDesignations[motion.Identity] = motion.Designation;
            repeaterAmendments.DataSource = motion.Amendments;
            repeaterAmendments.DataBind();
        }

    }

    protected void RepeaterAmendments_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;
        if ((item.ItemType == ListItemType.Item) ||
            (item.ItemType == ListItemType.AlternatingItem))
        {
            Label labelDesignation = (Label) item.FindControl("LabelDesignation");
            Label labelText = (Label) item.FindControl("LabelText");
            Label labelRecommended = (Label) item.FindControl("LabelRecommended");

            Literal literalRadioGroup1 = (Literal)item.FindControl("LiteralRadioGroupYes");
            Literal literalRadioGroup2 = (Literal)item.FindControl("LiteralRadioGroupNo");
            Literal literalRadioGroup3 = (Literal)item.FindControl("LiteralRadioGroupAbstain");

            Literal literalCheckedYes = (Literal)item.FindControl("LiteralCheckedYes");
            Literal literalCheckedNo = (Literal)item.FindControl("LiteralCheckedNo");
            Literal literalCheckedAbstain = (Literal)item.FindControl("LiteralCheckedAbstain");

            MotionAmendment amendment = (MotionAmendment) item.DataItem;

            string text = amendment.Text;

            if (text.Length > 100)
            {
                text = text.Substring(0, 97) + "...";
            }

            labelDesignation.Text = String.Format("{0}-{1:D3}", motionDesignations[amendment.MotionId], amendment.SequenceNumber);
            labelText.Text = text;

            string radioGroupName = "A" + amendment.Identity;

            literalRadioGroup1.Text = radioGroupName;
            literalRadioGroup2.Text = radioGroupName;
            literalRadioGroup3.Text = radioGroupName;

            Literal literalToCheck = null;
            string recommendationString = null;

            int recommendation = amendment.Identity % 3;  // Read this from new table; "AmendmentVotingList" or something like that

            switch (recommendation)
            {
                case 0:
                    recommendationString = "NEJ";
                    literalToCheck = literalCheckedNo;
                    break;
                case 1:
                    recommendationString = "JA";
                    literalToCheck = literalCheckedYes;
                    break;
                case 2:
                    recommendationString = "AVSTÅR";
                    literalToCheck = literalCheckedAbstain;
                    break;
                default:
                    throw new InvalidOperationException("This is not possible; the switch should have caught all cases.");

            }

            // literalToCheck.Text = "checked";
            labelRecommended.Text = recommendationString;


        }
    }

    private Dictionary<int, string> motionDesignations;
}
