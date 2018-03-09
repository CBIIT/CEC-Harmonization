using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using CECHarmonization.DATA;
using CECHarmonization.Models;
using MicaData;
using Newtonsoft.Json;

namespace CECHarmonization.Controllers
{
    public class MicaApiController : ApiController
    {

        private MicaRepository micadb = new MicaRepository();

        // GET: api/MicaApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MicaApi/5
        public IEnumerable<mica_vw> Get(string id)
        {
            //List<svaData> d = new List<svaData>();

            //string comment = micadb.Get_SVA_Comment(id);
            //string script = micadb.Get_SVA_Script(id);
            //string status = micadb.Get_SVA_Status(id);

            //d.Add (new  svaData { svaId = id, comment = comment, script = script, status = status}) ;

            IEnumerable<mica_vw> d = micadb.Get_mica_vw(id);

            return d;

        }



        // Put method accepts two parameters, the Id of the updated resource which is set in URI, and the updated "Mapper” 
        //  which represents complex type deserialized in the request body
        // We return “HttpResponseMessage” for all possible scenarios that might happen when we execute this operation. 
        //  In case the resource is updated successfully, server should return HTTP response 200 (OK) along with the 
        //  resource created. If the resource is not modified the server should return HTTP response 304 (Not modified).
        public HttpResponseMessage Put(string svaId, [FromBody]mica_vw m)
        {

            int com = micadb.Save_SVA_Comment(svaId, m.field_sva_comment_value);
            int scr = micadb.Save_SVA_Script(svaId, m.field_sva_script_value);
            int sta = micadb.Save_SVA_Status(svaId, m.field_sva_status_value);

            if (com == 1)
                return Request.CreateResponse(HttpStatusCode.Created, m);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }


        // POST: api/MicaApi
        public void Post(string svaId, [FromBody]mica_vw m)
        {

            int com = micadb.Save_SVA_Comment(svaId, m.field_sva_comment_value);
            int scr = micadb.Save_SVA_Script(svaId, m.field_sva_script_value);
            int sta = micadb.Save_SVA_Status(svaId, m.field_sva_status_value);

            micadb.clear_cache();
        }

        // PUT: api/MicaApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MicaApi/5
        public void Delete(int id)
        {
        }
    }
}
