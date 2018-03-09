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
    public class StudyApiController : ApiController
    {

           private IMicaRepository _repo;
        public StudyApiController(IMicaRepository repo)
        {
            _repo = repo;
        }


        // GET: api/StudyApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/StudyApi/5
        public IEnumerable<dataset_vw> Get(string id)
        {
            var data = _repo.GetStudiesByTargetVariable(id);

            return data;
        }

        // POST: api/StudyApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/StudyApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/StudyApi/5
        public void Delete(int id)
        {
        }
    }
}
