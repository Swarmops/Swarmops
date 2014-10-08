using System;

namespace Swarmops.Basic.Types
{
    public class BasicExceptionLog
    {
         public BasicExceptionLog (int ExceptionId,DateTime ExceptionDateTime, string Source, string ExceptionText)
        {
            this.ExceptionId = ExceptionId;
            this.ExceptionDateTime = ExceptionDateTime;
            this.Source = Source;
            this.ExceptionText = ExceptionText;
        }

        public BasicExceptionLog (BasicExceptionLog original):
             this(original.ExceptionId, original.ExceptionDateTime, original.Source, original.ExceptionText)
        {
            // empty copy ctor
        }

        public int ExceptionId { get; private set; }
        public DateTime ExceptionDateTime { get; private set; }
        public string Source { get; private set; }
        public string ExceptionText { get; private set; }
    }
}
