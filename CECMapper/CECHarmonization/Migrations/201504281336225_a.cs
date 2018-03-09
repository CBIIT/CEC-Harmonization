namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Mappers", new[] { "selection_Id" });
            AddColumn("dbo.Mappers", "CreatedBy", c => c.String());
            AddColumn("dbo.Mappers", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Mappers", "ModifiedBy", c => c.String());
            CreateIndex("dbo.Mappers", "Selection_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Mappers", new[] { "Selection_Id" });
            DropColumn("dbo.Mappers", "ModifiedBy");
            DropColumn("dbo.Mappers", "ModifiedDate");
            DropColumn("dbo.Mappers", "CreatedBy");
            CreateIndex("dbo.Mappers", "selection_Id");
        }
    }
}
