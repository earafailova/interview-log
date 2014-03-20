using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using interview_log.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Collections.Generic;

namespace interview_log.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: /Users/
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: /Users/Details/5
        public ActionResult Details(string id)
        {
            string userId = id == null ? User.Identity.GetUserId() : id;
            User user = db.Users.Find(userId);
            
            if (user == null)
                return HttpNotFound();

            ViewBag.Images = Helpers.FilesHelper.GetImages(userId);
            ViewBag.FileInfos = Helpers.FilesHelper.GetFileInfos(userId);
            return View(user);
        }

        [HttpPost]
        public ActionResult Details(HttpPostedFileBase file)
        {
            string userId = User.Identity.GetUserId();
            User user = db.Users.Find(userId);
            user.Attachments.Add(new Attachment(ref file, userId));
            Helpers.FilesHelper.UploadFile(ref file, userId);
            return RedirectToAction("Details");
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
        public ActionResult Edit([Bind(Include="Id,UserName,PasswordHash,SecurityStamp,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
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
