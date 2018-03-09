namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class map10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MapConditions", "ParentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MapConditions", "ParentId");
        }
    }
}
