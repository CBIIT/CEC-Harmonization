using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using MicaData;

namespace CECHarmonization.Models
{

    public class MapContext : DbContext
    {
        public MapContext()
            : base("MappingModel")  // call into base class with a connection name
        { }

        // properties on the context that represent the table in the DB
        //-------------------------------------------------------------------
        public DbSet<Mapper> Mappers { get; set; }
        //public DbSet<MapSelection> mapselections { get; set; }

        public DbSet<MapRecord> MapRecords { get; set; }
       

    }


    public class MapSession
    {
        public int Id { get; set; }
        public virtual ICollection<Mapper> Maps { get; set; }
        public string User { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

    }

    public class Mapper
    {

        public Mapper() { }

        // The mappings property is a navigation property. Navigation properties hold other entities that are related to this entity. In this case, 
        //   the mappings property of a Mapper entity will hold all of the map entities that are related to that Mapper entity. 

        // Navigation properties are typically defined as virtual so that they can take advantage of certain Entity Framework functionality such as 
        //   lazy loading.

        // If a navigation property can hold multiple entities (as in many-to-many or one-to-many relationships), its type must be a list in 
        //   which entries can be added, deleted, and updated, such as ICollection.

        public int Id { get; set; }

        public virtual ICollection<MapRecord> MapRecs { get; set; }
        public string User { get; set; }

        public string TargetDatasetId { get; set; }
        public string TargetDatasetName { get; set; }
        public string TargetFieldId { get; set; }
        public string TargetFieldName { get; set; }
        public string StudyVariableAttributeId { get; set; }
        public string CohortDatasetId { get; set; }
        public string CohortDatasetName { get; set; }
        
        // Mica Data
        public string Status { get; set; }
        public string Script { get; set; }
        public string Comment { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

    }

    //public class Cohort
    //{
    //    public int Id { get; set; }
    //    public string Text { get; set; }
    //}
    /// <summary>
    /// this is the selection criteria used to do the mapping
    /// -- There can be only one Target with several Cohorts
    /// 
    /// -- Each Target/Cohort combo will have it's own mapping table
    /// </summary>
    /// 
    //public class MappingTable
    //{
    //    public int Id { get; set; }
    //    public virtual ICollection<MapRecord> maps { get; set; }
    //    public string status { get; set; }
    //    public string script { get; set; }
    //    public string comment { get; set; }

    //    public int MapperId { get; set; }
    //}


    //public class MapSelection
    //{
    //    public int Id { get; set; }
    //    public string selectedTargetDatasetId { get; set; }
    //    public string selectedTargetVariableId { get; set; }
    //    public string selectedCohortDatasetId { get; set; }
    //    public string selectedCohortVariableId { get; set; }
    //    public short_variable_vw selectedTarget { get; set; }
    //    public List<short_variable_vw> selectedCohorts { get; set; }

    //}



    public class MapRecord
    {

        public int Id { get; set; }
        public string TargetFieldId { get; set; }
        public string TargetFieldName { get; set; }
        public string TargetValue { get; set; }
        public string TargetLabel { get; set; }
        public string TargetMissing { get; set; }  // indicator that shows if this representation a 'missing' value 
        public string TargetType { get; set; }
        public string TargetUnits { get; set; }


        public string filter { get; set; }
        public string scriptSection { get; set; }  //output
        public string json { get; set; }

        public string selectedAction { get; set; }  // set; if; else if;
//        public virtual ICollection<MapGroup> MapGroups { get; set; }

        //public string CohortId { get; set; }
        //public string CohortLabel { get; set; }
        //public string CohortValue { get; set; }
        //public string CohortType { get; set; }

        //public string Condition { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }


    }

    // MapRecord could have many conditions
    //public class MapGroup
    //{
    //    public int Id {get; set;}
    //    public string FieldId { get; set; }    // if null, then this was free text entry
    //    public string FieldName { get; set; }
    //    public string Operation { get; set; }  // = / * <> 
    //    public string Value { get; set; }  // the value that makes this contdition true 
    //    public string ValueLabel { get; set; }  // the text representation of the value that makes this contdition true 
    //    public string ValueMissing { get; set; }  // indicator that shows if this representation a 'missing' value 
    //    public string ValueType { get; set; }
    //    public string ValueUnit { get; set; }

    //    public DateTime? CreatedDate { get; set; }
    //    public string CreatedBy { get; set; }
    //    public DateTime? ModifiedDate { get; set; }
    //    public string ModifiedBy { get; set; }

    //    // there can be one to many related conditions separated by the LogicOperator
    //    public int ParentId { get; set; }  
    //    public string Operator { get; set; }  // AND  OR
    //    public string LogicSort { get; set; }  // AND  OR

    //}








}