namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wed : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.varmappings", new[] { "Mapper_ID" });
            DropIndex("dbo.maprecords", new[] { "varmapping_ID" });
            DropIndex("dbo.Cohorts", new[] { "Mapper_ID" });
            AddColumn("dbo.Mappers", "user", c => c.String());
            CreateIndex("dbo.varmappings", "Mapper_Id");
            CreateIndex("dbo.maprecords", "varmapping_Id");
            CreateIndex("dbo.Cohorts", "Mapper_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Cohorts", new[] { "Mapper_Id" });
            DropIndex("dbo.maprecords", new[] { "varmapping_Id" });
            DropIndex("dbo.varmappings", new[] { "Mapper_Id" });
            DropColumn("dbo.Mappers", "user");
            CreateIndex("dbo.Cohorts", "Mapper_ID");
            CreateIndex("dbo.maprecords", "varmapping_ID");
            CreateIndex("dbo.varmappings", "Mapper_ID");
        }
    }
}
