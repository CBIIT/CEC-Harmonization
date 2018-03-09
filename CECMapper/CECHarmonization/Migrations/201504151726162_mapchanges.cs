namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mapchanges : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.varmappings", newName: "mappingtables");
            DropForeignKey("dbo.Cohorts", "Mapper_Id", "dbo.Mappers");
            DropIndex("dbo.Cohorts", new[] { "Mapper_Id" });
            RenameColumn(table: "dbo.maprecords", name: "varmapping_Id", newName: "mappingtable_Id");
            RenameIndex(table: "dbo.maprecords", name: "IX_varmapping_Id", newName: "IX_mappingtable_Id");
            CreateTable(
                "dbo.mapselections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        selectedTargetDatasetId = c.String(),
                        selectedTargetVariableId = c.String(),
                        selectedCohortDatasetId = c.String(),
                        selectedCohortVariableId = c.String(),
                        selectedTarget_id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.short_variable_vw", t => t.selectedTarget_id)
                .Index(t => t.selectedTarget_id);
            
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
                        mapselection_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.mapselections", t => t.mapselection_Id)
                .Index(t => t.mapselection_Id);
            
            AddColumn("dbo.Mappers", "selection_Id", c => c.Int());
            CreateIndex("dbo.Mappers", "selection_Id");
            AddForeignKey("dbo.Mappers", "selection_Id", "dbo.mapselections", "Id");
            DropColumn("dbo.Mappers", "selectedTargetId");
            DropTable("dbo.Cohorts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Cohorts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Mapper_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Mappers", "selectedTargetId", c => c.String());
            DropForeignKey("dbo.Mappers", "selection_Id", "dbo.mapselections");
            DropForeignKey("dbo.mapselections", "selectedTarget_id", "dbo.short_variable_vw");
            DropForeignKey("dbo.short_variable_vw", "mapselection_Id", "dbo.mapselections");
            DropIndex("dbo.short_variable_vw", new[] { "mapselection_Id" });
            DropIndex("dbo.mapselections", new[] { "selectedTarget_id" });
            DropIndex("dbo.Mappers", new[] { "selection_Id" });
            DropColumn("dbo.Mappers", "selection_Id");
            DropTable("dbo.short_variable_vw");
            DropTable("dbo.mapselections");
            RenameIndex(table: "dbo.maprecords", name: "IX_mappingtable_Id", newName: "IX_varmapping_Id");
            RenameColumn(table: "dbo.maprecords", name: "mappingtable_Id", newName: "varmapping_Id");
            CreateIndex("dbo.Cohorts", "Mapper_Id");
            AddForeignKey("dbo.Cohorts", "Mapper_Id", "dbo.Mappers", "Id");
            RenameTable(name: "dbo.mappingtables", newName: "varmappings");
        }
    }
}
