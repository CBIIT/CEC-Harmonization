namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class map12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MapRecords", "TargetFieldId", c => c.String());
            AddColumn("dbo.MapRecords", "TargetFieldName", c => c.String());
            AddColumn("dbo.MapRecords", "TargetMissing", c => c.String());
            DropColumn("dbo.MapRecords", "TargetId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MapRecords", "TargetId", c => c.String());
            DropColumn("dbo.MapRecords", "TargetMissing");
            DropColumn("dbo.MapRecords", "TargetFieldName");
            DropColumn("dbo.MapRecords", "TargetFieldId");
        }
    }
}
