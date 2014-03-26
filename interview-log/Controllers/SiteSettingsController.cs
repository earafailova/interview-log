using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using interview_log.Models;
using CsvHelper;
using System.IO;
using System.Collections;
using MvcFlashMessages;
using interview_log.Helpers;

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




       [HttpGet]
       public ActionResult WriteCSV()
       {
                String path = System.Web.HttpContext.Current.Server.MapPath(@"~\CSV\db.csv");
                StreamWriter writer = new StreamWriter(path);
                var csv = new CsvWriter(writer);
                csv.Configuration.RegisterClassMap<UserMap>();
                csv.Configuration.HasHeaderRecord = true;
                csv.WriteHeader(typeof(User));
                ApplicationDbContext db = new ApplicationDbContext();
                IEnumerable allUsers = db.Users.ToList();
                foreach( User user in allUsers)
                {
                  csv.WriteRecord(user);
                }
                writer.Close();
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment; filename=db.csv");
                Response.TransmitFile(Server.MapPath(@"~\CSV\db.csv"));
                Response.End();
                return View();
           }
        
        [HttpPost]
        public ActionResult ReadCSV(HttpPostedFileBase file)
       {
            string fileName = System.Web.HttpContext.Current.Server.MapPath(@"~\CSV\inputCSV.csv");
            try
            {
                file.SaveAs(fileName);
                ApplicationDbContext db = new ApplicationDbContext();
                StreamReader reader = new StreamReader(fileName);
                var csv = new CsvReader(reader);
                while (csv.Read())
                {
                    var user = new User();
                    user.Email = csv.GetField<string>("Email");
                    if (db.Users.Where<User>(m => m.Email == user.Email).ToList().Count == 0) 
                    { 
                         user.UserName = csv.GetField<string>("UserName");
                         user.State = csv.GetField<byte>("State");
                         user.Position = csv.GetField<string>("Position");
                         HashSet<Tag> tags = (HashSet<Tag>)csv.GetField(typeof(HashSet<Tag>),"Tags", new TagsConverter());
                         AddUserTags(user, tags);
                         db.Users.Add(user);
                    }
                }
                db.SaveChanges();
            }
            catch(Exception)
            {
                this.Flash("error", "Something has gone wrong. DataBase is not changed");
                return RedirectToAction("Index");
            }
           this.Flash("success", "New users added to database");
           return RedirectToAction("Index");
       }
        
        
        [HttpPost]
        public ActionResult CreateInterview(string interviewer, string interviewee, DateTime time)
        {

            return View(CurrentCalendar);
        }

        private static void AddUserTags(Models.User user, HashSet<Tag> tags)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Tag newTag = new Tag();
            foreach (Tag tag in tags)
            {
                var existingTag = "hasn't defined yet";
               // if (existingTag == tag.Name)
                   // user.Tags.Add(tag); //change later
                //else
                    user.Tags.Add(tag);
            }
        }
	} 

}