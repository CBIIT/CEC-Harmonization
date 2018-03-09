namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.varmappings", "status", c => c.String());
            AddColumn("dbo.varmappings", "script", c => c.String());
            AddColumn("dbo.varmappings", "comment", c => c.String());
            AddColumn("dbo.maprecords", "targetId", c => c.String());
            AddColumn("dbo.maprecords", "targetLabel", c => c.String());
            AddColumn("dbo.maprecords", "targetValue", c => c.String());
            AddColumn("dbo.maprecords", "targetType", c => c.String());
            AddColumn("dbo.maprecords", "cohortId", c => c.String());
            AddColumn("dbo.maprecords", "cohortLabel", c => c.String());
            AddColumn("dbo.maprecords", "cohortValue", c => c.String());
            AddColumn("dbo.maprecords", "cohortType", c => c.String());
            AddColumn("dbo.maprecords", "condition", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.maprecords", "condition");
            DropColumn("dbo.maprecords", "cohortType");
            DropColumn("dbo.maprecords", "cohortValue");
            DropColumn("dbo.maprecords", "cohortLabel");
            DropColumn("dbo.maprecords", "cohortId");
            DropColumn("dbo.maprecords", "targetType");
            DropColumn("dbo.maprecords", "targetValue");
            DropColumn("dbo.maprecords", "targetLabel");
            DropColumn("dbo.maprecords", "targetId");
            DropColumn("dbo.varmappings", "comment");
            DropColumn("dbo.varmappings", "script");
            DropColumn("dbo.varmappings", "status");
        }
    }
}
