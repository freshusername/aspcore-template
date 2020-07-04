using System;

namespace service.Exceptions
{
    public class InvalidActionException : Exception
    {
        public InvalidActionException(string message) : base(message)
        {
        }
    }
}
