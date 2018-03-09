namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MapRecords", "MappingTable_Id", "dbo.MappingTables");
            DropForeignKey("dbo.MappingTables", "MapperId", "dbo.Mappers");
            DropForeignKey("dbo.short_variable_vw", "MapSelection_Id", "dbo.MapSelections");
            DropForeignKey("dbo.MapSelections", "selectedTarget_id", "dbo.short_variable_vw");
            DropForeignKey("dbo.Mappers", "Selection_Id", "dbo.MapSelections");
            DropIndex("dbo.Mappers", new[] { "Selection_Id" });
            DropIndex("dbo.MappingTables", new[] { "MapperId" });
            DropIndex("dbo.MapRecords", new[] { "MappingTable_Id" });
            DropIndex("dbo.MapSelections", new[] { "selectedTarget_id" });
            DropIndex("dbo.short_variable_vw", new[] { "MapSelection_Id" });
            AddColumn("dbo.Mappers", "MapSessionId", c => c.Int(nullable: false));
            AddColumn("dbo.Mappers", "TargetDatasetId", c => c.String());
            AddColumn("dbo.Mappers", "TargetVariableId", c => c.String());
            AddColumn("dbo.Mappers", "CohortDatasetId", c => c.String());
            AddColumn("dbo.Mappers", "CohortVariableId", c => c.String());
            AddColumn("dbo.Mappers", "Status", c => c.String());
            AddColumn("dbo.Mappers", "Script", c => c.String());
            AddColumn("dbo.Mappers", "Comment", c => c.String());
            AddColumn("dbo.MapRecords", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.MapRecords", "CreatedBy", c => c.String());
            AddColumn("dbo.MapRecords", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.MapRecords", "ModifiedBy", c => c.String());
            AddColumn("dbo.MapRecords", "Mapper_Id", c => c.Int());
            CreateIndex("dbo.MapRecords", "Mapper_Id");
            AddForeignKey("dbo.MapRecords", "Mapper_Id", "dbo.Mappers", "Id");
            DropColumn("dbo.Mappers", "Selection_Id");
            DropColumn("dbo.MapRecords", "MappingTable_Id");
            DropTable("dbo.MappingTables");
            DropTable("dbo.MapSelections");
            DropTable("dbo.short_variable_vw");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.short_variable_vw",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        nid = c.Long(nullable: false),
                        vid = c.Long(),
                        title = c.String(),
                        status = c.Int(nullable: false),
                        dataset_id = c.Long(),
                        dataset_name = c.String(),
                        MapSelection_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.MapSelections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        selectedTargetDatasetId = c.String(),
                        selectedTargetVariableId = c.String(),
                        selectedCohortDatasetId = c.String(),
                        selectedCohortVariableId = c.String(),
                        selectedTarget_id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MappingTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        status = c.String(),
                        script = c.String(),
                        comment = c.String(),
                        MapperId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MapRecords", "MappingTable_Id", c => c.Int());
            AddColumn("dbo.Mappers", "Selection_Id", c => c.Int());
            DropForeignKey("dbo.MapRecords", "Mapper_Id", "dbo.Mappers");
            DropIndex("dbo.MapRecords", new[] { "Mapper_Id" });
            DropColumn("dbo.MapRecords", "Mapper_Id");
            DropColumn("dbo.MapRecords", "ModifiedBy");
            DropColumn("dbo.MapRecords", "ModifiedDate");
            DropColumn("dbo.MapRecords", "CreatedBy");
            DropColumn("dbo.MapRecords", "CreatedDate");
            DropColumn("dbo.Mappers", "Comment");
            DropColumn("dbo.Mappers", "Script");
            DropColumn("dbo.Mappers", "Status");
            DropColumn("dbo.Mappers", "CohortVariableId");
            DropColumn("dbo.Mappers", "CohortDatasetId");
            DropColumn("dbo.Mappers", "TargetVariableId");
            DropColumn("dbo.Mappers", "TargetDatasetId");
            DropColumn("dbo.Mappers", "MapSessionId");
            CreateIndex("dbo.short_variable_vw", "MapSelection_Id");
            CreateIndex("dbo.MapSelections", "selectedTarget_id");
            CreateIndex("dbo.MapRecords", "MappingTable_Id");
            CreateIndex("dbo.MappingTables", "MapperId");
            CreateIndex("dbo.Mappers", "Selection_Id");
            AddForeignKey("dbo.Mappers", "Selection_Id", "dbo.MapSelections", "Id");
            AddForeignKey("dbo.MapSelections", "selectedTarget_id", "dbo.short_variable_vw", "id");
            AddForeignKey("dbo.short_variable_vw", "MapSelection_Id", "dbo.MapSelections", "Id");
            AddForeignKey("dbo.MappingTables", "MapperId", "dbo.Mappers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MapRecords", "MappingTable_Id", "dbo.MappingTables", "Id");
        }
    }
}
