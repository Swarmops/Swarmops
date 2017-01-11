using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Falconwing.Basic;
using Falconwing.Logic;
using Falconwing.Logic.ExtensionMethods;
using NBitcoin.BouncyCastle.Crypto.Generators;
using Newtonsoft.Json.Linq;
using Swarmops.Common.Exceptions;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;
using WebSocketSharp;

namespace Falconwing.Frontend
{
    public partial class Dashboard : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Stories.Reward (new DateTime(2015,10,1));

            this.PageTitle = "FWN Dashboard";
            InfoBoxLiteral = "These are stories in the edit queue. Edit or greenlight them for FACTS, FORMAT, FIT, and FUNNY.";

            PageAccessRequired = new Access(AccessAspect.Null); // (AccessAspect.Participant);

            this.RepeaterStories.DataSource = Stories.GetGlobalEditQueue(CurrentUser, 10);
            this.RepeaterStories.DataBind();

            this.RepeaterPublishQueue.DataSource = Stories.GetGlobalPublicationQueue (10);
            this.RepeaterPublishQueue.DataBind();

            this.RepeaterPublished.DataSource = Stories.GetLastPublished (10);
            this.RepeaterPublished.DataBind();

            this.RepeaterRejected.DataSource = Stories.GetLastRejected (10);
            this.RepeaterRejected.DataBind();

            this.RepeaterStars.DataSource = Stories.GetMostShared (20, 28);
            this.RepeaterStars.DataBind();

            this.RepeaterAuthor.DataSource = Stories.GetStoriesFromAuthor (CurrentUser, 10);
            this.RepeaterAuthor.DataBind();

            /*
            this.DropCategories.Style[HtmlTextWriterStyle.Width] = "324px";
            this.TextBody.Style[HtmlTextWriterStyle.Width] = "304px";*/

            if (!Page.IsPostBack)
            {
                this.DropCategories.Items.Add(new ListItem("-- DRAFT --", "8"));
                this.DropCategories.Items.Add(new ListItem("War and Conflict", "1"));
                this.DropCategories.Items.Add(new ListItem("Law and Politics", "7"));
                this.DropCategories.Items.Add(new ListItem("Finance / Cryptocurrency", "2"));
                this.DropCategories.Items.Add(new ListItem("General Local News", "3"));
                this.DropCategories.Items.Add(new ListItem("Feelgood News", "4"));
                this.DropCategories.Items.Add(new ListItem("Science and Tech", "5"));
                this.DropCategories.Items.Add(new ListItem("Gaming / Entertainment", "6"));

                this.DropCountries.Items.Add(new ListItem("-- Global", "--"));
                this.DropCountries.Items.Add(new ListItem("AT Austria", "AT"));
                this.DropCountries.Items.Add(new ListItem("BE Belgium", "BE"));
                this.DropCountries.Items.Add(new ListItem("BG Bulgaria", "BG"));
                this.DropCountries.Items.Add(new ListItem("CH Switzerland", "CH"));
                this.DropCountries.Items.Add(new ListItem("CY Cyprus", "CY"));
                this.DropCountries.Items.Add(new ListItem("CZ Czech Rep.", "CZ"));
                this.DropCountries.Items.Add(new ListItem("DE Germany", "DE"));
                this.DropCountries.Items.Add(new ListItem("DK Denmark", "DK"));
                this.DropCountries.Items.Add(new ListItem("EE Estonia", "EE"));
                this.DropCountries.Items.Add(new ListItem("ES Spain", "ES"));
                this.DropCountries.Items.Add(new ListItem("FI Finland", "FI"));
                this.DropCountries.Items.Add(new ListItem("FR France", "FR"));
                this.DropCountries.Items.Add(new ListItem("GR Greece", "GR"));
                this.DropCountries.Items.Add(new ListItem("HR Croatia", "HR"));
                this.DropCountries.Items.Add(new ListItem("HU Hungary", "HU"));
                this.DropCountries.Items.Add(new ListItem("IE Ireland", "IE"));
                this.DropCountries.Items.Add(new ListItem("IS Iceland", "IS"));
                this.DropCountries.Items.Add(new ListItem("IT Italy", "IT"));
                this.DropCountries.Items.Add(new ListItem("LV Latvia", "LV"));
                this.DropCountries.Items.Add(new ListItem("LT Lithuania", "LT"));
                this.DropCountries.Items.Add(new ListItem("LU Luxembourg", "LU"));
                this.DropCountries.Items.Add(new ListItem("MT Malta", "MT"));
                this.DropCountries.Items.Add(new ListItem("NO Norway", "NO"));
                this.DropCountries.Items.Add(new ListItem("NL Netherlands", "NL"));
                this.DropCountries.Items.Add(new ListItem("PL Poland", "PL"));
                this.DropCountries.Items.Add(new ListItem("PT Portugal", "PT"));
                this.DropCountries.Items.Add(new ListItem("RO Romania", "RO"));
                this.DropCountries.Items.Add(new ListItem("SE Sweden", "SE"));
                this.DropCountries.Items.Add(new ListItem("SI Slovenia", "SI"));
                this.DropCountries.Items.Add(new ListItem("SK Slovakia", "SK"));
                this.DropCountries.Items.Add(new ListItem("UK United Kingdom", "UK"));
            }
        }

        [WebMethod]
        public static AjaxCallResult RenderPreview(int topicId, string geographyCode, string headline, string body, int photoId)
        {
            try
            {
                if (string.IsNullOrEmpty (geographyCode))
                {
                    geographyCode = "--";
                }

                int geographyId = 1;
                if (geographyCode != "--")
                {
                    geographyId = -Country.FromCode(geographyCode).Identity;
                }

                Bitmap renderedStory = Story.RenderThumbnail(headline, body, Topic.FromIdentity(topicId), geographyId, photoId);
                MemoryStream stream = new MemoryStream();
                renderedStory.Save(stream, ImageFormat.Png);

                byte[] imageBytes = stream.ToArray();
                string base64Image = Convert.ToBase64String(imageBytes);

                return new AjaxCallResult { Success = true, DisplayMessage = base64Image };
            }
            catch (Exception e)
            {
                return new AjaxCallResult { Success = false, DisplayMessage = e.ToString() };
            }
        }


        [WebMethod]
        public static AjaxCallResult RenderThumbnail(int storyId)
        {
            try
            {
                string cacheKey = "StoryThumbnail" + storyId;
                string renderedThumbnailBase64 = (string) GuidCache.Get (cacheKey);

                if (string.IsNullOrEmpty (renderedThumbnailBase64))
                {
                    Story story = Story.FromIdentity(storyId);
                    renderedThumbnailBase64 = story.ThumbnailBase64;
                }

                return new AjaxCallResult { Success = true, DisplayMessage = renderedThumbnailBase64 };
            }
            catch (Exception e)
            {
                return new AjaxCallResult { Success = false, DisplayMessage = e.ToString() };
            }
        }



        [WebMethod]
        public static StoryDataCallResult LockStoryForEditing(int storyId, int revisionId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            try
            {
                Story story = Story.FromIdentity(storyId);
                story.Lock(authData.CurrentUser, StoryEditType.Edit);

                return new StoryDataCallResult { Success = true, 
                    StoryHeadline = story.Headline, 
                    StoryBody = story.Body, 
                    StorySources = story.SourceLinks, 
                    StoryPhotoDescription = (story.PhotoId != 0? story.Photo.Description : "---"),
                    StoryPhotoId = story.PhotoId,
                    StoryTopicId = story.TopicId, StoryCountryCode = (story.GeographyId == 1 ? "--" : (story.GeographyId < 0 ? Country.FromIdentity(-story.GeographyId).Code : story.Geography.Country.Code)) };
            }
            catch (Exception exception)
            {
                return new StoryDataCallResult { Success = false, DisplayMessage = exception.ToString() };
            }
        }

        [WebMethod]
        public static AjaxCallResult CancelStoryEdit(int storyId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            Story story = Story.FromIdentity(storyId);
            story.Unlock(authData.CurrentUser);

            return new AjaxCallResult { Success = true };
        }

        [WebMethod]
        public static AjaxCallResult CommitStoryEdit(int storyId, int revisionId, string countryCode, int topicId, string headline, string body, string sources, string comment, int photoId)
        {
            try
            {

                AuthenticationData authData = GetAuthenticationDataAndCulture();

                headline = Uri.UnescapeDataString (headline);

                headline = headline.Trim();
                if (headline.EndsWith ("."))
                {
                    headline = headline.TrimEnd ('.');
                }
                headline = headline.Replace (" : ", ": ").Replace (" ?", "?").Replace ("  ", " ");

                headline = Uri.EscapeDataString (headline);

                if (string.IsNullOrEmpty(countryCode))
                {
                    countryCode = "--";
                }

                string uri = "ws://fwn-internal:15615/Editing?Notify=false&Auth=" +
                             Uri.EscapeDataString(authData.Authority.ToEncryptedXml());
                using (
                    var socketClient =
                        new WebSocket (uri))
                {
                    socketClient.Connect();

                    if (storyId > 0)
                    {
                        Country country = null;
                        if (countryCode != "--")
                        {
                            country = Country.FromCode (countryCode);
                        }

                        Story story = Story.FromIdentity (storyId);
                        StoryEdit storyEdit = story.Edit (StoryEditType.Edit, authData.CurrentUser,
                            Topic.FromIdentity (topicId), null,
                            country, headline, body, sources.Replace ("  ", " ").Trim(), comment, photoId);

                        story = Story.FromIdentity (storyId); // reload

                        string personString = "#" + authData.CurrentUser.Identity.ToString("N0");
                        if (authData.CurrentUser.Identity == story.CreatedByPersonId)
                        {
                            personString = "Author";
                        }

                        JObject storyEditedNotify = new JObject();
                        storyEditedNotify["messageType"] = "StoryEdited";
                        storyEditedNotify["StoryId"] = story.Identity;
                        storyEditedNotify["Headline"] = headline;
                        storyEditedNotify["Body"] = body;
                        storyEditedNotify["Sources"] = story.SourceLinksHtml;
                        storyEditedNotify["TopicGeography"] = story.Topic.Name + ", " + story.GeographyName;

                        socketClient.Send (storyEditedNotify.ToString());
                        socketClient.Ping();
                        socketClient.Send("{\"messageType\":\"AddStoryEdit\",\"EditTimestamp\":\"" + DateTime.UtcNow.ToUnix() + "\",\"StoryId\":\"" + story.Identity +
                                           "\",\"StoryEditId\":\"" + storyEdit.Identity +
                                           "\",\"EditType\":\"Edit\",\"Comment\":\"" + comment.Replace ("\"", "\\\"") +
                                           "\",\"PersonIdString\":\"" + personString +
                                           "\"}");

                        story.Unlock (authData.CurrentUser);
                    }
                    else
                    {
                        Story story = Story.Create (headline, body, Topic.FromIdentity (topicId),
                            (countryCode == "--" ? 1 : -Country.FromCode (countryCode).Identity), sources,
                            authData.CurrentUser);
                        if (photoId > 0)
                        {
                            story.Photo = Photo.FromIdentity (photoId);
                        }
                        socketClient.Send("{\"serverRequest\":\"UpdateQueueCounts\"}");
                        socketClient.Ping();
                        socketClient.Send("{\"messageType\":\"StoryCreated\"}");
                    }

                    socketClient.Ping();
                    socketClient.Close();
                }

                return new AjaxCallResult { Success = true };
            }
            catch (Exception exception)
            {
                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = "An exception was thrown [CommitStoryEdit]:<br/><br/>" + exception.ToString()
                };
            }
        }

        [WebMethod]
        public static AjaxCallResult LightStory(int storyId, int revisionId, string lightType, string comment)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            // TODO: Authorize

            Story story = Story.FromIdentity(storyId);

            try
            {

                // Begin by not confirming GREENLIGHTS unless the RevisionId is correct. Work from there.

                string encryptedAuth = authData.Authority.ToEncryptedXml();
                encryptedAuth = Uri.EscapeDataString (encryptedAuth);

                string uri = "ws://fwn-internal:15615/Editing?Notify=false&Auth=" + encryptedAuth;

                using (
                    var socketClient =
                        new WebSocket (uri))
                {
                    socketClient.Connect();
                    StoryEdit newEdit = null;

                    string personString = "#" + authData.CurrentUser.Identity.ToString("N0");
                    if (authData.CurrentUser.Identity == story.CreatedByPersonId)
                    {
                        personString = "Author";
                    }

                    switch (lightType.ToLowerInvariant())
                    {
                        case "comment":
                            newEdit = story.Edit (StoryEditType.Comment, authData.CurrentUser, comment);
                            socketClient.Send("{\"messageType\":\"AddStoryEdit\",\"EditTimestamp\":\"" + DateTime.UtcNow.ToUnix() + "\",\"StoryId\":\"" + story.Identity + "\",\"StoryEditId\":\"" + newEdit.Identity + "\",\"EditType\":\"Comment\",\"Comment\":\"" + comment.Replace("\"", "\\\"") + "\",\"PersonIdString\":\"" + personString + "\"}");

                            break;
                        case "defer":
                            story.Edit (StoryEditType.Defer, authData.CurrentUser);
                            break;
                        case "red":
                            // Ignore revision, just redlight

                            newEdit = story.Edit (StoryEditType.Redlight, authData.CurrentUser, comment);
                            socketClient.Send("{\"messageType\":\"AddStoryEdit\",\"EditTimestamp\":\"" + DateTime.UtcNow.ToUnix() + "\",\"StoryId\":\"" + story.Identity + "\",\"StoryEditId\":\"" + newEdit.Identity + "\",\"EditType\":\"Redlight\",\"Comment\":\"" + comment.Replace("\"", "\\\"") + "\",\"PersonIdString\":\"" + personString + "\"}");
                            if (GetStoryApprovalRating (story) < -3)
                            {
                                newEdit = story.Edit (StoryEditType.Rejected, authData.CurrentUser,
                                    "Below rejection threshold");
                                story.ChangeState (StoryState.Rejected);
                                socketClient.Send("{\"serverRequest\":\"UpdateQueueCounts\"}");
                                socketClient.Send("{\"messageType\":\"AddStoryEdit\",\"EditTimestamp\":\"" + DateTime.UtcNow.ToUnix() + "\",\"StoryId\":\"" + story.Identity + "\",\"StoryEditId\":\"" + newEdit.Identity + "\",\"EditType\":\"Rejected\",\"Comment\":\"Below rejection threshold\",\"PersonIdString\":\"" + personString + "\"}");
                                socketClient.Send("{\"messageType\":\"StoryStateChange\",\"StoryId\":\"" + story.Identity + "\",\"StateChange\":\"StoryRejected\"}");
                            }

                            break;
                        case "green":
                            if (story.TopicId == 8)
                            {
                                return new AjaxCallResult
                                {
                                    Success = false,
                                    DisplayMessage =
                                        "This is a DRAFT story. It cannot be greenlit until taken out of draft mode. Check for placeholders, then move it to a publication-grade topic before greenlighting."
                                };
                            }
                            
                            if (story.RevisionCount > revisionId)
                            {
                                StoryEdits edits = story.Edits;
                                for (int index = story.RevisionCount; index < edits.Count; index++)
                                {
                                    if (edits[index].EditType == StoryEditType.Edit)
                                    {
                                        // The story was edited after being sent to client; concurrency error, order reload
                                        return new AjaxCallResult() {Success = false, DisplayMessage = "Reload"};
                                    }
                                }
                            }

                            newEdit = story.Edit (StoryEditType.Greenlight, authData.CurrentUser, comment);
                            socketClient.Send("{\"messageType\":\"AddStoryEdit\",\"EditTimestamp\":\"" + DateTime.UtcNow.ToUnix() + "\",\"StoryId\":\"" + story.Identity + "\",\"StoryEditId\":\"" + newEdit.Identity + "\",\"EditType\":\"Greenlight\",\"Comment\":\"\",\"PersonIdString\":\"" + personString + "\"}");
                            if (GetStoryApprovalRating (story) > 2) // Three people
                            {
                                newEdit = story.Edit(StoryEditType.ToPublishQueue, authData.CurrentUser, "Moved to publication queue");
                                story.ChangeState(StoryState.PublicationQueue);
                                socketClient.Send("{\"serverRequest\":\"UpdateQueueCounts\"}");
                                socketClient.Send("{\"messageType\":\"AddStoryEdit\",\"EditTimestamp\":\"" + DateTime.UtcNow.ToUnix() + "\",\"StoryId\":\"" + story.Identity + "\",\"StoryEditId\":\"" + newEdit.Identity + "\",\"EditType\":\"ToPublishQueue\",\"Comment\":\"Moved to publication queue\",\"PersonIdString\":\"" + personString + "\"}");
                                socketClient.Send("{\"messageType\":\"StoryStateChange\",\"StoryId\":\"" + story.Identity + "\",\"StateChange\":\"ApprovedForPublishing\"}");
                            }

                            break;
                        default:
                            return new AjaxCallResult
                            {
                                Success = false,
                                DisplayMessage = "Unknown lighting color in call"
                            };
                    }

                    socketClient.Ping(); // causes a small delay
                    socketClient.Close();
                }

                return new AjaxCallResult { Success = true };
            }
            catch (DatabaseConcurrencyException dbException)
            {
                return new AjaxCallResult { Success = false, DisplayMessage = "Exception thrown:<br/><br/>" + dbException.ToString() };
            }
        }

        private static int GetStoryApprovalRating(Story story)
        {
            StoryEdits edits = story.Edits;
            int approvalRating = 0;

            foreach (StoryEdit edit in edits)
            {
                if (edit.EditType == StoryEditType.Greenlight)
                {
                    approvalRating++;
                    if (edit.PersonId == 1)
                    {
                        approvalRating++; // mine are slightly heavier
                    }
                }
                else if (edit.EditType == StoryEditType.Redlight)
                {
                    approvalRating -= 2;
                    if (edit.PersonId == 1)
                    {
                        approvalRating -= 10000; // yeah, I get to redlight with a vengeance
                    }
                }
                else if (edit.EditType == StoryEditType.Edit)
                {
                    approvalRating = 0; // An edit resets rating
                }
            }

            return approvalRating;
        }


        [WebMethod]
        public static StoryQueueCallResult GetStoryEditPool(int count)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            return GetDynamicStoryList (Stories.GetGlobalEditQueue (authData.CurrentUser, count), true);
        }

        private static StoryQueueCallResult GetDynamicStoryList (Stories stories, bool unpublished = false)
        {
            List<QueuedStory> responseList = new List<QueuedStory>();
            foreach (Story story in stories)
            {
                responseList.Add(new QueuedStory
                {
                    Body = story.Body + " " + story.SourceLinksHtml,
                    StoryId = story.Identity,
                    Priority = story.Priority,
                    Headline = story.Headline,
                    RevisionId = story.RevisionCount,
                    TopicGeography = story.Topic.Name + ", " + story.GeographyName,
                    CreatedTimestampUnix = story.CreatedUnix,
                    PublishedTimestampUnix = unpublished? 0: story.PublishedUnix
                });
            }

            if (!unpublished)
            {
                responseList.Reverse(); // published stories come newest-first, unpublished oldest-first
            }

            return new StoryQueueCallResult { Success = true, Stories = responseList.ToArray() };
        }

        [WebMethod]
        public static StoryQueueCallResult GetStoryPublishQueue(int count)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            return GetDynamicStoryList (Stories.GetGlobalPublicationQueue (count));
        }

        [WebMethod]
        public static StoryQueueCallResult GetStoriesPublished(int count)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            return GetDynamicStoryList (Stories.GetLastPublished (count));
        }

        [WebMethod]
        public static StoryQueueCallResult GetStoriesRejected (int count)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            return GetDynamicStoryList (Stories.GetLastRejected (count));
        }

        [WebMethod]
        public static StoryQueueCallResult GetStoriesAuthor (int count)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            return GetDynamicStoryList (Stories.GetStoriesFromAuthor (authData.CurrentUser, count));
        }

        [WebMethod]
        public static AjaxCallResult GetEditPoolCount()
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            return new AjaxCallResult
            {
                Success = true,
                DisplayMessage = Stories.GetGlobalEditQueueLength (authData.CurrentUser).ToString ("N0")
            };
        }

        [WebMethod]
        public static StoryQueueCallResult GetStory (int storyId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            Story story = Story.FromIdentity (storyId);

            return GetDynamicStoryList (Stories.FromSingle (story));
        }


        [WebMethod]
        public static StoryEditsCallResult GetStoryEdits (int storyId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            List<QueuedStoryEdit> result = new List<QueuedStoryEdit>();

            Story story = Story.FromIdentity (storyId);
            StoryEdits edits = story.Edits;
            foreach (StoryEdit edit in edits)
            {
                if (edit.EditType != StoryEditType.Defer)
                {
                    string personString = "#" + edit.PersonId.ToString ("N0");
                    if (edit.PersonId == 0)
                    {
                        personString = "System";
                    }
                    if (edit.PersonId == story.CreatedByPersonId)
                    {
                        personString = "Author";
                    }

                    result.Add (new QueuedStoryEdit { Comment = edit.Comment, EditType = edit.EditType.ToString(), PersonIdString = personString, StoryEditId = edit.Identity, EditTimestamp = edit.DateTimeUtc.ToUnix() });
                }
            }

            return new StoryEditsCallResult {Success = true, StoryId = storyId, StoryEdits = result.ToArray()};
        }


        [WebMethod]
        public static StoryEditsCallResult GetStoryHistory (int storyId)
        {
            List<QueuedStoryEdit> result = new List<QueuedStoryEdit>();

            Dictionary<StoryEditType, bool> approvedTypes = new Dictionary<StoryEditType, bool>();
            approvedTypes[StoryEditType.Redlight] =
                approvedTypes[StoryEditType.Greenlight] =
                approvedTypes[StoryEditType.Rejected] =
                approvedTypes[StoryEditType.TimedOut] =
                approvedTypes[StoryEditType.Warning] =
                approvedTypes[StoryEditType.ToPublishQueue] =
                approvedTypes[StoryEditType.Published] = true;

            Story story = Story.FromIdentity(storyId);
            StoryEdits edits = story.Edits;
            foreach (StoryEdit edit in edits)
            {
                if (approvedTypes.ContainsKey (edit.EditType) || edit.EditType.ToString().StartsWith ("StarAward"))
                {
                    result.Add(new QueuedStoryEdit { Comment = edit.Comment, EditType = edit.EditType.ToString(), PersonIdString = "", StoryEditId = edit.Identity, EditTimestamp = edit.DateTimeUtc.ToUnix() });
                }
            }

            return new StoryEditsCallResult { Success = true, StoryId = storyId, StoryEdits = result.ToArray() };
        }

        [WebMethod]
        public static AjaxCallResult CreatePhoto (string sourceUrl, string description, string personName,
            string photoCredit)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            sourceUrl = sourceUrl.Replace ("https://", "http://"); // because of damn broken Mono cert store

            string guidString = Guid.NewGuid().ToString ("N");

            string fileNameBase = guidString + "-" + authData.CurrentUser.Identity.ToString("X8").ToLowerInvariant() + "-" +
                            authData.CurrentOrganization.Identity.ToString("X4").ToLowerInvariant();

            DateTime utcNow = DateTime.UtcNow;

            string fileFolder = utcNow.Year.ToString("0000") + Path.DirectorySeparatorChar +
                                utcNow.Month.ToString("00") + Path.DirectorySeparatorChar +
                                utcNow.Day.ToString("00");

            string storageRoot = "/var/lib/swarmops/upload/";

            if (!Directory.Exists("/var/lib/swarmops/upload/" + fileFolder))
            {
                Directory.CreateDirectory(storageRoot + fileFolder);
            }

            int fileCounter = 0;
            string fileName = string.Empty;


            do
            {
                fileCounter++;
                fileName = fileNameBase + "-" + fileCounter.ToString("X2").ToLowerInvariant();
            } while (File.Exists(storageRoot + fileFolder + Path.DirectorySeparatorChar + fileName) && fileCounter < 512);

            if (fileCounter >= 512)
            {
                return new AjaxCallResult {Success = false, DisplayMessage = "Unable to determine filename"};
            }

            long fileLength = 0;

            using (WebClient client = new WebClient())
            {
                try
                {
                    byte[] newImage = client.DownloadData (sourceUrl);
                    fileLength = newImage.LongLength;
                    File.WriteAllBytes (storageRoot + fileFolder + Path.DirectorySeparatorChar + fileName, newImage);
                }
                catch (Exception e)
                {
                    return new AjaxCallResult {Success = false, DisplayMessage = "Exception thrown:<br/><br/>" + e.ToString()};
                }
            }

            try
            {
                Document document = Document.Create(fileFolder + Path.DirectorySeparatorChar + fileName, sourceUrl,
                            fileLength, guidString, null, authData.CurrentUser);

                Photo photo = Photo.Create(document, description, personName, photoCredit, sourceUrl);

                return new AjaxCallResult { Success = true, DisplayMessage = photo.Identity.ToString() };
            }
            catch (Exception e)
            {
                return new AjaxCallResult { Success = false, DisplayMessage = "Exception thrown:<br/><br/>" + e.ToString() };
            }

        }

    }


    public class QueuedStory
    {
        public int StoryId { get; set; }
        public int RevisionId { get; set; }
        public int Priority { get; set; }
        public string TopicGeography { get; set; }
        public string Headline { get; set; }
        public string Body { get; set; }
        public int CreatedTimestampUnix { get; set; }
        public int PublishedTimestampUnix { get; set; }
    }


    public class StoryQueueCallResult : AjaxCallResult
    {
        public QueuedStory[] Stories { get; set; }
    }


    public class QueuedStoryEdit
    {
        public int StoryEditId { get; set; }
        public string PersonIdString { get; set; }
        public string Comment { get; set; }
        public string EditType { get; set; }
        public int EditTimestamp { get; set; }
    }


    public class StoryEditsCallResult : AjaxCallResult
    {
        public int StoryId { get; set; }
        public QueuedStoryEdit[] StoryEdits { get; set; }
    }


    public class StoryDataCallResult : AjaxCallResult
    {
        public string StoryHeadline { get; set; }
        public string StoryBody { get; set; }
        public string StorySources { get; set; }
        public string StoryPhotoDescription { get; set; }
        public int StoryPhotoId { get; set; }
        public int StoryGeographyId { get; set; }
        public int StoryGeographyName { get; set; }
        public int StoryTopicId { get; set; }
        public int StoryTopicName { get; set; }
        public string StoryCountryCode { get; set; }
        public int StoryRevisionId { get; set; }
        public bool StoryVisible { get; set; }
    }
}