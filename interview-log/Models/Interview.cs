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
        public Interview(DateTime date)
        {
            Date = date;
            Id = Guid.NewGuid();
            Users = new HashSet<User>();
        }

        [Key]
        public Guid Id { get; private set; }
        public DateTime Date { get; set; }

        public virtual ICollection<User> Users { get; set; }
       
    }
}