using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Linq;

namespace interview_log.Models
{
    public enum State : byte 
    {
        Unknown, Accepted, Postponed
    };
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public User()
        {
            Attachments = new HashSet<Attachment>();
            Comments = new HashSet<Comment>();
        }
        public string Email {get; set;}
        public string Position { get; set; }
        public byte State { get; set; }
        public bool Admin { get; set; }
        public bool Interviewer { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public IEnumerable<Attachment> Images()
        {
            return Attachments.Where(item => item.Type == Type.Image);
        }
        public IEnumerable<Attachment> Files()
        {
            return Attachments.Where(item => item.Type == Type.File);
        }
    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}