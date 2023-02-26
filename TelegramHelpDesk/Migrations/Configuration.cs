namespace TelegramHelpDesk.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TelegramHelpDesk.Models.HelpdeskContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TelegramHelpDesk.Models.HelpdeskContext context)
        {
            //  Этот метод будет вызван после перехода на последнюю версию

            
        }
    }
}
