using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Jayrock.Json;
using Jayrock.Json.Conversion;
using Jayrock.JsonRpc;
using Jayrock.JsonRpc.Web;



namespace cec_publicservice
{

    //[JsonRpcService("CECHarmPublicService")]
    public class CECHarmPublicService : Jayrock.JsonRpc.Web.JsonRpcHandler
    {
        private string database_connection_string;

        private WebServiceHelper srvhelp;

        #region Constructor

        public CECHarmPublicService()
        {
            database_connection_string = WebConfigurationManager.ConnectionStrings["prdCEC"].ConnectionString;
            srvhelp = new WebServiceHelper();
        }
        #endregion

        #region Private Routines

        private bool IsValidSessionId(SecurityToken sec)
        {
            bool valid = true;

            // public website does not require user logins and so, all sessions are valid
            return valid;
        }

        private string ComputeSHA256(string rawStr)
        {
            byte[] braw = Encoding.ASCII.GetBytes(rawStr + GetHashSalt());
            byte[] boutput = (new SHA256Managed()).ComputeHash(braw);

            string hashed = string.Empty;
            foreach (byte b in boutput)
                hashed += b.ToString("x2");

            return hashed;
        }

        private string GetHashSalt()
        {
            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call to get user account
                SqlCommand myCommand = new SqlCommand(String.Format("select configuration from tbl_user_policies where policy='passwordSalt'"), myConnection);
                myCommand.CommandType = CommandType.Text;

                object salt = myCommand.ExecuteScalar();
                if (salt == null)
                    throw new Exception("something went wrong when the webservice retreived the password salt from the database");

                return (salt as string);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }
        }

        private UserData FORDEBUG_UserInfo()
        {
            UserData ud = new UserData();
            ud.access_level = 0;
            ud.email = "noone@nowhere.com";
            ud.display_name = "no one";
            ud.help_shown = true;
            ud.password_expired = false;
            ud.password_reset_required = false;

            return ud;
        }
        #endregion

        #region Audit Log Routines

        public bool AuditLog_AddActivity(int userId, string activity)
        {
            bool success = false;

            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@uid", userId);
                para[1] = new SqlParameter("@activity", activity);

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand();
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.CommandText = "fsp_append_audit_log";
                myCommand.Connection = myConnection;

                myCommand.Parameters.AddRange(para);

                myCommand.ExecuteNonQuery();

                success = true;
            }
            catch
            {
                success = false;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return success;
        }
        
        public DataTable AuditLog_GetActivities(SecurityToken sec, string activity, int record_id)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("invalid session id");
            
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);
            DataTable results = new DataTable();
            try
            {
                // open connection
                myConnection.Open();

                string query = String.Format("select * from tbl_audit_log where activity like '%{0}%' ", activity);
                if (record_id != 0)
                    query += String.Format(" and activity like '%{0}%'", record_id);

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(query, myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(results);

            }
            catch
            {
                results = new DataTable();
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return results;
        }
        #endregion

        #region Web Cohort Fields/Web Filter Fields Routines

        [JsonRpcMethod("getCohortWebFieldLabelByColumnName", Idempotent = true)]
        public string GetCohortWebFieldLabelByColumnName(SecurityToken sec, string columnName)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");
            
            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            string name = string.Empty;
            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select field_label from tbl_web_cohort_fields where data_field like '%{0}'", srvhelp.SterilizeDBText(columnName)), myConnection);
                myCommand.CommandType = CommandType.Text;

                object rawResult = myCommand.ExecuteScalar();
                if (rawResult != null)
                    name = rawResult.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return name;
        }

        [JsonRpcMethod("getCohortWebFieldByColumnName", Idempotent = true)]
        public DataTable GetCohortWebFieldByColumnName(SecurityToken sec, string columnName)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);
            // create data set
            DataTable dt = new DataTable("tbl_web_cohort_fields");

            try
            {
                // support for virtual fields
                if (columnName.Contains("v.."))
                    columnName = String.Format("v..{0}", columnName.Substring(columnName.LastIndexOf(".") + 1, (columnName.Length - (columnName.LastIndexOf(".") + 1))));

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_web_cohort_fields where data_field='{0}'", columnName), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return dt;
        }

        [JsonRpcMethod("getCohortWebFieldsForSummaryGrid", Idempotent = true)]
        public DataSet GetCohortWebFieldsForSummaryGrid(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                    // call stored proc to get user account summary
                    SqlCommand myCommand = new SqlCommand("select * from tbl_web_cohort_fields where summary_display=1 order by summary_position", myConnection);
                    myCommand.CommandType = CommandType.Text;
                    // use SqlDataAdapter to pass dataset to client			
                    SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                    // insert into table
                    myDataAdapter.Fill(ds, "tbl_web_cohort_fields");

                    // use command object to retrieve result code
                    SqlCommand myResultCommand = new SqlCommand();
                    myResultCommand.CommandType = CommandType.StoredProcedure;
                    myResultCommand.CommandText = "fsp_get_result_code";
                    myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = 100; // Success!
                    myResultCommand.Connection = myConnection;
                    // call SqlDataAdapter
                    SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                    // add the result codes table to dataaset
                    myResultAdapter.Fill(ds, "tbl_result_codes");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        [JsonRpcMethod("getCohortWebFieldsForDetailGrid", Idempotent = true)]
        public DataSet GetCohortWebFieldsForDetailGrid(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                    // call stored proc to get user account summary
                    SqlCommand myCommand = new SqlCommand("select * from tbl_web_cohort_fields where detail_display=1 order by detail_position", myConnection);
                    myCommand.CommandType = CommandType.Text;
                    // use SqlDataAdapter to pass dataset to client			
                    SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                    // insert into table
                    myDataAdapter.Fill(ds, "tbl_web_cohort_fields");

                    // use command object to retrieve result code
                    SqlCommand myResultCommand = new SqlCommand();
                    myResultCommand.CommandType = CommandType.StoredProcedure;
                    myResultCommand.CommandText = "fsp_get_result_code";
                    myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = 100; // Success!
                    myResultCommand.Connection = myConnection;
                    // call SqlDataAdapter
                    SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                    // add the result codes table to dataaset
                    myResultAdapter.Fill(ds, "tbl_result_codes");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        [JsonRpcMethod("getCohortWebFieldsForCompareGrid", Idempotent = true)]
        public DataSet GetCohortWebFieldsForCompareGrid(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand("select * from tbl_web_cohort_fields where compare_display=1 order by compare_position", myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_cohort_fields");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        public DataSet GetCohortWebFieldsForCompareGrid(SecurityToken sec, int tabId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_web_cohort_fields where compare_tab={0} and compare_display=1 order by compare_position", tabId), myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_cohort_fields");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        [JsonRpcMethod("getCohortWebFieldsForCompareGridByParentId", Idempotent = true)]
        public DataSet GetCohortWebFieldsForCompareGridByParentId(SecurityToken sec, int parentFieldId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_web_cohort_fields where compare_parent_id={0} and compare_display=1 order by compare_position", parentFieldId), myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_cohort_fields");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        [JsonRpcMethod("getCohortWebFieldsForEnrollmentGrid", Idempotent = true)]
        public DataSet GetCohortWebFieldsForEnrollmentGrid(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand("select * from tbl_web_cohort_fields where demographics_display=1 order by demographics_position", myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_cohort_fields");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        [JsonRpcMethod("getCohortWebFieldsForEnrollmentGridByGender", Idempotent = true)]
        public DataSet GetCohortWebFieldsForEnrollmentGrid(SecurityToken sec, string gender)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_web_cohort_fields where demographics_display=1 and data_field like '%-_{0}' escape '-' order by demographics_position",
                                                        gender.ToLower()), myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_cohort_fields");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        [JsonRpcMethod("getCohortWebFieldsForEnrollmentGridByCriteria", Idempotent = true)]
        public DataSet GetCohortWebFieldsForEnrollmentGrid(SecurityToken sec, string gender, string[] ethnicity, string[] race)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                foreach(string e in ethnicity)
                {

                    foreach (string r in race)
                    {
                        // call stored proc to get user account summary
                        SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_web_cohort_fields where demographics_display=1 and data_field like '%-{0}_{1}_{2}' escape '-' order by demographics_position",
                                                                r.ToLower(), e.ToLower(), gender.ToLower()), myConnection);
                        myCommand.CommandType = CommandType.Text;
                        // use SqlDataAdapter to pass dataset to client			
                        SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                        // insert into table
                        myDataAdapter.Fill(ds, "tbl_web_cohort_fields");
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        [JsonRpcMethod("getCohortWebFieldsForCancerGrid", Idempotent = true)]
        public DataSet GetCohortWebFieldsForCancerGrid(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand("select * from tbl_web_cohort_fields where cancer_display=1 order by cancer_position", myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_cohort_fields");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        [JsonRpcMethod("getCohortWebFieldsForBiospecimenGrid", Idempotent = true)]
        public DataSet GetCohortWebFieldsForBiospecimenGrid(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session not valid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand("select * from tbl_web_cohort_fields where biospecimen_display=1", myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_cohort_fields");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// get all defined web filters sorted for position
        /// </summary>
        [JsonRpcMethod("getAllWebFilterFields", Idempotent = true)]
        [JsonRpcHelp("get all defined web filters sorted for position")]
        public DataSet GetAllWebFilterFields(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand("select * from vWebFilterFields order by position", myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_filter_fields");

                // use command object to retrieve result code
                SqlCommand myResultCommand = new SqlCommand();
                myResultCommand.CommandType = CommandType.StoredProcedure;
                myResultCommand.CommandText = "fsp_get_result_code";
                myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = 100; // Success!
                myResultCommand.Connection = myConnection;
                // call SqlDataAdapter
                SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                // add the result codes table to dataaset
                myResultAdapter.Fill(ds, "tbl_result_codes");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// get defined web filters sorted for position for a given parent_category_id
        /// </summary>
        [JsonRpcMethod("getWebFilterFields", Idempotent = true)]
        [JsonRpcHelp("get defined web filters sorted for position for a given parent_category_id")]
        public DataSet GetWebFilterFields(SecurityToken sec, int parentCategoryID)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from vWebFilterFields where parent_category_id={0} order by position", parentCategoryID), myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_web_filter_fields");

                // use command object to retrieve result code
                SqlCommand myResultCommand = new SqlCommand();
                myResultCommand.CommandType = CommandType.StoredProcedure;
                myResultCommand.CommandText = "fsp_get_result_code";
                myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = 100; // Success!
                myResultCommand.Connection = myConnection;
                // call SqlDataAdapter
                SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                // add the result codes table to dataaset
                myResultAdapter.Fill(ds, "tbl_result_codes");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// get filter criteria by filter id
        /// </summary>
        [JsonRpcMethod("getWebFilterCriteriaByFilterId", Idempotent = true)]
        [JsonRpcHelp("get filter criteria by filter id")]
        public DataTable GetWebFilterByFilterId(SecurityToken sec, int filterId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("invalid session id");

            // create data set
            DataTable dt = new DataTable();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlStr = String.Format("select * from vWebFilterFields where id={0}", filterId);

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(sqlStr, myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataAdapter adap = new SqlDataAdapter(myCommand);
                adap.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }
        }

        /// <summary>
        /// get the cancer sites
        /// </summary>
        [JsonRpcMethod("getCancerSites", Idempotent = true)]
        [JsonRpcHelp("get the cancer sites")]
        public DataTable GetCancerSites(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("invalid session id");

            // create data set
            DataTable dt = new DataTable();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlStr = "select * from tbl_web_cancer_types order by cancer_type";

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(sqlStr, myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataAdapter adap = new SqlDataAdapter(myCommand);
                adap.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }
        }

        /// <summary>
        /// get the tab table for compare displays
        /// </summary>
        [JsonRpcMethod("getTabs", Idempotent = true)]
        [JsonRpcHelp("get the tab table supporting the compare displays")]
        public DataTable GetTabs(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("invalid session id");

            // create data set
            DataTable dt = new DataTable();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlStr = "select * from tbl_web_tabs";

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(sqlStr, myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataAdapter adap = new SqlDataAdapter(myCommand);
                adap.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }
        }

        /// <summary>
        /// get boolean value if column name is valid in the tbl_web_cohort_fields
        /// </summary>
        [JsonRpcMethod("isValidColumnName", Idempotent = true)]
        [JsonRpcHelp("get boolean value if column name is valid in the tbl_web_cohort_fields")]
        public bool IsValidColumnName(SecurityToken sec, string columnName)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("invalid session id");

            bool isValid = false;

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlStr = String.Format("select data_field from tbl_web_cohort_fields where data_field='{0}'", columnName);
                System.Data.SqlClient.SqlCommand sql = new SqlCommand(sqlStr, myConnection);
                object dbObj = sql.ExecuteScalar();
                if (dbObj != DBNull.Value)
                    isValid = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return isValid;
        }
        #endregion

        [JsonRpcMethod("getCancerTypes", Idempotent = true)]
        public DataTable GetCancerTypes(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // create dataset
            DataSet ds = new DataSet();

            // set connection
            SqlConnection mySqlConnection = new SqlConnection(this.database_connection_string);

            // open the connection
            mySqlConnection.Open();

            // call stored proc to retrieve record
            SqlCommand mySelectCommand = new SqlCommand();
            mySelectCommand.CommandType = CommandType.Text;
            mySelectCommand.CommandText = "select * from tbl_web_cancer_types";
            mySelectCommand.Connection = mySqlConnection;
            // call SqlDataAdapter
            SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
            // file the dataset
            myAdapter.Fill(ds, "tbl_cancers");

            // close connection
            mySqlConnection.Close();

            // return the data set
            return ds.Tables["tbl_cancers"];
        }

        [JsonRpcMethod("getBiospecimenTypes", Idempotent = true)]
        public DataTable GetBiospecimenTypes(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // create dataset
            DataSet ds = new DataSet();

            // set connection
            SqlConnection mySqlConnection = new SqlConnection(this.database_connection_string);

            // open the connection
            mySqlConnection.Open();

            // call stored proc to retrieve record
            SqlCommand mySelectCommand = new SqlCommand();
            mySelectCommand.CommandType = CommandType.Text;
            mySelectCommand.CommandText = "select * from tbl_web_biospecimen_types";
            mySelectCommand.Connection = mySqlConnection;
            // call SqlDataAdapter
            SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
            // file the dataset
            myAdapter.Fill(ds, "tbl_biospecimen");

            // close connection
            mySqlConnection.Close();

            // return the data set
            return ds.Tables["tbl_biospecimen"];
        }

        /// <summary>
        /// Returns a dataset of all cohort records
        /// </summary>
        [JsonRpcMethod("getAllCohortRecords")]
        [JsonRpcHelp("Returns a dataset of all cohort records")]
        public DataSet GetAllCohortRecords(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            // open connection
            myConnection.Open();

            // call stored proc to get user account summary
            SqlCommand myCommand = new SqlCommand();
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.CommandText = "fsp_get_all_cohort_records";
            myCommand.Connection = myConnection;
            // use SqlDataAdapter to pass dataset to client			
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
            // insert into table
            myDataAdapter.Fill(ds, "tbl_cohorts");

            // always close connection
            myConnection.Close();

            // return the data set
            return ds;
        }

        /// <summary>
        /// Retrieves a cohort record from the database.
        /// </summary>
        /// <returns>cohort record, response code</returns>
        //[JsonRpcMethod("zzzzzz", Idempotent = true)]
        //[JsonRpcHelp("Retrieves a cohort record from the database")]
        public DataSet GetCohortRecordById(SecurityToken sec, int intId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // create dataset
            DataSet ds = new DataSet();

            // set connection
            SqlConnection mySqlConnection = new SqlConnection(this.database_connection_string);

            // open the connection
            mySqlConnection.Open();

            // call stored proc to retrieve record
            SqlCommand mySelectCommand = new SqlCommand();
            mySelectCommand.CommandType = CommandType.StoredProcedure;
            mySelectCommand.CommandText = "fsp_get_cohort_record_by_id";
            mySelectCommand.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intId;
            mySelectCommand.Connection = mySqlConnection;
            // call SqlDataAdapter
            SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
            // file the dataset
            myAdapter.Fill(ds, "tbl_cohorts");

            // close connection
            mySqlConnection.Close();

            // return the data set
            return ds;
        }

        /// <summary>
        /// Returns a dataset of filtered cohort records 
        /// </summary>
        [JsonRpcMethod("getFilteredCohortRecords", Idempotent = true)]
        [JsonRpcHelp("Returns a dataset of filtered cohort records")]
        public DataSet GetFilteredCohortRecords(SecurityToken sec, string strSelectColumns, string strWhereClause)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            // open connection
            myConnection.Open();

            if (!strSelectColumns.Contains("cohort_acronym") && strSelectColumns != "*")
                strSelectColumns += (!String.IsNullOrWhiteSpace(strSelectColumns) ? ", cohort_acronym" : "cohort_acronym");

            //TODO: review 'A' alias in the table
            // build final SQL query
            string strQuerySql = String.Format("SELECT DISTINCT {0} FROM tbl_web_cohorts_v4_0 WHERE (published=1 AND [status]='published') ", strSelectColumns);

            if (!String.IsNullOrEmpty(strWhereClause))
                strQuerySql += String.Format(" AND ({0})", strWhereClause);

            strQuerySql += " ORDER BY cohort_acronym";
            
            // call stored proc to get user account summary
            SqlCommand myCommand = new SqlCommand();
            myCommand.CommandType = CommandType.Text;
            myCommand.CommandText = strQuerySql;
            myCommand.Connection = myConnection;
            // use SqlDataAdapter to pass dataset to client			
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
            // insert into table
            myDataAdapter.Fill(ds, "tbl_web_cohorts_v4_0");


            // always close connection
            myConnection.Close();

            // return the data set
            return ds;
        }

        /// <summary>
        /// generate the dataset for summary grid
        /// </summary>
        [JsonRpcMethod("getCohortForSummaryGrid", Idempotent = true)]
        public DataSet GetCohortForSummaryGrid(SecurityToken sec, string strFilter)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                System.Data.DataSet sgdt = GetCohortWebFieldsForSummaryGrid(sec);
                if (int.Parse(sgdt.Tables["tbl_result_codes"].Rows[0]["result_code"].ToString()) != 100)
                    throw new Exception("unable to retrieve web fields from the server");

                string selectColumns = " cohort_id,";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (!selectColumns.Contains(dr["data_field"].ToString()))
                        selectColumns += String.Format("{0},", dr["data_field"]);
                }
                selectColumns = selectColumns.Trim(',');

                ds = GetFilteredCohortRecords(sec, selectColumns, strFilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// generate the dataset for compare grid
        /// </summary>
        [JsonRpcMethod("getCohortForCompareGrid", Idempotent = true)]
        public DataSet GetCohortForCompareGrid(SecurityToken sec, string[] cohortIDs)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                System.Data.DataSet sgdt = GetCohortWebFieldsForCompareGrid(sec);

                string selectColumns = "";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (selectColumns.Contains(dr["data_field"].ToString()))
                        continue;

                    if ((dr["sql_case_active"] != DBNull.Value) && (bool)dr["sql_case_active"] && !String.IsNullOrWhiteSpace(dr["sql_case"].ToString()))
                        selectColumns += String.Format("{0} as [{1}],", dr["sql_case"], dr["data_field"]);
                    else
                        selectColumns += String.Format("{0},", dr["data_field"]);
                }
                selectColumns = selectColumns.Trim(',');

                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                ds = GetFilteredCohortRecords(sec, selectColumns, cohortFilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return ds;
        }

        /// <summary>
        /// generate the dataset for compare grid
        /// </summary>
        [JsonRpcMethod("getCohortForCompareGridByTabId", Idempotent = true)]
        public DataSet GetCohortForCompareGridByTabId(SecurityToken sec, string[] cohortIDs, int tabId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                System.Data.DataSet sgdt = GetCohortWebFieldsForCompareGrid(sec, tabId);

                string selectColumns = "";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (selectColumns.Contains(dr["data_field"].ToString()))
                        continue;

                    if ((dr["sql_case_active"] != DBNull.Value) && (bool)dr["sql_case_active"] && !String.IsNullOrWhiteSpace(dr["sql_case"].ToString()))
                        selectColumns += String.Format("{0} as [{1}],", dr["sql_case"], dr["data_field"]);
                    else
                        selectColumns += String.Format("{0},", dr["data_field"]);
                }
                selectColumns = selectColumns.Trim(',');

                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                ds = GetFilteredCohortRecords(sec, selectColumns, cohortFilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return ds;
        }

        /// <summary>
        /// generate the dataset for compare grid
        /// </summary>
        [JsonRpcMethod("getCohortForCompareGridByParentId", Idempotent = true)]
        public DataSet GetCohortForCompareGridByParentID(SecurityToken sec, string[] cohortIDs, int parentFieldId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                System.Data.DataSet sgdt = GetCohortWebFieldsForCompareGridByParentId(sec, parentFieldId);

                string selectColumns = "";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (selectColumns.Contains(dr["data_field"].ToString()))
                        continue;

                    if ((dr["sql_case_active"] != DBNull.Value) && (bool)dr["sql_case_active"] && !String.IsNullOrWhiteSpace(dr["sql_case"].ToString()))
                        selectColumns += String.Format("{0} as [{1}],", dr["sql_case"], dr["data_field"]);
                    else
                        selectColumns += String.Format("{0},", dr["data_field"]);
                }
                selectColumns = selectColumns.TrimEnd().Trim(',');

                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                ds = GetFilteredCohortRecords(sec, selectColumns, cohortFilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return ds;
        }

        /// <summary>
        /// generate the dataset for enrollment grid, acceptable gender: males, females, unknown
        /// </summary>
        [JsonRpcMethod("getCohortForEnrollmentGrid", Idempotent = true)]
        public DataSet GetCohortForEnrollmentGrid(SecurityToken sec, string[] cohortIDs, string gender, string[] race, string[] ethnicity)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                System.Data.DataSet sgdt = GetCohortWebFieldsForEnrollmentGrid(sec, gender, ethnicity, race);

                string selectColumns = " cohort_id,cohort_acronym,";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (dr["data_field"] != DBNull.Value && !selectColumns.Contains(dr["data_field"].ToString()))
                        selectColumns += String.Format("{0},", dr["data_field"]);
                }
                selectColumns = selectColumns.Trim(',');

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts_v4_0 WHERE (published=1 AND [status]='published') ", selectColumns);
                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += String.Format(" AND ({0})", cohortFilter);

                strQuerySql += " ORDER BY cohort_acronym";

                // open connection
                myConnection.Open();
                
                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_enrollment");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// generate the dataset for enrollment grid summary table
        /// </summary>
        public DataSet DEPRICATE_GetCohortForEnrollmentGrid_SummaryTable(SecurityToken sec, string[] cohortIDs)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                ///----------------------------------------------------------------
                /// hard coding the necessary columns for summary table display
                ///  for sake of time.
                //System.Data.DataSet sgdt = GetCohortWebFieldsForEnrollmentGrid(sec);

                string selectColumns = "cohort_id, cohort_acronym, date_form_completed, total_subjects_enrolled, gender_total_males_enrolled, gender_total_females_enrolled ";
                ///----------------------------------------------------------------
                /// hard coding the necessary columns for summary table display
                ///  for sake of time.
                //foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                //{
                //    if (dr["data_field"] != DBNull.Value && !selectColumns.Contains(dr["data_field"].ToString()))
                //        selectColumns += String.Format("{0},", dr["data_field"]);
                //}
                //selectColumns = selectColumns.Trim(',');

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts_v4_0 ", selectColumns);
                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += " WHERE " + cohortFilter;

                strQuerySql += " ORDER BY cohort_acronym";

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_enrollment");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        #region Depricated Routine for Populating cancer grid
        /// <summary>
        /// generate the dataset for cancer grid
        /// </summary>
        /*[JsonRpcMethod("getCohortForCancerGrid", Idempotent = true)]
        public DataSet GetCohortForCancerGrid(SecurityToken sec, string cancerTable, int cancerTypeID, string[] cohortIDs)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " a.cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                System.Data.DataSet sgdt;
                sgdt = GetCohortWebFieldsForCancerGrid(sec);

                string selectColumns = "a.cohort_id, a.cohort_acronym, bc.cancer_type_id,";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (!selectColumns.Contains(dr["data_field"].ToString()))
                        selectColumns += String.Format("{0},", dr["data_field"]).Trim();
                }
                selectColumns = selectColumns.Trim(',');

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts a left join tbl_web_biospecimen_counts bc on bc.cohort_id=a.cohort_id", selectColumns);
                if (cancerTable.ToLower() == "prevalent")
                    strQuerySql += " and bc.indicator='P'";
                else
                    strQuerySql += " and bc.indicator='I'";

                if (cancerTypeID != -1)
                    strQuerySql += String.Format(" and bc.cancer_type_id={0} ", cancerTypeID);

                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += " where " + cohortFilter;

                strQuerySql += " ORDER BY a.cohort_acronym";

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_cancer");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }
         */
        #endregion

        /// <summary>
        /// generate the dataset for cancer grid; DEPRICATED
        /// </summary>
        //[JsonRpcMethod("getCohortForCancerGridByCancerIds", Idempotent = true)]
        public DataSet DEPRICATE_GetCohortForCancerGrid(SecurityToken sec, string cancerTable, int[] cancerTypeIDs, string[] cohortIDs)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                System.Data.DataSet sgdt;
                sgdt = GetCohortWebFieldsForCancerGrid(sec);

                string selectColumns = "cohort_id, cohort_acronym, bc.cancer_type_id, bc.males, bc.females,";
                //foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                //{
                //    if (!selectColumns.Contains(dr["data_field"].ToString()))
                //        selectColumns += String.Format("{0},", dr["data_field"]).Trim();
                //}
                selectColumns = selectColumns.Trim(',');

                string indicator = "I";
                if (cancerTable.ToLower() == "prevalent")
                    indicator = "P";

                string theIn = string.Empty;
                foreach (int c in cancerTypeIDs)
                    theIn += String.Format("{0},", c);
                theIn = theIn.TrimEnd(',');

                if (theIn != string.Empty)
                    theIn = String.Format(" and c.cancer_type_id in ({0})", theIn);

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts_v4_0 a left join (select c.*, ct.cancer_type from tbl_web_biospecimen_counts c join tbl_web_cancer_types ct on ct.cancer_type_id=c.cancer_type_id and c.indicator='{1}' {2}) bc on bc.cohort_id=a.cohort_id ", 
                    selectColumns, indicator, theIn);

                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += " where " + cohortFilter;

                strQuerySql += " order by  bc.cancer_type, cohort_acronym";

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_cancer");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// generate the dataset for cancer grid
        /// </summary>
        [JsonRpcMethod("getCohortForCancerGridByCancers", Idempotent = true)]
        public DataSet GetCohortForCancerGrid(SecurityToken sec, string[] cancers, string[] genders, string[] cohortIDs)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                System.Data.DataSet sgdt;
                sgdt = GetCohortWebFieldsForCancerGrid(sec);

                string selectColumns = "cohort_id, cohort_acronym,";
                //foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                //{
                //    if (!selectColumns.Contains(dr["data_field"].ToString()))
                //        selectColumns += String.Format("{0},", dr["data_field"]).Trim();
                //}
                foreach (string c in cancers)
                {
                    foreach (string g in genders)
                        selectColumns += String.Format("ci_{0}_{1},", c, g);
                }
                selectColumns = selectColumns.Trim(',');

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts_v4_0  WHERE (published=1 AND [status]='published') ",
                    selectColumns);

                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += String.Format(" AND ({0})", cohortFilter);

                strQuerySql += " order by cohort_acronym";

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_cancer");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// generate the dataset for cancer grid; DEPRICATED
        /// </summary>
        //[JsonRpcMethod("getCohortForCancerGridByCancerIds", Idempotent = true)]
        public DataSet DEPRICATE_GetCohortForCancerGrid(SecurityToken sec, string cancerTable, int[] cancerTypeIDs, string[] cohortIDs, string[] genders)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " a.cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                System.Data.DataSet sgdt;
                sgdt = GetCohortWebFieldsForCancerGrid(sec);

                string selectColumns = "a.cohort_id, a.cohort_acronym, bc.cancer_type_id,";
                foreach (string g in genders)
                    selectColumns += String.Format("{0},", g);
                selectColumns = selectColumns.Trim(',');

                string indicator = "I";
                if (cancerTable.ToLower() == "prevalent")
                    indicator = "P";

                string theIn = string.Empty;
                foreach (int c in cancerTypeIDs)
                    theIn += String.Format("{0},", c);
                theIn = theIn.TrimEnd(',');

                if (theIn != string.Empty)
                    theIn = String.Format(" and c.cancer_type_id in ({0})", theIn);

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts_v4_0 a left join (select c.*, ct.cancer_type from tbl_web_biospecimen_counts c join tbl_web_cancer_types ct on ct.cancer_type_id=c.cancer_type_id and c.indicator='{1}' {2}) bc on bc.cohort_id=a.cohort_id ",
                    selectColumns, indicator, theIn);

                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += " where " + cohortFilter;

                strQuerySql += " order by  bc.cancer_type, a.cohort_acronym";

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_cancer");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// check if cohort has incident or prevalent cancers
        /// </summary>
        [JsonRpcMethod("hasCancerIndicatorByCohortID", Idempotent = true)]
        public bool CohortHasCancerIndicator(SecurityToken sec, string cancerIndicator, int cohortID)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            if (cohortID <= 0)
                throw new ArgumentNullException("cohortID", "cohortID cannot be unassigned");

            // return value
            bool has_type = false;

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string indicator = "I";
                if (cancerIndicator.ToLower() == "prevalent")
                    indicator = "P";

                string strQuerySql = String.Format("select top 1 c.females from tbl_web_cohorts_v4_0 a left join tbl_web_biospecimen_counts c on a.cohort_id=c.cohort_id where a.cohort_id={0} and c.indicator='{1}'", cohortID, indicator);

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;

                object dbrslt = myCommand.ExecuteScalar();
                if (dbrslt != null && dbrslt != DBNull.Value)
                    has_type = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return has_type;
        }

        /// <summary>
        /// check if cohort has incident or prevalent cancers
        /// </summary>
        [JsonRpcMethod("hasCancerIndicatorByCohort", Idempotent = true)]
        public bool CohortHasCancerIndicator(SecurityToken sec, string cancerIndicator, string cohort)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            if (String.IsNullOrWhiteSpace(cohort))
                throw new ArgumentNullException("cohort", "cohort cannot be unassigned");

            // return value
            bool has_type = false;

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string indicator = "I";
                if (cancerIndicator.ToLower() == "prevalent")
                    indicator = "P";

                string strQuerySql = String.Format("select top 1 c.tumor_tissue_1 from tbl_web_cohorts_v4_0 a left join tbl_web_biospecimen_counts c on a.cohort_id=c.cohort_id where a.cohort_acronym='{0}' and c.indicator='{1}'", cohort, indicator);

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;

                object dbrslt = myCommand.ExecuteScalar();
                if ((dbrslt != null && dbrslt != DBNull.Value) && (int)dbrslt != -1)
                    has_type = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return has_type;
        }

        /// <summary>
        /// generate the dataset for biospecimen counts grid
        /// </summary>
       // [JsonRpcMethod("getCohortForBiospecimenGrid", Idempotent = true)]
        public DataSet GetCohortForBiospecimenGrid(SecurityToken sec, int cancerTypeID, string[] cohortIDs)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                System.Data.DataSet sgdt = GetCohortWebFieldsForBiospecimenGrid(sec);

                string selectColumns = "cohort_id, cohort_acronym,";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (!selectColumns.Contains(dr["data_field"].ToString()))
                        selectColumns += String.Format("{0},", dr["data_field"]);
                }
                selectColumns = selectColumns.Trim(',');

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts_v4_0 a left join tbl_web_biospecimen_counts bc on bc.cohort_id=a.cohort_id ", selectColumns);

                if (cancerTypeID == -1)
                    strQuerySql += " and bc.cancer_type_id not in (0) ";
                else
                    strQuerySql += String.Format(" and bc.cancer_type_id={0} ", cancerTypeID);

                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += " where " + cohortFilter;

                strQuerySql += " ORDER BY a.cohort_acronym";

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_biospecimen");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// generate the dataset for biospecimen counts grid
        /// </summary>
        [JsonRpcMethod("getCohortForBiospecimenGrid", Idempotent = true)]
        public DataSet GetCohortForBiospecimenGrid(SecurityToken sec, string[] cancerTypes, string[] cohortIDs, string[] specimenTypes)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = " cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }

                string selectColumns = "cohort_id, cohort_acronym,";
                foreach (string s in specimenTypes)
                {
                    foreach (string c in cancerTypes)
                    {
                        selectColumns += String.Format("bio_{0}_{1},", c, s);
                    }
                }
                selectColumns = selectColumns.Trim(',');

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts_v4_0 WHERE (published=1 AND [status]='published') ", selectColumns);

                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += String.Format(" AND ({0})", cohortFilter);

                strQuerySql += " ORDER BY cohort_acronym";

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_biospecimen");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }


        /// <summary>
        /// generate the dataset for biospecimen counts grid
        /// </summary>
        //[JsonRpcMethod("getCohortForBiospecimenGrid", Idempotent = true)]
        public DataSet GetCohortForBiospecimenGrid(SecurityToken sec, int[] cancerTypeIDs, string[] cohortIDs, string[] specimenTypes)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string cohortFilter = string.Empty;
                if (cohortIDs != null && cohortIDs.Length > 0)
                {
                    cohortFilter = "bc.indicator='I' AND a.cohort_id in (";
                    foreach (string id in cohortIDs)
                        cohortFilter += String.Format("'{0}',", id);
                    cohortFilter = cohortFilter.TrimEnd(',') + ")";
                }
                
                string selectColumns = "a.cohort_id, a.cohort_acronym, ct.cancer_type,";
                foreach(string s in specimenTypes)
                    selectColumns += String.Format("{0},", s);
                selectColumns = selectColumns.Trim(',');

                string strQuerySql = String.Format("select {0} from tbl_web_cohorts_v4_0 a left join tbl_web_biospecimen_counts bc on bc.cohort_id=a.cohort_id join tbl_web_cancer_types ct on bc.cancer_type_id=ct.cancer_type_id ", selectColumns);

                string cancerFilters = string.Empty;
                foreach (int i in cancerTypeIDs)
                    cancerFilters += String.Format("{0},", i);
                cancerFilters = String.Format(" bc.cancer_type_id in ({0})", cancerFilters.TrimEnd(','));

                if (!String.IsNullOrEmpty(cohortFilter))
                    strQuerySql += String.Format(" where {0} and {1}", cohortFilter, cancerFilters);

                strQuerySql += " ORDER BY a.cohort_acronym";

                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(strQuerySql, myConnection);
                myCommand.CommandType = CommandType.Text;
                // use SqlDataAdapter to pass dataset to client			
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
                // insert into table
                myDataAdapter.Fill(ds, "tbl_biospecimen");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// generate the dataset for cohort details
        /// </summary>
        [JsonRpcMethod("getCohortDetailsByID", Idempotent = true)]
        public DataSet GetCohortDetails(SecurityToken sec, int cohortId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                System.Data.DataSet sgdt = GetCohortWebFieldsForDetailGrid(sec);
                if ((int)sgdt.Tables["tbl_result_codes"].Rows[0][0] != 100)
                    throw new Exception("unable to retrieve web fields from the server");

                string selectColumns = "id, cohort_id, cohort_acronym, ";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (selectColumns.Contains(dr["data_field"].ToString()))
                        continue;

                    if ((dr["sql_case_active"] != DBNull.Value) && (bool)dr["sql_case_active"] && !String.IsNullOrWhiteSpace(dr["sql_case"].ToString()))
                        selectColumns += String.Format("{0} as [{1}],", dr["sql_case"], dr["data_field"]);
                    else
                        selectColumns += String.Format("{0},", dr["data_field"]);
                }
                selectColumns = selectColumns.Trim(',');

                ds = GetFilteredCohortRecords(sec, selectColumns, String.Format(" cohort_id={0} ", cohortId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// generate the dataset for cohort details
        /// </summary>
        [JsonRpcMethod("getCohortDetailsByAcronym", Idempotent = true)]
        public DataSet GetCohortDetails(SecurityToken sec, string cohortId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                System.Data.DataSet sgdt = GetCohortWebFieldsForDetailGrid(sec);
                if ((int)sgdt.Tables["tbl_result_codes"].Rows[0][0] != 100)
                    throw new Exception("unable to retrieve web fields from the server");

                string selectColumns = "cohort_id, cohort_acronym,";
                foreach (DataRow dr in sgdt.Tables["tbl_web_cohort_fields"].Rows)
                {
                    if (selectColumns.Contains(dr["data_field"].ToString()))
                        continue;

                    if ((dr["sql_case_active"] != DBNull.Value) && (bool)dr["sql_case_active"] && !String.IsNullOrWhiteSpace(dr["sql_case"].ToString()))
                        selectColumns += String.Format("{0} as [{1}],", dr["sql_case"], dr["data_field"]);
                    else
                        selectColumns += String.Format("{0},", dr["data_field"]);
                }
                selectColumns = selectColumns.Trim(',');

                ds = GetFilteredCohortRecords(sec, selectColumns, String.Format(" cohort_acronym='{0}'", cohortId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// get boolean value indicating if the cohort has included an attachment of all PIs
        /// involved in the study
        /// </summary>
        [JsonRpcMethod("cohortHasPIListAttachment", Idempotent= true)]
        public bool CohortHasPIListAttachment(SecurityToken sec, int cohortId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            bool hasList = false;

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlQ = String.Format("select ca.id from tbl_web_cohort_attachments ca join tbl_web_document_types dt on ca.document_type_id=dt.document_type_id AND dt.document_type_id=7 where cohort_id={0} order by ca.document_type_id", cohortId);

                System.Data.SqlClient.SqlCommand com =
                    new SqlCommand(sqlQ, myConnection);
                com.CommandType = CommandType.Text;

                object raw = com.ExecuteScalar();
                if ((raw != null) && raw != DBNull.Value)
                    hasList = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return hasList;
        }

        /// <summary>
        /// get the PI list attachment for a given cohort
        /// </summary>
        [JsonRpcMethod("getCohortPIListAttachment", Idempotent= true)]
        public DataSet GetCohortPIListAttachment(SecurityToken sec, int cohortId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlQ = String.Format("select ca.*, dt.document_type from tbl_web_cohort_attachments ca join tbl_web_document_types dt on ca.document_type_id=dt.document_type_id AND dt.document_type_id=7 where cohort_id={0} order by ca.document_type_id", cohortId);

                System.Data.SqlClient.SqlCommand com =
                    new SqlCommand(sqlQ, myConnection);
                com.CommandType = CommandType.Text;

                System.Data.SqlClient.SqlDataAdapter adap =
                    new SqlDataAdapter(com);

                DataTable dt =
                    new DataTable("tbl_web_cohort_attachments");
                adap.Fill(dt);

                ds.Tables.Add(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// get the list of attachments for a given cohort id
        /// </summary>
        [JsonRpcMethod("getCohortAttachments", Idempotent = true)]
        public DataSet GetCohortAttachmentList(SecurityToken sec, int web_cohort_id)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlQ = String.Format("select ca.*, dt.document_type from tbl_web_cohort_attachments ca join tbl_web_document_types dt on dt.document_type_id=ca.document_type_id where ca.web_cohort_id={0}", web_cohort_id);

                System.Data.SqlClient.SqlCommand com =
                    new SqlCommand(sqlQ, myConnection);
                com.CommandType = CommandType.Text;

                System.Data.SqlClient.SqlDataAdapter adap =
                    new SqlDataAdapter(com);

                DataTable dt =
                    new DataTable("tbl_web_cohort_attachments");
                adap.Fill(dt);

                ds.Tables.Add(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            // return the data set
            return ds;
        }

        /// <summary>
        /// get the attached document in binary form
        /// </summary>
        [JsonRpcMethod("getCohortDocument", Idempotent = true)]
        public string GetCohortDocument(SecurityToken sec, int documentId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            byte[] doc;
            string path = String.Format("{0}/{1}", HttpContext.Current.Server.MapPath("/user_files"), sec.userid),
                   webpath = String.Format("/user_files/{0}", sec.userid);

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // open connection
                myConnection.Open();

                string sqlQ = String.Format("select file_contents, file_name from tbl_web_cohort_attachments where id={0}", documentId);

                System.Data.SqlClient.SqlCommand com =
                    new SqlCommand(sqlQ, myConnection);
                com.CommandType = CommandType.Text;

                System.Data.SqlClient.SqlDataReader rdr =
                    com.ExecuteReader();
                rdr.Read();

                if (rdr["file_contents"] == DBNull.Value)
                    throw new Exception("unable to retrieve file contents, file contents null");

                path += String.Format("/{0}", rdr["file_name"]);
                webpath += String.Format("/{0}", rdr["file_name"]);

                doc = (byte[])rdr["file_contents"];

                rdr.Close();

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(doc, 0, doc.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("failed to retrieve and write the document", ex);
            }
            finally
            {
                myConnection.Close();
            }

            return webpath;
        }

        /// <summary>
        /// get boolean whether id belongs to a real cohort
        /// </summary>
        [JsonRpcMethod("isCohortByID", Idempotent=true)]
        public bool IsCohort(SecurityToken sec, int cohortId)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            bool acohort = false;

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                System.Data.DataTable dt = GetFilteredCohortRecords(sec, "cohort_id", String.Format(" cohort_id={0}", cohortId)).Tables[0];
                if (dt.Rows.Count > 0)
                    acohort = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return acohort;
        }

        /// <summary>
        /// get boolean whether acronym belongs to a real cohort
        /// </summary>
        [JsonRpcMethod("isCohortByAcronym", Idempotent = true)]
        public bool IsCohort(SecurityToken sec, string cohortAcronym)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            bool acohort = false;

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                System.Data.DataTable dt = GetFilteredCohortRecords(sec, "cohort_acronym", String.Format(" cohort_acronym='{0}'", srvhelp.HTMLDecode(cohortAcronym))).Tables[0];
                if (dt.Rows.Count > 0)
                    acohort = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }

            return acohort;
        }

        /// <summary>
        /// get cancer label for given cancer_type_id
        /// </summary>
        [JsonRpcMethod("getCancerLabel", Idempotent = true)]
        public string GetCancerLabel(SecurityToken sec, int cancerTypeID)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlQ = String.Format("select cancer_type from tbl_web_cancer_types where cancer_type_id={0}", cancerTypeID);

                System.Data.SqlClient.SqlCommand com =
                    new SqlCommand(sqlQ, myConnection);
                com.CommandType = CommandType.Text;

                object _raw = com.ExecuteScalar();
                if (_raw != null)
                    return _raw.ToString();
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }
        }

        /// <summary>
        /// get boolean value if field label is enabled for field descriptions
        /// </summary>
        [JsonRpcMethod("isFieldDescriptionEnabled", Idempotent = true)]
        public bool IsFieldDescriptionEnabled(SecurityToken sec, string dataField)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            bool isEnabled = false;

            try
            {
                // open connection
                myConnection.Open();

                string sqlQ = String.Format("select show_description, field_description from tbl_web_cohort_fields where data_field='{0}'", dataField);

                System.Data.SqlClient.SqlCommand com =
                    new SqlCommand(sqlQ, myConnection);
                com.CommandType = CommandType.Text;

                System.Data.SqlClient.SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["show_description"] != DBNull.Value && (bool)rdr["show_description"])
                        isEnabled = true;
                }
                rdr.Close();

                return isEnabled;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }
        }

        /// <summary>
        /// get field description for a given dataField
        /// </summary>
        [JsonRpcMethod("getDataFieldDescription", Idempotent = true)]
        public string GetDataFieldDescription(SecurityToken sec, string dataField)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // create data set
            DataSet ds = new DataSet();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);
                
            string desc = string.Empty;

            try
            {
                // open connection
                myConnection.Open();

                string sqlQ = String.Format("select field_description from tbl_web_cohort_fields where data_field='{0}'", dataField);

                System.Data.SqlClient.SqlCommand com =
                    new SqlCommand(sqlQ, myConnection);
                com.CommandType = CommandType.Text;

                System.Data.SqlClient.SqlDataReader rdr = com.ExecuteReader();
                rdr.Read();
                
                if (rdr["field_description"] != DBNull.Value)
                        desc = rdr["field_description"].ToString();
                rdr.Close();

                return desc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // always close connection
                myConnection.Close();
            }
        }
    }
}