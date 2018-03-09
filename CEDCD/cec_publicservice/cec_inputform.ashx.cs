using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Jayrock.Json;
using Jayrock.Json.Conversion;
using Jayrock.JsonRpc;
using Jayrock.JsonRpc.Web;

using NPOI;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;



namespace cec_publicservice
{
    internal class WebServiceHelper
    {
        protected static int EMAIL_MAX = 150;
        protected static int EMAIL_CNT = 0;

        #region Properties

        internal string HelpDeskEmail
        {
            get;
            set;
        }

        internal string EmailRecipient
        {
            get;
            set;
        }

        protected string SMTPRelay
        {
            get;
            set;
        }

        protected int SMTPPort
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        internal WebServiceHelper()
        {
            System.Configuration.Configuration configs =
                WebConfigurationManager.OpenWebConfiguration("/");

            if (configs.AppSettings != null && configs.AppSettings.Settings.Count > 0)
            {
                System.Configuration.KeyValueConfigurationElement smtp_relay =
                    configs.AppSettings.Settings["SMTP_Relay"];
                SMTPRelay = smtp_relay.Value;

                System.Configuration.KeyValueConfigurationElement smtp_port =
                    configs.AppSettings.Settings["SMTP_Port"];
                SMTPPort = int.Parse(smtp_port.Value);

                System.Configuration.KeyValueConfigurationElement helpdesk =
                    configs.AppSettings.Settings["HelpDeskEmail"];
                HelpDeskEmail = helpdesk.Value;

                System.Configuration.KeyValueConfigurationElement recemails =
                    configs.AppSettings.Settings["EmailRecipient"];
                EmailRecipient = recemails.Value;
            }
        }
        #endregion

        /// <summary>
        /// send an email
        /// </summary>
        internal bool SendEmail(MailMessage msg)
        {
            if (msg == null)
                throw new ArgumentNullException("MailMessage msg");

            EMAIL_CNT++;
            if (!(EMAIL_CNT < EMAIL_MAX))
                throw new Exception("maximum number of allowable messages sent, will not send this email");
            
            System.Net.Mail.SmtpClient email =
                new SmtpClient(SMTPRelay, SMTPPort);

            if (email.Port != 25)
                email.EnableSsl = true;
            else
                email.EnableSsl = false;
            email.DeliveryMethod = SmtpDeliveryMethod.Network;
            email.UseDefaultCredentials = false;
            email.Timeout = 6000;

            ServicePointManager.ServerCertificateValidationCallback =
            delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            bool success = true;
            try
            {
                email.Send(msg);
            }
            catch (SmtpException ex)
            {
                success = false;

                WriteToLog(ex.Message);
            }

            return success;
        }

        /// <summary>
        /// check if string is null, string.empty or whitespace (non-printing characters)
        /// </summary>
        internal static bool IsStringNullWhiteSpace(string str)
        {
            // trim spacing from beginning and end of string
            if (str != null)
                str = str.Trim();

            if (String.IsNullOrEmpty(str))
                return true;

            // carriage return, bell character, or new line
            if (Regex.IsMatch(str, @"^[\a\r\n]+"))
                return true;

            return false;
        }

        /// <summary>
        /// writes text to log
        /// </summary>
        /// <param name='text'>
        /// text to write out
        /// </param>
        internal void WriteToLog(string text)
        {
            try
            {
                if (text == string.Empty)
                    text = "writing nothing";

                string logName = String.Format("messages_{0}.log", DateTime.Today.ToString("dd-MM-yyyy"));
                string fullText = String.Format("{0}{1}", text, Environment.NewLine);

                using (System.IO.StreamWriter log = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath(String.Format("/tmp/{0}", logName)), true))
                {
                    log.WriteLine(text);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// strip html tags from string
        /// </summary>
        internal string StripHTML(string raw)
        {
            System.Text.RegularExpressions.Regex reg =
                new Regex(@"<.*?>");

            return reg.Replace(raw, string.Empty);
        }

        /// <summary>
        /// sterilize text for database use
        /// </summary>
        internal string SterilizeDBText(string raw)
        {
            string _medium = raw.Replace("'", "''");

            return _medium;
        }

        /// <summary>
        /// html encode string
        /// </summary>
        internal string HTMLEncode(string raw)
        {
            string _medium = raw.Replace("'", "&apos;");
            _medium = _medium.Replace("’", "&apos;");
            _medium = _medium.Replace("<", "&lt;");
            _medium = _medium.Replace(">", "&gt;");
            _medium = _medium.Replace(" & ", " &amp; ");
            _medium = _medium.Replace("\"", "&quot;");
            _medium = _medium.Replace("“", "&quot;");
            _medium = _medium.Replace("”", "&quot;");
            _medium = _medium.Replace("!", "&#33;");
            _medium = _medium.Replace("$", "&#38;");
            _medium = _medium.Replace("(", "&#40;");
            _medium = _medium.Replace(")", "&#41;");
            _medium = _medium.Replace("*", "&#42;");

            return _medium;
        }

        /// <summary>
        /// html decode string
        /// </summary>
        internal string HTMLDecode(string raw)
        {
            string _medium = raw.Replace("&lt;", "<");
            _medium = _medium.Replace("&gt;", ">");
            _medium = _medium.Replace("&amp;", "&");
            _medium = _medium.Replace("&apos;", "'");
            _medium = _medium.Replace("&#39;", "'");
            _medium = _medium.Replace("&quot;", "\"");
            _medium = _medium.Replace("&#33;", "!");
            _medium = _medium.Replace("&#38;", "$");
            _medium = _medium.Replace("&#40;", "(");
            _medium = _medium.Replace("&#41;", ")");
            _medium = _medium.Replace("&#42;", "*");

            return _medium;
        }
    }


    public class CECInputFormService : Jayrock.JsonRpc.Web.JsonRpcHandler
    {
        private string database_connection_string;

        private WebServiceHelper srvhelp;

        #region Constructor

        public CECInputFormService()
        {
            database_connection_string = WebConfigurationManager.ConnectionStrings["prdCEC"].ConnectionString;

            srvhelp = new WebServiceHelper();
        }
        #endregion

        #region Private Routines

        private bool IsValidSessionId(SecurityToken sec)
        {
            bool valid = false;

            if (sec != null && (sec.userid != 0 && sec.session != null) && sec.session != "NEW")
            {
                try
                {
                    GetSecurityToken(sec.userid, sec.session);
                    valid = true;
                }
                catch
                {
                    valid = false;
                }
            }
            else if (sec.session == "NEW")
                valid = true;

            return valid;
        }

        private DataTable GetDataTable(string sqlQuery)
        {
            DataTable dt = new DataTable();

            // set connection
            SqlConnection con = new SqlConnection(this.database_connection_string);

            try
            {
                // open the connection
                con.Open();
                SqlCommand com = new SqlCommand(sqlQuery, con);
                com.CommandType = CommandType.Text;

                using (SqlDataAdapter myAdapter = new SqlDataAdapter(com))
                {
                    myAdapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // close connection
                con.Close();
            }

            return dt;
        }

        private int GetCohortIdByWebId(int web_cohort_id)
        {
            int cohort_id = 0;
            try
            {
                string queryStr = string.Format("SELECT cohort_id FROM tbl_web_cohorts_v4_0 WHERE id={0}", web_cohort_id);

                DataTable dt = GetDataTable(queryStr);
                if (dt.Rows.Count > 0)
                    cohort_id = (int)dt.Rows[0]["cohort_id"];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cohort_id;
        }

        private int GetWebIdByCohortId(int cohort_id, string status)
        {
            int web_cohort_id = 0;
            try
            {
                string queryStr = string.Format("SELECT id FROM tbl_web_cohorts_v4_0 WHERE cohort_id={0} AND LOWER(status)=LOWER('{1}')", cohort_id, status);

                DataTable dt = GetDataTable(queryStr);
                if (dt.Rows.Count > 0)
                    web_cohort_id = (int)dt.Rows[0]["id"];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return web_cohort_id;
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

        private string ComputeSHA256(string rawStr)
        {
            byte[] braw = Encoding.ASCII.GetBytes(rawStr + GetHashSalt());
            byte[] boutput = (new SHA256Managed()).ComputeHash(braw);

            string hashed = string.Empty;
            foreach (byte b in boutput)
                hashed += b.ToString("x2");

            return hashed;
        }

        private void SendEmail(string template_name, NameValueCollection data)
        {
            if (String.IsNullOrWhiteSpace(template_name))
                throw new ArgumentException("template name cannot be blank", "template_name");

            if (data == null || data.Count == 0)
                throw new ArgumentException("datarow appears empty");
            //else if (data["to"] == null)
            //throw new Exception("data object must contain an element \"to\", indicating the recipient");

            DataTable dt_template = GetDataTable(String.Format("select * from tbl_email_templates where name='{0}'", template_name));
            if (dt_template.Rows.Count == 0)
                throw new ArgumentException(String.Format("template name '{0}' does not exist", template_name));

            string message = dt_template.Rows[0]["message_text"].ToString(),
                message_from = "cedcdhelpdesk@westat.com";
            try { message_from = srvhelp.HelpDeskEmail; }
            catch { }

            if (message.Contains("{{"))
            {
                int last_start_index = 0;
                string t_message = message;
                bool done = false;
                while (!done)
                {
                    if (message.IndexOf("}}", last_start_index) == -1)
                        throw new Exception("does not have a matching closing bracket");

                    string data_field = t_message;
                    int start = data_field.IndexOf("{{", last_start_index) + 2,
                        end = data_field.IndexOf("}}", start);
                    data_field = data_field.Substring(start, end - start);

                    if (data[data_field] == null)
                        throw new Exception(String.Format("unrecognized text substitution: {0}", data_field));

                    message = message.Replace("{{" + data_field + "}}", data[data_field]);

                    last_start_index = end;
                    if (message.IndexOf("{{", last_start_index) == -1)
                        done = true;
                }
            }

            if (data["to"] == null)
                data["to"] = message_from;

            System.Net.Mail.MailMessage emsg =
                new MailMessage(message_from, data["to"]);
            emsg.CC.Add(message_from);
            emsg.Subject = dt_template.Rows[0]["subject_text"].ToString();
            emsg.Body = message;
            emsg.IsBodyHtml = (bool)dt_template.Rows[0]["is_html"];

            srvhelp.SendEmail(emsg);
        }

        /*************************************************************************************
         * Function:		SQL Data Export -- PROD
         * Purpose:		    Exports a SQL query to an Excel file
         * Note:            SQL Query needs to return at least one row
         *
         */
        private string SqlDataExport(string sqlQuery, string headers, string outputFilePath)
        {
            // set connection
            SqlConnection myConnection = new SqlConnection(database_connection_string);
            XSSFWorkbook wkbk = new XSSFWorkbook();
            FileStream fs = null;

            // this has not been implemented!
            string[] do_not_export = new string[] { "id", "published", "status", "status_timestamp" };
            try
            {

                // open the deve connection
                myConnection.Open();

                wkbk.CreateSheet();
                wkbk.SetActiveSheet(0);

                // use command object to retrieve result code
                SqlCommand mySelectCommand = new SqlCommand(sqlQuery, myConnection);
                mySelectCommand.CommandType = CommandType.Text;

                // call SqlDataAdapter
                SqlDataAdapter myDataAdapter = new SqlDataAdapter(mySelectCommand);

                DataSet dataset = new DataSet();
                // add the result codes table to dataaset
                myDataAdapter.Fill(dataset);
                // get refernce to DataTable
                DataTable dt = dataset.Tables[0];

                // define row counter
                int rowIndex = 0;

                // if headers was specified, otherwise
                // loop through the DataTable for the headers
                object[] h;
                if (headers != string.Empty)
                    h = (string[])headers.Split('|');
                else
                {

                    h = new object[dt.Columns.Count];
                    dt.Columns.CopyTo(h, 0);
                }

                // populate header row names
                for (int i = 0; i < h.Length; i++)
                    TextToWorkSheetCell(h[i].ToString(), rowIndex, i, ref wkbk);

                // increment rowindex
                rowIndex++;

                foreach (DataRow dr in dt.Rows)
                {
                    int cellIndex = 0;
                    foreach (object dc in dr.ItemArray)
                        TextToWorkSheetCell(dc.ToString(), rowIndex, cellIndex++, ref wkbk);

                    rowIndex++;
                }

                // create the output file
                fs = new FileStream(outputFilePath, FileMode.Create);

                // write the workshett to the file
                wkbk.Write(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();

                myConnection.Close();
            }

            return outputFilePath;
        }

        /*************************************************************************************
        * Function:     Text To Worksheet Cell
        * Purpose:      Insert the text in the desired row and cell; sets word wrap and veritcal alignment
        */
        private void TextToWorkSheetCell(string text, int rowIndex, int cellIndex, ref XSSFWorkbook wkbk)
        {
            if (text == string.Empty)
                return;

            IRow row = wkbk.GetSheetAt(wkbk.ActiveSheetIndex).GetRow(rowIndex);
            if (row == null)
                row = wkbk.GetSheetAt(wkbk.ActiveSheetIndex).CreateRow(rowIndex);

            ICell cell = row.GetCell(cellIndex);
            if (cell == null)
                cell = row.CreateCell(cellIndex);

            cell.SetCellValue(text);
            cell.CellStyle.WrapText = true;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Top;
        }
        #endregion

        #region User-centric Routines

        [JsonRpcMethod("processUserLogin", Idempotent = true)]
        public string ProcessUserLogin(string username, string hashed_passcode)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("ProcessUserLogin", "username cannot be blank");

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call to get user account
                SqlCommand myCommand = new SqlCommand(String.Format("select uid, username, password from tbl_user_accounts where lower(username)=lower('{0}')", username), myConnection);
                myCommand.CommandType = CommandType.Text;

                // return reader and check for rows
                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception("Error: User does not exist");

                // read and assign user login info to variables
                rdr.Read();
                int uid = (int)rdr["uid"];
                string user_name = (string)rdr["username"];
                string stored_password = (string)rdr["password"]; //Encoding.Unicode.GetString((byte[])rdr["password"]);
                rdr.Close();

                // test if supplied hash matches the user record password
                if (hashed_passcode != stored_password)
                    throw new Exception("Error: Password does not match user record");

                Random rand = new Random();
                int startingPoint = rand.Next(1, 10);
                string sessionId = String.Format("{0}{1}:{2}", hashed_passcode.Substring(startingPoint, 10), rand.Next(100), DateTime.UtcNow.ToString("ddhhmmss"));

                // write the session id to the backend for retrieval later 
                myCommand.CommandText = "fsp_set_user_session_id";
                myCommand.CommandType = CommandType.StoredProcedure;

                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = uid;
                myCommand.Parameters.Add("@session_id", SqlDbType.VarChar).Value = sessionId;
                myCommand.ExecuteNonQuery();

                return sessionId;
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

        [JsonRpcMethod("processUserLogout")]
        public bool ProcessUserLogout(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string query = String.Format("UPDATE tbl_user_accounts SET session_id=null WHERE uid={0}", sec.userid);

                SqlCommand myCommand = new SqlCommand(query, myConnection);
                myCommand.CommandType = CommandType.Text;

                myCommand.ExecuteNonQuery();

                using (CECHarmPublicService ps = new CECHarmPublicService())
                {
                    ps.AuditLog_AddActivity(sec.userid, "logout");
                }

                return true;
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

        [JsonRpcMethod("validateSecurityToken")]
        public bool ValidateSecurityToken(SecurityToken sec)
        {
            return IsValidSessionId(sec);
        }
        
        [JsonRpcMethod("getUserPolicies")]
        public DataTable GetUserPolicies(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            DataTable dt = new DataTable();

            // set connection
            SqlConnection mySqlConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open the connection
                mySqlConnection.Open();

                // call stored proc to retrieve record
                SqlCommand mySelectCommand = new SqlCommand();
                mySelectCommand.CommandType = CommandType.Text;
                mySelectCommand.CommandText = "select * from tbl_user_policies where enable=1";
                mySelectCommand.Connection = mySqlConnection;
                // call SqlDataAdapter
                SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
                // file the dataset
                myAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // close connection
                mySqlConnection.Close();
            }

            return dt;
        }

        [JsonRpcMethod("getSecurityToken", Idempotent = true)]
        public SecurityToken GetSecurityToken(int userid, string sessionid)
        {
            if (userid == 0)
                throw new ArgumentException("GetSecurityToken.userid cannot be 0");
            else if (String.IsNullOrWhiteSpace(sessionid))
                throw new ArgumentException("GetSecurityToken.sessionid cannot be blank");

            SecurityToken token = new SecurityToken();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_user_accounts where uid={0} and session_id='{1}'", userid, sessionid), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception(String.Format("no user record with the user id {0}", userid));

                rdr.Read();
                token.userid = (int)rdr["uid"];
                token.session = (string)rdr["session_id"];
                token.email = (string)rdr["username"];
                token.access_level = (int)rdr["access_level"];
                rdr.Close();
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

            return token;
        }

        [JsonRpcMethod("getUserPasswordHistory")]
        public string[] GetUserPasswordHistory(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");
            
            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select top 6 password_history from tbl_user_password_history where uid={0}", sec.userid), myConnection);
                myCommand.CommandType = CommandType.Text;

                ArrayList history = new ArrayList();
                SqlDataReader rdr = myCommand.ExecuteReader();
                while (rdr.Read())
                    history.Add(rdr["password_history"]);
                rdr.Close();

                return (string[])history.ToArray(typeof(string));
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

        [JsonRpcMethod("getUserInformationStubByUsername", Idempotent = true)]
        public UserData GetUserInformationByUsername(string username)
        {
            // since this routine does not expect a security token, return stub instead

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("GetUserInformation", "username cannot be blank");

            UserData user = new UserData();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_user_accounts where lower(username)='{0}'", username.ToLower()), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception(String.Format("no user record with the username {0}", username));

                rdr.Read();
                user.userid = (int)rdr["uid"];
                user.email = (string)rdr["email"];
                user.user_name = (string)rdr["username"];

                rdr.Close();
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

            return user;
        }

        [JsonRpcMethod("getUserInformationByUsername")]
        public UserData GetUserInformationByUsername(SecurityToken sec, string username)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("GetUserInformation", "username cannot be blank");

            UserData user = new UserData();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_user_accounts where lower(username)='{0}'", username.ToLower()), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception(String.Format("no user record with the email address {0}", username));

                rdr.Read();
                user.userid = (int)rdr["uid"];
                user.access_level = (int)rdr["access_level"];
                user.account_lockout = (bool)rdr["account_lockout"];
                user.email = (string)rdr["email"];
                user.display_name = (string)rdr["display_name"];
                user.password_expired = (bool)rdr["password_expired"];
                user.password_reset_required = (bool)rdr["password_reset"];
                user.help_shown = (bool)rdr["help_shown"];

                user.user_name = (string)rdr["username"];
                user.cohort_id = (int)rdr["cohort_id"];

                if (rdr["last_login_date"] != DBNull.Value)
                    user.last_login = rdr.GetDateTime(rdr.GetOrdinal("last_login_date"));

                if (rdr["account_lockout_date"] != DBNull.Value)
                    user.account_lockout_date = rdr.GetDateTime(rdr.GetOrdinal("account_lockout_date"));

                if (rdr["password_date"] != DBNull.Value)
                    user.password_change_date = rdr.GetDateTime(rdr.GetOrdinal("password_date"));

                rdr.Close();
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

            return user;
        }

        [JsonRpcMethod("getUserInformationStubByUserID", Idempotent = true)]
        public UserData GetUserInformationByUserID(int userid)
        {
            // since this routine does not expect a security token, return stub instead

            if (userid == 0)
                throw new ArgumentNullException("GetUserInformation", "userid cannot be 0");

            UserData user = new UserData();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_user_accounts where uid={0}", userid), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception(String.Format("no user record with the user id {0}", userid));

                rdr.Read();
                user.userid = (int)rdr["uid"];
                user.email = (string)rdr["email"];
                user.user_name = (string)rdr["username"];
                rdr.Close();
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

            return user;
        }

        [JsonRpcMethod("getUserInformationByUserID")]
        public UserData GetUserInformationByUserID(SecurityToken sec, int userid)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            if (userid == 0)
                throw new ArgumentNullException("GetUserInformation", "userid cannot be 0");

            UserData user = new UserData();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_user_accounts where uid={0}", userid), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception(String.Format("no user record with the user id {0}", userid));

                rdr.Read();
                user.userid = (int)rdr["uid"];
                user.access_level = (int)rdr["access_level"];
                user.account_lockout = (bool)rdr["account_lockout"];
                user.email = (string)rdr["email"];
                user.display_name = (string)rdr["display_name"];
                user.password_expired = (bool)rdr["password_expired"];
                user.password_reset_required = (bool)rdr["password_reset"];
                user.help_shown = (bool)rdr["help_shown"];

                user.user_name = (string)rdr["username"];

                if(rdr["cohort_id"] != DBNull.Value)
                    user.cohort_id = (int)rdr["cohort_id"];

                if (rdr["last_login_date"] != DBNull.Value)
                    user.last_login = rdr.GetDateTime(rdr.GetOrdinal("last_login_date"));

                if (rdr["account_lockout_date"] != DBNull.Value)
                    user.account_lockout_date = rdr.GetDateTime(rdr.GetOrdinal("account_lockout_date"));

                if (rdr["password_date"] != DBNull.Value)
                    user.password_change_date = rdr.GetDateTime(rdr.GetOrdinal("password_date"));
                rdr.Close();
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

            return user;
        }

        [JsonRpcMethod("getUserInformationStubByEmail", Idempotent = true)]
        public UserData GetUserInformationByEmail(string email)
        {
            // since this routine does not expect a security token, return stub instead

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("GetUserInformation", "email cannot be blank");

            UserData user = new UserData();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_user_accounts where email='{0}'", email), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception(String.Format("no user record with the user email {0}", email));

                rdr.Read();
                user.userid = (int)rdr["uid"];
                user.email = (string)rdr["email"];
                user.user_name = (string)rdr["username"];
                rdr.Close();
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

            return user;
        }

        [JsonRpcMethod("getUserInformationByEmail")]
        public UserData GetUserInformationByEmail(SecurityToken sec, string email)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("GetUserInformation", "email cannot be blank");

            UserData user = new UserData();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_user_accounts where email='{0}'", email), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception(String.Format("no user record with the user email {0}", email));

                rdr.Read();
                user.userid = (int)rdr["uid"];
                user.access_level = (int)rdr["access_level"];
                user.account_lockout = (bool)rdr["account_lockout"];
                user.email = (string)rdr["email"];
                user.display_name = (string)rdr["display_name"];
                user.password_expired = (bool)rdr["password_expired"];
                user.password_reset_required = (bool)rdr["password_reset"];
                user.help_shown = (bool)rdr["help_shown"];

                user.user_name = (string)rdr["username"];
                if(rdr["cohort_id"] != DBNull.Value)
                    user.cohort_id = (int)rdr["cohort_id"];

                if (rdr["last_login_date"] != DBNull.Value)
                    user.last_login = rdr.GetDateTime(rdr.GetOrdinal("last_login_date"));

                if (rdr["account_lockout_date"] != DBNull.Value)
                    user.account_lockout_date = rdr.GetDateTime(rdr.GetOrdinal("account_lockout_date"));

                if (rdr["password_date"] != DBNull.Value)
                    user.password_change_date = rdr.GetDateTime(rdr.GetOrdinal("password_date"));
                rdr.Close();
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

            return user;
        }

        [JsonRpcMethod("getUserInformationByCohortId")]
        public UserData GetUserInformationByCohortId(SecurityToken sec, int cohort_id)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");
            
            UserData user = new UserData();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select * from tbl_user_accounts where cohort_id={0}", cohort_id), myConnection);
                myCommand.CommandType = CommandType.Text;

                SqlDataReader rdr = myCommand.ExecuteReader();
                if (!rdr.HasRows)
                    throw new Exception(String.Format("no user record tied to cohort id {0}", cohort_id));

                rdr.Read();
                user.userid = (int)rdr["uid"];
                user.access_level = (int)rdr["access_level"];
                user.account_lockout = (bool)rdr["account_lockout"];
                user.email = (string)rdr["email"];
                user.display_name = (string)rdr["display_name"];
                user.password_expired = (bool)rdr["password_expired"];
                user.password_reset_required = (bool)rdr["password_reset"];
                user.help_shown = (bool)rdr["help_shown"];

                user.user_name = (string)rdr["username"];
                user.cohort_id = (int)rdr["cohort_id"];

                if (rdr["last_login_date"] != DBNull.Value)
                    user.last_login = rdr.GetDateTime(rdr.GetOrdinal("last_login_date"));

                if (rdr["account_lockout_date"] != DBNull.Value)
                    user.account_lockout_date = rdr.GetDateTime(rdr.GetOrdinal("account_lockout_date"));

                if (rdr["password_date"] != DBNull.Value)
                    user.password_change_date = rdr.GetDateTime(rdr.GetOrdinal("password_date"));
                rdr.Close();
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

            return user;
        }

        [JsonRpcMethod("getUsers")]
        public DataTable GetUsers(SecurityToken sec, string user_fields)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            string select_clause = "*";
            if (!string.IsNullOrWhiteSpace(user_fields))
                select_clause = user_fields;

            DataTable dt_users = new DataTable();

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand(String.Format("select {0} from tbl_user_accounts order by display_name", select_clause), myConnection);
                myCommand.CommandType = CommandType.Text;

                using (SqlDataAdapter myAdapter = new SqlDataAdapter(myCommand))
                    myAdapter.Fill(dt_users);

                return dt_users;
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

        [JsonRpcMethod("createUser")]
        public UserData CreateUser(SecurityToken sec, UserData userInfo)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string query;
                if (userInfo.access_level == 100)
                {
                    string cohort_name = string.Empty,
                            cohort_acronym = string.Empty;

                    cohort_acronym = userInfo.user_name.ToUpper();
                    if (GetDataTable(String.Format("select cohort_id from tbl_web_cohorts_v4_0 where cohort_acronym='{0}'", cohort_acronym)).Rows.Count > 0)
                        throw new Exception(String.Format("Cohort with acronym {0} already exists, please try again", cohort_acronym));

                    userInfo.user_name = String.Format("{0}_REP", cohort_acronym);
                    userInfo.user_name = Regex.Replace(userInfo.user_name, @"\s+", string.Empty);
                    userInfo.user_name = Regex.Replace(userInfo.user_name, @"\W+", "_");
                    cohort_name = userInfo.display_name.Split('|')[0];
                    userInfo.display_name = userInfo.display_name.Split('|')[1];

                    query = String.Format("insert into tbl_web_cohorts_v4_0 (cohort_id, cohort_name, cohort_acronym, status, status_timestamp) select (MAX(cohort_id) + 1), '{0}', '{1}', 'draft', GETDATE() FROM tbl_web_cohorts_v4_0",
                    cohort_name, cohort_acronym);
                    using (SqlCommand sql = new SqlCommand(query, myConnection))
                    {
                        sql.CommandType = CommandType.Text;
                        sql.ExecuteNonQuery();
                    }

                    query = String.Format("select cohort_id from tbl_web_cohorts_v4_0 where cohort_acronym='{0}'", cohort_acronym);
                    DataTable cohort_dt = GetDataTable(query);
                    if (cohort_dt.Rows.Count == 0)
                        throw new Exception("failed to create cohort record");

                    userInfo.cohort_id = (int)cohort_dt.Rows[0]["cohort_id"];
                }
                
                query = String.Format("insert into tbl_user_accounts (username, display_name, email, access_level, cohort_id, password_reset) values ('{0}', '{1}', '{2}', {3}, {4}, 1)",
                    new object[] { userInfo.user_name, userInfo.display_name, userInfo.email, userInfo.access_level, userInfo.cohort_id } );
                using (SqlCommand sql = new SqlCommand(query, myConnection))
                {
                    sql.CommandType = CommandType.Text;
                    sql.ExecuteNonQuery();
                }

                using (CECHarmPublicService ps = new CECHarmPublicService())
                {
                    ps.AuditLog_AddActivity(sec.userid, String.Format("new user created: {0}", userInfo.email));
                }

                return GetUserInformationByEmail(userInfo.email);
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

        [JsonRpcMethod("setUserInformation")]
        public UserData SetUserInformation(SecurityToken sec, UserData userInfo)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            if (userInfo.userid == 0)
                throw new Exception("UserInfo.userid must be set");

            if (sec.userid != userInfo.userid && sec.access_level <= 100)
                throw new Exception("SecurityToken is invalid for the UserData supplied, no update will be made");

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand("fsp_set_user_information", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.Add("@displayName", SqlDbType.NVarChar).Value = userInfo.display_name;
                myCommand.Parameters.Add("@email", SqlDbType.NVarChar).Value = userInfo.email;

                if (sec.userid > 0)
                    myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = userInfo.userid;

                myCommand.ExecuteNonQuery();

                return GetUserInformationByEmail(userInfo.email);
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

        [JsonRpcMethod("setUserSecurityAttributes")]
        public UserData SetUserSecurityAttributes(SecurityToken sec, UserData userInfo)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            if ((sec.session != "NEW") && sec.userid != userInfo.userid && sec.access_level <= 100)
                throw new Exception("SecurityToken is invalid for the UserData supplied, no update will be made");

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand("fsp_set_user_attributes", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.Add("@accessLevel", SqlDbType.Int).Value = userInfo.access_level;
                myCommand.Parameters.Add("@accountLockout", SqlDbType.Bit).Value = userInfo.account_lockout;
                myCommand.Parameters.Add("@passwordReset", SqlDbType.Bit).Value = userInfo.password_reset_required;
                myCommand.Parameters.Add("@helpShown", SqlDbType.Bit).Value = userInfo.help_shown;
                myCommand.Parameters.Add("@passwordExpired", SqlDbType.Bit).Value = userInfo.password_expired;

                if (userInfo.last_login != DateTime.MinValue)
                    myCommand.Parameters.Add("@lastLoginDate", SqlDbType.DateTime).Value = userInfo.last_login;

                if (userInfo.account_lockout_date != DateTime.MinValue)
                    myCommand.Parameters.Add("@lockoutDate", SqlDbType.Date).Value = userInfo.account_lockout_date;

                if (userInfo.password_change_date != DateTime.MinValue)
                    myCommand.Parameters.Add("@passwordChangeDate", SqlDbType.Date).Value = userInfo.password_change_date;

                myCommand.Parameters.Add("@uid", SqlDbType.Int).Value = userInfo.userid;

                myCommand.ExecuteNonQuery();

                return GetUserInformationByEmail(userInfo.email);
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

        [JsonRpcMethod("setUserPassword")]
        public UserData SetUserPassword(SecurityToken sec, int userid, string password)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            if (sec.userid != userid && sec.access_level <= 100)
                throw new Exception("SecurityToken is invalid for this type of call due to insufficient access rights, no update will be made");

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                // call stored proc to get user account summary
                SqlCommand myCommand = new SqlCommand("fsp_set_user_password", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                myCommand.Parameters.Add("@user_id", SqlDbType.Int).Value = userid;

                myCommand.ExecuteNonQuery();

                /// now update the password histroy table
                myCommand.Parameters.Clear();
                myCommand.CommandText = "fsp_set_user_password_history";
                myCommand.Parameters.Add("@user_id", SqlDbType.Int).Value = userid;
                myCommand.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

                myCommand.ExecuteNonQuery();

                /// return user information
                return GetUserInformationByUserID(sec, userid);
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

        [JsonRpcMethod("unlockUserAccount")]
        public void UnlockUserAccount(SecurityToken sec, int userid)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            try
            {
                UserData ud = GetUserInformationByUserID(sec, userid);
                ud.account_lockout = false;

                SetUserSecurityAttributes(sec, ud);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [JsonRpcMethod("lockUserAccount")]
        public void LockUserAccount(SecurityToken sec, int userid)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            try
            {
                UserData ud = GetUserInformationByUserID(sec, userid);
                ud.account_lockout = true;
                ud.account_lockout_date = DateTime.Now;

                SetUserSecurityAttributes(sec, ud);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public void CreateEmailAndSend(SecurityToken sec, string template_name, NameValueCollection data)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            SendEmail(template_name, data);

            using (CECHarmPublicService ps = new CECHarmPublicService())
            {
                ps.AuditLog_AddActivity(sec.userid, String.Format("email sent: {0} template, to {1}", template_name, data["to"]));
            }
        }

        [JsonRpcMethod("getInputFields")]
        public DataTable GetInputFields(SecurityToken sec)
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
            mySelectCommand.CommandText = "select i.*, cf.data_table, cf.data_field from tbl_web_input_fields i left join tbl_cohort_fields cf on cf.id=i.cohort_field_id order by section, position";
            mySelectCommand.Connection = mySqlConnection;
            // call SqlDataAdapter
            SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
            // file the dataset
            myAdapter.Fill(ds, "tbl_web_input_fields");

            // close connection
            mySqlConnection.Close();

            return ds.Tables["tbl_web_input_fields"];
        }

        [JsonRpcMethod("getInputFieldsSections")]
        public DataTable GetInputFieldsSections(SecurityToken sec)
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
            mySelectCommand.CommandText = "select * from tbl_web_input_fields where type='section' order by position";
            mySelectCommand.Connection = mySqlConnection;
            // call SqlDataAdapter
            SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
            // file the dataset
            myAdapter.Fill(ds, "tbl_web_input_fields");

            // close connection
            mySqlConnection.Close();

            return ds.Tables["tbl_web_input_fields"];
        }

        [JsonRpcMethod("getInputFieldsBySection")]
        public DataTable GetInputFieldsBySection(SecurityToken sec, int sectionId)
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
            mySelectCommand.CommandText = String.Format("select i.*, cf.data_table, cf.data_field from tbl_web_input_fields i left join tbl_cohort_fields cf on cf.id=i.cohort_field_id where i.id={0} or i.section={0} order by section, position", sectionId);
            mySelectCommand.Connection = mySqlConnection;
            // call SqlDataAdapter
            SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
            // file the dataset
            myAdapter.Fill(ds, "tbl_web_input_fields");

            // close connection
            mySqlConnection.Close();

            return ds.Tables["tbl_web_input_fields"];
        }

        [JsonRpcMethod("getInputFieldQuestionText")]
        public string GetInputFieldQuestionText(SecurityToken sec, string data_field)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // set connection
            SqlConnection con = new SqlConnection(this.database_connection_string);

            string result_question = string.Empty;

            try
            {

                // open the connection
                con.Open();

                string query = String.Format("select i.*, cf.data_table, cf.data_field from tbl_web_input_fields i " +
                                            "left join tbl_cohort_fields cf on cf.id=i.cohort_field_id where data_field='{0}'", data_field);

                SqlCommand com = new SqlCommand(query, con);
                com.CommandType = CommandType.Text;

                DataTable dt_field = new DataTable();
                using (SqlDataAdapter adp = new SqlDataAdapter(com))
                {
                    adp.Fill(dt_field);
                }

                if (dt_field.Rows.Count == 0)
                    throw new Exception(String.Format("unable to find field {0}", data_field));

                int section_id = (int)dt_field.Rows[0]["section"];
                query = String.Format("select * from tbl_web_input_fields where section={0} ", section_id);
                if ((double)dt_field.Rows[0]["position"] > 0)
                    query += String.Format(" and CAST(position as int)={0}", (int)Math.Truncate((double)dt_field.Rows[0]["position"]));
                else if (dt_field.Rows[0]["use_with_input_id"] != DBNull.Value && (int)dt_field.Rows[0]["use_with_input_id"] > 1)
                    query += String.Format(" and (id={0} or CAST(position as int)=(select CAST(position as INT) from tbl_web_input_fields where id={0}))", dt_field.Rows[0]["use_with_input_id"]);

                DataTable dt_group = new DataTable();
                com.CommandText = query;
                using (SqlDataAdapter adp = new SqlDataAdapter(com))
                {
                    adp.Fill(dt_group);
                }

                if (section_id == 17 || section_id == 30)
                {
                    if (data_field.Contains("mdc_other_tobacco_"))
                        result_question += dt_group.Select("id=412")[0]["label_text"].ToString();
                    else if (data_field.Contains("bio_blood_baseline_") || data_field.Contains("bio_blood_other_times_"))
                        result_question += dt_group.Select("id=311")[0]["label_text"].ToString();
                    else if (dt_group.Select(String.Format("type='label' and use_with_input_id={0}", dt_field.Rows[0]["use_with_input_id"])).Length > 0)
                    {
                        DataRow[] dr = dt_group.Select(String.Format("type='label' and use_with_input_id={0}", dt_field.Rows[0]["use_with_input_id"]));
                        result_question += String.Format("{0}", dr[0]["label_text"]);
                    }
                }
                else if (section_id == 23 && data_field.Contains("race_"))
                {
                    result_question += dt_group.Select("id=314")[0]["label_text"].ToString();
                }
                else if (section_id == 22 && data_field.Contains("ci_") && data_field.EndsWith("male"))
                {
                    result_question += dt_group.Select("id=313")[0]["label_text"].ToString();
                }
                else
                {
                    if (dt_group.Select("type='group'").Length > 0)
                    {
                        DataRow[] dr = dt_group.Select("type='group'");
                        result_question = String.Format("{0}", dr[0]["label_text"]);
                    }

                    if ((String.IsNullOrWhiteSpace(result_question)) && (dt_field.Rows[0]["use_with_input_id"] != DBNull.Value && (int)dt_field.Rows[0]["use_with_input_id"] > 1) &&
                        dt_group.Select(String.Format("id={0}", dt_field.Rows[0]["use_with_input_id"])).Length > 0)
                    {
                        DataRow[] dr = dt_group.Select(String.Format("id={0}", dt_field.Rows[0]["use_with_input_id"]));
                        if (!String.IsNullOrWhiteSpace(dr[0]["label_text"].ToString()))
                            result_question += String.Format("{0}", dr[0]["label_text"]);
                    }

                    if ((String.IsNullOrWhiteSpace(result_question)) && !String.IsNullOrWhiteSpace(dt_field.Rows[0]["label_text"].ToString()))
                        result_question += String.Format("{0}", dt_field.Rows[0]["label_text"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }

            return result_question;
        }

        [JsonRpcMethod("getCancerTypes")]
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

        [JsonRpcMethod("getBiospecimenTypes")]
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
        /// Retrieves a cohort record from the database. for input
        /// </summary>
        /// <returns>cohort record, response code</returns>
        [JsonRpcMethod("getCohortRecordById")]
        public DataTable GetCohortRecordById(SecurityToken sec, int cohort_id, bool draft_version)
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
            mySelectCommand.Parameters.Add("@cohort_id", SqlDbType.Int).Value = cohort_id;
            mySelectCommand.Parameters.Add("@draft_version", SqlDbType.Bit).Value = draft_version;
            mySelectCommand.Connection = mySqlConnection;
            // call SqlDataAdapter
            SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
            // file the dataset
            myAdapter.Fill(ds, "tbl_cohorts");

            // close connection
            mySqlConnection.Close();

            // return the data set
            return ds.Tables["tbl_cohorts"];
        }

        [JsonRpcMethod("getDraftWebCohortId")]
        public int GetDraftWebCohortId(SecurityToken sec, int cohort_id)
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
            mySelectCommand.Parameters.Add("@cohort_id", SqlDbType.Int).Value = cohort_id;
            mySelectCommand.Parameters.Add("@draft_version", SqlDbType.Bit).Value = true;
            mySelectCommand.Connection = mySqlConnection;
            // call SqlDataAdapter
            SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
            // file the dataset
            myAdapter.Fill(ds, "tbl_cohorts");

            // close connection
            mySqlConnection.Close();

            // return the data set
            return (int)ds.Tables["tbl_cohorts"].Rows[0]["id"];
        }

        [JsonRpcMethod("getCohortRecordByWebId")]
        public DataTable GetCohortRecordByWebId(SecurityToken sec, int web_cohort_id)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            DataTable dt = new DataTable();

            // set connection
            SqlConnection mySqlConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open the connection
                mySqlConnection.Open();

                string queryStr = String.Format("select * from tbl_web_cohorts_v4_0 where id={0}", web_cohort_id);

                // call stored proc to retrieve record
                SqlCommand mySelectCommand = new SqlCommand(queryStr, mySqlConnection);
                mySelectCommand.CommandType = CommandType.Text;
                // call SqlDataAdapter
                SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
                // file the dataset
                myAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // close connection
                mySqlConnection.Close();
            }

            return dt;
        }

        /// <summary>
        /// Retrieve cohort records from the database
        /// </summary>
        /// <returns>cohort record, response code</returns>
        [JsonRpcMethod("getCohortsByStatus")]
        public DataTable GetCohortsByStatus(SecurityToken sec, string record_status)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            DataTable dt = new DataTable();

            // set connection
            SqlConnection mySqlConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open the connection
                mySqlConnection.Open();

                string queryStr = String.Format("select * from tbl_web_cohorts_v4_0 where [status]='{0}'", record_status.ToLower());
                if (record_status.ToLower() == "published")
                    queryStr += " and published=1";
                else
                    queryStr += " and published=0";

                queryStr += " order by [status] desc, status_timestamp";

                SqlCommand mySelectCommand = new SqlCommand(queryStr, mySqlConnection);
                mySelectCommand.CommandType = CommandType.Text;
                // call SqlDataAdapter
                SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
                // file the dataset
                myAdapter.Fill(dt);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mySqlConnection.Close();
            }

            return dt;
        }

        /// <summary>
        /// Retrieve cohort records from the database
        /// </summary>
        /// <returns>cohort record, response code</returns>
        [JsonRpcMethod("getCohortsByStatusWithColumns")]
        public DataTable GetCohortsByStatusWithColumns(SecurityToken sec, string record_status, string database_columns)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            DataTable dt = new DataTable();

            // set connection
            SqlConnection mySqlConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string select_columns = "*";
                if (!String.IsNullOrWhiteSpace(database_columns))
                    select_columns = database_columns;

                // open the connection
                mySqlConnection.Open();

                string queryStr = String.Format("select {0} from tbl_web_cohorts_v4_0 where [status]='{1}'", select_columns, record_status.ToLower());
                if (record_status.ToLower() == "published")
                    queryStr += " and published=1";
                else
                    queryStr += " and published=0";
                queryStr += " order by cohort_acronym";

                SqlCommand mySelectCommand = new SqlCommand(queryStr, mySqlConnection);
                mySelectCommand.CommandType = CommandType.Text;
                // call SqlDataAdapter
                SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
                // file the dataset
                myAdapter.Fill(dt);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mySqlConnection.Close();
            }

            return dt;
        }

        /// <summary>
        /// Retrieve cohort records from the database
        /// </summary>
        /// <returns>cohort record, response code</returns>
        [JsonRpcMethod("getCohortsWithStatusesWithColumns")]
        public DataTable GetCohortsWithStatusesWithColumns(SecurityToken sec, string[] record_statuses, string database_columns)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            DataTable dt = new DataTable();

            // set connection
            SqlConnection mySqlConnection = new SqlConnection(this.database_connection_string);

            try
            {
                string select_columns = "*";
                if (!String.IsNullOrWhiteSpace(database_columns))
                    select_columns = database_columns;

                // open the connection
                mySqlConnection.Open();

                string queryStr = String.Format("select {0} from tbl_web_cohorts_v4_0 where ", select_columns);
                string queryStatuses = "",
                    queryPublished = " published=0";
                foreach (string s in record_statuses)
                {
                    if (s.ToLower() == "published")
                        queryPublished = " published=1";

                    queryStatuses += String.Format("'{0}',", s);
                }
                queryStatuses = queryStatuses.TrimEnd(',');
                queryStr += String.Format("[status] in ({0}) and {1} order by cohort_acronym", queryStatuses, queryPublished);

                SqlCommand mySelectCommand = new SqlCommand(queryStr, mySqlConnection);
                mySelectCommand.CommandType = CommandType.Text;
                // call SqlDataAdapter
                SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
                // file the dataset
                myAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mySqlConnection.Close();
            }

            return dt;
        }

        /// <summary>
        /// get the list of attachments for a given cohort id
        /// </summary>
        [JsonRpcMethod("getCohortAttachments")]
        public DataTable GetCohortAttachments(SecurityToken sec, int web_cohort_id)
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
            return ds.Tables[0];
        }

        /// <summary>
        /// get the attachment for a given attachment id
        /// </summary>
        [JsonRpcMethod("getCohortAttachment")]
        public DataTable GetCohortAttachment(SecurityToken sec, int attachment_id)
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

                string sqlQ = String.Format("select ca.*, dt.document_type from tbl_web_cohort_attachments ca join tbl_web_document_types dt on dt.document_type_id=ca.document_type_id where ca.id={0}", attachment_id);

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
            return ds.Tables[0];
        }

        /// <summary>
        /// get the attached document in binary form
        /// </summary>
        [JsonRpcMethod("getCohortDocument")]
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

        [JsonRpcMethod("doesValueExist")]
        public bool DoesValueExist(SecurityToken sec, string table_name, string field_name, object field_value)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("Session is invalid");

            bool value_exists = false;

            // set connection
            SqlConnection myConnection = new SqlConnection(this.database_connection_string);

            try
            {
                // open connection
                myConnection.Open();

                string sqlQ = String.Format("select {0} from {1} where {0}='{2}'", field_name, table_name, field_value);

                System.Data.SqlClient.SqlCommand com =
                    new SqlCommand(sqlQ, myConnection);
                com.CommandType = CommandType.Text;

                System.Data.SqlClient.SqlDataReader rdr = com.ExecuteReader();
                if (rdr.HasRows)
                    value_exists = true;

            }
            catch (Exception ex)
            {
                throw new Exception("failed to retrieve and write the document", ex);
            }
            finally
            {
                myConnection.Close();
            }

            return value_exists;
        }

        /// <summary>
        /// for input
        /// </summary>
        [JsonRpcMethod("setCohortWebData")]
        public string SetCohortWebData(SecurityToken sec, int web_cohort_id, string jsonObj)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            IDictionary dic = JsonConvert.Import<IDictionary>(jsonObj);

            DataTable dt_cohort = GetCohortRecordByWebId(sec, web_cohort_id);

            string sql = "UPDATE tbl_web_cohorts_v4_0 SET ";
            foreach (DictionaryEntry de in dic)
            {
                if (de.Key.ToString().ToLower() == "id" || de.Key.ToString().ToLower() == "cohort_id")
                    continue;

                if (dt_cohort.Columns[de.Key.ToString()].DataType == typeof(int))
                {
                    if(String.IsNullOrWhiteSpace(de.Value.ToString()))
                        sql += String.Format("{0}=-1,", de.Key);
                    else
                        sql += String.Format("{0}={1},", de.Key, de.Value);
                }
                else
                    sql += String.Format("{0}='{1}',", de.Key, de.Value);
            }
            sql += String.Format(" status_timestamp=GETDATE() WHERE id={0}", dic["id"]);

            // return message
            string returnMessage = "success";
            // set connection
            SqlConnection con = new SqlConnection(this.database_connection_string);

            try
            {
                // open the connection
                con.Open();

                // call stored proc to retrieve record
                SqlCommand execQuery = new SqlCommand();
                execQuery.CommandType = CommandType.Text;
                execQuery.CommandText = sql;
                execQuery.Connection = con;

                execQuery.ExecuteNonQuery();

                using (CECHarmPublicService ps = new CECHarmPublicService())
                {
                    ps.AuditLog_AddActivity(sec.userid, String.Format("cohort input form saved, record id {0}", web_cohort_id));
                }
            }
            catch (Exception ex)
            {
                returnMessage = String.Format("failure: {0}", ex.Message);
            }
            finally
            {
                con.Close();
            }

            return returnMessage;
        }

        [JsonRpcMethod("createCohortAttachment")]
        public string CreateCohortAttachment(SecurityToken sec, int web_cohort_id, string attachment_type, int document_type_id, string url, string file_name)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // set connection
            SqlConnection con = new SqlConnection(this.database_connection_string);

            string returnMessage = string.Empty;
            try
            {
                // open the connection
                con.Open();

                int intFileLength = 0;
                byte[] fileContent = new byte[0];

                DateTime dttFileDate = DateTime.Now;

                if (!String.IsNullOrWhiteSpace(file_name))
                {
                    // build path to user session folder
                    string strUserFolder = HttpContext.Current.Server.MapPath("~/user_files") + "\\" + sec.userid.ToString() + "\\";

                    // build path to the user file 
                    string strFilePath = strUserFolder + "\\" + file_name;

                    // get file length
                    using (FileStream fs = new FileStream(strFilePath, FileMode.Open))
                    {
                        // instantiate the byte array object to the length of the file stream (which is the same as file size)
                        fileContent = new byte[fs.Length];

                        // read the stream into the byte array and assign the length of the buffer to intFileLength
                        intFileLength = fs.Read(fileContent, 0, (int)fs.Length);
                    }

                    // get file info and extract the date
                    FileInfo fi = new FileInfo(strFilePath);
                    // extract the file date
                    dttFileDate = fi.CreationTime;
                }

                // call stored proc to append the web attachement record -- Note this appends it to the "draft" web cohort record only
                SqlCommand myAppendCommand = new SqlCommand("fsp_append_web_attachment_record", con);
                myAppendCommand.CommandType = CommandType.StoredProcedure;
                myAppendCommand.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = web_cohort_id;
                myAppendCommand.Parameters.Add("@document_type_id", SqlDbType.Int).Value = document_type_id;
                myAppendCommand.Parameters.Add("@attachment_type", SqlDbType.VarChar, 10).Value = attachment_type;
                myAppendCommand.Parameters.Add("@url", SqlDbType.NVarChar, 250).Value = url;
                myAppendCommand.Parameters.Add("@file_name", SqlDbType.NVarChar, 250).Value = file_name;
                myAppendCommand.Parameters.Add("@file_size", SqlDbType.Int).Value = intFileLength;
                myAppendCommand.Parameters.Add("@file_date", SqlDbType.DateTime).Value = dttFileDate;
                myAppendCommand.Parameters.Add("@file_content", SqlDbType.VarBinary).Value = fileContent;
                // execute it
                myAppendCommand.ExecuteNonQuery();

                returnMessage = "success";
            }
            catch (Exception ex)
            {
                returnMessage = String.Format("failure: {0}", ex.Message);
            }
            finally
            {
                con.Close();
            }

            return JsonConvert.ExportToString(returnMessage);

        }

        [JsonRpcMethod("deleteCohortAttachment")]
        public string DeleteCohortAttachment(SecurityToken sec, int attachment_id)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // set connection
            SqlConnection con = new SqlConnection(this.database_connection_string);

            string returnMessage = string.Empty;
            try
            {
                // open the connection
                con.Open();

                // call stored proc to retrieve record
                SqlCommand execQuery = new SqlCommand();
                execQuery.CommandType = CommandType.Text;
                execQuery.CommandText = String.Format("DELETE FROM tbl_web_cohort_attachments WHERE id={0}", attachment_id);
                execQuery.Connection = con;

                execQuery.ExecuteNonQuery();

                returnMessage = "success";
            }
            catch (Exception ex)
            {
                returnMessage = String.Format("failure: {0}", ex.Message);
            }
            finally
            {
                con.Close();
            }

            return returnMessage;

        }

        [JsonRpcMethod("updateCohortAttachment")]
        public string UpdateCohortAttachment(SecurityToken sec, int attachment_id, int document_type_id, string url)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            // set connection
            SqlConnection con = new SqlConnection(this.database_connection_string);

            string returnMessage = string.Empty;
            try
            {
                // open the connection
                con.Open();

                string query = String.Format("UPDATE tbl_web_cohort_attachments SET document_type_id={0}", document_type_id);
                if (!String.IsNullOrWhiteSpace(url))
                    query += String.Format(", url='{0}'", url);

                query += String.Format(" WHERE id={0}", attachment_id);

                // call stored proc to retrieve record
                SqlCommand execQuery = new SqlCommand();
                execQuery.CommandType = CommandType.Text;
                execQuery.CommandText = query;
                execQuery.Connection = con;

                execQuery.ExecuteNonQuery();

                returnMessage = "success";
            }
            catch (Exception ex)
            {
                returnMessage = String.Format("failure: {0}", ex.Message);
            }
            finally
            {
                con.Close();
            }

            return returnMessage;

        }

        /*************************************************************************************
         * Function:		Export Cohort Data -- PROD
         * Purpose:		    Exports all of the cohort fields from Production Database to an Excel file
         *
         */
        [JsonRpcMethod("exportCohortData")]
        [JsonRpcHelp("Export Cohort Data.")]
        public string ExportCohortData(SecurityToken sec)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            string output_path = string.Empty;
            SqlConnection myConnection = new SqlConnection(database_connection_string);

            try
            {
                // open the connection
                myConnection.Open();

                output_path = Server.MapPath("/user_files") + String.Format("/{0}/cohort_export_{1}.xlsx", sec.userid, DateTime.Now.ToString("ddMMyyyy"));

                string sql_query = "SELECT * FROM tbl_web_cohorts_v4_0 where published=1 and [status]='published'";
                SqlDataExport(sql_query, string.Empty, output_path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // close the connection
                myConnection.Close();
            }

            return output_path;
        }

        [JsonRpcMethod("exportCohortDraftData")]
        [JsonRpcHelp("Export Draft Cohort Data.")]
        public string ExportCohortDraftData(SecurityToken sec, int web_cohort_id)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            string output_path = string.Empty,
                web_path = string.Empty;
            SqlConnection myConnection = new SqlConnection(database_connection_string);

            try
            {
                // open the connection
                myConnection.Open();

                web_path = String.Format("/user_files/{0}/cohort_export_{1}.xlsx", sec.userid, DateTime.Now.ToString("ddMMyyyy"));
                output_path = Server.MapPath("/user_files") + String.Format("/{0}/cohort_export_{1}.xlsx", sec.userid, DateTime.Now.ToString("ddMMyyyy"));

                string sql_query = String.Format("SELECT * FROM tbl_web_cohorts_v4_0 where id={0}", web_cohort_id);
                SqlDataExport(sql_query, string.Empty, output_path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // close the connection
                myConnection.Close();
            }

            return web_path;
        }

        [JsonRpcMethod("validateCohortRecord")]
        public string ValidateCohortRecord(SecurityToken sec, int web_cohort_id)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            SqlConnection con = new SqlConnection(database_connection_string);
            string results = string.Empty;
            try
            {
                con.Open();

                System.Data.DataTable dt_input = new DataTable(),
                    dt_cohort = new DataTable();
                
                // call stored proc to retrieve record
                SqlCommand mySelectCommand = new SqlCommand("select i.*, f.data_field from tbl_web_input_fields i join tbl_cohort_fields f on i.cohort_field_id=f.id where i.cohort_field_id is not null", con);
                mySelectCommand.CommandType = CommandType.Text;
                // call SqlDataAdapter
                SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
                // file the dataset
                myAdapter.Fill(dt_input);

                mySelectCommand.CommandText = String.Format("select * from tbl_web_cohorts_v4_0 where published=0 and [status] in ('draft','inprogress','rejected') and id={0}", web_cohort_id);
                myAdapter.Fill(dt_cohort);

                con.Close();
                // removed from skipables: contact_position;contact_email;
                string skipable = "contact_name;contact_email;collab_contact_name;collab_contact_email;collab_contact_position;collab_contact_phone;enrollment_target;enrollment_year_complete;" +
                    "mort_death_code_used_icd9;mort_death_code_used_icd10;mort_death_not_coded;mort_death_code_used_other;dlh_nih_cedr;dlh_nih_dbgap;dlh_nih_biolincc;dlh_nih_other;ci_treatment_data_surgery;ci_treatment_data_radiation;" +
                    "ci_treatment_data_chemotherapy;ci_treatment_data_hormonal_therapy;ci_treatment_data_bone_stem_cell;ci_treatment_data_other;ci_data_source_admin_claims;ci_data_source_electronic_records;ci_data_source_chart_abstraction;" +
                    "ci_data_source_patient_reported;ci_data_source_other;bio_blood_baseline_serum;bio_blood_baseline_plasma;bio_blood_baseline_buffy_coat;bio_blood_baseline_other_derivative;bio_blood_other_time_serum;bio_blood_other_time_plasma;" +
                    "bio_blood_other_time_buffy_coat;bio_blood_other_time_other_derivative;bio_tumor_block_info;" +
                    "pi_name_2;pi_institution_2;pi_email_2;pi_name_3;pi_institution_3;pi_email_3;pi_name_4;pi_institution_4;pi_email_4;pi_name_5;pi_institution_5;pi_email_5;pi_name_6;pi_institution_6;pi_email_6";
                foreach (DataRow dr in dt_input.Rows)
                {
                    string data_field = dr["data_field"].ToString(),
                        value = dt_cohort.Rows[0][data_field].ToString(),
                        validation_pattern = dr["validation_check"].ToString();

                    if (value == "-1")
                        value = string.Empty;

                    if ((bool)dr["required_field"] && String.IsNullOrWhiteSpace(value))
                        results += (!results.Contains(data_field) ? String.Format("{0}|{1}|required;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                    else if (skipable.Contains(data_field) && String.IsNullOrWhiteSpace(value))
                    {
                        // person to contact
                        if (data_field.StartsWith("contact_") && (dt_cohort.Rows[0]["clarification_contact"] == DBNull.Value || ((bool)dt_cohort.Rows[0]["clarification_contact"]) == false))
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        // person to collaborate
                        else if (data_field.StartsWith("collab_") && ((dt_cohort.Rows[0]["same_as_a3a"] == DBNull.Value || (bool)dt_cohort.Rows[0]["same_as_a3a"] == false) && (dt_cohort.Rows[0]["same_as_a3b"] == DBNull.Value || (bool)dt_cohort.Rows[0]["same_as_a3b"] == false)))
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        // currently enrolling
                        else if ((data_field == "enrollment_target" || data_field == "enrollment_year_complete") && dt_cohort.Rows[0]["enrollment_ongoing"].ToString().ToLower() == "yes")
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        // mortality coding
                        else if(data_field.StartsWith("mort_death_") && dt_cohort.Rows[0]["mort_have_cause_of_death"].ToString().ToLower() == "yes")
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        // data linking
                        else if ((data_field.StartsWith("dlh_nih_") && data_field != "dlh_nih_repository") && dt_cohort.Rows[0]["dlh_nih_repository"].ToString().ToLower() == "yes")
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        // cancer treatment data
                        else if ((data_field.StartsWith("ci_treatment_data_") || data_field.StartsWith("ci_data_source_")) && dt_cohort.Rows[0]["ci_cancer_treatment_data"].ToString().ToLower() == "yes")
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        // bio blood at baseline
                        else if(data_field.StartsWith("bio_blood_baseline_") && dt_cohort.Rows[0]["bio_blood_baseline"].ToString().ToLower() == "yes")
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        // bio blood at other times
                        else if(data_field.StartsWith("bio_blood_other_time_") && dt_cohort.Rows[0]["bio_blood_other_time"].ToString().ToLower() == "yes")
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        // bio tumor information
                        else if(data_field == "bio_tumor_block_info" && (dt_cohort.Rows[0]["bio_tissue_baseline"].ToString().ToLower() == "yes" || dt_cohort.Rows[0]["bio_tissue_other_time"].ToString().ToLower() == "yes"))
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                    }
                    else if ((!String.IsNullOrWhiteSpace(value) && !String.IsNullOrWhiteSpace(validation_pattern)) && !Regex.IsMatch(value, validation_pattern))
                    {

                        if (dr["use_with_input_id"] != DBNull.Value && (int)dr["use_with_input_id"] != 0 && dt_input.Select(String.Format("id={0}", dr["use_with_input_id"])).Length > 0)
                        {
                            DataRow[] i_dr = dt_input.Select(String.Format("id={0}", dr["use_with_input_id"]));
                            if (i_dr[0]["cohort_field_id"] == DBNull.Value || (int)dr["use_with_input_id"] == 0)
                                continue;

                            string[] a = i_dr[0]["attributes"].ToString().Split('|');
                            string parent_input_option = string.Empty;
                            foreach (string s in a)
                            {
                                if ((s.Contains("[") && s.Contains(":")) && !s.StartsWith("["))
                                {
                                    int start_pos = s.Split(':')[1].IndexOf('[');
                                    parent_input_option = s.Split(':')[1].Substring(0, start_pos);

                                    string i_val_dr = dt_cohort.Rows[0][i_dr[0]["data_field"].ToString()].ToString();

                                    if (parent_input_option.ToLower() == i_val_dr.ToLower())
                                        results += (!results.Contains(data_field) ? String.Format("{0}|{1}|invalid_pattern;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                                }
                                else if (s.StartsWith("["))
                                {
                                    string i_val_dr = dt_cohort.Rows[0][i_dr[0]["data_field"].ToString()].ToString();
                                    if (i_val_dr.ToLower() == "true")
                                        results += (!results.Contains(data_field) ? String.Format("{0}|{1}|invalid_pattern;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                                }
                            }
                        }
                        else
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|invalid_pattern;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                    }
                    else if (String.IsNullOrWhiteSpace(value) && !skipable.Contains(data_field))
                    {
                        if (dr["use_with_input_id"] == DBNull.Value || dt_input.Select(String.Format("id={0}", dr["use_with_input_id"])).Length == 0)
                            results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                        else if (dt_input.Select(String.Format("id={0}", dr["use_with_input_id"])).Length > 0)
                        {
                            DataRow[] i_dr = dt_input.Select(String.Format("id={0}", dr["use_with_input_id"]));
                            string[] a = i_dr[0]["attributes"].ToString().Split('|');
                            string parent_input_option = string.Empty;
                            foreach (string s in a)
                            {
                                if ((s.Contains("[") && s.Contains(":")) && !s.StartsWith("["))
                                {
                                    int start_block = s.Split(':')[1].IndexOf('['),
                                        end_block = s.Split(':')[1].IndexOf(']');
                                    parent_input_option = s.Split(':')[1].Substring(0, start_block);

                                    string i_val_dr = dt_cohort.Rows[0][i_dr[0]["data_field"].ToString()].ToString(),
                                        i_dr_id = s.Split(':')[1].Substring(start_block + 1, (end_block - start_block) - 1);

                                    if (parent_input_option.ToLower() == i_val_dr.ToLower() && dr["id"].ToString() == i_dr_id)
                                        results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                                }
                                else if (s.StartsWith("["))
                                {
                                    string i_val_dr = dt_cohort.Rows[0][i_dr[0]["data_field"].ToString()].ToString();
                                    if (i_val_dr.ToLower() == "true")
                                        results += (!results.Contains(data_field) ? String.Format("{0}|{1}|missing;", data_field, GetInputFieldQuestionText(sec, data_field)) : "");
                                }
                            }
                        }
                        else
                        {
                            //should be ?
                        }
                    }
                }
                con.Close();

                // enrollment table verification
                ArrayList ethnicity = new ArrayList();
                ethnicity.Add("nonhispanic");
                ethnicity.Add("hispanic");
                ethnicity.Add("unknown");

                ArrayList gender = new ArrayList();
                gender.Add("females");
                gender.Add("males");
                gender.Add("unknown");

                ArrayList race = new ArrayList();
                race.Add("ai");
                race.Add("asian");
                race.Add("pi");
                race.Add("black");
                race.Add("white");
                race.Add("multiple");
                race.Add("unknown");

                foreach (string e in ethnicity)
                {
                    foreach (string g in gender)
                    {
                        foreach (string r in race)
                        {
                            string field_name = String.Format("race_{0}_{1}_{2}", r, e, g);
                            if(dt_cohort.Rows[0][field_name] == DBNull.Value || dt_cohort.Rows[0][field_name].ToString() == "-1")
                                results += (!results.Contains(field_name) ? String.Format("{0}|{1}|missing;", field_name, GetDataTable("select label_text from tbl_web_input_fields where id=314").Rows[0]["label_text"]) : "");
                        }
                    }
                }

                // cancer table verification
                gender.Clear();
                gender.Add("male");
                gender.Add("female");
                ArrayList cancers = new ArrayList();
                using (DataTable dt = GetCancerTypes(sec))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if ((int)dr["cancer_type_id"] == 0)
                            continue;
                        cancers.Add(dr["data_field_crumb"].ToString());
                    }
                }

                foreach (string g in gender)
                {
                    foreach (string c in cancers)
                    {
                        string field_name = String.Format("ci_{0}_{1}", c, g);
                        if(dt_cohort.Rows[0][field_name] == DBNull.Value || dt_cohort.Rows[0][field_name].ToString() == "-1")
                            results += (!results.Contains(field_name) ? String.Format("{0}|{1}|missing;", field_name, GetDataTable("select label_text from tbl_web_input_fields where id=313").Rows[0]["label_text"]) : ""); 
                    }
                }

                // biospecimen table verification
                ArrayList biospecimens = new ArrayList();
                using (DataTable dt = GetBiospecimenTypes(sec))
                {
                    foreach (DataRow dr in dt.Rows)
                        biospecimens.Add(dr["data_field_crumb"].ToString());
                }

                foreach (string b in biospecimens)
                {
                    foreach (string c in cancers)
                    {
                        string field_name = String.Format("bio_{0}_{1}", c, b);
                        if (dt_cohort.Rows[0][field_name] == DBNull.Value || dt_cohort.Rows[0][field_name].ToString() == "-1")
                            results += (!results.Contains(field_name) ? String.Format("{0}|{1}|missing;", field_name, GetDataTable("select label_text from tbl_web_input_fields where id=41").Rows[0]["label_text"]) : ""); 
                    }
                }
                results = results.Trim().TrimEnd(';');

                using (CECHarmPublicService ps = new CECHarmPublicService())
                {
                    ps.AuditLog_AddActivity(sec.userid, String.Format("validated cohort record {0} " + (!String.IsNullOrEmpty(results) ? "with alerts" : "without alerts"), web_cohort_id));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }

            return srvhelp.StripHTML(results);
        }

        /// <summary>
        /// identify data field changes between a cohort record, this routine will always
        /// compare against the published version of the cohort record and returns a comma-delimited
        /// string of column names containing different responses.
        /// </summary>
        [JsonRpcMethod("getChangesInCohortRecord")]
        public string IdentifyChangesInCohortRecord(SecurityToken sec, int cohort_id, string record_status)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            SqlConnection con = new SqlConnection(database_connection_string);
            string results = string.Empty;
            try
            {
                con.Open();

                System.Data.DataTable dt_cohort = new DataTable(),
                    dt_cohort_published = new DataTable(),
                    dt_input_fields = new DataTable();

                // call stored proc to retrieve record
                SqlCommand mySelectCommand = new SqlCommand(String.Format("select * from tbl_web_cohorts_v4_0 where published=1 and [status]='published' and cohort_id={0}", cohort_id), con);
                mySelectCommand.CommandType = CommandType.Text;
                // call SqlDataAdapter
                SqlDataAdapter myAdapter = new SqlDataAdapter(mySelectCommand);
                // file the dataset
                myAdapter.Fill(dt_cohort_published);

                if (dt_cohort_published.Rows.Count == 0)
                    return results;

                mySelectCommand.CommandText = String.Format("select * from tbl_web_cohorts_v4_0 where published=0 and [status]='{0}' and cohort_id={1}", record_status.ToLower(), cohort_id);
                myAdapter.Fill(dt_cohort);

                mySelectCommand.CommandText = "select wif.type, cf.data_field from tbl_web_input_fields wif join tbl_cohort_fields cf on wif.cohort_field_id=cf.id";
                myAdapter.Fill(dt_input_fields);

                con.Close();

                string ignoreColumns = "id,cohort_acronym,published,status,status_timestamp";
                for(int i=0; i < dt_cohort.Columns.Count; i++)
                {
                    if (ignoreColumns.Contains(dt_cohort.Columns[i].ColumnName))
                        continue;

                    if (!dt_cohort.Rows[0][i].Equals(dt_cohort_published.Rows[0][i]))
                    {
                        DataRow[] dr_input_field = dt_input_fields.Select(String.Format("data_field='{0}'", dt_cohort.Columns[i].ColumnName));
                        if ((dr_input_field.Length == 1) && (dr_input_field[0]["type"].ToString() == "radio" || dr_input_field[0]["type"].ToString() == "radio_text") &&
                            dt_cohort.Rows[0][i].ToString().ToLower() == "yes" || dt_cohort.Rows[0][i].ToString().ToLower() == "no")
                        {
                            results += String.Format("{0}_{1},", dt_cohort.Columns[i].ColumnName, dt_cohort.Rows[0][i]).ToLower();
                        }
                        else
                            results += String.Format("{0},", dt_cohort.Columns[i].ColumnName);
                    }
                }
                results = results.TrimEnd(',');
            }
            catch (IndexOutOfRangeException ex)
            {
                results = string.Empty;
            }
            finally
            {
                con.Close();
            }

            return srvhelp.StripHTML(results);            
        }

        /*************************************************************************************
        * Function:		Submit Cohort Record  
        * Purpose:		Changes the status of the 'draft' record to 'pending'.         
        *
        */
        [JsonRpcMethod("submitCohortRecord")]
        public string SubmitCohortRecord(SecurityToken sec, int web_cohort_id)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            SqlConnection myConnection = new SqlConnection(database_connection_string);

            try
            {
                myConnection.Open();

                //
                // Step 1: Check if there is a 'draft' web cohort record for this Cohort ID 
                //
                SqlCommand mySelectCommand2 = new SqlCommand();
                mySelectCommand2.CommandType = CommandType.Text;
                mySelectCommand2.CommandText = "SELECT Count(id) as cohort_count FROM tbl_web_cohorts_v4_0 WHERE id = " + web_cohort_id.ToString() + " and status in ('draft','inprogress','rejected') ";
                mySelectCommand2.Connection = myConnection;
                // use scalar to get the first (only) column
                int intWebCohortCount = (int)mySelectCommand2.ExecuteScalar();

                if (intWebCohortCount > 0)
                {
                    //                    
                    // If there is, get the ID of the for this matching web cohort record
                    //
                    SqlCommand mySelectCommand3 = new SqlCommand();
                    mySelectCommand3.CommandType = CommandType.Text;
                    mySelectCommand3.CommandText = "SELECT id FROM tbl_web_cohorts_v4_0 WHERE id = " + web_cohort_id.ToString() + " and status in ('draft','inprogress','rejected')";
                    mySelectCommand3.Connection = myConnection;
                    // use scalar to get the first (only) column
                    int intPendingWebCohortId = (int)mySelectCommand3.ExecuteScalar();

                    // set it's status to 'pending' and sets 'published' to 0       
                    string strSql = "UPDATE tbl_web_cohorts_v4_0 SET status='pending', status_timestamp=GETDATE(), published=0 WHERE id = @web_cohort_id";
                    // execute the command
                    SqlCommand myUpdateCommand = new SqlCommand();
                    myUpdateCommand.CommandType = CommandType.Text;
                    myUpdateCommand.CommandText = strSql;
                    myUpdateCommand.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intPendingWebCohortId;
                    myUpdateCommand.Connection = myConnection;
                    myUpdateCommand.ExecuteNonQuery();

                    // email to researcher
                    int cohort_id = GetCohortIdByWebId(web_cohort_id);
                    System.Collections.Specialized.NameValueCollection data =
                        new NameValueCollection();
                    UserData ud = GetUserInformationByCohortId(sec, cohort_id);
                    data.Add("name", ud.display_name);
                    data.Add("to", ud.email);
                    
                    SendEmail("form_submission_researcher", data);

                    data.Clear();
                    // email to nci reviewer
                    data.Add("reviewer", "NCI Reviewer");
                    string send_to = string.Empty;
                    using(DataTable tmp_dt = GetDataTable("select email from tbl_user_accounts where access_level=200")) {
                        foreach(DataRow tmp_dr in tmp_dt.Rows)
                            send_to += String.Format("{0},", tmp_dr["email"]);
                    }
                    send_to = send_to.TrimEnd(',');
                    data.Add("to", send_to);

                    SendEmail("form_submission_reviewer", data);

                    using (CECHarmPublicService ps = new CECHarmPublicService())
                    {
                        ps.AuditLog_AddActivity(sec.userid, String.Format("cohort input form submitted, record id {0}", web_cohort_id));
                    }
                }
                else
                    throw new Exception(String.Format("error submitting cohort with web id {0}", web_cohort_id));
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                myConnection.Close();
            }

            return "success";
        }
        
        /*************************************************************************************
        * Function:		Reject Cohort Record  
        * Purpose:		Changes the status of the 'pending' record back to 'draft'.         
        *
        */
        [JsonRpcMethod("rejectCohortRecord")]
        [JsonRpcHelp("Reject Cohort record.")]
        public string RejectCohortRecord(SecurityToken sec, int web_cohort_id, string rationale)
        {
            if (!IsValidSessionId(sec))
                throw new Exception("session is invalid");

            SqlConnection myConnection = new SqlConnection(database_connection_string);

            try
            {
                myConnection.Open();

                //
                // Step 1: Check if there is a 'pending' web cohort record for this Cohort ID 
                //
                SqlCommand mySelectCommand2 = new SqlCommand();
                mySelectCommand2.CommandType = CommandType.Text;
                mySelectCommand2.CommandText = "SELECT Count(id) as cohort_count FROM tbl_web_cohorts_v4_0 WHERE id = " + web_cohort_id.ToString() + " and status = 'pending'";
                mySelectCommand2.Connection = myConnection;
                // use scalar to get the first (only) column
                int intWebCohortCount = (int)mySelectCommand2.ExecuteScalar();

                if (intWebCohortCount > 0)
                {
                    int intPendingWebCohortId = web_cohort_id;

                    // set it's status to 'draft' and sets 'published' to 0       
                    string strSql = "UPDATE tbl_web_cohorts_v4_0 SET status='rejected', status_timestamp=GETDATE(), published=0 WHERE id = @web_cohort_id";
                    // execute the command
                    SqlCommand myUpdateCommand = new SqlCommand();
                    myUpdateCommand.CommandType = CommandType.Text;
                    myUpdateCommand.CommandText = strSql;
                    myUpdateCommand.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intPendingWebCohortId;
                    myUpdateCommand.Connection = myConnection;
                    myUpdateCommand.ExecuteNonQuery();
                }
                else
                    throw new Exception("invalid web cohort id");

                DataTable user_rec = GetDataTable(String.Format("select uid from tbl_user_accounts where cohort_id={0}", GetCohortIdByWebId(web_cohort_id)));
                UserData cohort_rep = GetUserInformationByUserID(sec, (int)user_rec.Rows[0]["uid"]);

                // email to researcher & nci reviewer
                System.Collections.Specialized.NameValueCollection data =
                    new NameValueCollection();
                UserData ud = GetUserInformationByUserID(sec, sec.userid);
                data.Add("name", ud.display_name);
                data.Add("to", String.Format("{0},{1}", cohort_rep.email, ud.email));
                data.Add("rationale", rationale);
                data.Add("complete_by", DateTime.Today.AddDays(17).ToString("MMM dd, yyyy"));
                data.Add("url", Request.Url.Host);

                SendEmail("form_revisions", data);

                using (CECHarmPublicService ps = new CECHarmPublicService())
                {
                    ps.AuditLog_AddActivity(sec.userid, String.Format("cohort form rejected, record id {0}", web_cohort_id));
                }
                
                return "success";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                myConnection.Close();
            }
        }

        /*************************************************************************************
        * Function:		Publish Cohort Record  
        * Purpose:		Changes the status of the 'pending' record to 'published'.  Makes 'draft' copy of the web cohort record
        *               Also archives any previously published web cohort record.               
        *
        */
        [JsonRpcMethod("publishCohortRecord")]
        [JsonRpcHelp("Publish Cohort record.")]
        public DataSet PublishCohortRecord(SecurityToken sec, int web_cohort_id)
        {

            if (web_cohort_id == 0)
                throw new ArgumentException("web_cohort_id cannot be zero");

            bool blnValidSession;
            int intWebCohortId = web_cohort_id,
                intCohortId = GetCohortIdByWebId(web_cohort_id);

            // create dataset
            DataSet ds = new DataSet();

            // set connections            
            SqlConnection myProdConnection = new SqlConnection(database_connection_string);

            // open the connections            
            myProdConnection.Open();

            // check for valid session_id and user_id
            blnValidSession = IsValidSessionId(sec);

            // deal with result
            if (blnValidSession)
            {

                //
                // Step 1: Check if there is a published web cohort record for this Cohort ID 
                //
                SqlCommand mySelectCommand2 = new SqlCommand();
                mySelectCommand2.CommandType = CommandType.Text;
                mySelectCommand2.CommandText = "SELECT Count(id) as cohort_count FROM tbl_web_cohorts_v4_0 WHERE cohort_id = " + intCohortId.ToString() + " and status = 'published'";
                mySelectCommand2.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intCohortId;
                mySelectCommand2.Connection = myProdConnection;
                // use scalar to get the first (only) column
                int intWebCohortCount = (int)mySelectCommand2.ExecuteScalar();

                if (intWebCohortCount > 0)
                {
                    //                    
                    // If there is, get the ID of the for this matching web cohort record
                    //
                    SqlCommand mySelectCommand3 = new SqlCommand();
                    mySelectCommand3.CommandType = CommandType.Text;
                    mySelectCommand3.CommandText = "SELECT id FROM tbl_web_cohorts_v4_0 WHERE cohort_id = " + intCohortId.ToString() + " and status = 'published'";
                    mySelectCommand3.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intCohortId;
                    mySelectCommand3.Connection = myProdConnection;
                    // use scalar to get the first (only) column
                    int intArchiveWebCohortId = (int)mySelectCommand3.ExecuteScalar();

                    // set it's status to 'archived' and sets 'published' to 0       
                    string strSql = "UPDATE tbl_web_cohorts_v4_0 SET status='archived', status_timestamp=GETDATE(), published=0 WHERE id = @web_cohort_id";
                    // execute the command
                    SqlCommand myUpdateCommand = new SqlCommand();
                    myUpdateCommand.CommandType = CommandType.Text;
                    myUpdateCommand.CommandText = strSql;
                    myUpdateCommand.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intArchiveWebCohortId;
                    myUpdateCommand.Connection = myProdConnection;
                    myUpdateCommand.ExecuteNonQuery();
                }

                //
                // Step 2: Check if there is a 'pending' web cohort record for this Cohort ID
                //
                SqlCommand mySelectCommand4 = new SqlCommand();
                mySelectCommand4.CommandType = CommandType.Text;
                mySelectCommand4.CommandText = "SELECT Count(id) as cohort_count FROM tbl_web_cohorts_v4_0 WHERE cohort_id = " + intCohortId.ToString() + " and status = 'pending'";
                mySelectCommand4.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intCohortId;
                mySelectCommand4.Connection = myProdConnection;
                // use scalar to get the first (only) column
                intWebCohortCount = (int)mySelectCommand4.ExecuteScalar();

                if (intWebCohortCount > 0)
                {
                    //                    
                    // If there is, get the ID of the for this matching web cohort record 
                    //
                    SqlCommand mySelectCommand5 = new SqlCommand();
                    mySelectCommand5.CommandType = CommandType.Text;
                    mySelectCommand5.CommandText = "SELECT id FROM tbl_web_cohorts_v4_0 WHERE cohort_id = " + intCohortId.ToString() + " and status = 'pending'";
                    mySelectCommand5.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intCohortId;
                    mySelectCommand5.Connection = myProdConnection;
                    // use scalar to get the first (only) column
                    intWebCohortId = (int)mySelectCommand5.ExecuteScalar();

                    // set it's status to 'published' and set published=1           
                    string strSql = "UPDATE tbl_web_cohorts_v4_0 SET status='published', status_timestamp=GETDATE(), published=1 WHERE id = @web_cohort_id";
                    // execute the command
                    SqlCommand myUpdateCommand2 = new SqlCommand();
                    myUpdateCommand2.CommandType = CommandType.Text;
                    myUpdateCommand2.CommandText = strSql;
                    myUpdateCommand2.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intWebCohortId;
                    myUpdateCommand2.Connection = myProdConnection;
                    myUpdateCommand2.ExecuteNonQuery();


                    //
                    // Step 3: Copy the 'published' record to a new 'draft' record
                    //

                    // call SQL stored proc to clone a web cohort record
                    SqlCommand mySelectCommand6 = new SqlCommand();
                    mySelectCommand6.CommandType = CommandType.StoredProcedure;
                    mySelectCommand6.CommandText = "fsp_clone_web_cohort_record";
                    mySelectCommand6.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intWebCohortId;
                    mySelectCommand6.Parameters.Add("@newId", SqlDbType.Int).Direction = ParameterDirection.Output;
                    mySelectCommand6.Connection = myProdConnection;
                    mySelectCommand6.ExecuteNonQuery();
                    // retrieve the return value
                    int intNewWebCohortId = (int)mySelectCommand6.Parameters["@newId"].Value;

                    // set it's status to 'draft' and published=0                  
                    string strSql2 = "UPDATE tbl_web_cohorts_v4_0 SET status='draft', published=0, status_timestamp=GETDATE() WHERE id = @web_cohort_id";
                    // execute the command
                    SqlCommand myUpdateCommand3 = new SqlCommand();
                    myUpdateCommand3.CommandType = CommandType.Text;
                    myUpdateCommand3.CommandText = strSql2;
                    myUpdateCommand3.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intNewWebCohortId;
                    myUpdateCommand3.Connection = myProdConnection;
                    myUpdateCommand3.ExecuteNonQuery();


                    /*
                    //
                    // Step 4: Copy the web cohort status record 
                    //

                    // get the metadata flags on for the original web cohort record
                    SqlCommand mySelectCommand7 = new SqlCommand();
                    mySelectCommand7.CommandType = CommandType.StoredProcedure;
                    mySelectCommand7.CommandText = "fsp_get_web_cohort_meta_record";
                    mySelectCommand7.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intWebCohortId;
                    mySelectCommand7.Connection = myProdConnection;

                    // use reader to get the values
                    SqlDataReader metadataReader = mySelectCommand7.ExecuteReader();

                    // set default values                                
                    bool blnQuestionnairePending = true;
                    bool blnPoliciesPending = true;
                    bool blnPublicationsPending = true;
                    bool blnGrantsPending = true;
                    bool blnProtocolsPending = true;

                    // read the rows
                    if (metadataReader.HasRows)
                    {
                        while (metadataReader.Read())
                        {
                            // read the values, checking for nulls
                            if (metadataReader["questionnaire_pending"] != DBNull.Value)
                            {
                                blnQuestionnairePending = metadataReader.GetBoolean(0);
                            }
                            if (metadataReader["policies_pending"] != DBNull.Value)
                            {
                                blnPoliciesPending = metadataReader.GetBoolean(1);
                            }
                            if (metadataReader["publications_pending"] != DBNull.Value)
                            {
                                blnPublicationsPending = metadataReader.GetBoolean(2);
                            }
                            if (metadataReader["grants_pending"] != DBNull.Value)
                            {
                                blnGrantsPending = metadataReader.GetBoolean(3);
                            }
                            if (metadataReader["protocols_pending"] != DBNull.Value)
                            {
                                blnProtocolsPending = metadataReader.GetBoolean(4);
                            }
                        }
                    }

                    // close the data reader
                    metadataReader.Close();

                    // append a new web cohort meta record -- PROD database
                    SqlCommand myUpdateStatusCommand3 = new SqlCommand();
                    myUpdateStatusCommand3.CommandType = CommandType.StoredProcedure;
                    myUpdateStatusCommand3.CommandText = "fsp_append_web_cohort_meta_record";
                    myUpdateStatusCommand3.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intNewWebCohortId;
                    myUpdateStatusCommand3.Parameters.Add("@questionnaire_pending", SqlDbType.Bit).Value = blnQuestionnairePending;
                    myUpdateStatusCommand3.Parameters.Add("@policies_pending", SqlDbType.Bit).Value = blnPoliciesPending;
                    myUpdateStatusCommand3.Parameters.Add("@publications_pending", SqlDbType.Bit).Value = blnPublicationsPending;
                    myUpdateStatusCommand3.Parameters.Add("@grants_pending", SqlDbType.Bit).Value = blnGrantsPending;
                    myUpdateStatusCommand3.Parameters.Add("@protocols_pending", SqlDbType.Bit).Value = blnProtocolsPending;
                    myUpdateStatusCommand3.Connection = myProdConnection;
                    myUpdateStatusCommand3.ExecuteNonQuery();
                    */


                    //
                    // Step 5: copy any attachements to the new record
                    //                    

                    // create destination connection
                    System.Data.SqlClient.SqlConnection destCon = new SqlConnection(this.database_connection_string);
                    destCon.Open();

                    // build source sql
                    string sourceSql = String.Format("SELECT document_type_id, file_name, file_size, file_date, file_contents, attachment_type, url FROM tbl_web_cohort_attachments WHERE web_cohort_id={0}", intWebCohortId);

                    SqlCommand mySelectCommand8 = new SqlCommand();
                    mySelectCommand8.CommandType = CommandType.Text;
                    mySelectCommand8.CommandText = sourceSql;
                    mySelectCommand8.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intWebCohortId;
                    mySelectCommand8.Connection = myProdConnection;

                    System.Data.SqlClient.SqlDataReader srcRdr = mySelectCommand8.ExecuteReader();
                    while (srcRdr.Read())
                    {
                        System.Data.SqlClient.SqlCommand destCom =
                            new SqlCommand("fsp_append_web_attachment_record", destCon);

                        destCom.CommandType = CommandType.StoredProcedure;
                        destCom.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intNewWebCohortId;
                        destCom.Parameters.Add("@document_type_id", SqlDbType.Int).Value = srcRdr["document_type_id"];
                        destCom.Parameters.Add("@file_name", SqlDbType.NVarChar).Value = srcRdr["file_name"];
                        destCom.Parameters.Add("@file_size", SqlDbType.Int).Value = srcRdr["file_size"];
                        destCom.Parameters.Add("@file_date", SqlDbType.Date).Value = srcRdr["file_date"];
                        destCom.Parameters.Add("@file_content", SqlDbType.VarBinary).Value = srcRdr["file_contents"];
                        destCom.Parameters.Add("@attachment_type", SqlDbType.VarChar).Value = srcRdr["attachment_type"];
                        destCom.Parameters.Add("@url", SqlDbType.NVarChar).Value = srcRdr["url"];

                        destCom.ExecuteNonQuery();
                    }
                    // close the destination connection
                    destCon.Close();
                    // close the soure reader
                    srcRdr.Close();


                    // use command object to retrieve the success result code -- DEV database
                    SqlCommand myResultCommand = new SqlCommand();
                    myResultCommand.CommandType = CommandType.StoredProcedure;
                    myResultCommand.CommandText = "fsp_get_result_code";
                    myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = 100; // Success!
                    myResultCommand.Connection = myProdConnection;
                    // call SqlDataAdapter
                    SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                    // add the result codes table to dataaset
                    myResultAdapter.Fill(ds, "tbl_result_codes");

                    DataTable user_rec = GetDataTable(String.Format("select uid from tbl_user_accounts where cohort_id={0}", GetCohortIdByWebId(web_cohort_id)));
                    UserData cohort_rep = GetUserInformationByUserID(sec, (int)user_rec.Rows[0]["uid"]);

                    // email to researcher & nci reviewer
                    System.Collections.Specialized.NameValueCollection data =
                        new NameValueCollection();
                    UserData ud = GetUserInformationByUserID(sec, sec.userid);
                    data.Add("name", cohort_rep.display_name);
                    data.Add("to", cohort_rep.email);
                    data.Add("url", Request.Url.Host);

                    SendEmail("form_published", data);
                }
                else
                {
                    // use command object to retrieve result code
                    SqlCommand myResultCommand = new SqlCommand();
                    myResultCommand.CommandType = CommandType.StoredProcedure;
                    myResultCommand.CommandText = "fsp_get_result_code";
                    myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = -204; // Invalid Web Cohort ID
                    myResultCommand.Connection = myProdConnection;
                    // call SqlDataAdapter
                    SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                    // add the result codes table to dataaset
                    myResultAdapter.Fill(ds, "tbl_result_codes");
                }
            }
            else
            {
                // use command object to retrieve result code 
                SqlCommand myResultCommand = new SqlCommand();
                myResultCommand.CommandType = CommandType.StoredProcedure;
                myResultCommand.CommandText = "fsp_get_result_code";
                myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = -101; // Invalid User / Session ID
                myResultCommand.Connection = myProdConnection;
                // call SqlDataAdapter
                SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                // add the result codes table to dataaset
                myResultAdapter.Fill(ds, "tbl_result_codes");
            }


            // close the connections            
            myProdConnection.Close();

            using (CECHarmPublicService ps = new CECHarmPublicService())
            {
                ps.AuditLog_AddActivity(sec.userid, String.Format("cohort form published, cohort id {0}", intCohortId));
            }

            // return data set
            return ds;
        }

        /*************************************************************************************
        * Function:		Unpublish Cohort Record  
        * Purpose:		Changes the status of the 'published' record to 'unpublished'.         
        *
        */
        [JsonRpcMethod("unpublishCohortRecord")]
        [JsonRpcHelp("Unpublish Cohort record.")]
        public DataSet UnpublishCohortRecord(SecurityToken sec, int intCohortId)
        {
            bool blnValidSession;

            // create dataset
            DataSet ds = new DataSet();

            // set connections            
            SqlConnection myProdConnection = new SqlConnection(database_connection_string);

            // open the connections            
            myProdConnection.Open();

            // check for valid session_id and user_id
            blnValidSession = IsValidSessionId(sec);

            // deal with result
            if (blnValidSession)
            {

                //
                // Step 1: Check if there is a 'published' web cohort record for this Cohort ID 
                //
                SqlCommand mySelectCommand2 = new SqlCommand();
                mySelectCommand2.CommandType = CommandType.Text;
                mySelectCommand2.CommandText = "SELECT Count(id) as cohort_count FROM tbl_web_cohorts_v4_0 WHERE cohort_id = " + intCohortId.ToString() + " and status = 'published'";
                mySelectCommand2.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intCohortId;
                mySelectCommand2.Connection = myProdConnection;
                // use scalar to get the first (only) column
                int intWebCohortCount = (int)mySelectCommand2.ExecuteScalar();

                if (intWebCohortCount > 0)
                {
                    //                    
                    // If there is, get the ID of the for this matching web cohort record
                    //
                    SqlCommand mySelectCommand3 = new SqlCommand();
                    mySelectCommand3.CommandType = CommandType.Text;
                    mySelectCommand3.CommandText = "SELECT id FROM tbl_web_cohorts_v4_0 WHERE cohort_id = " + intCohortId.ToString() + " and status = 'published'";
                    mySelectCommand3.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intCohortId;
                    mySelectCommand3.Connection = myProdConnection;
                    // use scalar to get the first (only) column
                    int intPublishedWebCohortId = (int)mySelectCommand3.ExecuteScalar();

                    // set it's status to 'unpublished' and sets 'published' to 0       
                    string strSql = "UPDATE tbl_web_cohorts_v4_0 SET status='unpublished', status_timestamp=GETDATE(), published=0 WHERE id = @web_cohort_id";
                    // execute the command
                    SqlCommand myUpdateCommand = new SqlCommand();
                    myUpdateCommand.CommandType = CommandType.Text;
                    myUpdateCommand.CommandText = strSql;
                    myUpdateCommand.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intPublishedWebCohortId;
                    myUpdateCommand.Connection = myProdConnection;
                    myUpdateCommand.ExecuteNonQuery();

                    // use command object to retrieve the success result code
                    SqlCommand myResultCommand = new SqlCommand();
                    myResultCommand.CommandType = CommandType.StoredProcedure;
                    myResultCommand.CommandText = "fsp_get_result_code";
                    myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = 100; // Success!
                    myResultCommand.Connection = myProdConnection;
                    // call SqlDataAdapter
                    SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                    // add the result codes table to dataaset
                    myResultAdapter.Fill(ds, "tbl_result_codes");
                }
                else
                {
                    // use command object to retrieve result code
                    SqlCommand myResultCommand = new SqlCommand();
                    myResultCommand.CommandType = CommandType.StoredProcedure;
                    myResultCommand.CommandText = "fsp_get_result_code";
                    myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = -204; // Invalid Cohort ID
                    myResultCommand.Connection = myProdConnection;
                    // call SqlDataAdapter
                    SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                    // add the result codes table to dataaset
                    myResultAdapter.Fill(ds, "tbl_result_codes");
                }
            }
            else
            {
                // use command object to retrieve result code 
                SqlCommand myResultCommand = new SqlCommand();
                myResultCommand.CommandType = CommandType.StoredProcedure;
                myResultCommand.CommandText = "fsp_get_result_code";
                myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = -101; // Invalid User / Session ID
                myResultCommand.Connection = myProdConnection;
                // call SqlDataAdapter
                SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                // add the result codes table to dataaset
                myResultAdapter.Fill(ds, "tbl_result_codes");
            }


            // close the connections            
            myProdConnection.Close();

            using (CECHarmPublicService ps = new CECHarmPublicService())
            {
                ps.AuditLog_AddActivity(sec.userid, String.Format("cohort form unpublished, cohort id {0}", intCohortId));
            }

            // return data set
            return ds;
        }

        /*************************************************************************************
        * Function:		Delete Cohort Record
        * Purpose:		Permanently deletes a cohort from the database -- not finished 
        *                
        *
        */
        [JsonRpcMethod("deleteCohortRecord")]
        [JsonRpcHelp("Delete Cohort record.")]
        public DataSet DeleteCohortRecord(SecurityToken sec, int intCohortId)
        {
            throw new NotImplementedException("not implemented");
            /*bool blnValidSession;
            int intWebCohortId = 0;

            // create dataset
            DataSet ds = new DataSet();

            // set connections            
            SqlConnection myProdConnection = new SqlConnection(database_connection_string);

            // open the connections            
            myProdConnection.Open();

            // check for valid session_id and user_id
            blnValidSession = IsValidSessionId(sec);

            // deal with result
            if (blnValidSession)
            {
                //
                // Step 1: Check if there is an 'unpublished' cohort record for this Cohort ID
                //
                SqlCommand mySelectCommand2 = new SqlCommand();
                mySelectCommand2.CommandType = CommandType.Text;
                mySelectCommand2.CommandText = "SELECT Count(id) as cohort_count FROM tbl_web_cohorts_v4_0 WHERE cohort_id = " + intCohortId.ToString() + " and status = 'unpublished'";
                mySelectCommand2.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intCohortId;
                mySelectCommand2.Connection = myProdConnection;
                // use scalar to get the first (only) column
                int intWebCohortCount = (int)mySelectCommand2.ExecuteScalar();


                if (intWebCohortCount > 0)
                {

                    // create temp_ds dataset
                    DataSet temp_ds = new DataSet();

                    // 
                    // Step 2: Get all of the matching web cohort ids
                    //
                    SqlCommand mySelectAllIds = new SqlCommand();
                    mySelectAllIds.CommandType = CommandType.Text;
                    mySelectAllIds.CommandText = "SELECT id FROM tbl_web_cohorts_v4_0 WHERE cohort_id = " + intCohortId.ToString() + "";
                    mySelectAllIds.Parameters.Add("@cohort_id", SqlDbType.Int).Value = intCohortId;
                    mySelectAllIds.Connection = myProdConnection;

                    // use SqlDataAdapter to get the ids
                    SqlDataAdapter mySelectAllIdsDataAdapter = new SqlDataAdapter(mySelectAllIds);

                    // insert into temp table
                    mySelectAllIdsDataAdapter.Fill(temp_ds, "temp_table");

                    // open data table -- note there is only one table
                    DataTable myDataTable = temp_ds.Tables[0];

                    // check the number of rows in data table
                    if (myDataTable.Rows.Count > 0)
                    {
                        // loop thru the rows
                        for (int i = 0; i < myDataTable.Rows.Count; i++)
                        {
                            // read the row
                            DataRow myDataRow = myDataTable.Rows[i];

                            // read teh web cohort id
                            intWebCohortId = (int)myDataRow["id"];

                            //
                            // step 2a: delete any web cohort attachments
                            //                       
                            SqlCommand myDeleteCommand5 = new SqlCommand("fsp_delete_web_attachments_by_web_cohort_id", myProdConnection);
                            myDeleteCommand5.CommandType = CommandType.StoredProcedure;
                            myDeleteCommand5.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intWebCohortId;
                            myDeleteCommand5.Connection = myProdConnection;
                            myDeleteCommand5.ExecuteNonQuery();

                            //
                            // Step 4b: delete any web cohort metadata records
                            //                       
                            SqlCommand myDeleteCommand4 = new SqlCommand("fsp_delete_web_cohort_metadata_by_web_cohort_id", myProdConnection);
                            myDeleteCommand4.CommandType = CommandType.StoredProcedure;
                            myDeleteCommand4.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intWebCohortId;
                            myDeleteCommand4.Connection = myProdConnection;
                            myDeleteCommand4.ExecuteNonQuery();


                            //
                            // Step 4c: delete any existing web cohort record with this cohort id
                            //

                            // build SQL string to delete the web cohort record
                            string strDeleteSql = "DELETE FROM tbl_web_cohorts_v4_0 WHERE id = @web_cohort_id";
                            SqlCommand myDeleteCommand = new SqlCommand();
                            myDeleteCommand.CommandType = CommandType.Text;
                            myDeleteCommand.CommandText = strDeleteSql;
                            myDeleteCommand.Parameters.Add("@web_cohort_id", SqlDbType.Int).Value = intWebCohortId;
                            myDeleteCommand.Connection = myProdConnection;
                            myDeleteCommand.ExecuteNonQuery();
                        }
                    }


                    // use command object to retrieve the success result code -- DEV dataabse
                    SqlCommand myResultCommand = new SqlCommand();
                    myResultCommand.CommandType = CommandType.StoredProcedure;
                    myResultCommand.CommandText = "fsp_get_result_code";
                    myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = 100; // Success!
                    myResultCommand.Connection = myProdConnection;
                    // call SqlDataAdapter
                    SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                    // add the result codes table to dataaset
                    myResultAdapter.Fill(ds, "tbl_result_codes");

                }
                else // error: no unpublished record for this cohort id
                {
                    // use command object to retrieve result code
                    SqlCommand myResultCommand = new SqlCommand();
                    myResultCommand.CommandType = CommandType.StoredProcedure;
                    myResultCommand.CommandText = "fsp_get_result_code";
                    myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = -217; // No unpublished record for this Cohort ID
                    myResultCommand.Connection = myProdConnection;
                    // call SqlDataAdapter
                    SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                    // add the result codes table to dataaset
                    myResultAdapter.Fill(ds, "tbl_result_codes");
                }
            }
            else
            {
                // use command object to retrieve result code
                SqlCommand myResultCommand = new SqlCommand();
                myResultCommand.CommandType = CommandType.StoredProcedure;
                myResultCommand.CommandText = "fsp_get_result_code";
                myResultCommand.Parameters.Add("@result_code", SqlDbType.Int).Value = -101; // Invalid User / Session ID
                myResultCommand.Connection = myProdConnection;
                // call SqlDataAdapter
                SqlDataAdapter myResultAdapter = new SqlDataAdapter(myResultCommand);
                // add the result codes table to dataaset
                myResultAdapter.Fill(ds, "tbl_result_codes");
            }

            // close the connections            
            myProdConnection.Close();

            // return data set
            return ds;*/
        }

    }
}