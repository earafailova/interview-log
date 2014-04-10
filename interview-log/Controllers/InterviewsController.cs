using interview_log.Exceptions;
using interview_log.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace interview_log.Controllers
{
    public class InterviewsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
       
        public ActionResult Index()
        {
            return View(db.Interviews.ToList());
        }

        public ActionResult DeleteInterview(string eventId)
        {
            Interview interview = db.Interviews.First(i => i.Id == eventId);
            DateTime date = interview.Date;
            String intervieweeId = interview.Users.First( u => !u.Interviewer).Id;
            db.Interviews.Remove(interview);
            db.SaveChanges();
            return RedirectToAction("DeleteInterview","Calendar", new { eventId = eventId});
        }

        public async Task<ActionResult> EditInterview(string interviewers, string eventId, DateTime? date)
        {
            var interview = db.Interviews.First(i => i.Id == eventId);
            if (interviewers != null && interviewers != "")
                {
                    User interviewer = interview.Users.First(user => !user.Interviewer);
                    string[] interviewersSplitResult = interviewers.Split(new[] { ',' });
                    interview.Users = FindInterviewers(interviewersSplitResult);
                    interview.Users.Add(interviewer);
                    db.SaveChanges();
                }
                if (date != null)
                {
                    interview.Date = (DateTime)date;
                    db.SaveChanges();
                    return RedirectToAction("EditInterview", "Calendar", new { eventId = eventId, date = date });
                }
           return RedirectToAction("Index");
        }

        public List<User> FindInterviewers(string[] users)
        {
            List<User> usersList = new List<User>();
            foreach (string email in users)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    throw new UserDoesNotExistException("Some interviewers are not in the database");
                usersList.Add(user);
            }
            return usersList;
        }
	}
}