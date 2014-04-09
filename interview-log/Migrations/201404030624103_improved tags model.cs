namespace interview_log.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class improvedtagsmodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tags", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Tags", new[] { "User_Id" });
            CreateTable(
                "dbo.TagUsers",
                c => new
                    {
                        Tag_Name = c.String(nullable: false, maxLength: 128),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Tag_Name, t.User_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Name, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Tag_Name)
                .Index(t => t.User_Id);
            
            DropColumn("dbo.Tags", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tags", "User_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.TagUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TagUsers", "Tag_Name", "dbo.Tags");
            DropIndex("dbo.TagUsers", new[] { "User_Id" });
            DropIndex("dbo.TagUsers", new[] { "Tag_Name" });
            DropTable("dbo.TagUsers");
            CreateIndex("dbo.Tags", "User_Id");
            AddForeignKey("dbo.Tags", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
