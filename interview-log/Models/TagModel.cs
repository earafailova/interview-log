using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace interview_log.Models
{
    public class Tag 
    {
        [Key] public string Name { get; set; }
        public string Info { get; set; }
    }

    public class TagsDbContext : DbContext
    {
        public TagsDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Tag> Tags { get; set; }
    }
}