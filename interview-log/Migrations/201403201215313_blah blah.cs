namespace interview_log.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blahblah : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropTable("Attachments");
        }
    }
}
