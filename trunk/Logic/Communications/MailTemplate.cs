using Activizr.Logic.Cache;
using Activizr.Logic.Structure;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Pirates;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System;
using System.Web;

namespace Activizr.Logic.Communications
{
    public class MailTemplate : BasicMailTemplate
    {
        private string originallyLoadedContent = "";
        private HtmlDocument htmlDoc = null;

        internal MailTemplate ()
            : base(0, "", "", "", 0, "")
        {
        }

        public MailTemplate (BasicMailTemplate basic)
            : base(basic)
        {
            originallyLoadedContent = basic.TemplateBody;
        }

        public void ResetContent ()
        {
            this.htmlDoc = null;
            this.TemplateBody = originallyLoadedContent;
        }

        public void NormalizeHtml ()
        {
            //Make sure there is a full html document defined
            this.htmlDoc = NormalizeHtmlExtracted(this.HtmlDoc, this.TemplateName.EndsWith("Plain"));
        }

        private HtmlDocument NormalizeHtmlExtracted (HtmlDocument htDoc, bool forPlainText)
        {
            if (htDoc == null)
            {
                throw new Exception("NormalizeHtmlExtracted was called with null document:\nTemplateBody:" + this.TemplateBody);
            }

            if (htDoc.DocumentNode == null)
            {
                throw new Exception("NormalizeHtmlExtracted was called with null DocumentNode:\nTemplateBody:" + this.TemplateBody);
            }

            string newHtml = "";

            HtmlNode headNode = htDoc.DocumentNode.SelectSingleNode("//head");
            HtmlNode bodyNode = htDoc.DocumentNode.SelectSingleNode("//body");
            HtmlNode htmlNode = htDoc.DocumentNode.SelectSingleNode("//html");
            HtmlNode titleNode = htDoc.DocumentNode.SelectSingleNode("//head/title");
            if (bodyNode == null && headNode == null && htmlNode == null)
                newHtml = "<html><head><title>Untitled</title></head><body>" + htDoc.DocumentNode.OuterHtml + "</body></html>";
            else if (bodyNode != null && headNode == null && htmlNode == null)
                newHtml = "<html><head><title>Untitled</title></head>" + bodyNode.OuterHtml + "</html>";
            else if (bodyNode != null && headNode != null && htmlNode == null)
                newHtml = "<html>" + bodyNode.OuterHtml + "</html>";
            else if (headNode == null && htmlNode != null)
            {
                headNode = htDoc.CreateElement("head");
                htmlNode.InsertAfter(headNode, null);
                titleNode = htDoc.CreateElement("title");
                titleNode.InnerHtml = "Untitled";
                headNode.AppendChild(titleNode);
            }
            else if (titleNode == null && headNode != null)
            {
                titleNode = htDoc.CreateElement("title");
                titleNode.InnerHtml = "Untitled";
                headNode.AppendChild(titleNode);
            }

            if (newHtml != "")
            {
                htDoc = new HtmlDocument();
                htDoc.LoadHtml(newHtml);
            }

            if (htDoc.DocumentNode == null)
            {
                throw new Exception("NormalizeHtmlExtracted Document Node is null for " + newHtml);
            }

            //"Plain" templates should have the whole body in a pre tag
            HtmlNode bodyNode2 = htDoc.DocumentNode.SelectSingleNode("//body");

            if (bodyNode2 == null)
            {
                throw new Exception("NormalizeHtmlExtracted bodyNode2 is null for " + newHtml);
            }

            int preIx = 0;
            for (preIx = 0; preIx < bodyNode2.ChildNodes.Count; ++preIx)
            {
                if (bodyNode2.ChildNodes[preIx].Name.ToLower() == "pre") break;
            }

            if (forPlainText && preIx == bodyNode2.ChildNodes.Count)
            {
                bodyNode2.InsertAfter(htDoc.CreateElement("pre"), null);
                for (int i = bodyNode2.ChildNodes.Count - 1; i > 0; --i)
                {
                    HtmlNode nodeToMove = bodyNode2.ChildNodes[i];
                    bodyNode2.RemoveChild(nodeToMove);
                    bodyNode2.FirstChild.InsertAfter(nodeToMove, null);
                }
                string html = htDoc.DocumentNode.OuterHtml;
                htDoc = new HtmlDocument();
                htDoc.LoadHtml(html);
            }

            return htDoc;
        }

        public void MarkPlaceholderSpans ()
        {
            MarkPlaceholderSpans(this.HtmlDoc);
        }

        public void MarkPlaceholderSpans (Dictionary<string, string> allowedPlaceholders)
        {
            this.MarkPlaceholderSpans();
            HtmlNodeCollection allTags = this.HtmlBody.SelectNodes("//*[@class='placeholder2']");
            Regex reCleanup = new Regex(@"[\s%]");
            if (allTags != null)
            {
                foreach (HtmlNode n in allTags)
                {
                    string key = reCleanup.Replace(n.InnerText, "");
                    string keyLow = key.ToLower();
                    if (!allowedPlaceholders.ContainsKey(keyLow))
                    {
                        string classname = n.GetAttributeValue("class", "");
                        classname = classname.Replace("placeholder2", "placeholderUnkn");
                        n.SetAttributeValue("class", classname);
                    }
                    else if (allowedPlaceholders[keyLow] != key
                        && allowedPlaceholders[keyLow].ToUpper() != key
                        && allowedPlaceholders[keyLow].ToLower() != key)
                    {
                        n.InnerHtml = "%" + allowedPlaceholders[keyLow] + "%";
                    }
                }
            }
        }

        public static void MarkPlaceholderSpans (HtmlDocument htDoc)
        {
            //Code below is for inserting span's around %tag% placeholders so they can be redered with bg-color in editor
            //(It is a bit hairy, because it is meddling with the same structure it is looping on
            //and that gives some strange effects...)
            Regex reTags = new Regex(@"(%[a-zедц0-9]+%)", RegexOptions.IgnoreCase);
            HtmlNodeCollection allWithTextNodes = htDoc.DocumentNode.SelectNodes("//body//text()/..");//select nodes that have text below them
            if (allWithTextNodes != null)
            {
                foreach (HtmlNode n in allWithTextNodes)
                {
                    HtmlNodeCollection allTextNodes = n.SelectNodes("./text()");//select the #text children
                    if (allTextNodes != null)
                    {
                        foreach (HtmlNode tn in allTextNodes)
                        {
                            if (tn.PreviousSibling is HtmlTextNode)
                            {   //try to normalize textnodes, accumulating the text.
                                tn.InnerHtml = tn.PreviousSibling.InnerHtml + tn.InnerHtml;
                                tn.PreviousSibling.InnerHtml = "";
                            }

                            //insert span tags around %tag%, trying to avoid doing it twice.
                            if (reTags.IsMatch(tn.InnerText) && !n.OuterHtml.Contains("placeholder2") && !tn.OuterHtml.Contains("placeholder2"))
                            {
                                tn.InnerHtml = reTags.Replace(tn.InnerHtml, "<span class=\"placeholder2\">$1</span>");
                            }
                        }
                    }
                }
            }
        }

        public void RemovePlaceholderSpans ()
        {
            RemovePlaceholderSpans(this.HtmlDoc);
        }

        public static void RemovePlaceholderSpans (HtmlDocument htDoc)
        {
            HtmlNodeCollection placeholder2Spans = htDoc.DocumentNode.SelectNodes("//*[@class='placeholder2']");
            if (placeholder2Spans != null)
            {
                foreach (HtmlNode n in placeholder2Spans)
                {
                    n.ParentNode.RemoveChild(n, true);
                }
            }
            placeholder2Spans = htDoc.DocumentNode.SelectNodes("//*[@class='placeholderUnkn']");
            if (placeholder2Spans != null)
            {
                foreach (HtmlNode n in placeholder2Spans)
                {
                    n.ParentNode.RemoveChild(n, true);
                }
            }
        }

        public new string TemplateBody
        {
            get
            {
                if (htmlDoc != null)
                {
                    return htmlDoc.DocumentNode.OuterHtml;
                }
                else
                    return base.TemplateBody;
            }

            set
            {
                try
                {
                    if (htmlDoc != null)
                    {
                        htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(value);
                    }
                }
                catch
                {
                    throw;
                }
                base.TemplateBody = value;
            }
        }

        public HtmlNode HtmlBody
        {
            get
            {
                if (HtmlDoc.DocumentNode == null
                    || HtmlDoc.DocumentNode.SelectSingleNode("//body") == null)
                {
                    this.NormalizeHtml();
                }
                return HtmlDoc.DocumentNode.SelectSingleNode("//body");
            }
        }

        public string Html
        {
            get
            {
                return HtmlDoc.DocumentNode.OuterHtml;
            }
        }

        public string PlainText
        {
            get
            {
                string htmlText = HtmlDoc.DocumentNode.OuterHtml;
                Regex re = new Regex(@"\r{0,1}\n +");

                //Templates for plain text are supposedly already in a reasonable <PRE> format
                if (this.TemplateName.EndsWith("Plain"))
                    htmlText = re.Replace(htmlText, "\r\n");
                else
                    htmlText = re.Replace(htmlText, " ");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlText);

                if (htmlDoc.DocumentNode == null)
                {
                    throw new Exception("MailTemplate.PlainText detected htmlDoc.DocumentNode== null:\nHTML:" + htmlText);
                }

                HtmlNodeCollection tagsArray = htmlDoc.DocumentNode.SelectNodes("//a");
                if (tagsArray != null)
                {
                    foreach (HtmlNode n in tagsArray)
                    {
                        if (n.InnerText.Trim().ToLower() != n.GetAttributeValue("href", "").Trim().ToLower())
                            n.AppendChild(htmlDoc.CreateTextNode(" (" + n.GetAttributeValue("href", "") + ")"));
                    }
                }

                tagsArray = htmlDoc.DocumentNode.SelectNodes("//h1");
                if (tagsArray != null)
                {
                    foreach (HtmlNode n in tagsArray)
                    {
                        n.InsertAfter(htmlDoc.CreateTextNode("\r\n\r\n"), null);
                    }
                }
                tagsArray = htmlDoc.DocumentNode.SelectNodes("//h2");
                if (tagsArray != null)
                {
                    foreach (HtmlNode n in tagsArray)
                    {
                        n.InsertAfter(htmlDoc.CreateTextNode("\r\n\r\n"), null);
                        n.AppendChild(htmlDoc.CreateTextNode("\r\n\r\n"));
                    }
                }

                tagsArray = htmlDoc.DocumentNode.SelectNodes("//h3");
                if (tagsArray != null)
                {
                    foreach (HtmlNode n in tagsArray)
                    {
                        n.InsertAfter(htmlDoc.CreateTextNode("\r\n\r\n"), null);
                        n.AppendChild(htmlDoc.CreateTextNode("\r\n\r\n"));
                    }
                }

                tagsArray = htmlDoc.DocumentNode.SelectNodes("//li");
                if (tagsArray != null)
                {
                    foreach (HtmlNode n in tagsArray)
                    {
                        n.InsertAfter(htmlDoc.CreateTextNode("\r\n\r\n- "), null);
                        n.AppendChild(htmlDoc.CreateTextNode("\r\n"));
                    }
                }

                tagsArray = htmlDoc.DocumentNode.SelectNodes("//tr");
                if (tagsArray != null)
                {
                    foreach (HtmlNode n in tagsArray)
                    {
                        n.InsertAfter(htmlDoc.CreateTextNode("\r\n"), null);
                    }
                }
                tagsArray = htmlDoc.DocumentNode.SelectNodes("//p");
                if (tagsArray != null)
                {
                    foreach (HtmlNode n in tagsArray)
                    {
                        n.InsertAfter(htmlDoc.CreateTextNode("\r\n\r\n"), null);
                        n.AppendChild(htmlDoc.CreateTextNode("\r\n"));
                    }
                }
                tagsArray = htmlDoc.DocumentNode.SelectNodes("//br");
                if (tagsArray != null)
                {
                    foreach (HtmlNode n in tagsArray)
                    {
                        n.AppendChild(htmlDoc.CreateTextNode("\r\n"));
                    }
                }
                HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
                string retval = "";

                if (bodyNode != null)
                    retval = htmlDoc.DocumentNode.SelectSingleNode("//body").InnerText;
                else
                    throw new Exception("ERROR: No body in document in MailTemplate.PlainText");


                re = new Regex(@"\r\n\s+(?=\r\n)"); //replace rows with only whitespace with empty
                retval = re.Replace(retval, "\r\n");

                re = new Regex(@"\r\n\r\n(\r\n)+");//replace three or more linefeeds with two.
                retval = re.Replace(retval, "\r\n\r\n");

                return HttpUtility.HtmlDecode(retval);
            }

        }

        public HtmlDocument HtmlDoc
        {
            get
            {
                if (htmlDoc == null)
                {
                    htmlDoc = new HtmlDocument();
                    try
                    {
                        htmlDoc.LoadHtml(base.TemplateBody);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to load Html in MailTemplate", ex);
                    }
                }
                return htmlDoc;
            }
        }

        public HtmlNode GetElementById (string id)
        {
            return HtmlDoc.DocumentNode.SelectSingleNode("//*[@id='" + id + "']");
        }

        public HtmlNodeCollection GetElementsById (string id)
        {
            return HtmlDoc.DocumentNode.SelectNodes("//*[@id='" + id + "']");
        }

        public void RemoveElementById (string id)
        {
            HtmlNodeCollection nodes = HtmlDoc.DocumentNode.SelectNodes("//*[@id='" + id + "']");
            if (nodes != null)
                foreach (HtmlNode node in nodes)
                {
                    node.ParentNode.RemoveChild(node);
                }
        }

        public void RemoveEnclosingTag (string id)
        {
            HtmlNodeCollection nodes = HtmlDoc.DocumentNode.SelectNodes("//*[@id='" + id + "']");
            if (nodes != null)
                foreach (HtmlNode node in nodes)
                {
                    node.ParentNode.RemoveChild(node,true);
                }
        }

        public bool ReplaceContentById (string id, string InnerHtml)
        {
            HtmlNodeCollection nodes = HtmlDoc.DocumentNode.SelectNodes("//*[@id='" + id + "']");
            bool retval = false;
            if (nodes != null)
                foreach (HtmlNode node in nodes)
                {
                    node.InnerHtml = InnerHtml;
                    node.ParentNode.RemoveChild(node,true);
                    retval = true;
                }

            return retval;
        }

        public void ReplaceContentByPattern (string pattern, string InnerHtml)
        {
            // Replace pattern tags taking case into account.
            Regex UpperPattern = new Regex("%" + pattern.ToUpper() + "%");
            Regex LowerPattern = new Regex("%" + pattern.ToLower() + "%");
            Regex JustPattern = new Regex("%" + pattern + "%", RegexOptions.IgnoreCase);
            string content = UpperPattern.Replace(this.TemplateBody, InnerHtml.ToUpper());
            content = LowerPattern.Replace(content, InnerHtml.ToLower());
            this.TemplateBody = JustPattern.Replace(content, InnerHtml);
        }

        private HtmlNode TitleNode ()
        {
            //Since NormalizeHtml is expensive try without first
            HtmlNode titlenode = null;
            if (this.HtmlDoc != null && this.HtmlDoc.DocumentNode != null)
                titlenode = this.HtmlDoc.DocumentNode.SelectSingleNode("//head/title");

            if (titlenode == null)
            {
                this.NormalizeHtml();
                titlenode = this.HtmlDoc.DocumentNode.SelectSingleNode("//head/title");
                if (titlenode == null)
                {
                    return null;
                }
            }

            titlenode.InnerHtml = titlenode.InnerHtml.Replace("&lt;", "<").Replace("&gt;", ">");
            return titlenode;
        }
  
        public string TemplateTitle
        {
            get
            {
                HtmlNode titleNode = TitleNode();
                if (titleNode != null)
                    return titleNode.InnerHtml;
                else
                    return "";
            }
            set
            {
                HtmlNode titleNode = TitleNode();
                if (titleNode != null)
                    titleNode.InnerHtml = value;
            }
        }

        public string TemplateTitleText
        {
            get
            {
                HtmlNode titleNode = TitleNode();
                if (titleNode != null)
                    return HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(titleNode.InnerText));
                else
                    return "";
            }

        }


        //Returns an empty template to be filled by the caller
        public static MailTemplate None
        {
            get
            {
                return new MailTemplate();
            }
        }


        #region Data layer access

        public static MailTemplate FromIdentity (int templateId)
        {
            BasicMailTemplate basic = PirateDb.GetDatabaseForReading().GetMailTemplateById(templateId);
            return FromBasic(basic);
        }

        internal static MailTemplate FromBasic (BasicMailTemplate basic)
        {
            MailTemplate templ = new MailTemplate(basic);
            return templ;
        }

        public static List<MailTemplate> GetAll ()
        {
            BasicMailTemplate[] basic = PirateDb.GetDatabaseForReading().GetAllMailTemplates();
            List<MailTemplate> resultlist = new List<MailTemplate>();
            foreach (BasicMailTemplate bt in basic)
            {
                resultlist.Add(FromBasic(bt));
            }

            return resultlist;
        }



        public static MailTemplate Create (
                                    string templateName,
                                    string languageCode,
                                    string countryCode,
                                    int organizationId,
                                    string templateBody)
        {
            int retId = PirateDb.GetDatabaseForWriting().SetMailTemplate(0,
                                                    templateName,
                                                    languageCode,
                                                    countryCode,
                                                    organizationId,
                                                    templateBody);
            MailTemplateCache.loadCache = true;

            return FromIdentity(retId);
        }

        public void Update ()
        {
            TemplateBody = TemplateBody; //to make sure the HTMlDoc is taken into account
            PirateDb.GetDatabaseForWriting().SetMailTemplate(this); // saves changes
            MailTemplateCache.loadCache = true;
        } 


        //----------------------------------------------//
        // Cached retreval

        public static MailTemplate FromNameCountryAndOrg (string name, string countrycode, int orgId)
        {
            if (name == "")
                return MailTemplate.None;

            Country country = Country.FromCode(countrycode);
            string lang = country.Culture.Substring(0, 2);
            Organization org = Organization.FromIdentity(orgId);

            BasicMailTemplate basic = MailTemplateCache.GetBestMatch(name, lang, countrycode, org);

            return FromBasic(basic);
        }


        public static MailTemplate FromNameAndOrg (string name, Organization sender)
        {
            if (name == "")
                return MailTemplate.None;

            Country country = sender.DefaultCountry;
            string countrycode = country.Code;
            string lang = country.Culture.Substring(0, 2);

            BasicMailTemplate basic = MailTemplateCache.GetBestMatch(name, lang, countrycode, sender);
            return FromBasic(basic);
        }
        #endregion

    }
}