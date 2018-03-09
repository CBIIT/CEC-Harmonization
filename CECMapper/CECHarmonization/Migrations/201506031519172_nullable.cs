namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Mappers", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.Mappers", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.MapRecords", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.MapRecords", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.MapConditions", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.MapConditions", "ModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MapConditions", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MapConditions", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MapRecords", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MapRecords", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Mappers", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Mappers", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
