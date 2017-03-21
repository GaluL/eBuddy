namespace eBuddyService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scheduleRunFinal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduledRunItems", "InitializerFacebookId", c => c.String());
            AddColumn("dbo.ScheduledRunItems", "BuddyFacebookId", c => c.String());
            AddColumn("dbo.ScheduledRunItems", "BuddyApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduledRunItems", "BuddyApproval");
            DropColumn("dbo.ScheduledRunItems", "BuddyFacebookId");
            DropColumn("dbo.ScheduledRunItems", "InitializerFacebookId");
        }
    }
}
