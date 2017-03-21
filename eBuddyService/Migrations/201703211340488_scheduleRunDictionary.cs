namespace eBuddyService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scheduleRunDictionary : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ScheduledRunItems", "User1FacebookId");
            DropColumn("dbo.ScheduledRunItems", "User2FacebookId");
            DropColumn("dbo.ScheduledRunItems", "User1Approved");
            DropColumn("dbo.ScheduledRunItems", "User2Approved");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScheduledRunItems", "User2Approved", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledRunItems", "User1Approved", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScheduledRunItems", "User2FacebookId", c => c.String());
            AddColumn("dbo.ScheduledRunItems", "User1FacebookId", c => c.String());
        }
    }
}
