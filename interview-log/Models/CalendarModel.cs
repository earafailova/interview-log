using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;



namespace interview_log.Models
{
    public class Calendar
    {
        private const string  DefaultAddress = @"qknusl2s3m2nau74obbr804fjk";
        public string CalendarAddress { get;
               set;
           // { 
                  //validation is here
           // }
        }

       public Calendar()
        {
            String path = HttpContext.Current.Server.MapPath(@"~\calendar_address.txt");
            try
            {
                this.CalendarAddress = System.IO.File.ReadAllText(path);
            }
           catch(System.IO.FileNotFoundException)
            {
                StreamWriter file = new StreamWriter(path);
                file.Write(DefaultAddress);
                file.Close();
                this.CalendarAddress = DefaultAddress;
            }
        }


    }
}