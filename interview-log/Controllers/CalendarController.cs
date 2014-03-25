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

   }
}