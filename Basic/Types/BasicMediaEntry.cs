using System;
using Swarmops.Common;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicMediaEntry : IHasIdentity
    {
        private readonly DateTime dateTime;
        private readonly int id;
        private readonly bool isBlog;
        private readonly int keywordId;
        private readonly string mediaName;
        private readonly string title;
        private readonly string url;

        #region IHasIdentity Members

        public int Identity
        {
            get { return Id; }
        }

        #endregion

        public BasicMediaEntry (int id, int keywordId, string mediaName, bool isBlog, string title, string url,
            DateTime dateTime)
        {
            this.id = id;
            this.keywordId = keywordId;
            this.mediaName = mediaName;
            this.isBlog = isBlog;
            this.title = title;
            this.url = url;
            this.dateTime = dateTime;
        }

        public BasicMediaEntry (BasicMediaEntry original)
            : this (
                original.id, original.keywordId, original.mediaName, original.isBlog, original.title, original.url,
                original.dateTime)
        {
        }

        public BasicMediaEntry()
            : this (0, 0, string.Empty, false, string.Empty, string.Empty, Constants.DateTimeLow)
        {
        }

        public int Id
        {
            get { return this.id; }
        }

        public int KeywordId
        {
            get { return this.keywordId; }
        }

        public string Title
        {
            get { return this.title; }
        }

        public string MediaName
        {
            get { return this.mediaName; }
        }


        public string Url
        {
            get { return this.url; }
        }

        public DateTime DateTime
        {
            get { return this.dateTime; }
        }

        public bool IsBlog
        {
            get { return this.isBlog; }
        }
    }
}