using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace interview_log.Models
{
    public class File
    {
        public HttpPostedFileBase MyFile { get; set; }
    }
    
}