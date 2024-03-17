namespace TelegramHelpDesk.Migrations
{
    using System;
    using System.Collections.Generic;
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

            Sql("INSERT INTO Roles VALUES('cece27f6-c782-4698-94e4-1125f9a72e49', 'Модератор')");
            Sql("INSERT INTO Roles VALUES('03d10c87-2f20-4aed-a65e-6fb760bcd7ee', 'Исполнитель')");
            Sql("INSERT INTO Roles VALUES('7e06896d-2d11-4df2-86e9-8d034390e35a', 'Пользователь')");
            Sql("INSERT INTO Roles VALUES('579384d1-1533-46b7-8b6f-d40f8e97557e', 'Администратор')");

            Sql("INSERT INTO Departments VALUES('62a04b5f-f9dd-428f-8c19-7a2761dbbece', 'Администрация')");
            Sql("INSERT INTO Departments VALUES('4c57765c-405b-4259-af58-90a5ddfb1317', 'ИТ')");
            Sql("INSERT INTO Departments VALUES('612f2418-9989-4959-a1ae-b32bc29f3c78', 'АХО')");

            Sql("INSERT INTO Districts VALUES('fab9dc94-b524-402e-96f8-0cce6f83b3e5', 'г. Калуга','ул. Хрустальная','fab9dc94-b524-402e-96f8-0cce6f83b3e4')");

            Sql("INSERT INTO Users VALUES('fab9dc94-b524-402e-96f8-0cce6f83b3ea', 'Администратор','admin','admin','Администратор','62a04b5f-f9dd-428f-8c19-7a2761dbbece','579384d1-1533-46b7-8b6f-d40f8e97557e',NULL)");
            Sql("INSERT INTO Users VALUES('8a2e7907-0502-48dc-896e-48e2c08d1f62', 'Петров Петр Петрович','pet','pet','Сотрудник','4c57765c-405b-4259-af58-90a5ddfb1317','03d10c87-2f20-4aed-a65e-6fb760bcd7ee',NULL)");
            Sql("INSERT INTO Users VALUES('25845db4-9217-409f-95c8-aa7a16441e62', 'Иванов Иван Иванович','ivan','ivan','Начальник отдела','4c57765c-405b-4259-af58-90a5ddfb1317','cece27f6-c782-4698-94e4-1125f9a72e49',NULL)");
            Sql("INSERT INTO Users VALUES('25845db4-9217-409f-95c8-aa7a16441e63', 'Филиал','fil','fil','Филиал',NULL,'7e06896d-2d11-4df2-86e9-8d034390e35a','fab9dc94-b524-402e-96f8-0cce6f83b3e5')");

            Sql("INSERT INTO Lifecycles VALUES('c5dc57c8-d7ec-4012-af8d-3977cafdf244', CONVERT(DATETIME,'2024-03-16 01:11:16.203',121),CONVERT(DATETIME,'2024-03-16 01:11:16.203',121),NULL,NULL)");
            Sql("INSERT INTO Lifecycles VALUES('347fc2e3-554a-4f99-9bd1-3ee107a77c55', CONVERT(DATETIME,'2018-05-07 09:10:47.770',121),NULL,NULL,NULL)");
            Sql("INSERT INTO Lifecycles VALUES('0797a487-8e59-4991-b8b1-ade42ecee3de', CONVERT(DATETIME,'2024-03-16 01:06:48.980',121),CONVERT(DATETIME,'2024-03-16 01:06:48.980',121),NULL,NULL)");

            Sql("INSERT INTO Categories VALUES('f79acf29-8a65-471e-b2e0-2c2a48cabb44', 'Фигня какая-то','4c57765c-405b-4259-af58-90a5ddfb1317')");
            Sql("INSERT INTO Categories VALUES('7d1e159c-8232-4821-a8ae-c8670f5c46e4', 'Проблемы с оборудованием','4c57765c-405b-4259-af58-90a5ddfb1317')");


            Sql("INSERT INTO AppTasks VALUES('89d63080-8f47-41a3-ad5d-07ea97a48c53',4,'г. Калуга','ул. Хрустальная','rgrg',NULL,0,1,'04.47.2018 10_47.jpg','7d1e159c-8232-4821-a8ae-c8670f5c46e4','25845db4-9217-409f-95c8-aa7a16441e62','c5dc57c8-d7ec-4012-af8d-3977cafdf244','8a2e7907-0502-48dc-896e-48e2c08d1f62',NULL,'4c57765c-405b-4259-af58-90a5ddfb1317',NULL,NULL,'Проблемы с оборудованием')");
            Sql("INSERT INTO AppTasks VALUES('54697da8-b8dd-4b73-b8e0-717a999a88ef',7, 'erb','erb','trh',NULL,0,1,NULL,'7d1e159c-8232-4821-a8ae-c8670f5c46e4','8a2e7907-0502-48dc-896e-48e2c08d1f62','347fc2e3-554a-4f99-9bd1-3ee107a77c55','8a2e7907-0502-48dc-896e-48e2c08d1f62',NULL,'4c57765c-405b-4259-af58-90a5ddfb1317',NULL,NULL,'Проблемы с оборудованием')");
            Sql("INSERT INTO AppTasks VALUES('347fc2e3-554a-4f99-9bd1-3ee107a77c55',6, 'г. Калуга','ул. Хрустальная','ytjty',NULL,0,1,NULL,'f79acf29-8a65-471e-b2e0-2c2a48cabb44','8a2e7907-0502-48dc-896e-48e2c08d1f62','0797a487-8e59-4991-b8b1-ade42ecee3de','8a2e7907-0502-48dc-896e-48e2c08d1f62',NULL,'4c57765c-405b-4259-af58-90a5ddfb1317',NULL,NULL,'Фигня какая-то')");

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
