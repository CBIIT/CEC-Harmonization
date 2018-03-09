namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3rd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cohorts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Mapper_ID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mappers", t => t.Mapper_ID)
                .Index(t => t.Mapper_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cohorts", "Mapper_ID", "dbo.Mappers");
            DropIndex("dbo.Cohorts", new[] { "Mapper_ID" });
            DropTable("dbo.Cohorts");
        }
    }
}
