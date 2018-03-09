namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class map3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MapConditions", "LogicOperator", c => c.String());
            AddColumn("dbo.MapConditions", "MapCondition_Id", c => c.Int());
            CreateIndex("dbo.MapConditions", "MapCondition_Id");
            AddForeignKey("dbo.MapConditions", "MapCondition_Id", "dbo.MapConditions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MapConditions", "MapCondition_Id", "dbo.MapConditions");
            DropIndex("dbo.MapConditions", new[] { "MapCondition_Id" });
            DropColumn("dbo.MapConditions", "MapCondition_Id");
            DropColumn("dbo.MapConditions", "LogicOperator");
        }
    }
}
