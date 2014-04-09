using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace interview_log.Models
{
    public class Interview
    {
        public Interview() 
        {
            Users = new HashSet<User>();
        }
        public Interview(DateTime date, string eventId)
        {
            Date = date;
            Id = eventId;
            Users = new HashSet<User>();
        }

        [Key]
        public string Id { get; private set; }
        public DateTime Date { get; set; }

        public virtual ICollection<User> Users { get; set; }
       
    }
}