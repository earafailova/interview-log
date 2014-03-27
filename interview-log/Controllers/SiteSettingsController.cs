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
            StreamReader reader = null;
            try
            {
                file.SaveAs(fileName);
                reader = new StreamReader(fileName);
                ApplicationDbContext db = new ApplicationDbContext();
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
                         String[] tags = (String[])csv.GetField(typeof(String[]), "Tags", new TagsConverter());
                         AddUserTags(user, tags);
                         db.Users.Add(user);
                    }
                }
                db.SaveChanges();
            }
            catch(Exception)
            {
                reader.Close();
                this.Flash("error", "Something has gone wrong. DataBase is not changed");
                return RedirectToAction("Index");
            }
            reader.Close();
           this.Flash("success", "New users added to database");
           return RedirectToAction("Index");
       }
        
        
        

        private static void AddUserTags(Models.User user, String [] tags)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            for (int i = 0; i < tags.Count() && tags[i] != ""; i ++ )
            {
                string tagName  = tags[i];
                var existingTag = db.Tags.First(t => t.Name == tagName);
                if (existingTag != null)
                   user.Tags.Add(existingTag); 
                else
                   user.Tags.Add(new Tag(tags[i]));
            }
        }
	} 

}