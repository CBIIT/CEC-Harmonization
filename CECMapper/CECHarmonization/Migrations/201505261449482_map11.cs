namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class map11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MapConditions", "fieldId", c => c.String());
            AddColumn("dbo.MapConditions", "fieldName", c => c.String());
            AddColumn("dbo.MapConditions", "valuelabel", c => c.String());
            AddColumn("dbo.MapConditions", "valuemissing", c => c.String());
            DropColumn("dbo.MapConditions", "valueId");
            DropColumn("dbo.MapConditions", "valueName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MapConditions", "valueName", c => c.String());
            AddColumn("dbo.MapConditions", "valueId", c => c.String());
            DropColumn("dbo.MapConditions", "valuemissing");
            DropColumn("dbo.MapConditions", "valuelabel");
            DropColumn("dbo.MapConditions", "fieldName");
            DropColumn("dbo.MapConditions", "fieldId");
        }
    }
}
