namespace interview_log.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewRoleadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Interviewer", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Interviewer");
        }
    }
}
