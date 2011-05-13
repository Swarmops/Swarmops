using System;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Exceptions;
using Activizr.Basic.Interfaces;


namespace Activizr.Basic.Interfaces
{
    public interface IHandleProvider
    {
        string GetPersonHandle (int personId);
        void SetPersonHandle (int personId, string newHandle);
        HandleErrorType CanSetHandle (string newHandle);

        int GetPersonByHandle (string handle);
    }
}

namespace Activizr.Basic.Enums
{
    public enum HandleErrorType
    {
        Unknown = 0,
        NoError,
        HandleNotFound,
        HandleOccupied
    }
}

namespace Activizr.Basic.Exceptions
{
    public class HandleException : Exception
    {
        public HandleException (string attemptedHandle, HandleErrorType errorType)
        {
            this.attemptedHandle = attemptedHandle;
            this.errorType = errorType;
        }


        public override string ToString()
        {
            return "HandleException: Handle '" + attemptedHandle + "' caused '" + errorType.ToString() + "'.\r\n" +
                   base.ToString();
        }


        public HandleErrorType ErrorType
        {
            get { return errorType; }
        }

        private readonly string attemptedHandle;
        private readonly HandleErrorType errorType;
    }
}