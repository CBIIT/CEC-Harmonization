using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CECHarmonization.Models;

namespace CECHarmonization.DATA
{
    public interface IMapperRepository
    {
        // NOTE: use IQueryable in case we need to do paging, ordering, filtering, or grouping  (if not needed use INumerable)

        IQueryable<Mapper> GetMappers();

        Mapper GetMapperByVariables(string targetId, string svaId);
        bool Save();
        bool AddMapper(Mapper m);
        bool UpdateMapper(Mapper m);
        bool UpdateMapRecord(MapRecord mr);
        bool DeleteMapper(Mapper m);
    }
}
