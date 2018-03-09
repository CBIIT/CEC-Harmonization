namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mappers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MapSessionId = c.Int(nullable: false),
                        User = c.String(),
                        TargetDatasetId = c.String(),
                        TargetVariableId = c.String(),
                        CohortDatasetId = c.String(),
                        CohortVariableId = c.String(),
                        Status = c.String(),
                        Script = c.String(),
                        Comment = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MapRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TargetId = c.String(),
                        TargetLabel = c.String(),
                        TargetValue = c.String(),
                        TargetType = c.String(),
                        CohortId = c.String(),
                        CohortLabel = c.String(),
                        CohortValue = c.String(),
                        CohortType = c.String(),
                        Condition = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        Mapper_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mappers", t => t.Mapper_Id)
                .Index(t => t.Mapper_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MapRecords", "Mapper_Id", "dbo.Mappers");
            DropIndex("dbo.MapRecords", new[] { "Mapper_Id" });
            DropTable("dbo.MapRecords");
            DropTable("dbo.Mappers");
        }
    }
}
