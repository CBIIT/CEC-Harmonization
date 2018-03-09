using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CECHarmonization.DATA;
using MicaData;

namespace CECHarmonization.Controllers
{
    public class VariableApiController : ApiController
    {

        private IMicaRepository _repo;
        public VariableApiController(IMicaRepository repo)
        {
            _repo = repo;
        }


       

        // GET api/<controller>/5
        public IEnumerable<short_variable_vw> Get(string id)
        {
            var data = _repo.GetVariablesByDataset(id);

            return data;
        }

        // get the list of variable values for the Mapper page
        public IEnumerable<variable_vw> Get(string action, string id, string studyId)
        {
            var data = new List<variable_vw>(); 

            if (action == "1")
                data = _repo.GetVariableValuesByVariable(id, studyId).ToList();
            else 
                data = _repo.GetVariableValuesByDataset(id).ToList();


            return data;
        }



        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}