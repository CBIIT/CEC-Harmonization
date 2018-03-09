namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemaps : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.mappingtables", "Mapper_Id", "dbo.Mappers");
            DropIndex("dbo.MappingTables", new[] { "Mapper_Id" });
            DropIndex("dbo.MapRecords", new[] { "mappingtable_Id" });
            DropIndex("dbo.short_variable_vw", new[] { "mapselection_Id" });
            RenameColumn(table: "dbo.MappingTables", name: "Mapper_Id", newName: "MapperId");
            AddColumn("dbo.Mappers", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MappingTables", "MapperId", c => c.Int(nullable: false));
            CreateIndex("dbo.MappingTables", "MapperId");
            CreateIndex("dbo.MapRecords", "MappingTable_Id");
            CreateIndex("dbo.short_variable_vw", "MapSelection_Id");
            AddForeignKey("dbo.MappingTables", "MapperId", "dbo.Mappers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MappingTables", "MapperId", "dbo.Mappers");
            DropIndex("dbo.short_variable_vw", new[] { "MapSelection_Id" });
            DropIndex("dbo.MapRecords", new[] { "MappingTable_Id" });
            DropIndex("dbo.MappingTables", new[] { "MapperId" });
            AlterColumn("dbo.MappingTables", "MapperId", c => c.Int());
            DropColumn("dbo.Mappers", "CreatedDate");
            RenameColumn(table: "dbo.MappingTables", name: "MapperId", newName: "Mapper_Id");
            CreateIndex("dbo.short_variable_vw", "mapselection_Id");
            CreateIndex("dbo.MapRecords", "mappingtable_Id");
            CreateIndex("dbo.MappingTables", "Mapper_Id");
            AddForeignKey("dbo.mappingtables", "Mapper_Id", "dbo.Mappers", "Id");
        }
    }
}
