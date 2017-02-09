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
    }
}
