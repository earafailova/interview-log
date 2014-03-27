using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace interview_log.Exceptions
{
    public class UserDoesNotExistException : KeyNotFoundException
    {
        public UserDoesNotExistException(string msg) : base(msg) { }
    }
}