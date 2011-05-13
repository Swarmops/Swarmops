<%@ Page Language="C#" Debug="true" %>
<%@ Import Namespace="Activizr.Logic.Governance"%>
<%@ Import Namespace="System.Collections.Generic"%>
<%@ Import Namespace="Activizr.Logic.Structure"%>

<%@ Import Namespace="Activizr.Interface.Objects" %>
<%
    OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);

    string pollIdString = Request.QueryString["InternalPollId"];

    int pollId = Int32.Parse(pollIdString);

    MeetingElectionCandidates candidates = MeetingElectionCandidates.ForPoll(MeetingElection.Primaries2010);

    int result = candidates.Count;

    string variableName = Request.QueryString["VariableName"];

    string genderParameter = Request.QueryString["Gender"];

    string geographyParameter = Request.QueryString["Geographies"];

    if (!String.IsNullOrEmpty (genderParameter))
    {
        if (genderParameter.ToLower() == "female")
        {
            result = 0;
            foreach (MeetingElectionCandidate candidate in candidates)
            {
                if (candidate.Person.IsFemale)
                {
                    result++;
                }
            }
        }
    }

    if (!String.IsNullOrEmpty (geographyParameter))
    {
        List<int> geoIds = new List<int> ();

        string[] geoIdStrings = geographyParameter.Split (',');
        foreach (string geoIdString in geoIdStrings)
        {
            geoIds.Add(Int32.Parse(geoIdString));
        }

        result = 0;

        foreach (MeetingElectionCandidate candidate in candidates)
        {
            Geography candidateGeo = candidate.Person.Geography;

            bool inGeo = false;

            foreach (int geographyId in geoIds)
            {
                if (candidateGeo.Identity == geographyId || candidateGeo.Inherits (geographyId))
                {
                    inGeo = true;
                    break;
                }
            }

            if (inGeo)
            {
                result++;
            }
        }
    }

    if (String.IsNullOrEmpty(variableName))
    {
        variableName = "candidateCount";
    }
  
    Response.ContentType = "text/plain";
    Response.Write("<?php $" + variableName + " = '" + result.ToString() + "'; ?>"); 
%>
