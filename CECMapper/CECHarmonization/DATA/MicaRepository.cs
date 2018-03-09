using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using CECHarmonization.Models;
using MicaData;
using MySql.Data.MySqlClient;

namespace CECHarmonization.DATA
{
    public class MicaRepository : IMicaRepository
    {

        private Entities db = new Entities();

        private string connStr = ConfigurationManager.ConnectionStrings["MicaConnection"].ToString();


        /// <summary>
        /// get the list of taxonomies from MICA
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Taxonomy> GetMicaTaxonomyVocabularies()
        {
            List<Taxonomy> Taxomonies = new List<Taxonomy>();

            //string spName = "dbo.spGetFLASHEMainList";
            string cmdText = "SELECT vid,name,machine_name,description,hierarchy,module,weight FROM taxonomy_vocabulary";


            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                //using (SqlCommand cmd = new SqlCommand(spName, cn))
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    //cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Taxomonies.Add(new Taxonomy
                                {
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    name = (rdr["name"] == DBNull.Value) ? string.Empty : (string)rdr["name"],
                                    machine_name = (rdr["machine_name"] == DBNull.Value) ? string.Empty : (string)rdr["machine_name"],
                                    description = (rdr["description"] == DBNull.Value) ? string.Empty : (string)rdr["description"],
                                    hierarchy = rdr["hierarchy"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["hierarchy"]),
                                    module = (rdr["module"] == DBNull.Value) ? string.Empty : (string)rdr["module"],
                                    weight = rdr["weight"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["weight"]),


                                });
                            }
                        }
                        return Taxomonies;
                    }
                }
            }


        }


        /// <summary>
        /// get the list of terms from MICA
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Term> GetMicaTerms(string[] selectedTaxonomies)
        {
            List<Term> Terms = new List<Term>();

            string result = string.Join(", ", selectedTaxonomies);


            //string spName = "dbo.spGetFLASHEMainList";
            string cmdText = string.Format("SELECT tid, vid, name, description, format, weight FROM taxonomy_term_data where vid in ({0})", result);


            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                //using (SqlCommand cmd = new SqlCommand(spName, cn))
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    //cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Terms.Add(new Term
                                {
                                    tid = rdr["tid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["tid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    name = (rdr["name"] == DBNull.Value) ? string.Empty : (string)rdr["name"],
                                    description = (rdr["description"] == DBNull.Value) ? string.Empty : (string)rdr["description"],
                                    weight = rdr["weight"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["weight"]),
                                    format = (rdr["format"] == DBNull.Value) ? string.Empty : (string)rdr["format"]


                                });
                            }
                        }
                        return Terms;
                    }
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDList"></param>
        /// <returns></returns>
        public IEnumerable<taxonomy_vw> GetTaxonomy_vw(string IDList, string where)
        {
            List<taxonomy_vw> tl = new List<taxonomy_vw>();

            string cmdText;
            if (string.IsNullOrEmpty(where))
                //string spName = "dbo.spGetFLASHEMainList";
                cmdText = string.Format("SELECT vid,tid,name,description, parent FROM taxonomy_vw where tid in ({0})", IDList);
            else
                cmdText = string.Format("SELECT vid,tid,name,description, parent FROM taxonomy_vw where ({0})", where);
            try
            {

                using (MySqlConnection cn = new MySqlConnection(connStr))
                {
                    //using (SqlCommand cmd = new SqlCommand(spName, cn))
                    using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                    {
                        //cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandType = CommandType.Text;

                        cn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    taxonomy_vw tax = new taxonomy_vw();

                                    tax.vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]);
                                    tax.tid = rdr["tid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["tid"]);
                                    tax.name = (rdr["name"] == DBNull.Value) ? string.Empty : (string)rdr["name"];
                                    tax.description = (rdr["description"] == DBNull.Value) ? string.Empty : (string)rdr["description"];
                                    tax.parent = rdr["parent"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["parent"]);

                                    tl.Add(tax);


                                }
                            }
                            return tl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return null;
            }


        }

        public IEnumerable<variable_vw> GetVariable_vw(Dictionary<string, string> IDs)
        {

            List<variable_vw> vr = new List<variable_vw>();

            //string result = string.Join(", ", IDs);

            string whereclauses = null;
            string val = null;


            foreach (string key in IDs.Keys)
            {
                IDs.TryGetValue(key, out val);
                whereclauses = whereclauses + "(" + val + ")  and ";
            }

            if (!String.IsNullOrEmpty(whereclauses))
                whereclauses = "where " + whereclauses;

            // remove the last 
            whereclauses = whereclauses.Remove(whereclauses.Length - 4);

            string cmdText =
            "SELECT v.nid, v.vid,title, v.status, v.field_label_value, v.study_name, v.dataset_name, v.entity_id, " +
                "v.entity_type, v.delta, v.field_variable_categories_name, v.field_variable_categories_label, v.field_variable_categories_missing, " +
                "v.field_value_type_value, v.field_unit_value, v.body_value " +
            "From ( " +
            "   SELECT entity_id, GROUP_CONCAT(DISTINCT tid ORDER BY tid DESC SEPARATOR '_') as tids " +
            "   FROM taxvarmap_vw GROUP BY entity_id " +
            "   ) a " +
            " join variable_vw v on a.entity_id = v.nid " + whereclauses;


            //where (a.tids REGEXP '888' or a.tids REGEXP 885 or a.tids REGEXP 887 or a.tids REGEXP 886 or a.tids REGEXP 884 or a.tids REGEXP 889 or a.tids REGEXP 883 or a.tids REGEXP 882 )  and (a.tids REGEXP '771')  


            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                //using (SqlCommand cmd = new SqlCommand(spName, cn))
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    //cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                vr.Add(new variable_vw
                                {
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"]),
                                    field_label_value = (rdr["field_label_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_label_value"],
                                    study_name = (rdr["study_name"] == DBNull.Value) ? string.Empty : (string)rdr["study_name"],
                                    dataset_name = (rdr["dataset_name"] == DBNull.Value) ? string.Empty : (string)rdr["dataset_name"],
                                    entity_id = rdr["entity_id"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["entity_id"]),
                                    entity_type = (rdr["entity_type"] == DBNull.Value) ? string.Empty : (string)rdr["entity_type"],
                                    delta = rdr["delta"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["delta"]),
                                    field_variable_categories_name = (rdr["field_variable_categories_name"] == DBNull.Value) ? string.Empty : (string)rdr["field_variable_categories_name"],
                                    field_variable_categories_label = (rdr["field_variable_categories_label"] == DBNull.Value) ? string.Empty : (string)rdr["field_variable_categories_label"],
                                    field_variable_categories_missing = (rdr["field_variable_categories_missing"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["field_variable_categories_missing"]),
                                    field_value_type_value = (rdr["field_value_type_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_value_type_value"],
                                    field_unit_value = (rdr["field_unit_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_unit_value"],
                                    body_value = (rdr["body_value"] == DBNull.Value) ? string.Empty : (string)rdr["body_value"]
                                });
                            }
                        }
                        return vr;
                    }
                }
            }


        }


        public IEnumerable<dataset_vw> GetDatasets(string type)
        {
            List<dataset_vw> ds = new List<dataset_vw>();

            string cmdText = string.Format("SELECT distinct nid, vid, type, title, status, entity_id, field_dataset_type_value" +
                " FROM dataset_vw where field_dataset_type_value = '{0}'", type);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ds.Add(new dataset_vw
                                {
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    type = (rdr["type"] == DBNull.Value) ? string.Empty : (string)rdr["type"],
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"])
                                });

                            };
                        }

                        return ds;
                    }
                }
            }
        }


        public IEnumerable<dataset_vw> GetCohortStudies(int datasetId)
        {
            List<dataset_vw> studies = new List<dataset_vw>();

            string cmdText = string.Format("SELECT * FROM dataset_vw where entity_id = '{0}'", datasetId);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                studies.Add(new dataset_vw
                                {
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    type = (rdr["type"] == DBNull.Value) ? string.Empty : (string)rdr["type"],
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    acronym = (rdr["acronym"] == DBNull.Value) ? string.Empty : (string)rdr["acronym"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"]),
                                    entity_id = rdr["entity_id"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["entity_id"]),
                                    dataset_title = (rdr["dataset_title"] == DBNull.Value) ? string.Empty : (string)rdr["dataset_title"],
                                    field_dataset_type_value = (rdr["field_dataset_type_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_dataset_type_value"],
                                    study_id = rdr["study_id"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["study_id"]),
                                    study_title = (rdr["study_title"] == DBNull.Value) ? string.Empty : (string)rdr["study_title"]

                                });

                            };
                        }

                        return studies;
                    }
                }
            }
        }



        public IEnumerable<short_variable_vw> GetVariablesByDataset(string datasetId)
        {

            List<short_variable_vw> vr = new List<short_variable_vw>();

            //string spName = "dbo.spGetFLASHEMainList";
            string cmdText = string.Format("SELECT id, nid, vid, title, status, dataset_id, dataset_name" +
                " FROM short_variable_vw where dataset_id = '{0}' order by title", datasetId);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                vr.Add(new short_variable_vw
                                {
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"]),
                                    dataset_id = (rdr["dataset_id"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["dataset_id"]),
                                    dataset_name = (rdr["dataset_name"] == DBNull.Value) ? string.Empty : (string)rdr["dataset_name"]
                                });

                            };
                        }

                        return vr;
                    }
                }
            }

        }


        public IEnumerable<dataset_vw> GetDatasetsByStudy(string studyId)
        {

            List<dataset_vw> ds = new List<dataset_vw>();

            string cmdText = string.Format("SELECT nid, vid, type, title, status, entity_id, field_dataset_type_value, " +
                "study_id, study_title" +
                " FROM dataset_vw where study_id = '{0}' order by title", studyId);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ds.Add(new dataset_vw
                                {
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    type = (rdr["type"] == DBNull.Value) ? string.Empty : (string)rdr["type"],
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"]),
                                    entity_id = (rdr["entity_id"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["entity_id"]),
                                    field_dataset_type_value = (rdr["field_dataset_type_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_dataset_type_value"],
                                    study_id = (rdr["study_id"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["study_id"]),
                                    study_title = (rdr["study_title"] == DBNull.Value) ? string.Empty : (string)rdr["study_title"]
                                });

                            };
                        }

                        return ds;
                    }
                }
            }

        }

        public IEnumerable<cohort_script_vw> GetCohortScriptByStudy(string studyId)
        {
            List<cohort_script_vw> ds = new List<cohort_script_vw>();

            string cmdText = string.Format("SELECT * FROM cohort_script_vw where entity_id = '{0}' order by title", studyId);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ds.Add(new cohort_script_vw
                                {

                                    entity_id = (rdr["entity_id"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["entity_id"]),
                                    field_study_study_variable_att_nid = rdr["field_study_study_variable_att_nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["field_study_study_variable_att_nid"]),
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    cohort_title = (rdr["cohort_title"] == DBNull.Value) ? string.Empty : (string)rdr["cohort_title"],
                                    field_sva_status_value = (rdr["field_sva_status_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_sva_status_value"],
                                    field_sva_comment_value = (rdr["field_sva_comment_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_sva_comment_value"],
                                    field_sva_script_value = (rdr["field_sva_script_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_sva_script_value"]

                                });

                            };
                        }

                        return ds;
                    }
                }
            }

        }

        public IEnumerable<variable_vw> GetVariableValuesByVariable(string variableId, string studyId)
        {
            List<variable_vw> vr = new List<variable_vw>();

            string cmdText = string.Format("SELECT distinct nid, vid, title, status, field_label_value, dataset_id, dataset_name, " +
                "field_variable_categories_name, field_variable_categories_label, field_value_type_value, field_variable_categories_missing, field_unit_value" +
                " FROM variable_vw where nid = '{0}' and study_id = '{1}'", variableId, studyId);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                vr.Add(new variable_vw
                                {
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"]),
                                    field_label_value = (rdr["field_label_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_label_value"],
                                    dataset_id = (rdr["dataset_id"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["dataset_id"]),
                                    dataset_name = (rdr["dataset_name"] == DBNull.Value) ? string.Empty : (string)rdr["dataset_name"],
                                    field_variable_categories_name = (rdr["field_variable_categories_name"] == DBNull.Value) ? string.Empty : (string)rdr["field_variable_categories_name"],
                                    field_variable_categories_label = ((rdr["field_variable_categories_label"] == DBNull.Value) ? string.Empty : (string)rdr["field_variable_categories_label"]),
                                    field_variable_categories_missing = (rdr["field_variable_categories_missing"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["field_variable_categories_missing"]),
                                    field_value_type_value = (rdr["field_value_type_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_value_type_value"],
                                    field_unit_value = (rdr["field_unit_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_unit_value"]


                                });

                            };
                        }

                        return vr;
                    }
                }
            }
        }


        public IEnumerable<dataset_vw> GetStudiesByTargetVariable(string datasetId)
        {

            List<dataset_vw> studies = new List<dataset_vw>();

            string cmdText = string.Format("SELECT * FROM dataset_vw where entity_id = {0}", datasetId);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                            while (rdr.Read())
                            {
                                studies.Add(new dataset_vw
                                {
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    type = (rdr["type"] == DBNull.Value) ? string.Empty : (string)rdr["type"],
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    acronym = (rdr["acronym"] == DBNull.Value) ? string.Empty : (string)rdr["acronym"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"]),
                                    entity_id = rdr["entity_id"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["entity_id"]),
                                    dataset_title = (rdr["dataset_title"] == DBNull.Value) ? string.Empty : (string)rdr["dataset_title"],
                                    field_dataset_type_value = (rdr["field_dataset_type_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_dataset_type_value"],
                                    study_id = rdr["study_id"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["study_id"]),
                                    study_title = (rdr["study_title"] == DBNull.Value) ? string.Empty : (string)rdr["study_title"]

                                });

                            };
                        

                        return studies;
                    }
                }
            }


            //List<study_vw> vr = new List<study_vw>();

            //string cmdText = string.Format("SELECT variable_id, sva_id, study_id, study_title " +
            //    " FROM study_vw where variable_id = '{0}' order by study_title", variableId);

            //using (MySqlConnection cn = new MySqlConnection(connStr))
            //{
            //    using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
            //    {
            //        cmd.CommandType = CommandType.Text;

            //        cn.Open();
            //        using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
            //        {
            //            if (rdr.HasRows)
            //            {
            //                while (rdr.Read())
            //                {
            //                    vr.Add(new study_vw
            //                    {
            //                        variable_id = rdr["variable_id"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["variable_id"]),
            //                        sva_id = rdr["sva_id"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["sva_id"]),
            //                        study_id = (rdr["study_id"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["study_id"]),
            //                        study_title = rdr["study_title"] == DBNull.Value ? string.Empty : (string)rdr["study_title"]

            //                    });

            //                };
            //            }

            //            return vr;
            //        }
            //    }
            //}
        }


        public IEnumerable<variable_vw> GetVariableValuesByDataset(string datasetId)
        {
            List<variable_vw> vr = new List<variable_vw>();

            string cmdText = string.Format("SELECT nid, vid, study_name, title, status, field_label_value, dataset_id, dataset_name, " +
                "field_variable_categories_name, field_variable_categories_label, field_variable_categories_missing, field_unit_value" +
                " FROM variable_vw where dataset_id = '{0}' order by nid,field_variable_categories_name ", datasetId);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                vr.Add(new variable_vw
                                {
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    study_name = (rdr["study_name"] == DBNull.Value) ? string.Empty : (string)rdr["study_name"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"]),
                                    field_label_value = (rdr["field_label_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_label_value"],
                                    dataset_id = (rdr["dataset_id"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["dataset_id"]),
                                    dataset_name = (rdr["dataset_name"] == DBNull.Value) ? string.Empty : (string)rdr["dataset_name"],
                                    field_variable_categories_name = (rdr["field_variable_categories_name"] == DBNull.Value) ? string.Empty : (string)rdr["field_variable_categories_name"],
                                    field_variable_categories_label = (rdr["field_variable_categories_label"] == DBNull.Value) ? string.Empty : (string)rdr["field_variable_categories_label"],
                                    field_variable_categories_missing = (rdr["field_variable_categories_missing"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["field_variable_categories_missing"]),
                                    field_unit_value = (rdr["field_unit_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_unit_value"]

                                });

                            };
                        }

                        return vr;
                    }
                }
            }
        }


        public IEnumerable<variable_vw> GetVariableValuesByStudy(string studyId)
        {
            List<variable_vw> vr = new List<variable_vw>();

            string cmdText = string.Format("SELECT study_name, nid, vid, title, status, field_label_value, dataset_id, dataset_name, " +
                "field_variable_categories_name, field_variable_categories_label, field_variable_categories_missing, " +
                "field_unit_value, field_value_type_value" +
                " FROM variable_vw where study_id = '{0}' order by dataset_name, title, field_variable_categories_name ", studyId);

            using (MySqlConnection cn = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                {
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                vr.Add(new variable_vw
                                {
                                    study_name = (rdr["study_name"] == DBNull.Value) ? string.Empty : (string)rdr["study_name"],
                                    nid = rdr["nid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["nid"]),
                                    vid = rdr["vid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["vid"]),
                                    title = (rdr["title"] == DBNull.Value) ? string.Empty : (string)rdr["title"],
                                    status = rdr["status"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["status"]),
                                    field_label_value = (rdr["field_label_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_label_value"],
                                    dataset_id = (rdr["dataset_id"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["dataset_id"]),
                                    dataset_name = (rdr["dataset_name"] == DBNull.Value) ? string.Empty : (string)rdr["dataset_name"],
                                    field_variable_categories_name = (rdr["field_variable_categories_name"] == DBNull.Value) ? string.Empty : (string)rdr["field_variable_categories_name"],
                                    field_variable_categories_label = (rdr["field_variable_categories_label"] == DBNull.Value) ? string.Empty : (string)rdr["field_variable_categories_label"],
                                    field_variable_categories_missing = (rdr["field_variable_categories_missing"] == DBNull.Value) ? 0 : Convert.ToInt32(rdr["field_variable_categories_missing"]),
                                    field_unit_value = (rdr["field_unit_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_unit_value"],
                                    field_value_type_value = (rdr["field_value_type_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_value_type_value"]

                                });

                            };
                        }

                        return vr;
                    }
                }
            }
        }


        public string Get_SVA_Comment(string svaId)
        {
            try
            {

                field_data_field_sva_comment x = db.field_data_field_sva_comment.Where(o => o.entity_id.ToString() == svaId).Single();
                return x.field_sva_comment_value;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string Get_SVA_Script(string svaId)
        {
            try
            {

                field_data_field_sva_script x = db.field_data_field_sva_script.Where(o => o.entity_id.ToString() == svaId).Single();
                return x.field_sva_script_value;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string Get_SVA_Status(string svaId)
        {
            try
            {

                field_data_field_sva_status x = db.field_data_field_sva_status.Where(o => o.entity_id.ToString() == svaId).Single();
                return x.field_sva_status_value;

            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public IEnumerable<mica_vw> Get_mica_vw(string svaId)
        {
            List<mica_vw> list = new List<mica_vw>();

            string cmdText;
            cmdText = string.Format("SELECT entity_id, field_sva_comment_value, field_sva_script_value, field_sva_status_value FROM mica_vw where entity_id = ({0})", svaId);
            try
            {

                using (MySqlConnection cn = new MySqlConnection(connStr))
                {
                    //using (SqlCommand cmd = new SqlCommand(spName, cn))
                    using (MySqlCommand cmd = new MySqlCommand(cmdText, cn))
                    {
                        //cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandType = CommandType.Text;

                        cn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.Default))
                        {
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    mica_vw m = new mica_vw();

                                    m.Id = 1;
                                    m.entity_id = rdr["entity_id"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["entity_id"]);
                                    m.field_sva_comment_value = (rdr["field_sva_comment_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_sva_comment_value"];
                                    m.field_sva_script_value = (rdr["field_sva_script_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_sva_script_value"];
                                    m.field_sva_status_value = (rdr["field_sva_status_value"] == DBNull.Value) ? string.Empty : (string)rdr["field_sva_status_value"];

                                    list.Add(m);


                                }
                            }
                            return list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return null;
            }


        }





        public int Save_SVA_Comment(string svaId, string sva_comment)
        {
            if (sva_comment == null)
                return 0;

            List<field_data_field_sva_comment> entry = db.field_data_field_sva_comment.Where(o => o.entity_id.ToString() == svaId).ToList();

            if (entry.Count() == 0)
            {
                //insert
                // get a variable record (we will use most of these values for the new record)
                field_data_field_sva_variable var = db.field_data_field_sva_variable.Where(o => o.entity_id.ToString() == svaId).First();

                // insert the new record
                db.field_data_field_sva_comment.Add(
                    new field_data_field_sva_comment
                    {
                        entity_type = var.entity_type,
                        bundle = var.bundle,
                        deleted = var.deleted,
                        entity_id = var.entity_id,
                        revision_id = var.revision_id,
                        language = var.language,
                        delta = var.delta,
                        field_sva_comment_value = sva_comment,
                        field_sva_comment_format = null
                    });
            }
            else
            {
                //update
                entry.First().field_sva_comment_value = sva_comment;
            }

            return db.SaveChanges();
        }




        public int Save_SVA_Script(string svaId, string sva_script)
        {
            if (sva_script == null)
                return 0;

            List<field_data_field_sva_script> entry = db.field_data_field_sva_script.Where(o => o.entity_id.ToString() == svaId).ToList();

            if (entry.Count() == 0)
            {
                // get a variable record (we will use most of these values for the new record)
                field_data_field_sva_variable var = db.field_data_field_sva_variable.Where(o => o.entity_id.ToString() == svaId).First();

                // insert the new record
                db.field_data_field_sva_script.Add(
                    new field_data_field_sva_script
                    {
                        entity_type = var.entity_type,
                        bundle = var.bundle,
                        deleted = var.deleted,
                        entity_id = var.entity_id,
                        revision_id = var.revision_id,
                        language = var.language,
                        delta = var.delta,
                        field_sva_script_value = sva_script,
                        field_sva_script_format = null
                    });
            }
            else
            {
                entry.First().field_sva_script_value = sva_script;
            }

            return db.SaveChanges();
        }


        public int Save_SVA_Status(string svaId, string sva_status)
        {
            if (sva_status == null)
                return 0;

            List<field_data_field_sva_status> entry = db.field_data_field_sva_status.Where(o => o.entity_id.ToString() == svaId).ToList();

            if (entry.Count() == null)
            {
                // get a variable record (we will use most of these values for the new record)
                field_data_field_sva_variable var = db.field_data_field_sva_variable.Where(o => o.entity_id.ToString() == svaId).First();

                // insert the new record
                db.field_data_field_sva_status.Add(
                    new field_data_field_sva_status
                    {
                        entity_type = var.entity_type,
                        bundle = var.bundle,
                        deleted = var.deleted,
                        entity_id = var.entity_id,
                        revision_id = var.revision_id,
                        language = var.language,
                        delta = var.delta,
                        field_sva_status_value = sva_status
                      
                    });
            }
            else
            {
                entry.First().field_sva_status_value = sva_status;
            }



            return db.SaveChanges();
        }


        public void clear_cache()
        {

            db.clear_cache();
        }

    }
}