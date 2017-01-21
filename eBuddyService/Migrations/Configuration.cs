using System.Collections.Generic;
using eBuddyService.DataObjects;
using Microsoft.Azure.Mobile.Server.Tables;

namespace eBuddyService.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<eBuddyService.Models.eBuddyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new EntityTableSqlGenerator());
        }

        protected override void Seed(eBuddyService.Models.eBuddyContext context)
        {
            List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false }
            };

            foreach (TodoItem todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }

            List<UserItem> userItems = new List<UserItem>
            {
                new UserItem { Id = Guid.NewGuid().ToString(), PrivateName = "Gal", SurName = "Kleinman", Age = 27, Height = 1.79, Weight = 67},
                new UserItem { Id = Guid.NewGuid().ToString(), PrivateName = "Ofek", SurName = "Ben-Ephraim", Age = 27.5, Height = 1.75, Weight = 72},
            };

            foreach (UserItem userItem in userItems)
            {
                context.Set<UserItem>().Add(userItem);
            }

            base.Seed(context);
        }
    }
}
