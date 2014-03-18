using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace interview_log.Models
{
    public enum State : byte 
    {
        Unknown, Accepted, Postopen
    };
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        [NotMapped]
        public HttpPostedFileBase File {get; set;}
        public string Email {get; set;}
        public string Position { get; set; }
        public byte State { get; set; }
        public bool Admin { get; set; }
        public bool Interviewer { get; set; }
        

    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}