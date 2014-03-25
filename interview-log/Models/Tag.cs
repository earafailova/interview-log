using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace interview_log.Models
{
    public class Tag
    {
        public Tag() { }
        public Tag(string name, string info = "superman in all branches of IT")
        {
            Name = name;
            Info = info;
        }
        [Key] 
        public string Name { get; set; }
        public string Info { get; set; }
    }
}