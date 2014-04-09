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
        public ActionResult Index(string state = "")
        {
            switch (state)
            {
                case "error":
                    {

                    }
                    break;
                case "succeess":
                    {
                        this.Flash("success", "Interview has been created");
                    }
                    break;
                default:
                    break;
            }
            return View();
        }

        public async Task<ActionResult> ChangeCalendarAddress(string newAddress)
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
            foreach (User user in admins)
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
            if (Session["valid"] == null || !(bool)Session["valid"])
            {
                SaveParametres(interviewer, interviewee, time, true);
            }
            if (AuthResult.Credential == null)
            {

                return new RedirectResult(AuthResult.RedirectUri, true);
            }
            CalendarService service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = AuthResult.Credential,
                ApplicationName = "Interview Log"
            });
            try
            {
                string eventId = CreateInterview((DateTime)Session["time"], service, (string)Session["interviewer"], (string)Session["interviewee"]);
                if (eventId == null)
                {
                    this.Flash("error", "Something has gone wrong. Interview has not been created");
                    return RedirectToAction("Index", "Calendar");
                }
                try
                {
                   AddInterview((DateTime)Session["time"], (string)Session["interviewer"], (string)Session["interviewee"], eventId);
                }
                catch (UserDoesNotExistException e)
                {
                    this.Flash("error", e.Message);
                    return RedirectToAction("Index", "Calendar");
                }
                this.Flash("success", "Interview has been created");
                return RedirectToAction("Index", "Calendar");
             }
             finally
            {
                SaveParametres(null, null, null, false);
            }
        }

        private void SaveParametres(string interviewer, string interviewee, DateTime? time, Boolean valid)
        {
            Session["interviewer"] = interviewer;
            Session["interviewee"] = interviewee;
            Session["time"] = time;
            Session["valid"] = valid;
        }

        private string CreateInterview(DateTime timeStart, CalendarService service, string interviewer, string interviewee)
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
                return service.Events.Insert(interview, CurrentCalendar.CalendarAddress).Execute().Id;
                
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Interview AddInterview(DateTime dateTime, string interviewee, string interviewer, string eventId)
        {
            string[] splitResult = interviewer.Split(new[] { ',' });
            int length = splitResult.Length;
            string[] users = new string[length + 1];
            splitResult.CopyTo(users, 0);
            new string[] { interviewee }.CopyTo(users, length);
            List<User> foundUsers = FindInterviewersAndInterviewee(users);
            Interview interview = new Interview(dateTime, eventId);
            foreach (User user in foundUsers)
            {
                user.Interviews.Add(interview);
                user.Interviewer = true;
                interview.Users.Add(user);
            }
            foundUsers.Last().Interviewer = false;
            db.SaveChanges();
            return interview;
        }

        public async Task<ActionResult> DeleteInterview(string eventId)
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
            service.Events.Delete(CurrentCalendar.CalendarAddress, eventId);
           return RedirectToAction("Index", "Interviews");
        }

        public List<User> FindInterviewersAndInterviewee(string[] users)
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