using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;


namespace interview_log.Models
{
    public class Comment
    {
        public Comment() { }
        public Comment(string author, string text)
        {
            this.AuthorId = author;
            this.Text = text;
            this.Date = DateTime.Now;
            ApplicationDbContext db = new ApplicationDbContext();
            this.Author = db.Users.Find(author).UserName;
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; private set; }
        public string AuthorId { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}