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

        public static IQueryable<User> Between(DateTime? first, DateTime? second)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var byTime = db.Interviews.Where(interview => interview.Date > first && interview.Date < second).SelectMany(t => t.Users);
            return byTime;
        }

        [Key]
        public Guid Id { get; private set; }
        public DateTime Date { get; set; }

        public virtual ICollection<User> Users { get; set; }
       
    }
}