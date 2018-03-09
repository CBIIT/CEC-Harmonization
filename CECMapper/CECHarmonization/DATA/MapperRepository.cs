using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CECHarmonization.Models;
using Ninject;

namespace CECHarmonization.DATA
{
    public class MapperRepository : IMapperRepository
    {

        /// <summary>
        /// We pass in the context so that we do not create it over and over (dependency injection)
        ///   To use this we bind this context in the NinjectWebCommon.sc file
        /// </summary>
        private MapContext _ctx;

        public MapperRepository(MapContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Mapper> GetMappers()
        {
            return _ctx.Mappers;

        }

        public Mapper GetMapperByVariables(string targetId, string svaId)
        {
            try
            {
                //return _ctx.Mappers.Where(o => o.TargetVariableId == targetId && o.StudyVariableAttributeId == svaId).First();


                var m = _ctx.Mappers.Where(o => o.TargetFieldId == targetId && o.StudyVariableAttributeId == svaId).First();
                return m;


            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public bool Save()
        {
            try
            {
                // return how many row changes were made (as long as it is greater than 0
                return _ctx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                // to do - log error
                return false;

            }
        }

        public bool AddMapper(Mapper m)
        {
            try
            {
                m.CreatedDate = DateTime.Now;

                _ctx.Mappers.Add(m);
                return true;
            }
            catch (Exception)
            {
                //to do - log error
                return false;
            }
        }

        public bool UpdateMapper(Mapper m)
        {
            try
            {

                m.ModifiedDate = DateTime.Now;

                _ctx.Entry(m).State = EntityState.Modified;

                // Do some more work...  

                _ctx.SaveChanges();

                foreach (MapRecord mr in m.MapRecs)
                    UpdateMapRecord(mr);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool UpdateMapRecord(MapRecord mr)
        {
            try
            {
                mr.ModifiedDate = DateTime.Now;

                _ctx.Entry(mr).State = EntityState.Modified;

                // Do some more work...  

                _ctx.SaveChanges();



                return true;
            }
            catch
            {
                return false;
            }
        }



        public bool DeleteMapper(Mapper m)
        {
            throw new NotImplementedException();
        }

    }
}