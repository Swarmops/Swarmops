using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using HtmlAgilityPack;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Logic.Media;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    public class TypedMailTemplate
    {
        private SerializableDictionary<string, PlaceHolder> placeholders =
            new SerializableDictionary<string, PlaceHolder>();

        private MailTemplate template;
        private TemplateType templatetype = TemplateType.None;

        #region Class-methods

        public enum TemplateType
        {
            None = 0,
            ActivistMail = 1,
            ExpiredMail = 2,
            MemberMail = 3,
            NewsletterMail = 4,
            OfficerMail = 5,
            PressReleaseMail = 6,
            ReminderMail = 7,
            WelcomeMail = 8,
            ChangeOrgMail = 9
        }

        private static readonly Dictionary<TemplateType, TypedMailTemplate> templateTypes = initTemplateTypes();

        /// <summary>
        ///     Get a list of available TypedMailTemplate subclasses
        /// </summary>
        public static List<string> GetTemplateNames()
        {
            List<string> retval = new List<string>();
            foreach (TemplateType t in templateTypes.Keys)
            {
                retval.Add(templateTypes[t].BaseName);
            }
            return retval;
        }

        public static string GetNameFromMailType(int mailType)
        {
            try
            {
                return templateTypes[(TemplateType) mailType].BaseName;
            }
            catch (Exception ex)
            {
                throw new Exception("Can't translate mailtype(" + mailType + ") to template name", ex);
            }
        }


        public static TemplateType TypeFromName(string className)
        {
            if (className.ToLower().EndsWith("plain"))
                className = className.Substring(0, className.Length - 5);
            foreach (TemplateType t in templateTypes.Keys)
            {
                if (templateTypes[t].BaseName.ToLower() == className.ToLower())
                {
                    return t;
                }
            }
            return TemplateType.None;
        }

        private static Dictionary<TemplateType, TypedMailTemplate> initTemplateTypes()
        {
            Dictionary<TemplateType, TypedMailTemplate> retval = new Dictionary<TemplateType, TypedMailTemplate>();
            retval.Add(TemplateType.None, new TypedMailTemplate());
            retval.Add(TemplateType.ActivistMail, new ActivistMail());
            retval.Add(TemplateType.ExpiredMail, new ExpiredMail());
            retval.Add(TemplateType.MemberMail, new MemberMail());
            retval.Add(TemplateType.NewsletterMail, new NewsletterMail());
            retval.Add(TemplateType.OfficerMail, new OfficerMail());
            retval.Add(TemplateType.PressReleaseMail, new PressReleaseMail());
            retval.Add(TemplateType.ReminderMail, new ReminderMail());
            retval.Add(TemplateType.WelcomeMail, new WelcomeMail());
            retval.Add(TemplateType.ChangeOrgMail, new ChangeOrgMail());
            return retval;
        }

        #endregion

        #region Instantiation and initialization

        public TypedMailTemplate()
        {
            this.template = new MailTemplate();
            AddPlaceHolder(new PlaceHolder("RecieverIds", "RecieverIds", PlaceholderType.NoTag));
        }

        /// <summary>
        ///     Create TypedMailTemplate object from name
        /// </summary>
        public static TypedMailTemplate FromName(string className)
        {
            try
            {
                if (className.EndsWith("Plain"))
                    className = className.Substring(0, className.Length - 5);
                //Get the current assembly object
                Assembly assembly = Assembly.GetExecutingAssembly();
                //Get the name of the assembly (this will include the public token and version number
                AssemblyName assemblyName = assembly.GetName();
                //Use just the name concat to the class chosen to get the type of the object
                Type t = assembly.GetType(assemblyName.Name + ".Communications." + className, false, true);
                if (t == null)
                {
                    //Fail, no such class
                    throw new ArgumentException("There is no defined class for a TypedMailTemplate named:" + className);
                }
                //Create the object, cast it and return it to the caller
                return (TypedMailTemplate) Activator.CreateInstance(t);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create TypedMailTemplate " + className, e);
            }
        }

        protected virtual void Initialize(MailTemplate templ)
        {
            this.template = templ;
        }

        protected virtual void Initialize(string countryCode, int orgId, string variant)
        {
            try
            {
                this.template = MailTemplate.FromNameCountryAndOrg(BaseName + variant, countryCode, orgId);
            }
            catch
            {
                string name = BaseName + variant;
                if (name.EndsWith("Plain"))
                {
                    try
                    {
                        name = name.Substring(0, name.Length - 5);
                        this.template = MailTemplate.FromNameCountryAndOrg(name, countryCode, orgId);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to create MailTemplate.FromNameCountryAndOrg(" + name + "," + countryCode + "," +
                            orgId + ")", e);
                    }
                }
                else
                {
                    try
                    {
                        this.template = MailTemplate.FromNameCountryAndOrg(name + "Plain", countryCode, orgId);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to create (plain) MailTemplate.FromNameCountryAndOrg(" + name + "," + countryCode +
                            "," + orgId + ")", e);
                    }
                }
            }
        }


        /// <summary>
        ///     Initialize a TypedMailTemplate subclass for rendering
        /// </summary>
        public virtual void Initialize(string countryCode, int orgId, OutboundMail mail, string variant)
        {
            Initialize(countryCode, orgId, variant);
            try
            {
                DeserializePlaceHolders(mail.Body);
            }
            catch (Exception ex)
            {
                try
                {
                    //try to handle old mail format... just in case.
                    SetPlaceHolder("BodyContent", mail.Body);
                    SetPlaceHolder("IntroContent", string.Empty);
                    SetPlaceHolder("GeographyName", mail.Geography.Name);
                    SetPlaceHolder("OrgName", mail.Organization.Name);
                }
                catch (Exception)
                {
                    //didnt't work, throw the original error
                    throw new Exception("Failed to deserialize placeholders:", ex);
                }
            }
        }

        #endregion

        /// <summary>
        ///     MailTemplate used when rendering
        /// </summary>
        public MailTemplate Template
        {
            get { return this.template; }
        }

        /// <summary>
        ///     Integer representing this class
        /// </summary>
        public int MailType
        {
            get { return (int) Templatetype; }
        }

        /// <summary>
        ///     Type of this class as enum TemplateType
        /// </summary>
        public TemplateType Templatetype
        {
            get
            {
                if (this.templatetype == TemplateType.None)
                    this.templatetype = TypeFromName(BaseName);
                return this.templatetype;
            }
        }


        /// <summary>
        ///     Name of this class
        /// </summary>
        public virtual string BaseName
        {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     Dictionary of placeholders
        /// </summary>
        public SerializableDictionary<string, PlaceHolder> Placeholders
        {
            get { return this.placeholders; }
            internal set { this.placeholders = value; }
        }

        #region PlaceHolders and PlaceHolder manipulation

        public enum PlaceholderType
        {
            /// <summary>
            ///     id of a span, If placeholeder is assigned a value this value
            ///     replaces the content of the span. If no value is assigned,
            ///     content remains untouched.
            /// </summary>
            Id,

            /// <summary>
            ///     %tag%, content will replace.
            /// </summary>
            Tag,

            /// <summary>
            ///     span with %tag% inside, whole span is removed if tag is empty.
            ///     %tag% can be used in other places outside the span.
            /// </summary>
            TagInSpan,

            /// <summary>
            ///     Not rendered, to be used to transfer info from creation stage to rendering stage.
            ///     (To be implemented)
            /// </summary>
            NoTag
        }

        #region Inner class PlaceHolder

        [Serializable]
        public class PlaceHolder
        {
            //this is XmlSerialized so the following two attributes should be public and no others.
            // XmlSerializer uses public fields and read/write properties

            internal Type dataType = typeof (string);
            internal string formatString = "";
            internal string label; //  label to show
            public string name; //  Id for span or name for %tag%


            internal int seq;
            internal PlaceholderType tagOrId = PlaceholderType.Tag;
            public string value = "<notset>"; //  current value

            public PlaceHolder()
            {
            }

            internal PlaceHolder(string name, string label, PlaceholderType tagOrId)
            {
                this.name = name;
                this.label = label;
                this.tagOrId = tagOrId;
            }

            internal PlaceHolder(string name, string label, PlaceholderType tagOrId, Type dataType, string formatString)
                : this(name, label, tagOrId)
            {
                this.dataType = dataType;
                this.formatString = formatString;
            }

            public Type DataType
            {
                get { return this.dataType; }
            }

            public string Label
            {
                get { return this.label; }
            }

            public int Seq
            {
                get { return this.seq; }
            }

            public string Name
            {
                get { return this.name; }
            }

            public string Value
            {
                get { return this.value; }
            }

            public PlaceholderType TagOrId
            {
                get { return this.tagOrId; }
            }
        }

        #endregion

        /// <summary>
        ///     Used to add placeholder in constructor of subclasses
        /// </summary>
        protected void AddPlaceHolder(PlaceHolder ph)
        {
            ph.seq = this.placeholders.Count;
            this.placeholders.Add(ph.Name, ph);
        }

        protected void SetPlaceHolder(string name, string value)
        {
            this.placeholders[name].value = value;
        }

        protected void SetPlaceHolder(string name, DateTime value)
        {
            if (Template != null && Template.LanguageCode != "" && Template.CountryCode != "")
            {
                CultureInfo cult = new CultureInfo(Template.LanguageCode + "-" + Template.CountryCode);
                string formattedDate = value.ToString(this.placeholders[name].formatString, cult);
                SetPlaceHolder(name, formattedDate);
            }
            else
            {
                string formattedDate = value.Ticks.ToString();
                SetPlaceHolder(name, formattedDate);
            }
        }

        /// <summary>
        ///     Serialize values from placeholder dictionary
        /// </summary>
        /// <remarks>
        ///     Used to serialize the values from the placeholder dictionary wich are then placed in the body of the
        ///     OutboundMail record. from where they later are deserialized when the actual rendering takes place.
        /// </remarks>
        public virtual void FillIndividualPlaceholders(IEmailPerson personObject)
        {
        }

        /// <summary>
        ///     Called to insert all placeholders from dictionary into the MailTemplate when rendering
        /// </summary>
        public void InsertAllPlaceHoldersToTemplate()
        {
            if (Template == null)
                throw new ArgumentException("There is no template loaded into the TypedMailTemplate");

            // sort the placeholders according to the order they were defined in to guarantee that substitutions are in expected order.
            // "Inside" substitutions are done if the placeholder containing the substitution is defined before the substitutet one.

            List<PlaceHolder> sortedPlaceholders = new List<PlaceHolder>(Placeholders.Values);
            sortedPlaceholders.Sort(
                delegate(PlaceHolder ph1, PlaceHolder ph2) { return ph1.seq.CompareTo(ph2.seq); });

            foreach (PlaceHolder ph in sortedPlaceholders)
            {
                InsertOnePlaceholderToTemplate(ph);
            }
        }

        private void InsertOnePlaceholderToTemplate(PlaceHolder ph)
        {
            if (ph.TagOrId == PlaceholderType.Tag)
            {
                // .Tag is cleared if it isnt set
                if (HttpUtility.HtmlDecode(ph.value) != "<notset>")
                    Template.ReplaceContentByPattern(ph.Name, ph.value);
                else
                    Template.ReplaceContentByPattern(ph.Name, "");
            }
            else if (ph.TagOrId == PlaceholderType.Id)
            {
                // .Id is left as is if it isn't set
                if (HttpUtility.HtmlDecode(ph.value) != "<notset>")
                    Template.ReplaceContentById(ph.Name, ph.value);
            }
            else if (ph.TagOrId == PlaceholderType.TagInSpan)
            {
                // .TagInSpan if not set: All tags are emptied and all elements are removed
                if (ph.value != "" && HttpUtility.HtmlDecode(ph.value) != "<notset>")
                {
                    Template.ReplaceContentByPattern(ph.Name, ph.value);
                    Template.RemoveEnclosingTag(ph.Name);
                }
                else
                {
                    //Replace tags
                    if (HttpUtility.HtmlDecode(ph.value) != "<notset>")
                        Template.ReplaceContentByPattern(ph.Name, ph.value);
                    else
                        Template.ReplaceContentByPattern(ph.Name, "");
                    //and remove id-elements
                    Template.RemoveElementById(ph.Name);
                }
            }
        }

        protected string SerializePlaceHolders()
        {
            XmlSerializer s = new XmlSerializer(Placeholders.GetType());
            StringBuilder retSb = new StringBuilder();
            TextWriter w = new StringWriter(retSb);
            s.Serialize(w, Placeholders);
            w.Close();
            return retSb.ToString().Replace("%IntroContent%", string.Empty);
                // EXTREMELY UGLY HACK to empty %IntroContent%
        }

        /// <summary>
        ///     Load placeholders dictionary with serialized values
        /// </summary>
        protected void DeserializePlaceHolders(string content)
        {
            XmlSerializer s = new XmlSerializer(Placeholders.GetType());
            SerializableDictionary<string, PlaceHolder> tempPlaceHolders =
                new SerializableDictionary<string, PlaceHolder>();
            TextReader r = new StringReader(content);
            tempPlaceHolders = (SerializableDictionary<string, PlaceHolder>) s.Deserialize(r);
            r.Close();

            foreach (PlaceHolder ph in Placeholders.Values)
            {
                if (tempPlaceHolders.ContainsKey(ph.Name))
                {
                    PlaceHolder phI = tempPlaceHolders[ph.name];
                    try
                    {
                        long ticks = 0;
                        // If it is a date and it was saved when no template was present it will be in ticks
                        // SetPlaceHolder will format it according to templates culture.
                        if (Placeholders[ph.Name].DataType == typeof (DateTime) && long.TryParse(phI.Value, out ticks))
                            SetPlaceHolder(ph.Name, new DateTime(ticks));
                        else
                            SetPlaceHolder(ph.Name, phI.Value);
                    }
                    catch (Exception)
                    {
                        SetPlaceHolder(ph.Name, "[*Invalid Value*]");
                    }
                }
            }
        }

        #endregion

        public OutboundMail CreateOutboundMail(Person author, int mailPriority, Organization organization,
            Geography geography)
        {
            return CreateOutboundMail(author, mailPriority, organization, geography, DateTime.Now);
        }

        /// <summary>
        ///     Create OutboundMail with personal sender
        /// </summary>
        public OutboundMail CreateOutboundMail(Person author, int mailPriority,
            Organization organization, Geography geography, DateTime releaseDateTime)
        {
            return OutboundMail.Create(author, BaseName, SerializePlaceHolders(),
                mailPriority, MailType, organization, geography, releaseDateTime);
        }

        /// <summary>
        ///     Create OutboundMail mail with "functional" sender
        /// </summary>
        public OutboundMail CreateFunctionalOutboundMail(MailAuthorType authorType, int mailPriority,
            Organization organization, Geography geography)
        {
            return CreateFunctionalOutboundMail(authorType, mailPriority, organization, geography, DateTime.Now);
        }

        public OutboundMail CreateFunctionalOutboundMail(MailAuthorType authorType, int mailPriority,
            Organization organization, Geography geography, DateTime releaseDateTime)
        {
            return OutboundMail.CreateFunctional(authorType, BaseName, SerializePlaceHolders(),
                mailPriority, MailType, organization.Identity, geography.Identity, releaseDateTime);
        }

        public OutboundMail CreateOutboundFake(Person author, Organization organization, Geography geography)
        {
            return OutboundMail.CreateFake(author, BaseName, SerializePlaceHolders(), 0,
                MailType, organization, geography);
        }

        #region Handle different source formats in placeholders

        public void PreparePlaceholders()
        {
            TypedMailTemplate template = this;
            Regex reForumType = new Regex(@"(\[h.\])|(\[b\])|(\[i\])|(\[i\])|(\[blockquote\])|(\[br\])|(\[\/a\])",
                RegexOptions.IgnoreCase);
            Regex reHtmlType = new Regex(@"(</[a-z1-4]+>)|(<[^>]+?/>)", RegexOptions.IgnoreCase);

            foreach (PlaceHolder ph in template.Placeholders.Values)
            {
                if (reHtmlType.IsMatch(ph.value))
                {
                    //contains HTML, try to strip dangerous features
                    try
                    {
                        HtmlDocument tempDoc = new HtmlDocument();
                        tempDoc.LoadHtml(ph.value);
                        HtmlNodeCollection scriptNodes = tempDoc.DocumentNode.SelectNodes("//script");
                        if (scriptNodes != null)
                        {
                            foreach (HtmlNode n in scriptNodes)
                            {
                                n.InnerHtml = "";
                            }
                        }
                        HtmlNodeCollection iframeNodes = tempDoc.DocumentNode.SelectNodes("//iframe");
                        if (iframeNodes != null)
                        {
                            foreach (HtmlNode n in iframeNodes)
                            {
                                n.SetAttributeValue("src", "");
                            }
                        }
                        HtmlNodeCollection onhandlers =
                            tempDoc.DocumentNode.SelectNodes("//*[starts-with(local-name(@*),'on')]");
                        if (onhandlers != null)
                        {
                            foreach (HtmlNode n in onhandlers)
                            {
                                foreach (HtmlAttribute att in n.Attributes)
                                {
                                    if (att.Name.ToLower().StartsWith("on"))
                                    {
                                        att.Value = "";
                                    }
                                }
                            }
                        }
                        ph.value = tempDoc.DocumentNode.InnerHtml;
                    }
                    catch
                    {
                    }
                }

                if (reForumType.IsMatch(ph.value))
                {
                    //Is Forum type formatting
                    ph.value = ConvertForumToHtml(ph.value);
                }
                if (!reHtmlType.IsMatch(ph.value))
                {
                    ph.value = ConvertPlainToHtml(ph.value);
                }
            }
        }

        /// <summary>
        ///     Convert plain text to reasonable html
        /// </summary>
        private string ConvertPlainToHtml(string phvalue)
        {
            phvalue = HttpUtility.HtmlEncode(phvalue);

            phvalue = phvalue.
                Replace("\r\n", "\n").
                Replace("\n", "<br />\n").
                Replace("\n", "\r\n");

            return phvalue;
        }

        /// <summary>
        ///     Convert a sring with "forum" tags to html
        /// </summary>
        private static string ConvertForumToHtml(string phvalue)
        {
            phvalue = HttpUtility.HtmlEncode(phvalue);

            phvalue = phvalue.
                Replace("\r\n", "\n").
                Replace("\n", "<br />\n").
                Replace("[br]<br />\n", "<br />\n").
                Replace("<br />\n<br />\n", "</p>\n\n<p>").
                Replace("\n", "\r\n").
                Replace("&quot;", "\"").
                Replace("&amp;", "&") +
                      "</p>";

            // Encode approved HTML codes: h1, h2, h3, blockquote, a, b, i

            Regex regexLinks =
                new Regex("(?s)\\[a\\s+href=\\\"(?<link>[^\\\"]+)\\\"\\](?<description>.+?)\\[/a\\]",
                    RegexOptions.Multiline);

            phvalue = regexLinks.Replace(phvalue, RewriteUrlsInHtml);

            phvalue = phvalue.
                Replace("[h1]", "<h1>").
                Replace("[/h1]", "</h1>").
                Replace("[h2]", "<h2>").
                Replace("[/h2]", "</h2>").
                Replace("[h3]", "<h3>").
                Replace("[/h3]", "</h3>").
                Replace("[b]", "<b>").
                Replace("[/b]", "</b>").
                Replace("[blockquote]", "<blockquote>").
                Replace("[/blockquote]", "</blockquote>").
                Replace("[i]", "<i>").
                Replace("[/i]", "</i>").
                Replace("[br]", "<br>");
            return phvalue;
        }

        private static string RewriteUrlsInHtml(Match match)
        {
            return "<a href=\"" + match.Groups["link"].Value + "\" >" + match.Groups["description"].Value + "</a>";
        }

        //private static string RewriteUrlsInText (Match match)
        //{
        //    if (match.Groups["description"].Value.Trim().ToLower() != match.Groups["link"].Value.Trim().ToLower())
        //        return match.Groups["description"].Value + " (" + match.Groups["link"].Value + ")";
        //    else
        //        return match.Groups["description"].Value;
        //}

        #endregion
    }

    #region Specific types of mailtemplates as subclasses of TypedMailTemplate

    public class MemberMail : TypedMailTemplate
    {
        public MemberMail()
        {
            AddPlaceHolder(new PlaceHolder("Subject", "Subject", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("BodyContent", "BodyContent", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("OrgName", "OrgName", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("GeographyName", "GeographyName", PlaceholderType.TagInSpan));
            AddPlaceHolder(new PlaceHolder("MemberNumberWithCheck", "MemberNumberWithCheckDigit", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("MemberNumber", "MemberNumber", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("HexIdentifier", "HexIdentifier", PlaceholderType.Tag));
        }

        public string pSubject
        {
            set { SetPlaceHolder("Subject", value); }
        }

        public string pBodyContent
        {
            set { SetPlaceHolder("BodyContent", value); }
        }

        public string pOrgName
        {
            set { SetPlaceHolder("OrgName", value); }
        }

        public string pGeographyName
        {
            set { SetPlaceHolder("GeographyName", value); }
        }

        private string MemberNumberWithCheck
        {
            set { SetPlaceHolder("MemberNumberWithCheck", value); }
        }

        private string MemberNumber
        {
            set { SetPlaceHolder("MemberNumber", value); }
        }

        private string HexIdentifier
        {
            set { SetPlaceHolder("HexIdentifier", value); }
        }

        public override void FillIndividualPlaceholders(IEmailPerson personObject)
        {
            if (!(personObject is Person))
                throw new ArgumentException(BaseName +
                                            ".MakeIndividualReplacements called with wrong type of person object");
            Person person = (Person) personObject;
            MemberNumberWithCheck = CheckDigit.AppendCheckDigit(person.Identity).ToString();
            HexIdentifier = person.HexIdentifier();
            MemberNumber = person.PersonId.ToString();
        }
    }

    public class WelcomeMail : MemberMail
    {
    }

    public class OfficerMail : MemberMail
    {
    }

    public class ActivistMail : MemberMail
    {
    }

    public class ExpiredMail : TypedMailTemplate
    {
        public ExpiredMail()
        {
            AddPlaceHolder(new PlaceHolder("Subject", "Subject", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("Memberships", "Memberships", PlaceholderType.TagInSpan));
            AddPlaceHolder(new PlaceHolder("MemberPhoneNumber", "MemberPhoneNumber", PlaceholderType.TagInSpan));
            AddPlaceHolder(new PlaceHolder("OrgName", "OrgName", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("StdRenewLink", "StdRenewLink", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("MemberNumberWithCheck", "MemberNumberWithCheckDigit", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("MemberNumber", "MemberNumber", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("HexIdentifier", "HexIdentifier", PlaceholderType.Tag));
        }

        public string pSubject
        {
            set { SetPlaceHolder("Subject", value); }
        }

        public string pMemberships
        {
            set { SetPlaceHolder("Memberships", value); }
        }

        public string pOrgName
        {
            set { SetPlaceHolder("OrgName", value); }
        }

        public string pStdRenewLink
        {
            set { SetPlaceHolder("StdRenewLink", value); }
        }

        private string MemberPhoneNumber
        {
            set { SetPlaceHolder("MemberPhoneNumber", value); }
        }

        private string MemberNumberWithCheck
        {
            set { SetPlaceHolder("MemberNumberWithCheck", value); }
        }

        private string MemberNumber
        {
            set { SetPlaceHolder("MemberNumber", value); }
        }

        private string HexIdentifier
        {
            set { SetPlaceHolder("HexIdentifier", value); }
        }

        public override void FillIndividualPlaceholders(IEmailPerson personObject)
        {
            try
            {
                if (!(personObject is Person))
                    throw new ArgumentException(BaseName +
                                                ".MakeIndividualReplacements called with wrong type of person object");
                Person person = (Person) personObject;
                MemberNumberWithCheck = CheckDigit.AppendCheckDigit(person.Identity).ToString();
                HexIdentifier = person.HexIdentifier();
                MemberNumber = person.PersonId.ToString();
                if (People.FromPhoneNumber(person.Country.Code, person.Phone).Count == 1)
                {
                    MemberPhoneNumber = person.Phone;
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Failed in FillIndividualPlaceholders person is " + personObject == null
                        ? "null"
                        : personObject.Identity.ToString(), e);
            }
        }
    }

    public class ReminderMail : TypedMailTemplate
    {
        public ReminderMail()
        {
            AddPlaceHolder(new PlaceHolder("Subject", "Subject", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("Preamble", "Preamble", PlaceholderType.Id));
            AddPlaceHolder(new PlaceHolder("ExpirationDate", "ExpirationDate", PlaceholderType.Tag, typeof (DateTime),
                "d"));
            AddPlaceHolder(new PlaceHolder("NextDate", "NextDate", PlaceholderType.Tag, typeof (DateTime), "d"));

            AddPlaceHolder(new PlaceHolder("TooOldForYouthOrgSpan", "TooOldForYouthOrgSpan(SPAN)", PlaceholderType.Id));
            AddPlaceHolder(new PlaceHolder("CurrentOrg", "CurrentOrg", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("OtherOrg", "OtherOrg", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("CurrentAge", "Age this year", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("OtherRenewLink", "OtherOrgRenewLink", PlaceholderType.Tag));

            AddPlaceHolder(new PlaceHolder("WrongOrgSpan", "WrongOrgSpan(SPAN)", PlaceholderType.Id));
            AddPlaceHolder(new PlaceHolder("GeographyName", "GeographyName", PlaceholderType.Tag));

            AddPlaceHolder(new PlaceHolder("StdRenewLink", "StdRenewLink", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("MemberNumberWithCheck", "MemberNumberWithCheckDigit", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("MemberNumber", "MemberNumber", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("HexIdentifier", "HexIdentifier", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("OrgName", "OrgName", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("TerminateLink", "TerminateLink(Tag in SPAN)", PlaceholderType.TagInSpan));
        }

        public string pSubject
        {
            set { SetPlaceHolder("Subject", value); }
        }

        public DateTime pExpirationDate
        {
            set { SetPlaceHolder("ExpirationDate", value); }
        }

        public DateTime pNextDate
        {
            set { SetPlaceHolder("NextDate", value); }
        }

        public string pTooOldForYouthOrgSpan
        {
            set { SetPlaceHolder("TooOldForYouthOrgSpan", value); }
        }

        public string pCurrentOrg
        {
            set { SetPlaceHolder("CurrentOrg", value); }
        }

        public string pOtherOrg
        {
            set { SetPlaceHolder("OtherOrg", value); }
        }

        public string pCurrentAge
        {
            set { SetPlaceHolder("CurrentAge", value); }
        }

        public string pOtherRenewLink
        {
            set { SetPlaceHolder("OtherRenewLink", value); }
        }

        public string pWrongOrgSpan
        {
            set { SetPlaceHolder("WrongOrgSpan", value); }
        }

        public string pGeographyName
        {
            set { SetPlaceHolder("GeographyName", value); }
        }

        public string pStdRenewLink
        {
            set { SetPlaceHolder("StdRenewLink", value); }
        }

        public string pOrgName
        {
            set { SetPlaceHolder("OrgName", value); }
        }

        public string pPreamble
        {
            set { SetPlaceHolder("Preamble", value); }
        }

        public string pTerminateLink
        {
            set { SetPlaceHolder("TerminateLink", value); }
        }

        private string MemberNumberWithCheck
        {
            set { SetPlaceHolder("MemberNumberWithCheck", value); }
        }

        private string MemberNumber
        {
            set { SetPlaceHolder("MemberNumber", value); }
        }

        private string HexIdentifier
        {
            set { SetPlaceHolder("HexIdentifier", value); }
        }

        public override void FillIndividualPlaceholders(IEmailPerson personObject)
        {
            if (!(personObject is Person))
                throw new ArgumentException(BaseName +
                                            ".MakeIndividualReplacements called with wrong type of person object");
            Person person = (Person) personObject;
            MemberNumberWithCheck = CheckDigit.AppendCheckDigit(person.Identity).ToString();
            HexIdentifier = person.HexIdentifier();
            MemberNumber = person.PersonId.ToString();
        }
    }

    public class PressReleaseMail : TypedMailTemplate
    {
        public PressReleaseMail()
        {
            AddPlaceHolder(new PlaceHolder("Subject", "Subject", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("BodyContent", "BodyContent", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("Date", "Date", PlaceholderType.Tag, typeof (DateTime), "d"));
            AddPlaceHolder(new PlaceHolder("Time", "Time", PlaceholderType.Tag, typeof (DateTime), "t"));
            AddPlaceHolder(new PlaceHolder("PostedToCategories", "PostedToCategories", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("ReporterName", "ReporterName", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("ReporterEmail", "ReporterEmail", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("ReporterCategories", "ReporterCategories", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("OrgName", "OrgName", PlaceholderType.Tag));
        }

        public string pSubject
        {
            set { SetPlaceHolder("Subject", value); }
        }

        public string pBodyContent
        {
            set { SetPlaceHolder("BodyContent", value); }
        }

        public DateTime pDate
        {
            set
            {
                SetPlaceHolder("Date", value);
                SetPlaceHolder("Time", value);
            }
        }

        public string pPostedToCategories
        {
            set { SetPlaceHolder("PostedToCategories", value); }
        }

        public string pOrgName
        {
            set { SetPlaceHolder("OrgName", value); }
        }

        private string ReporterName
        {
            set { SetPlaceHolder("ReporterName", value); }
        }

        private string ReporterEmail
        {
            set { SetPlaceHolder("ReporterEmail", value); }
        }

        private string ReporterCategories
        {
            set { SetPlaceHolder("ReporterCategories", value); }
        }

        public override void FillIndividualPlaceholders(IEmailPerson personObject)
        {
            if (personObject is Reporter)
            {
                Reporter person = (Reporter) personObject;
                ReporterEmail = person.Email;
                ReporterName = person.Name;
                ReporterCategories = GetConcatenatedCategoryString(person.MediaCategories);
            }
            else if (personObject is Person)
            {
                Person person = (Person) personObject;
                ReporterEmail = person.Mail;
                ReporterName = person.Name;
                ReporterCategories = "Piratpartiet/Funktionär";
            }
            else
                throw new ArgumentException(BaseName +
                                            ".MakeIndividualReplacements called with wrong type of person object");
        }

        public static string GetConcatenatedCategoryString(MediaCategories categories)
        {
            if (categories.Count == 0)
                return "";
            string result = categories[0].Name;

            for (int index = 1; index < categories.Count - 1; index++)
            {
                result += ", " + categories[index].Name;
            }

            if (categories.Count > 1)
            {
                //TODO: Translate "och"
                result += " och " + categories[categories.Count - 1].Name;
            }

            return result;
        }
    }

    public class NewsletterMail : TypedMailTemplate
    {
        public NewsletterMail()
        {
            AddPlaceHolder(new PlaceHolder("Subject", "Subject", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("BodyContent", "BodyContent", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("Date", "Date", PlaceholderType.Tag, typeof (DateTime), "d MMMM yyyy"));
            AddPlaceHolder(new PlaceHolder("ForumPostUrl", "ForumPostUrl", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("OrgName", "OrgName", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("MemberNumberWithCheck", "MemberNumberWithCheckDigit", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("MemberNumber", "MemberNumber", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("HexIdentifier", "HexIdentifier", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("Name", "Name", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("NameUrl", "NameUrl", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("EmailBase64Url", "EmailBase64Url", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("PhoneBase64Url", "PhoneBase64Url", PlaceholderType.Tag));
        }

        public string pSubject
        {
            set { SetPlaceHolder("Subject", value); }
        }

        public string pBodyContent
        {
            set { SetPlaceHolder("BodyContent", value); }
        }

        public DateTime pDate
        {
            set { SetPlaceHolder("Date", value); }
        }

        public string pForumPostUrl
        {
            set { SetPlaceHolder("ForumPostUrl", value); }
        }

        public string pOrgName
        {
            set { SetPlaceHolder("OrgName", value); }
        }

        private string MemberNumberWithCheck
        {
            set { SetPlaceHolder("MemberNumberWithCheck", value); }
        }

        private string MemberNumber
        {
            set { SetPlaceHolder("MemberNumber", value); }
        }

        private string HexIdentifier
        {
            set { SetPlaceHolder("HexIdentifier", value); }
        }

        private string Name
        {
            set { SetPlaceHolder("Name", value); }
        }

        private string NameUrl
        {
            set { SetPlaceHolder("NameUrl", value); }
        }

        private string EmailBase64Url
        {
            set { SetPlaceHolder("EmailBase64Url", value); }
        }

        private string PhoneBase64Url
        {
            set { SetPlaceHolder("PhoneBase64Url", value); }
        }

        public override void FillIndividualPlaceholders(IEmailPerson personObject)
        {
            if (!(personObject is Person))
                throw new ArgumentException(BaseName +
                                            ".MakeIndividualReplacements called with wrong type of person object");
            Person person = (Person) personObject;
            MemberNumberWithCheck = CheckDigit.AppendCheckDigit(person.Identity).ToString();
            HexIdentifier = person.HexIdentifier();
            MemberNumber = person.PersonId.ToString();
            Name = person.Name;
            NameUrl = HttpUtility.UrlEncode(person.Name);
            EmailBase64Url = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(person.Mail)));
            PhoneBase64Url = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(person.Phone)));
        }
    }

    public class ChangeOrgMail : TypedMailTemplate
    {
        /* 
        Hej,
 
        Du är idag medlem i %CurrentOrg%.
        <span id=InactiveOrg> 
        Den föreningen är numera inaktiv, och vi vill därför föreslå att du flyttar ditt medlemskap till %InactiveOrg%
        </span>
        <span id=ChangeOrg> 
        Lokalavdelningen Y är en aktiv lokalavdelning och ligger dig närmare geografiskt. Vi vill därmed rekommendera dig till att byta till %ChangeOrg% 
        </span>
        ,samtidigt som du förlänger ditt medlemskap så att det gäller till och med %NextDate%. 
        <span id=NoLocalOrg> 
        Det finns tyvärr ingen lokal förening som tar hand om orten där du bor. %NoLocalOrg% är en förening 
        som organiserar våra medlemmar på orter där det saknas lokal organisation.
        </span>

        Tryck på länken  under för att göra detta.

        %StdRenewLink%
        
        Förslaget till ny lokalavdelning bygger på att medlemsregistret säger att du bor i %CurrentGeo%.
        Har du flyttat men inte ändrat din adress i vårt medlemsregister kan du göra det på adressen https://pirateweb.net/Pages/v4/v3/Account/BasicDetails.aspx

        Det här mailet är ett automatiskt mail som skickas ut med jämna mellanrum för att säkerställa att du är medlem i en aktiv och geografiskt nära lokalavdelning. 

        Kontakta medlemsservice@piratpartiet.se för frågor om ditt medlemskap. 
         
        <span id=InactiveQuestions> 
        Berör frågan varför en tidigare lokalavdelning inte längre är aktiv, eller om du är intresserad av att hjälpa till att dra igång verksamhet, hör av dig till info@ungpirat.se
        </span>
         
        Hälsningar
        Medlemssystemet
        */

        public ChangeOrgMail()
        {
            AddPlaceHolder(new PlaceHolder("Subject", "Subject", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("CurrentOrg", "CurrentOrg", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("InactiveOrg", "InactiveOrg(Tag in SPAN)", PlaceholderType.TagInSpan));
            AddPlaceHolder(new PlaceHolder("ChangeOrg", "ChangeOrg(Tag in SPAN)", PlaceholderType.TagInSpan));
            AddPlaceHolder(new PlaceHolder("NoLocalOrg", "NoLocalOrg(Tag in SPAN)", PlaceholderType.TagInSpan));
            AddPlaceHolder(new PlaceHolder("NextDate", "NextDate", PlaceholderType.Tag, typeof (DateTime), "d"));
            AddPlaceHolder(new PlaceHolder("CurrentGeo", "CurrentGeo", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("StdRenewLink", "StdRenewLink", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("InactiveEnding", "InactiveEnding(SPAN)", PlaceholderType.Id));

            AddPlaceHolder(new PlaceHolder("MemberNumberWithCheck", "MemberNumberWithCheckDigit", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("MemberNumber", "MemberNumber", PlaceholderType.Tag));
            AddPlaceHolder(new PlaceHolder("HexIdentifier", "HexIdentifier", PlaceholderType.Tag));
        }

        public string pSubject
        {
            set { SetPlaceHolder("Subject", value); }
        }

        public string pCurrentOrg
        {
            set { SetPlaceHolder("CurrentOrg", value); }
        }

        public string pInactiveOrg
        {
            set { SetPlaceHolder("InactiveOrg", value); }
        }

        public string pChangeOrg
        {
            set { SetPlaceHolder("ChangeOrg", value); }
        }

        public string pNoLocalOrg
        {
            set { SetPlaceHolder("NoLocalOrg", value); }
        }

        public DateTime pNextDate
        {
            set { SetPlaceHolder("NextDate", value); }
        }

        public string pCurrentGeo
        {
            set { SetPlaceHolder("CurrentGeo", value); }
        }

        public string pStdRenewLink
        {
            set { SetPlaceHolder("StdRenewLink", value); }
        }

        public string pInactiveEnding
        {
            set { SetPlaceHolder("InactiveEnding", value); }
        }

        private string MemberNumberWithCheck
        {
            set { SetPlaceHolder("MemberNumberWithCheck", value); }
        }

        private string MemberNumber
        {
            set { SetPlaceHolder("MemberNumber", value); }
        }

        private string HexIdentifier
        {
            set { SetPlaceHolder("HexIdentifier", value); }
        }

        public override void FillIndividualPlaceholders(IEmailPerson personObject)
        {
            if (!(personObject is Person))
                throw new ArgumentException(BaseName +
                                            ".MakeIndividualReplacements called with wrong type of person object");
            Person person = (Person) personObject;
            MemberNumberWithCheck = CheckDigit.AppendCheckDigit(person.Identity).ToString();
            HexIdentifier = person.HexIdentifier();
            MemberNumber = person.PersonId.ToString();
        }
    }

    #endregion
}