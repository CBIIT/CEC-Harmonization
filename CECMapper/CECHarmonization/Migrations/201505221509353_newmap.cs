namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmap : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MapConditions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        valueId = c.String(),
                        valueName = c.String(),
                        valueType = c.String(),
                        valueUnit = c.String(),
                        operation = c.String(),
                        value = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        MapRecord_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MapRecords", t => t.MapRecord_Id)
                .Index(t => t.MapRecord_Id);
            
            AddColumn("dbo.Mappers", "MapType", c => c.String());
            AddColumn("dbo.MapRecords", "TargetUnits", c => c.String());
            AddColumn("dbo.MapRecords", "Action", c => c.String());
            DropColumn("dbo.MapRecords", "MapType");
            DropColumn("dbo.MapRecords", "CohortId");
            DropColumn("dbo.MapRecords", "CohortLabel");
            DropColumn("dbo.MapRecords", "CohortValue");
            DropColumn("dbo.MapRecords", "CohortType");
            DropColumn("dbo.MapRecords", "Condition");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MapRecords", "Condition", c => c.String());
            AddColumn("dbo.MapRecords", "CohortType", c => c.String());
            AddColumn("dbo.MapRecords", "CohortValue", c => c.String());
            AddColumn("dbo.MapRecords", "CohortLabel", c => c.String());
            AddColumn("dbo.MapRecords", "CohortId", c => c.String());
            AddColumn("dbo.MapRecords", "MapType", c => c.String());
            DropForeignKey("dbo.MapConditions", "MapRecord_Id", "dbo.MapRecords");
            DropIndex("dbo.MapConditions", new[] { "MapRecord_Id" });
            DropColumn("dbo.MapRecords", "Action");
            DropColumn("dbo.MapRecords", "TargetUnits");
            DropColumn("dbo.Mappers", "MapType");
            DropTable("dbo.MapConditions");
        }
    }
}
