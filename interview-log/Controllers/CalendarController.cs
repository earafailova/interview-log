using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using interview_log.Models;

namespace interview_log.Controllers
{
    public class CalendarController : Controller
    {
        private Calendar CurrentCalendar;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeCalendarAdrdess(string newAddress)
        {
            CurrentCalendar.CalendarAddress = newAddress;
            return View();
        }
	}
}