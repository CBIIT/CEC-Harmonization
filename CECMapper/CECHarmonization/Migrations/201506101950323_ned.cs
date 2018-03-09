namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ned : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MapConditions", "MapRecord_Id", "dbo.MapRecords");
            DropIndex("dbo.MapConditions", new[] { "MapRecord_Id" });
            AddColumn("dbo.MapRecords", "json", c => c.String());
            AddColumn("dbo.MapRecords", "selectedAction", c => c.String());
            AddColumn("dbo.MapRecords", "scriptSection", c => c.String());
            DropColumn("dbo.MapRecords", "Action");
            DropTable("dbo.MapConditions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MapConditions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldId = c.String(),
                        FieldName = c.String(),
                        Operation = c.String(),
                        Value = c.String(),
                        ValueLabel = c.String(),
                        ValueMissing = c.String(),
                        ValueType = c.String(),
                        ValueUnit = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ParentId = c.Int(nullable: false),
                        LogicOperator = c.String(),
                        LogicSort = c.String(),
                        MapRecord_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MapRecords", "Action", c => c.String());
            DropColumn("dbo.MapRecords", "scriptSection");
            DropColumn("dbo.MapRecords", "selectedAction");
            DropColumn("dbo.MapRecords", "json");
            CreateIndex("dbo.MapConditions", "MapRecord_Id");
            AddForeignKey("dbo.MapConditions", "MapRecord_Id", "dbo.MapRecords", "Id");
        }
    }
}
