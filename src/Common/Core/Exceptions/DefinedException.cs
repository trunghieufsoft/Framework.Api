using System;
using Common.Core.Models;
using Common.Core.Enumerations;

namespace Common.Core.Exceptions
{
    public class DefinedException : Exception
    {
        public ErrorModel Error { get; set; }

        public DefinedException()
        {
            Error = new ErrorModel();
        }

        public DefinedException(ErrorCodeEnum error, dynamic data)
        {
            Error = new ErrorModel { ErrorCode = (int)error, Message = error.ToString(), Data = data };
        }

        public DefinedException(ErrorCodeEnum error)
        {
            Error = new ErrorModel { ErrorCode = (int)error, Message = error.ToString() };
        }
    }
}
