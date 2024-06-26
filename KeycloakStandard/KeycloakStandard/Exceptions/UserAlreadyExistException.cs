using System;

namespace KeycloakStandard.Exceptions
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException()
        {
        }

        public UserAlreadyExistException(string message)
            : base(message)
        {
        }

        public UserAlreadyExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
