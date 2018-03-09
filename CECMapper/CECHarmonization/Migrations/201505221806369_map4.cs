namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class map4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MapConditions", "MapCondition_Id", "dbo.MapConditions");
            DropIndex("dbo.MapConditions", new[] { "MapCondition_Id" });
            AddColumn("dbo.MapConditions", "LogicSort", c => c.String());
            DropColumn("dbo.MapConditions", "MapCondition_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MapConditions", "MapCondition_Id", c => c.Int());
            DropColumn("dbo.MapConditions", "LogicSort");
            CreateIndex("dbo.MapConditions", "MapCondition_Id");
            AddForeignKey("dbo.MapConditions", "MapCondition_Id", "dbo.MapConditions", "Id");
        }
    }
}
