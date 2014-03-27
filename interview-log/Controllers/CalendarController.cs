using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using interview_log.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System.Threading;
using Google.Apis.Interview_log;
using MvcFlashMessages;
using Google.Apis.Calendar.v3.Data;
using interview_log.Helpers;
using interview_log.Exceptions;

using Calendar = interview_log.Models.Calendar;
namespace interview_log.Controllers
{

    public class CalendarController : Controller
    {
       public Calendar CurrentCalendar = new Calendar();
        CancellationToken cancellationToken = new CancellationToken(false);
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string address)
        {
            return View();
        }

        public async Task<ActionResult>  ChangeCalendarAddress(string newAddress)
        {
            Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp.AuthResult AuthResult = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
              AuthorizeAsync(cancellationToken);
            if (AuthResult.Credential == null)
                return new RedirectResult(AuthResult.RedirectUri);
            CalendarService service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = AuthResult.Credential,
                ApplicationName = "Interview Log"
            });
            var calendarList = service.CalendarList.List().Execute().Items;
            var calendar = from c in calendarList
                           where c.Id == newAddress
                           select c;
            if (calendar.ToList<Google.Apis.Calendar.v3.Data.CalendarListEntry>().Count != 0 && CurrentCalendar.CalendarAddress != newAddress) 
            {
                CurrentCalendar.CalendarAddress = newAddress;
                GiveAccessToCalendar(interview_log.Models.User.AdminsWithoutMe(currentUserEmail()), service, "owner");
                this.Flash("success", "Calendar address has been successfully updated");
                return RedirectToAction("Index", "SiteSettings");
            }
            this.Flash("error", "Calendar address has not been changed");
           
            return RedirectToAction("Index", "SiteSettings");
           }

        private void GiveAccessToCalendar(User[] admins, CalendarService service, string role)
        {
            AclRule newRule = new AclRule();
            newRule.Role = role;
            newRule.Scope = new AclRule.ScopeData();
            newRule.Scope.Type = "user";
            foreach(User user in admins)
            {
              newRule.Scope.Value = user.Email;
              service.Acl.Insert(newRule, CurrentCalendar.CalendarAddress).Execute();
            }
        }

        public async Task<ActionResult> ChangeAccessLevel(string email, string role)
        {
            Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp.AuthResult AuthResult = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
              AuthorizeAsync(cancellationToken);
            if (AuthResult.Credential == null)
                return new RedirectResult(AuthResult.RedirectUri);
            CalendarService service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = AuthResult.Credential,
                ApplicationName = "Interview Log"
            });
            User newAdmin = new User();
            newAdmin.Email = email;
            GiveAccessToCalendar(new[] { newAdmin }, service, role);
            return RedirectToAction("Index", "SiteSettings");
        }

        private string currentUserEmail()
        {
            string userId = (string)User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);
            return currentUser.Email;
        }

        
      public async Task<ActionResult> CreateInterview(string interviewer, string interviewee, DateTime? time = null)
        {
            
            Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp.AuthResult AuthResult = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
             AuthorizeAsync(cancellationToken);
            if (AuthResult.Credential == null)
            {
                Session["interviewer"] = interviewer;
                Session["interviewee"] = interviewee;
                Session["time"] = time;
                return new RedirectResult(AuthResult.RedirectUri, true);
            }
            CalendarService service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = AuthResult.Credential,
                ApplicationName = "Interview Log"
            });
            try
            {
                AddInterview((DateTime)Session["time"], (string)Session["interviewer"], (string)Session["interviewee"]);
            }
            catch(UserDoesNotExistException e)
            {
                this.Flash("error", e.Message);
                return RedirectToAction("Index", "Calendar");
            }
            if (CreateInterview((DateTime)Session["time"], service, (string)Session["interviewer"], (string)Session["interviewee"]))
                this.Flash("success", "Interview has been created");
            else
                this.Flash("error", "Something has gone wrong. Interview has not been created");
           return RedirectToAction("Index","Calendar");
       }

        private bool CreateInterview(DateTime timeStart, CalendarService service, string interviewer, string interviewee)
        {
            DateTime timeEnd = timeStart + TimeSpan.FromMinutes(30);
            var curTZone = TimeZone.CurrentTimeZone;
            var dateStart = new DateTimeOffset(timeStart, curTZone.GetUtcOffset(timeStart));
            var dateEnd = new DateTimeOffset(timeEnd, curTZone.GetUtcOffset(timeEnd));
            Event interview = new Event();
            interview.Start = new EventDateTime()
            {
               DateTime = timeStart
            };
            interview.End = new EventDateTime()
            {
              DateTime = timeEnd
            };
            string intervieweeName = interview_log.Models.User.GetName(interviewee);
            interview.Location = "Omsk";
            interview.Summary = "Interview " + intervieweeName; 
            interview.Description = "Interview " + intervieweeName + " </br> Visit Profile!";
            try
            {
               service.Events.Insert(interview, CurrentCalendar.CalendarAddress).Execute();
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        public void AddInterview(DateTime dateTime, string interviewee, string interviewer)
        {
            string[] splitResult = interviewer.Split(new[] { ',' });
            int length = splitResult.Length;
            string[] users = new string[length + 1];
            splitResult.CopyTo(users, 0);
            new string[] { interviewee }.CopyTo(users, length);
            List<User> foundUsers = FindInterviewersAndInterviewee(users);
            Interview interview = new Interview(dateTime);
            foreach (User user in foundUsers)
            {
                user.Interviews.Add(interview);
                user.Interviewer = true;
                interview.Users.Add(user);
            }
            foundUsers.Last().Interviewer = false;
            db.SaveChanges();
        }

        public List<User> FindInterviewersAndInterviewee(string[] users)
        {
            List<User> usersList = new List<User>();
            foreach (string email in users)
            {
                var user = db.Users.First(u => u.Email == email);
                if (user == null)
                    throw new UserDoesNotExistException("Some interviewers are not in the data base");
                usersList.Add(user);
            }
            return usersList;
        }
   }
}