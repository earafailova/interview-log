﻿using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using LINQtoCSV;
using interview_log.Exceptions;
using System;

namespace interview_log.Models
{
     
    public enum State : byte 
    {
        Unknown, Accepted, Postponed
    };
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public User()
        {
            Attachments = new HashSet<Attachment>();
            Comments = new HashSet<Comment>();
            Tags = new HashSet<Tag>();
            Interviews = new HashSet<Interview>();
        }
         public string Email {get; set;}
        public string Position { get; set; }
        public byte State { get; set; }
        public bool Admin { get; set; }
        public bool Interviewer { get; set; }
        public string Info { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set;  }
        public virtual ICollection<Interview> Interviews { get; set; }

        public IEnumerable<Attachment> Images()
        {
            return Attachments.Where(item => item.Type == Type.Image);
        }
        public IEnumerable<Attachment> Files()
        {
            return Attachments.Where(item => item.Type == Type.File);
        }

        public static User[] Admins()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Users.Where<User>(user => user.Admin == true).ToArray<User>();
        }

        public static User[] AdminsWithoutMe(string myEmail)
        {
            User[] admins = User.Admins();
            return System.Array.FindAll<User>(admins, user => user.Email != myEmail);
        }
        
        public static User[] Search(string q, DateTime? first, DateTime? second)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Regex query = new Regex(Regex.Escape(q.ToLower()));

            var byUser = db.Users.ToList().Where(user => 
            {
                bool result = false;
                result |= query.IsMatch(user.UserName.ToLower());
                if (user.Info != null)
                {
                    result |= query.IsMatch(user.Info);
                }
                return result;
            });

            var byTag = db.Users.ToList().
                Where(user => user.Tags.
                    Any(tag => query.IsMatch(tag.Name.ToLower())));
            var byTime = Interview.Between(first, second);

            return byTag.Union(byUser).Union(byTime).ToArray();  
        }


       
        internal static string GetName(string interviewer)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.First(u => u.Email == interviewer);
            if (user != null) return user.UserName;
            return null;
        }

        public static User[] Interviewers()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Users.Where<User>(user => user.Interviewer == true).ToArray<User>();
        }

       
      
    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Interview> Interviews { get; set; }
    }
}