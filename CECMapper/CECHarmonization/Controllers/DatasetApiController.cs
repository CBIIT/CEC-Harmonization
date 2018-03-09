using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CECHarmonization.DATA;

namespace CECHarmonization.Controllers
{
    public class DatasetApiController : ApiController
    {

        private IMicaRepository _repo;
        public DatasetApiController(IMicaRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<MicaData.dataset_vw> Get(string type = "harmonization")
        {
            var data = _repo.GetDatasets(type);

            return data;
        }


        public IEnumerable<MicaData.dataset_vw> Get(string action, string id)
        {
            var data = _repo.GetDatasetsByStudy(id);

            return data;
        }

    }
}
