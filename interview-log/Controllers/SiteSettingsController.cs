using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using interview_log.Models;

namespace interview_log.Controllers
{
    public class SiteSettingsController : Controller
    {
        private Calendar CurrentCalendar;

        public ActionResult Index()
        {
            CurrentCalendar = CurrentCalendar == null ? new Calendar() : CurrentCalendar;
            return View(CurrentCalendar);
           
        }

        [HttpPost]
        public ActionResult CreateInterview(string interviewer, string interviewee, DateTime time)
        {

            return View(CurrentCalendar);
        }
	}
}