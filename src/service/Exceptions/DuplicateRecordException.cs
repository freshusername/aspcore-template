using System;

namespace service.Exceptions
{
    public class DuplicateRecordException : Exception
    {
        public DuplicateRecordException()
        {
        }

        public DuplicateRecordException(string message) : base(message)
        {
        }

        public DuplicateRecordException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
