namespace interview_log.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blahblahblah : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachments", "FullPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attachments", "FullPath");
        }
    }
}
