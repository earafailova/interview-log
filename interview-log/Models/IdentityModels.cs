using Microsoft.AspNet.Identity.EntityFramework;

namespace interview_log.Models
{
    enum State : byte 
    {
        Uknown, Accepted, Postopen
    };
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        private string email {get; set;}
        private string position { get; set; }
        private State state { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}