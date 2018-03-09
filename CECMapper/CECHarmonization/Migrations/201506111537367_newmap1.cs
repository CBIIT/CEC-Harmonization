namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmap1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MapRecords", "filter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MapRecords", "filter");
        }
    }
}
