namespace TelegramHelpDesk.Migrations
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Web.Security;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.HelpdeskContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.HelpdeskContext context)
        {

        }
    }
}
