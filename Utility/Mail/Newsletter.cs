using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.Mail
{
    public class Newsletter
    {
        public Newsletter (int templateId, string senderName, string senderAddress, string title, string body,
                           People recipients)
        {
            this.TemplateId = templateId;
            this.SenderName = senderName;
            this.SenderAddress = senderAddress;
            this.Title = title;
            this.Body = body;
            this.Recipients = recipients;
        }

        public readonly int TemplateId;
        public readonly string SenderName;
        public readonly string SenderAddress;
        public readonly string Title;
        public readonly string Body;
        public readonly People Recipients;
    }
}