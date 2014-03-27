using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using interview_log.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Text.RegularExpressions;
namespace interview_log.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        delegate bool del(string query, string thing);

        // GET: /Users/
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        [HttpPost]
        public ActionResult Index(string q, DateTime? from, DateTime? to)
        {
            var response = interview_log.Models.User.Search(q);
            return View(response);
        }

        // GET: /Users/Details/5
        public ActionResult Details(string id)
        {
            string currentUser = GetCurrentUserId();
            string userId = id == null ? currentUser : id;
            User user = db.Users.Find(userId);

            if (user == null)
                return HttpNotFound("The user you are watching for does not exist!  :(");

            ViewBag.CurrentUser = currentUser;

            return View(user);
        }

        [HttpPost]
        public ActionResult Details(HttpPostedFileBase file)
        {
            string userId = GetCurrentUserId();
            User user = db.Users.Find(userId);
            user.Attachments.Add(new Attachment(ref file, userId));
            db.SaveChanges(); 
            return RedirectToAction("Details");
        }

        public ActionResult DeleteAttachment(System.Guid Id)
        {
            string userId = GetCurrentUserId();
            User user = db.Users.Find(userId);

            if (user == null)
                return HttpNotFound();
            Attachment toDelete = user.Attachments.First(item => item.Id == Id);
            user.Attachments.Remove(toDelete);
            db.Attachments.Remove(toDelete);

            toDelete.Delete();
            db.SaveChanges();
            return RedirectToAction("Details");
        }

        public ActionResult LeaveComment(string text, string Id)
        {
            string userId = GetCurrentUserId();
            User user = db.Users.Find(Id);
            if (user == null) return HttpNotFound();

            user.Comments.Add(new Comment(author: userId, text: text));
            db.SaveChanges(); 

            return RedirectToAction("Details");
        }

        public ActionResult DeleteComment(System.Guid Id)
        {
            string userId = GetCurrentUserId();
            User user = db.Users.Find(userId);
            if (user == null)
                return HttpNotFound();
            
            Comment toDelete = user.Comments.First(item => item.Id == Id);
            user.Comments.Remove(toDelete);
            db.Comments.Remove(toDelete);

            db.SaveChanges();
            return RedirectToAction("Details");
        }

        public ActionResult Tag(string tag, string info)
        {
            string userId = GetCurrentUserId();
            User user = db.Users.Find(userId);

            var tagWeHave = db.Tags.FirstOrDefault(existingTag => existingTag.Name == tag);
         
            if (tagWeHave != null)
                user.Tags.Add(tagWeHave);
            else
                user.Tags.Add(new Tag(name: tag, info: info));

            db.SaveChanges();
            return RedirectToAction("Details");
        }

        private string GetCurrentUserId()
        {
            return User.Identity.GetUserId();
        }

        private void DeleteItem<T>()
        {
        }

        // GET: /Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (id != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(user);
        }

        // POST: /Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string userId, string name, string position, string info, byte state)
        {
            User currentUser = db.Users.Find(GetCurrentUserId());
            
            if (User.Identity.GetUserId() != userId && !currentUser.Admin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            User user = db.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.UserName = name;
            user.Info = info;
            user.Position = position;

            if (currentUser.Admin)
            {
                user.State = state;
            }

            return RedirectToAction("Edit");
        }

        // GET: /Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult IncDlCounter(Guid fileId)
        {
            db.Attachments.Find(fileId).IncDlCounter();
            db.SaveChanges();
            return RedirectToAction("Details");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
