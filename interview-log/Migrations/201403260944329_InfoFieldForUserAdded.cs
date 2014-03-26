namespace interview_log.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InfoFieldForUserAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Info", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Info");
        }
    }
}
