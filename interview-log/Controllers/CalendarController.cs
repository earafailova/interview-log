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
using Enums;

using Calendar = interview_log.Models.Calendar;
namespace interview_log.Controllers
{

    
    public class CalendarController : Controller
    {
       public Calendar CurrentCalendar = new Calendar();
        CancellationToken cancellationToken = new CancellationToken(false);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeCalendarAddress(string newAddress)
        {
            CalendarService service = (CalendarService)Session["service"];
            string fullAddress = newAddress + "@group.calendar.google.com"	;
            var calendarList = service.CalendarList.List().Execute().Items;
            var calendar = from c in calendarList
                           where c.Id == fullAddress
                           select c;

            if (calendar.ToList<Google.Apis.Calendar.v3.Data.CalendarListEntry>().Count != 0) 
            {
                CurrentCalendar.CalendarAddress = newAddress;
                GiveAccessToCalendar(null);
                this.Flash("success", "Calendar address has been successfully updated");
                return RedirectToAction("Index", "SiteSettings");
            }
            this.Flash("error", "This calendar dosnt exist or you dont have permission");
           
            return RedirectToAction("Index", "SiteSettings");
           }

        private void GiveAccessToCalendar(User [] Admins)
        {
            CalendarService service  = (CalendarService)Session["service"];
            string fullAddress = CurrentCalendar.CalendarAddress +"@group.calendar.google.com";
            AclRule newRule = new AclRule();
            newRule.Role = "owner";
            newRule.Scope = new AclRule.ScopeData();
            newRule.Scope.Type = "user";
            foreach(User user in Admins)
            {
              newRule.Scope.Value = user.Email;
              service.Acl.Insert(newRule, fullAddress).Execute();
            }
        }

        public ActionResult GiveAccessToNewAdmin(string email)
        {
            User newAdmin = new User();
            newAdmin.Email = email;
            GiveAccessToCalendar(new[] { newAdmin });
            return RedirectToAction("Index", "SiteSettings");
        }

        public async Task<ActionResult> GetCalendarAccess(Purpose purpose, string param)
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
            Session["service"] = service;

            switch(purpose)
            {
                case Purpose.NewAddress:
                    return RedirectToAction("ChangeCalendarAddress", new { newAddress  = param});
                case Purpose.NewAdmin:
                    return RedirectToAction("GiveAccessToNewAdmin", new { email = param});
                default:
                    return RedirectToAction("Index", "SiteSettins"); 
            }
        }


    }
}