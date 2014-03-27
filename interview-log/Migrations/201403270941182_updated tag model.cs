namespace interview_log.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedtagmodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Interviews", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInterviews", "Interview_Id", "dbo.Interviews");
            DropIndex("dbo.Interviews", new[] { "User_Id" });
            DropPrimaryKey("dbo.Interviews");
            CreateTable(
                "dbo.UserInterviews",
                c => new
                    {
                        User_Id = c.String(nullable: false, maxLength: 128),
                        Interview_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Interview_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Interviews", t => t.Interview_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Interview_Id);
            
            AddColumn("dbo.Interviews", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Interviews", "Id");
            DropColumn("dbo.Interviews", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Interviews", "User_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.UserInterviews", "Interview_Id", "dbo.Interviews");
            DropForeignKey("dbo.UserInterviews", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserInterviews", new[] { "Interview_Id" });
            DropIndex("dbo.UserInterviews", new[] { "User_Id" });
            DropPrimaryKey("dbo.Interviews");
            DropColumn("dbo.Interviews", "Id");
            DropTable("dbo.UserInterviews");
            AddPrimaryKey("dbo.Interviews", "date");
            CreateIndex("dbo.Interviews", "User_Id");
            AddForeignKey("dbo.UserInterviews", "Interview_Id", "dbo.Interviews", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Interviews", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
