namespace CECHarmonization.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class night : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mappers", "StudyVariableAttributeId", c => c.String());
            AddColumn("dbo.MapRecords", "MapType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MapRecords", "MapType");
            DropColumn("dbo.Mappers", "StudyVariableAttributeId");
        }
    }
}
