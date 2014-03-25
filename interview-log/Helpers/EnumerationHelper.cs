using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace interview_log.Helpers
{
    public enum Purpose
    {
        NewAdmin, NewAddress
    }
    
    public static class EnumerationHelper
    {
        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }
    }
}