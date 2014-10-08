namespace Swarmops.Basic.Types
{
    public class BasicNewsletterTemplate
    {
        public BasicNewsletterTemplate (string htmlTemplate, string textTemplate)
        {
            this.HtmlTemplate = htmlTemplate;
            this.TextTemplate = textTemplate;
        }

        public string HtmlTemplate;
        public string TextTemplate;
    }
}