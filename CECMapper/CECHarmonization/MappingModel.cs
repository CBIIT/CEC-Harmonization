namespace CECHarmonization
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using CECHarmonization.Models;

    public class MappingModel : DbContext
    {
        // Your context has been configured to use a 'MappingModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'CECHarmonization.MappingModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'MappingModel' 
        // connection string in the application configuration file.
        public MappingModel()
            : base("name=MappingModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Mapper> MapperEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}