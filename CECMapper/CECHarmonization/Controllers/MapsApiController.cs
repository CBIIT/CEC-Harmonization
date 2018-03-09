using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CECHarmonization.DATA;
using CECHarmonization.Models;
using MicaData;

namespace CECHarmonization.Controllers
{
    public class MapsApiController : ApiController
    {


        /// map api verbs - get, post, put, delete
        /// 
        /// use the Repository Pattern here - 
        ///     create a consructor and include repository as 
        ///     an injected resource into our controller

        private IMapperRepository _repo;
        private IMicaRepository _Micarepo;
        public MapsApiController(IMapperRepository repo, IMicaRepository Micarepo)
        {
            _repo = repo;
            _Micarepo = Micarepo;
        }

        public IEnumerable<Mapper> Get()
        {
            var MapList = _repo.GetMappers();
            return MapList;
        }

        public Mapper Get(string targetId, string svaId, string studyId)
        {
            try
            {
                //	1.  get list of values for this Variable from MICA
                List<variable_vw> MicaVars = _Micarepo.GetVariableValuesByVariable(targetId, studyId).ToList();
                Mapper mapper = _repo.GetMapperByVariables(targetId, svaId);

                //	2. If no Mapper record exists (for this target and sva) then
                //      a. Create a new mapper and one maprecord for each target value
                if (mapper == null)
                {

                    Mapper m = new Mapper { StudyVariableAttributeId = svaId, TargetFieldId = targetId, MapRecs = new List<MapRecord>() };

                    foreach (variable_vw v in MicaVars)
                    {
                        m.MapRecs.Add(new MapRecord
                        {
                            TargetFieldId = v.nid.ToString(),
                            TargetFieldName = v.title,
                            TargetLabel = v.field_variable_categories_label,
                            TargetValue = v.field_variable_categories_name,
                            TargetMissing = v.field_variable_categories_missing.ToString(),
                            TargetType = v.field_value_type_value,
                            TargetUnits = v.field_unit_value,
                            CreatedDate = DateTime.Now,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name
                        });
                    }


                    // In the API - we get the value of m through the body of the message as opossed to parameters 
                    if (_repo.AddMapper(m) && _repo.Save())
                    {
                        return m;
                    }

                    return m;
                }

                else
                    //  3. If any target value does not have a maprecord then
                    //      a. Create a new maprecord for that value

                    foreach (variable_vw v in MicaVars)
                    {
                        // if the record exists update it with any new MICA info
                        if (mapper.MapRecs.Any(o => o.TargetValue == v.field_variable_categories_name))
                        {
                            MapRecord mr = mapper.MapRecs.Where(o => o.TargetValue == v.field_variable_categories_name).First();

                            mr.TargetFieldId = v.nid.ToString();
                            mr.TargetFieldName = v.title;
                            mr.TargetLabel = v.field_variable_categories_label;
                            mr.TargetValue = v.field_variable_categories_name;
                            mr.TargetMissing = v.field_variable_categories_missing.ToString();
                            mr.TargetType = v.field_value_type_value;
                            mr.TargetUnits = v.field_unit_value;
                            mr.ModifiedDate = DateTime.Now;
                            mr.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        }
                        else
                        // insert a new record with MICA data
                        {
                            mapper.MapRecs.Add(new MapRecord
                            {
                                TargetFieldId = v.nid.ToString(),
                                TargetFieldName = v.title,
                                TargetLabel = v.field_variable_categories_label,
                                TargetValue = v.field_variable_categories_name,
                                TargetMissing = v.field_variable_categories_missing.ToString(),
                                TargetType = v.field_value_type_value,
                                TargetUnits = v.field_unit_value,
                                CreatedDate = DateTime.Now,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name
                            });
                        }
                    }



                //  4. return Mapper records 

                return mapper;

            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return null;
            }

        }


        public HttpResponseMessage Post([FromBody]Mapper m)
        {

            if (m.CreatedDate == default(DateTime))
            {
                m.CreatedDate = DateTime.UtcNow;
            }

            // In the API - we get the value of m through the body of the message as opossed to parameters 
            if (_repo.AddMapper(m) && _repo.Save())
            {
                // return Created ('201') and the new data m
                return Request.CreateResponse(HttpStatusCode.Created, m);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }


        // Put method accepts two parameters, the Id of the updated resource which is set in URI, and the updated "Mapper” 
        //  which represents complex type deserialized in the request body
        // We return “HttpResponseMessage” for all possible scenarios that might happen when we execute this operation. 
        //  In case the resource is updated successfully, server should return HTTP response 200 (OK) along with the 
        //  resource created. If the resource is not modified the server should return HTTP response 304 (Not modified).
        public HttpResponseMessage Put([FromBody]Mapper m)
        {
            if (_repo.UpdateMapper(m))
            {
                _repo.Save();
                return Request.CreateResponse(HttpStatusCode.Created, m);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

    }
}
