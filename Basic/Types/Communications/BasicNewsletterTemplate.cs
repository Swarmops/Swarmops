namespace Swarmops.Basic.Types.Communications
{
    public class BasicNewsletterTemplate
    {
        public string HtmlTemplate;
        public string TextTemplate;

        public BasicNewsletterTemplate (string htmlTemplate, string textTemplate)
        {
            this.HtmlTemplate = htmlTemplate;
            this.TextTemplate = textTemplate;
        }
    }
}