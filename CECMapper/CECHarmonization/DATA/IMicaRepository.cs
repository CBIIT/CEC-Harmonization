using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CECHarmonization.Models;
using MicaData;

namespace CECHarmonization.DATA
{
    public interface IMicaRepository
    {

        IEnumerable<Taxonomy> GetMicaTaxonomyVocabularies();
        IEnumerable<Term> GetMicaTerms(string[] selectedTaxonomies);

        IEnumerable<taxonomy_vw> GetTaxonomy_vw(string IDList, string where);
        IEnumerable<variable_vw> GetVariable_vw(Dictionary<string, string> IDs);        
        //IEnumerable<Mica_Variable> GetMicaVariables();

        // may not need these if I can use GetDatasetItems in MapperController
        IEnumerable<dataset_vw> GetDatasets(string type);
        IEnumerable<dataset_vw> GetDatasetsByStudy(string studyId);

        IEnumerable<dataset_vw> GetStudiesByTargetVariable(string datasetId);
              
        IEnumerable<short_variable_vw> GetVariablesByDataset(string datasetId);

        IEnumerable<variable_vw> GetVariableValuesByVariable(string variableId, string studyId);
        IEnumerable<variable_vw> GetVariableValuesByDataset(string datasetId);

        IEnumerable<cohort_script_vw> GetCohortScriptByStudy(string studyId);

        
        string Get_SVA_Comment(string svaId);
        string Get_SVA_Script(string svaId);
        string Get_SVA_Status(string svaId);

        int Save_SVA_Comment(string svaId, string sva_comment);
        int Save_SVA_Script(string svaId, string sva_script);
        int Save_SVA_Status(string svaId, string sva_status);
    }
}
