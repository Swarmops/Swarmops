using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;

namespace Activizr.Basic.Types
{
    /// <summary>
    /// This class encapsulates what a financial transaction was constructed from.
    /// </summary>
    public class BasicFinancialDependency
    {
        public BasicFinancialDependency (int objectId, FinancialDependencyType dependencyType, int foreignId)
        {
            this.ObjectId = objectId;
            this.DependencyType = dependencyType;
            this.ForeignId = foreignId;
        }

        public int ObjectId { get; private set; }
        public FinancialDependencyType DependencyType { get; private set; }
        public int ForeignId { get; private set; }
    }
}
