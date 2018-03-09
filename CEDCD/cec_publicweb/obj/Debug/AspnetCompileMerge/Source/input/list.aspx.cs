using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;


namespace cec_publicweb.input
{
    using cec_publicservice;


    public partial class List_Cohorts : System.Web.UI.Page
    {
        protected readonly string[] status_tabs = { "pending", "published", "unpublished" };
        protected readonly string[] admin_tabs = {  "reports", "users" };
        protected readonly string[] reports = { "Review Process Status", "Draft Status", "Published Listing", "Unpublished Listing", "All Cohort Data" };

        protected DataTable dt_cohorts, dt_users;

        //private System.Data.DataTable dt_input, dt_cohort;
        private cec_publicservice.CECInputFormService CECWebSrv;

        #region Properties

        /// <summary>
        /// get the security token for the given user
        /// </summary>
        protected SecurityToken UserToken
        {
            get
            {
                if (Session["UserSecurityToken"] == null)
                    return new SecurityToken();

                return (SecurityToken)Session["UserSecurityToken"];
            }
        }

        protected string TabName
        {
            get;
            set;
        }

        protected SortDirection SortOrder
        {
            get
            {
                if (ViewState["listSortOrder"] != null)
                    return (SortDirection)ViewState["listSortOrder"];
                else
                {
                    SortOrder = SortDirection.Descending;
                    return SortOrder;
                }
            }
            set
            {
                ViewState["listSortOrder"] = (SortDirection)value;
            }
        }

        protected string SortColumn
        {
            get
            {
                if (ViewState["listSortColumn"] != null)
                    return ViewState["listSortColumn"].ToString();
                else
                {
                    if (String.IsNullOrWhiteSpace(TabName) || Array.IndexOf(status_tabs, TabName) > -1)
                        SortColumn = "status";
                    else if (TabName == "users")
                        SortColumn = "username";
                    else
                        SortColumn = "status";
                    return SortColumn;
                }
            }
            set
            {
                ViewState["listSortColumn"] = value;
            }
        }

        protected string SearchTerm
        {
            get
            {
                if (ViewState["listSearchTerm"] != null)
                    return ViewState["listSearchTerm"].ToString();
                else
                {
                    SearchTerm = string.Empty;
                    return SearchTerm;
                }
            }
            set
            {
                ViewState["listSearchTerm"] = value;
            }
        }

        #endregion

        #region Private Routines
        private void RegisterJSAlert(string text)
        {
            string literalStr =
                    String.Format("<div class='modal' tabindex='-1' id='alertModal' role='alertdialog' aria-labeledby='alertTitle' aria-describedby='alertMsg'><div class='modal-dialog' role=\"document\"><div class='modal-content'><div class='modal-header'><h4 id='alertTitle' class='modal-title'>Alert</h4></div> " +
                    "<div id='alertMsg' class='modal-body' aria-atomic='true'>{0}</div><div class='modal-footer'><div class='pull-right'><button type='button' id='modalAlertClose' class='btn btn-default' data-dismiss='modal'>Close</button></div></div><div class='modal-footer'></div></div></div></div>", text);

            ClientScript.RegisterClientScriptBlock(GetType(), "alert", literalStr + " <script type='text/javascript'>$('#alertModal').modal({backdrop:'static', keyboard:true, show:true}); $('#modalAlertClose').focus();</script>");
        }

        private void RegisterJSError(string text)
        {
            string literalStr =
                    String.Format("<div class='modal' tabindex='-1' id='alertModal' role='alertdialog' aria-labeledby='alertTitle' aria-describedby='alertMsg'><div class='modal-dialog' role=\"document\"><div class='modal-content'><div class='modal-header'><h4 id='alertTitle' class='modal-title'>Alert</h4></div> " +
                    "<div id='alertMsg' class='modal-body' aria-atomic='true'>{0}<div class='pull-right'><button type='button' id='modalAlertClose' class='btn btn-default' data-dismiss='modal'>Close</button></div></div><div class='modal-footer'></div></div></div></div>", text);

            ClientScript.RegisterClientScriptBlock(GetType(), "error", literalStr + " <script type='text/javascript'>$('#alertModal').modal({backdrop:'static', keyboard:true, show:true}); $('#modalAlertClose').focus();</script>");
        }

        private void LogError(string text)
        {
            string logName = String.Format("messages_{0}.log", DateTime.Today.ToString("dd-MM-yyyy"));

            helper h = new helper();
            h.WriteToLog(text, MapPath(String.Format("/tmp/{0}", logName)));
        }

        private void LogError(string text, Exception ex)
        {
            string logName = String.Format("messages_{0}.log", DateTime.Today.ToString("dd-MM-yyyy"));
            string fullText = String.Format("{1}{0}{2}{0}", Environment.NewLine, text, ex.Message);
            fullText += ex.StackTrace + Environment.NewLine + Environment.NewLine;

            helper h = new helper();
            h.WriteToLog(fullText, MapPath(String.Format("/tmp/{0}", logName)));
        }

        private string SteralizeValueCode(string value_code)
        {
            return Regex.Replace(value_code, @"\W\s", string.Empty);
        }

        private string GetSortString(SortDirection sortDir)
        {
            if (sortDir == SortDirection.Descending)
                return "DESC";
            else
                return "ASC";
        }

        private string GenerateExcelReport(int report, string savePath)
        {
            /// excel writer row index
            int rowIndex = 0;

            try
            {
                NPOI.XSSF.UserModel.XSSFWorkbook workbook = 
                    new XSSFWorkbook();
                NPOI.SS.UserModel.ISheet wkst = workbook.CreateSheet();
                workbook.SetActiveSheet(0);

                // sets up basic styles for Excel
                NPOI.SS.UserModel.ICellStyle HeaderCellStyle = workbook.CreateCellStyle();
                HeaderCellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                HeaderCellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                HeaderCellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                HeaderCellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                HeaderCellStyle.VerticalAlignment = VerticalAlignment.Center;
                HeaderCellStyle.Alignment = HorizontalAlignment.Center;
                HeaderCellStyle.FillPattern = FillPattern.SolidForeground;
                HeaderCellStyle.FillForegroundColor = IndexedColors.LightCornflowerBlue.Index;
                IFont headerFont = workbook.CreateFont();
                headerFont.Boldweight = (short)FontBoldWeight.Bold;
                HeaderCellStyle.SetFont(headerFont);

                NPOI.SS.UserModel.ICellStyle DataCellStyle = workbook.CreateCellStyle();
                DataCellStyle.VerticalAlignment = VerticalAlignment.Top;
                DataCellStyle.WrapText = true;

                ArrayList query_for_status = new ArrayList();
                string query_columns = string.Empty,
                    ignore_columns = "id,cohort_id,status_timestamp,published",
                    column_headers = string.Empty;
                switch (report)
                {
                    case 0:
                        query_columns = "id,cohort_id,cohort_acronym,cohort_name,pi_name_1,pi_institution_1,pi_email_1,status,status_timestamp";
                        query_for_status.Add("pending");
                        query_for_status.Add("rejected");
                        query_for_status.Add("inprogress");
                        ignore_columns = "id,cohort_id";
                        column_headers = "Cohort Abbreviation,Cohort,Cohort PI,Institution,Cohort Contact Email,Date CEDCD Form Received,CEDCD Form Reviewed,Date CEDCD Form Published";
                        break;
                    case 1:
                        query_columns = "cohort_acronym,cohort_name,pi_name_1,pi_institution_1,pi_email_1,status,status_timestamp";
                        query_for_status.Add("pending");
                        query_for_status.Add("rejected");
                        query_for_status.Add("inprogress");
                        ignore_columns = string.Empty;
                        column_headers = "Cohort Abbreviation,Cohort,Cohort PI,Institution,Cohort Contact Email,Current Status,Current Status Date";

                        break;
                    case 2:
                        query_columns = "cohort_acronym,cohort_name,pi_name_1,pi_institution_1,pi_email_1,status_timestamp";
                        query_for_status.Add("published");
                        ignore_columns = string.Empty;
                        column_headers = "Cohort Abbreviation,Cohort,Cohort PI,Institution,Cohort Contact Email,Date CEDCD Form Published";
                        break;
                    case 3:
                        query_columns = "cohort_acronym,cohort_name,pi_name_1,pi_institution_1,pi_email_1,status_timestamp [Date CEDCD Form was Unpublished]";
                        query_for_status.Add("unpublished");
                        ignore_columns = string.Empty;
                        column_headers = "Cohort Abbreviation,Cohort,Cohort PI,Institution,Cohort Contact Email,Date CEDCD Form Unpublished";
                        break;
                    case 4:
                        query_columns = "*";
                        query_for_status.Add("published");
                        query_for_status.Add("pending");
                        break;
                    default:
                        query_columns = "*";
                        query_for_status.Add("published");
                        break;
                }
                DataTable dt_records = CECWebSrv.GetCohortsWithStatusesWithColumns(UserToken, (string[])query_for_status.ToArray(typeof(string)), query_columns);
                
                /// write header to excel
                ///                
                NPOI.SS.UserModel.IRow dataRow = wkst.CreateRow(rowIndex++);
                //dataRow.CreateCell(0).SetCellValue(String.Format("Cohort Data Export Generated from the CEDCD Website ({0})", Request.Url.Authority));

                //dataRow = wkst.CreateRow(rowIndex++);
                //dataRow.CreateCell(0).SetCellValue("Export Date:");
                //dataRow.CreateCell(1).SetCellValue(DateTime.Now.ToString("MM/dd/yyyy"));
                //rowIndex += 2;

                int colPos = 0;
                ///--------------------------------------------------------
                /// column headers
                /// 
                NPOI.SS.UserModel.IRow headerRow = wkst.CreateRow(rowIndex++);
                if (column_headers != string.Empty)
                {
                    foreach (string s in column_headers.Split(','))
                    {
                        ICell c = headerRow.GetCell(colPos++, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        c.SetCellValue(s);
                        c.CellStyle = HeaderCellStyle;
                    }
                }
                else
                {
                    using (CECHarmPublicService ps = new CECHarmPublicService())
                    {
                        for (int _c = 0; _c < dt_records.Columns.Count; _c++)
                        {
                            if (ignore_columns.Contains(dt_records.Columns[_c].ColumnName))
                                continue;

                            ICell c = headerRow.GetCell(colPos);
                            if (c == null)
                                c = headerRow.CreateCell(colPos);

                            string column_name = ps.GetCohortWebFieldLabelByColumnName(UserToken, dt_records.Columns[_c].ColumnName);
                            if (String.IsNullOrWhiteSpace(column_name))
                                column_name = dt_records.Columns[_c].ColumnName;

                            c.SetCellValue(column_name);
                            c.CellStyle = HeaderCellStyle;

                            colPos++;
                        }
                    }
                }

                if (report != 0 && dt_records.Rows.Count >= 1)
                {
                    /// data rows
                    for (int _i = 0; _i < dt_records.Rows.Count; _i++)
                    {
                        colPos = 0;

                        /// create data row object then step through each cell to populate the excel row
                        dataRow = wkst.CreateRow(rowIndex++);
                        for (int _p = 0; _p < dt_records.Columns.Count; _p++)
                        {
                            if (ignore_columns.Contains(dt_records.Columns[_p].ColumnName))
                                continue;

                            /// get first cell and check for null, if null create cell
                            ICell c = dataRow.GetCell(colPos);
                            if (c == null)
                                c = dataRow.CreateCell(colPos);

                            string cellVal = dt_records.Rows[_i][_p].ToString();
                            if (helper.IsStringEmptyWhiteSpace(cellVal) || cellVal == "&nbsp;" || cellVal == "-1")
                            {
                                cellVal = "N/P";
                                c.SetCellValue(cellVal);
                            }
                            else if (dt_records.Columns[_p].DataType == typeof(DateTime))
                                c.SetCellValue(DateTime.Parse(cellVal).ToString("MM/dd/yyyy"));
                            else if (dt_records.Columns[_p].ColumnName == "status")
                            {
                                switch (cellVal)
                                {
                                    case "inprogress":
                                        c.SetCellValue("Draft In Progress");
                                        break;
                                    case "pending":
                                        c.SetCellValue("Under NCI Review");
                                        break;
                                    case "rejected":
                                        c.SetCellValue("Returned to Cohort");
                                        break;
                                    default:
                                        c.SetCellValue(cellVal);
                                        break;
                                }
                            }
                            else
                                c.SetCellValue(cellVal);
                            c.CellStyle = DataCellStyle;

                            colPos++;
                        }
                    }
                }
                else if (report == 0 && dt_records.Rows.Count >= 1)
                {
                    foreach (DataRow dr in dt_records.Rows)
                    {
                        dataRow = wkst.CreateRow(rowIndex++);
                        
                        // cohort acronym
                        ICell c = dataRow.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        c.SetCellValue(dr["cohort_acronym"].ToString());
                        c.CellStyle = DataCellStyle;
                        // cohort name
                        c = dataRow.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        c.SetCellValue(dr["cohort_name"].ToString());
                        c.CellStyle = DataCellStyle;
                        // pi name
                        c = dataRow.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        c.SetCellValue(dr["pi_name_1"].ToString());
                        c.CellStyle = DataCellStyle;
                        // pi institution
                        c = dataRow.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        c.SetCellValue(dr["pi_institution_1"].ToString());
                        c.CellStyle = DataCellStyle;
                        // pi email
                        c = dataRow.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        c.SetCellValue(dr["pi_email_1"].ToString());
                        c.CellStyle = DataCellStyle;
                        // the more complicated stuff...
                        if(dr["status"].ToString() == "pending") 
                        {
                            c = dataRow.GetCell(5, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                            c.SetCellValue(((DateTime)dr["status_timestamp"]).ToString("MM/dd/yyyy"));
                            c.CellStyle = DataCellStyle;

                            c = dataRow.GetCell(6, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                            c.SetCellValue("No");
                            c.CellStyle = DataCellStyle;

                            using (DataTable dt_temp = CECWebSrv.GetCohortRecordById(UserToken, (int)dr["cohort_id"], false))
                            {
                                if (dt_temp.Rows.Count > 0)
                                {
                                    c = dataRow.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                                    c.SetCellValue(((DateTime)dt_temp.Rows[0]["status_timestamp"]).ToString("MM/dd/yyyy"));
                                    c.CellStyle = DataCellStyle;
                                }
                            }
                        }
                        else if (dr["status"].ToString() == "rejected" || dr["status"].ToString() == "inprogress")
                        {
                            c = dataRow.GetCell(5, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                            if (dr["status"].ToString() == "rejected")
                            {
                                using (DataTable dt_temp = (new CECHarmPublicService()).AuditLog_GetActivities(UserToken, "submitted", (int)dr["id"]))
                                {
                                    if (dt_temp.Rows.Count == 0)
                                        c.SetCellValue(" ");
                                    else
                                        c.SetCellValue(((DateTime)dt_temp.Rows[0]["create_date"]).ToString("MM/dd/yyyy"));
                                }
                            }
                            else
                                c.SetCellValue(((DateTime)dr["status_timestamp"]).ToString("MM/dd/yyyy"));
                            c.CellStyle = DataCellStyle;

                            c = dataRow.GetCell(6, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                            c.SetCellValue("Pending Revisions");
                            c.CellStyle = DataCellStyle;

                            using (DataTable dt_temp = CECWebSrv.GetCohortRecordById(UserToken, (int)dr["cohort_id"], false))
                            {
                                if (dt_temp.Rows.Count > 0)
                                {
                                    c = dataRow.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                                    c.SetCellValue(((DateTime)dt_temp.Rows[0]["status_timestamp"]).ToString("MM/dd/yyyy"));
                                    c.CellStyle = DataCellStyle;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ICell c = wkst.CreateRow(rowIndex++).GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    c.SetCellValue(String.Format("Nothing to report"));
                }

                for (int _ic = 0; _ic <= headerRow.PhysicalNumberOfCells; _ic++)
                    wkst.AutoSizeColumn(_ic);

                /// write output
                FileStream fs = new FileStream(savePath, FileMode.Create);
                workbook.Write(fs);
                fs.Close();

                return savePath;

            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        #endregion

        protected override void CreateChildControls()
        {
            foreach (string status in status_tabs)
            {
                if (status == "unpublished" && UserToken.access_level != 300)
                    break;

                System.Web.UI.HtmlControls.HtmlAnchor link =
                    new HtmlAnchor();
                link.InnerHtml = status[0].ToString().ToUpper() + status.Substring(1);
                link.HRef = String.Format("/input/list.aspx?tab={0}", status.ToLower());

                System.Web.UI.HtmlControls.HtmlGenericControl span =
                    new HtmlGenericControl("span");
                span.Attributes["class"] = "arrow down";

                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");
                li.Controls.Add(link);
                li.Controls.Add(span);

                if (Request.QueryString["tab"] == status)
                    li.Attributes["class"] = "active";

                navTabs.Controls.Add(li);
            }

            if (UserToken.access_level == 300)
            {

                foreach (string tab in admin_tabs)
                {
                    System.Web.UI.HtmlControls.HtmlAnchor link =
                        new HtmlAnchor();
                    link.InnerHtml = (tab == "users" ? "Manage " : "") + tab[0].ToString().ToUpper() + tab.Substring(1);
                    link.HRef = String.Format("/input/list.aspx?tab={0}", tab.ToLower());

                    System.Web.UI.HtmlControls.HtmlGenericControl span =
                        new HtmlGenericControl("span");
                    span.Attributes["class"] = "arrow down";

                    System.Web.UI.HtmlControls.HtmlGenericControl li =
                        new HtmlGenericControl("li");
                    li.Controls.Add(link);
                    li.Controls.Add(span);

                    if (Request.QueryString["tab"] == tab)
                        li.Attributes["class"] = "active";

                    navTabs.Controls.Add(li);
                }
            }
        }

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CECWebSrv = new CECInputFormService();

            if (Request.Cookies["sessionid"] != null && Session["UserSecurityToken"] == null)
            {
                try
                {
                    SecurityToken sec = CECWebSrv.GetSecurityToken(int.Parse(Request.Cookies["uid"].Value), Request.Cookies["sessionid"].Value);
                    Session["UserSecurityToken"] = sec;
                }
                catch 
                {
                    Response.Redirect("/", true);
                }
            }

            if (UserToken.access_level < 200)
                Response.Redirect("/input/edit.aspx", true);
        }

        protected override void OnLoad(EventArgs e)
        {
            string proxy_location = (Request.Url.Host == "localhost" ? "{0}://{1}/cec_service/cec_inputform.ashx?proxy" : "{0}://{1}/cec_inputform.ashx?proxy");
            Page.ClientScript.RegisterClientScriptInclude("webproxy", String.Format(proxy_location, Request.Url.Scheme, Request.Url.Host));

            TabName = status_tabs[0];
            if (Request.QueryString["tab"] != null)
                TabName = Request.QueryString["tab"].ToLower();

            if (UserToken.TokenSet && UserToken.access_level == 200)
            {
                edit_intro.Controls.Clear();
                edit_intro.Controls.Add(new LiteralControl("<div class='row col-sm-12'><h2>Welcome NCI Reviewer,</h2></div>"));
            }
            else if (UserToken.TokenSet && UserToken.access_level == 300)
            {
                if (TabName != "users")
                    addUserBtn.Visible = false;
            }

            if(Array.IndexOf(status_tabs, TabName) > -1)
            {
                if (TabName == "pending")
                {
                    dt_cohorts = CECWebSrv.GetCohortsWithStatusesWithColumns(UserToken, new string[] { "pending" }, "id, cohort_acronym, cohort_name, status_timestamp, [status]");
                    using (DataTable tmp_dt = CECWebSrv.GetCohortsWithStatusesWithColumns(UserToken, new string[] { "inprogress", "rejected" }, "id, cohort_acronym, cohort_name, status_timestamp, [status]"))
                    {
                        foreach (DataRow tmp_dr in tmp_dt.Rows)
                            dt_cohorts.ImportRow(tmp_dr);
                    }
                }
                else
                    dt_cohorts = CECWebSrv.GetCohortsByStatusWithColumns(UserToken, TabName, "id, cohort_acronym, cohort_name, [status]");

                cohortList.Sorting +=
                    new GridViewSortEventHandler(cohortList_Sorting);
                cohortList.RowDataBound +=
                    new GridViewRowEventHandler(cohortList_RowDataBound);
            }
            else if (TabName == "users")
            {
                // need to add display name to list of columns 
                dt_users = CECWebSrv.GetUsers(UserToken, "uid, username, display_name, email, access_level, cohort_id, account_lockout");

                cohortList.Sorting +=
                    new GridViewSortEventHandler(cohortList_Sorting);
                cohortList.RowDataBound +=
                    new GridViewRowEventHandler(userList_RowDataBound);
            }
            else
            {
                section.Controls.Clear();

                System.Web.UI.HtmlControls.HtmlGenericControl rl =
                    new HtmlGenericControl("div");
                rl.Attributes["class"] = "list-group";
                rl.ID = "reports";
                section.Controls.Add(rl);
                for(int i=0; i < reports.Length; i++)
                {
                    System.Web.UI.WebControls.HyperLink btn =
                        new HyperLink();
                    btn.CssClass = "list-group-item";
                    btn.NavigateUrl = String.Format("/input/list.aspx?tab=reports&name={0}", i);
                    btn.Controls.Add(new LiteralControl("<span class=\"glyphicon glyphicon-save-file\"></span> "));
                    btn.Controls.Add(new LiteralControl(reports[i]));
                    rl.Controls.Add(btn);

                    if (Request.QueryString["name"] != null && Request.QueryString["name"] == i.ToString())
                    {
                        string filepath = String.Format("/user_files/{0}/report_{1}.xlsx", UserToken.userid, DateTime.Now.ToString("yyyyMMMddmm"));

                        GenerateExcelReport(i, Server.MapPath(filepath));

                        Page.ClientScript.RegisterStartupScript(GetType(), "downloadExport",
                            String.Format("<script>window.open('{0}');</script>", filepath));
                    }
                }
            }
            
            base.OnLoad(e);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            if (Array.IndexOf(status_tabs, TabName) > -1)
            {
                if (SearchTerm != string.Empty)
                {
                    string search_for = string.Empty;
                    foreach (string s in new string[] { "cohort_acronym", "cohort_name" })
                        search_for += String.Format("{0} like '*{1}*',", s, SearchTerm);
                    search_for = search_for.TrimEnd(',').Replace(",", " OR ");
                    dt_cohorts.DefaultView.RowFilter = search_for;
                }

                cohortList.DataSource = dt_cohorts;
                cohortList.DataBind();
            }
            else if (TabName == "users")
            {
                if (SearchTerm != string.Empty)
                {
                    string search_for = string.Empty;
                    foreach (string s in new string[] { "username" })
                        search_for += String.Format("{0} like '*{1}*',", s, SearchTerm);
                    search_for = search_for.TrimEnd(',').Replace(",", " OR ");
                    dt_users.DefaultView.RowFilter = search_for;
                }
                
                cohortList.DataSource = dt_users;
                cohortList.DataBind();
            }
            else
            {
                // reports
            }

            base.OnLoadComplete(e);
        }

        protected void userList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.CssClass = "col-header";
                e.Row.TableSection = TableRowSection.TableHeader;

                e.Row.Cells.RemoveAt(0);

                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.Controls.Count == 0)
                        continue;

                    if (tc.Controls[0] is LinkButton)
                    {
                        tc.ID = (tc.Controls[0] as LinkButton).Text;
                        switch (tc.ID)
                        {
                            case "username":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("User Name"));
                                break;
                            case "access_level":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("User Type"));
                                break;
                            case "email":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("Email"));
                                break;
                            case "account_lockout":
                                tc.Text = "Lock";
                                break;
                            case "cohort_id":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("Cohort Acronym"));
                                break;
                            case "display_name":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("Name"));
                                break;
                        }
                    }

                    if (tc.ID != "account_lockout")
                    {
                        tc.CssClass = "sortable";
                        if (tc.ID == SortColumn)
                            tc.CssClass += String.Format(" columnSorting_{0}", GetSortString(SortOrder));

                        System.Web.UI.HtmlControls.HtmlGenericControl span = new HtmlGenericControl("span");
                        span.Attributes["class"] = "glyphicon glyphicon-sort";
                        (tc.Controls[0] as LinkButton).Controls.Add(span);
                    }
                }

                e.Row.Cells.Add(new TableHeaderCell());
                e.Row.Cells[6].Text = "Edit";
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells.RemoveAt(0);

                UserData ud = CECWebSrv.GetUserInformationByUserID(UserToken, (int)(e.Row.DataItem as DataRowView)["uid"]);

                e.Row.Cells[0].Text = ud.user_name;                
                // email
                e.Row.Cells[2].Text = ud.email;

                // access levels
                switch ((e.Row.DataItem as DataRowView)["access_level"].ToString())
                {
                    case "100":
                        e.Row.Cells[3].Text = "Cohort Representative";
                        break;
                    case "200":
                        e.Row.Cells[3].Text = "NCI Reviewer";
                        break;
                    case "300":
                        e.Row.Cells[3].Text = "Administrator";
                        break;
                }

                // cohort name
                if (ud.cohort_id != null && ud.cohort_id != 0)
                {
                    DataTable dt_cohort = CECWebSrv.GetCohortRecordById(UserToken, (int)ud.cohort_id, true);
                    if (ud.access_level == 100 && dt_cohort.Rows.Count > 0)
                        e.Row.Cells[4].Text = dt_cohort.Rows[0]["cohort_acronym"].ToString();
                }
                else 
                    e.Row.Cells[4].Text = String.Empty;

                // account lockout
                e.Row.Cells[5].Controls.Clear();

                System.Web.UI.HtmlControls.HtmlButton linkBtn_ii =
                    new HtmlButton();
                linkBtn_ii.Attributes["type"] = "button";
                linkBtn_ii.Attributes["class"] = "btn btn-primary";
                linkBtn_ii.Attributes["uid"] = (e.Row.DataItem as DataRowView)["uid"].ToString();
                if ((e.Row.DataItem as DataRowView)["account_lockout"] != null && (bool)(e.Row.DataItem as DataRowView)["account_lockout"])
                {
                    linkBtn_ii.ID = String.Format("unlock_user_{0}", (e.Row.DataItem as DataRowView)["uid"]);
                    linkBtn_ii.Controls.Add(new LiteralControl(String.Format(" <span class=\"glyphicon glyphicon-lock\"></span>", (e.Row.DataItem as DataRowView)["uid"])));
                    e.Row.Cells[5].Controls.Add(linkBtn_ii);
                }
                else
                {
                    linkBtn_ii.ID = String.Format("deactivate_user_{0}", (e.Row.DataItem as DataRowView)["uid"]);
                    linkBtn_ii.Controls.Add(new LiteralControl(String.Format(" <span class=\"glyphicon glyphicon-thumbs-up\"></span>", (e.Row.DataItem as DataRowView)["uid"])));
                    e.Row.Cells[5].Controls.Add(linkBtn_ii);
                }

                e.Row.Cells.Add(new TableCell());
                System.Web.UI.HtmlControls.HtmlButton linkBtn =
                    new HtmlButton();
                linkBtn.ID = String.Format("modify_user_{0}", (e.Row.DataItem as DataRowView)["uid"]);
                linkBtn.Attributes["type"] = "button";
                linkBtn.Attributes["uid"] = (e.Row.DataItem as DataRowView)["uid"].ToString();
                linkBtn.Attributes["class"] = "btn btn-primary";
                linkBtn.Controls.Add(new LiteralControl(" <span class=\"glyphicon glyphicon-pencil\"></span>"));
                e.Row.Cells[6].Controls.Add(linkBtn);
            }
        }

        protected void cohortList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.CssClass = "col-header";
                e.Row.TableSection = TableRowSection.TableHeader;

                e.Row.Cells.RemoveAt(0);

                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.Controls.Count == 0)
                        continue;

                    if (tc.Controls[0] is LinkButton)
                    {
                        tc.ID = (tc.Controls[0] as LinkButton).Text;
                        switch (tc.ID)
                        {
                            case "cohort_name":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("Cohort Name"));
                                tc.Width = Unit.Percentage(50);
                                break;
                            case "status_timestamp":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("Date/Time"));
                                break;
                            case "cohort_acronym":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("Cohort Acronym"));
                                break;
                            case "status":
                                (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl("Status"));
                                break;
                        }
                    }

                    tc.CssClass = "sortable"; 
                    if (tc.ID == SortColumn)
                        tc.CssClass += String.Format(" columnSorting_{0}", GetSortString(SortOrder));

                    System.Web.UI.HtmlControls.HtmlGenericControl span =
                        new HtmlGenericControl("span");
                    span.Attributes["class"] = "glyphicon glyphicon-sort";
                    (tc.Controls[0] as LinkButton).Controls.Add(span);
                }

                if (TabName != "pending")
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = " ";
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells.RemoveAt(0);

                // cohort name and link
                System.Web.UI.WebControls.HyperLink linkBtn =
                    new HyperLink();
                linkBtn.NavigateUrl = String.Format("/input/edit.aspx?id={0}", (e.Row.DataItem as DataRowView)["id"]);
                linkBtn.Controls.Add(new LiteralControl(e.Row.Cells[0].Text));
                linkBtn.Controls.Add(new LiteralControl(" <span class=\"glyphicon glyphicon-log-out\"></span>"));
                e.Row.Cells[0].Controls.Add(linkBtn);

                System.Web.UI.WebControls.HyperLink linkBtn_ii =
                    new HyperLink();
                linkBtn_ii.NavigateUrl = String.Format("/input/edit.aspx?id={0}", (e.Row.DataItem as DataRowView)["id"]);
                linkBtn_ii.Controls.Add(new LiteralControl(e.Row.Cells[1].Text));
                e.Row.Cells[1].Controls.Add(linkBtn_ii);

                // status text
                switch ((e.Row.DataItem as DataRowView)["status"].ToString())
                {
                    case "inprogress":
                        e.Row.Cells[3].Text = "Draft In Progress";
                        break;
                    case "pending":
                        e.Row.Cells[3].Text = "Under NCI Review";
                        break;
                    case "rejected":
                        e.Row.Cells[3].Text = "Returned to Cohort";
                        break;
                }

                if (TabName != "pending")
                {
                    System.Web.UI.HtmlControls.HtmlButton actionBtn =
                        new HtmlButton();
                    actionBtn.ID = String.Format("quick_action_{0}", (e.Row.DataItem as DataRowView)["id"]);
                    actionBtn.Attributes["type"] = "button";
                    actionBtn.Attributes["class"] = "btn btn-primary";
                    actionBtn.Attributes["record_id"] = (e.Row.DataItem as DataRowView)["id"].ToString();
                    if (TabName == "pending" || TabName == "unpublished")
                    {
                        actionBtn.Attributes["action"] = "publish";
                        actionBtn.InnerText = "publish";
                    }
                    else
                    {
                        actionBtn.Attributes["action"] = "unpublish";
                        actionBtn.InnerText = "unpublish";
                    }

                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(actionBtn);
                }
            }
        }

        protected void cohortList_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (e.SortExpression == SortColumn)
            {
                if (e.SortDirection == SortOrder)
                {
                    if (SortOrder == SortDirection.Ascending)
                        SortOrder = SortDirection.Descending;
                    else
                        SortOrder = SortDirection.Ascending;
                }
                else
                    SortOrder = e.SortDirection;
            }
            else
            {
                SortColumn = e.SortExpression;
                SortOrder = e.SortDirection;
            }

            if(dt_cohorts != null)
                dt_cohorts.DefaultView.Sort = String.Format("[{0}] {1}", SortColumn, GetSortString(SortOrder));

            if(dt_users != null)
                dt_users.DefaultView.Sort = String.Format("[{0}] {1}", SortColumn, GetSortString(SortOrder));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTerm = helper.SterilizeDBText(inSearch.Text.Trim());
        }
        #endregion
    }
}