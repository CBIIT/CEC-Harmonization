namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wed1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mappers", "TargetDatasetName", c => c.String());
            AddColumn("dbo.Mappers", "TargetFieldId", c => c.String());
            AddColumn("dbo.Mappers", "TargetFieldName", c => c.String());
            AddColumn("dbo.Mappers", "CohortDatasetName", c => c.String());
            DropColumn("dbo.Mappers", "MapSessionId");
            DropColumn("dbo.Mappers", "TargetVariableId");
            DropColumn("dbo.Mappers", "CohortVariableId");
            DropColumn("dbo.Mappers", "MapType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mappers", "MapType", c => c.String());
            AddColumn("dbo.Mappers", "CohortVariableId", c => c.String());
            AddColumn("dbo.Mappers", "TargetVariableId", c => c.String());
            AddColumn("dbo.Mappers", "MapSessionId", c => c.Int(nullable: false));
            DropColumn("dbo.Mappers", "CohortDatasetName");
            DropColumn("dbo.Mappers", "TargetFieldName");
            DropColumn("dbo.Mappers", "TargetFieldId");
            DropColumn("dbo.Mappers", "TargetDatasetName");
        }
    }
}
