namespace TelegramHelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TelegramHelpdesk : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppTasks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Number = c.Int(nullable: false),
                        District = c.String(nullable: false),
                        Office = c.String(nullable: false),
                        Description = c.String(nullable: false, maxLength: 300),
                        CommentId = c.Guid(),
                        Status = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        File = c.String(),
                        CategoryId = c.Guid(),
                        ExecutorId = c.Guid(),
                        LifecycleId = c.Guid(nullable: false),
                        CreateUserId = c.Guid(),
                        CreateDistrictId = c.Guid(),
                        DepartmentId = c.Guid(),
                        DistrictId = c.Guid(),
                        LastUpdate = c.DateTime(),
                        Subject = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Comments", t => t.CommentId)
                .ForeignKey("dbo.Districts", t => t.CreateDistrictId)
                .ForeignKey("dbo.Users", t => t.CreateUserId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Users", t => t.ExecutorId)
                .ForeignKey("dbo.Lifecycles", t => t.LifecycleId, cascadeDelete: true)
                .Index(t => t.CommentId)
                .Index(t => t.CategoryId)
                .Index(t => t.ExecutorId)
                .Index(t => t.LifecycleId)
                .Index(t => t.CreateUserId)
                .Index(t => t.CreateDistrictId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        DepartmentId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(),
                        TaskId = c.Guid(),
                        Text = c.String(nullable: false, maxLength: 300),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DistrictName = c.String(nullable: false, maxLength: 100),
                        OfficeName = c.String(nullable: false, maxLength: 100),
                        DistrictId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Login = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        Position = c.String(maxLength: 50),
                        DepartmentId = c.Guid(),
                        RoleId = c.Guid(nullable: false),
                        DistrictId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Districts", t => t.DistrictId)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.RoleId)
                .Index(t => t.DistrictId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Lifecycles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Opened = c.DateTime(nullable: false),
                        Proccesing = c.DateTime(),
                        Checking = c.DateTime(),
                        Closed = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TelegramTasks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ChatId = c.Int(nullable: false),
                        District = c.String(),
                        Office = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        File = c.String(),
                        BotId = c.String(),
                        ChannelId = c.String(),
                        UserId = c.String(),
                        ConversationId = c.String(),
                        ServiceUrl = c.String(),
                        UserName = c.String(),
                        IsGroup = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppTasks", "LifecycleId", "dbo.Lifecycles");
            DropForeignKey("dbo.AppTasks", "ExecutorId", "dbo.Users");
            DropForeignKey("dbo.AppTasks", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AppTasks", "CreateUserId", "dbo.Users");
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Users", "DistrictId", "dbo.Districts");
            DropForeignKey("dbo.Users", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AppTasks", "CreateDistrictId", "dbo.Districts");
            DropForeignKey("dbo.AppTasks", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.AppTasks", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Categories", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Users", new[] { "DistrictId" });
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropIndex("dbo.Users", new[] { "DepartmentId" });
            DropIndex("dbo.Categories", new[] { "DepartmentId" });
            DropIndex("dbo.AppTasks", new[] { "DepartmentId" });
            DropIndex("dbo.AppTasks", new[] { "CreateDistrictId" });
            DropIndex("dbo.AppTasks", new[] { "CreateUserId" });
            DropIndex("dbo.AppTasks", new[] { "LifecycleId" });
            DropIndex("dbo.AppTasks", new[] { "ExecutorId" });
            DropIndex("dbo.AppTasks", new[] { "CategoryId" });
            DropIndex("dbo.AppTasks", new[] { "CommentId" });
            DropTable("dbo.TelegramTasks");
            DropTable("dbo.Lifecycles");
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
            DropTable("dbo.Districts");
            DropTable("dbo.Comments");
            DropTable("dbo.Departments");
            DropTable("dbo.Categories");
            DropTable("dbo.AppTasks");
        }
    }
}
