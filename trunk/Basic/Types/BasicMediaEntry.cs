using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicMediaEntry : IHasIdentity
    {
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
            : this(
                original.id, original.keywordId, original.mediaName, original.isBlog, original.title, original.url,
                original.dateTime)
        {
        }

        public BasicMediaEntry()
            : this(0, 0, string.Empty, false, string.Empty, string.Empty, DateTime.MinValue)
        {
        }

        public int Id
        {
            get { return id; }
        }

        public int KeywordId
        {
            get { return keywordId; }
        }

        public string Title
        {
            get { return title; }
        }

        public string MediaName
        {
            get { return this.mediaName; }
        }


        public string Url
        {
            get { return url; }
        }

        public DateTime DateTime
        {
            get { return dateTime; }
        }

        public bool IsBlog
        {
            get { return this.isBlog; }
        }

        private int id;
        private DateTime dateTime;
        private string title;
        private string mediaName;
        private string url;
        private int keywordId;
        private bool isBlog;

        #region IHasIdentity Members

        public int Identity
        {
            get { return Id; }
        }

        #endregion
    }
}