using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;


namespace cec_publicweb
{
    using cec_publicservice;



    public partial class enrollment : CECPage
    {
        private System.Data.DataTable dt_totals,
            dt_males,
            dt_females,
            dt_unknown,
            webfieldsTbl;
        
        //private int _webfieldPosition;

        private bool _isfirstrow;

        private string _ethnicity,
            _curEthnic,
            _curRace,
            _curSumLbl;

        /// <summary>
        /// create the necessary childcontrols for filtering
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            enrollTblMales.ClientIDMode = ClientIDMode.Static;
            enrollTblFemales.ClientIDMode = ClientIDMode.Static;
            enrollTblSum.ClientIDMode = ClientIDMode.Static;
            enrollTblUnknown.ClientIDMode = ClientIDMode.Static;
            
            select_cohort.UserToken = UserToken;
        }

        #region Properties

        /// <summary>
        /// get or set string array of cohort ids
        /// </summary>
        protected string[] CohortIDsToCompare
        {
            get
            {
                if ((Session["EnrollmentCohortIDsToCompare"] != null) && ((string[])Session["EnrollmentCohortIDsToCompare"]).Length > 0)
                    return (string[])Session["EnrollmentCohortIDsToCompare"];
                else
                    return new string[0];
            }
            set
            {
                Session["EnrollmentCohortIDsToCompare"] = value;
            }
        }

        protected string[] Races
        {
            get
            {
                if ((Session["pgEnrollmentRaces"] != null) && ((string[])Session["pgEnrollmentRaces"]).Length > 0)
                    return (string[])Session["pgEnrollmentRaces"];
                else
                    return new string[0];
            }
            set
            {
                Session["pgEnrollmentRaces"] = value;
            }
        }

        protected string[] Ethnicities
        {
            get
            {
                if ((Session["pgEnrollmentEthnicities"] != null) && ((string[])Session["pgEnrollmentEthnicities"]).Length > 0)
                    return (string[])Session["pgEnrollmentEthnicities"];
                else
                    return new string[0];
            }
            set
            {
                Session["pgEnrollmentEthnicities"] = value;
            }
        }
        #endregion

        #region EventHandling

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                CohortIDsToCompare = new string[0];
                Races = new string[0];
                Ethnicities = new string[0];

                CECWebSrv.AuditLog_AddActivity(UserToken.userid, "enrollment page");
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            PopulateEnrollmentGrids();

            submitBtn.Attributes["disabled"] = "disabled";

            if (dt_males.Rows.Count > 0 || dt_females.Rows.Count > 0 || dt_unknown.Rows.Count > 0)
                exportTblBtn.Visible = true;
            else
                exportTblBtn.Visible = false;

            base.OnLoadComplete(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _ethnicity = "novalue";

            // instantiate data tables
            dt_males = new DataTable();
            dt_males.TableName = "dt_males";

            dt_females = new DataTable();
            dt_females.TableName = "dt_females";

            dt_unknown = new DataTable();
            dt_unknown.TableName = "dt_unknown";
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            EnsureChildControls();

            bool handled = false;
            if (args is CommandEventArgs)
            {
                CommandEventArgs cea = (args as CommandEventArgs);
                switch (cea.CommandName)
                {
                    case "export":
                        handled = true;
                                                
                        string filepath = String.Format("./user_files/{0}/enrollment_{1}.xlsx", UserToken.userid, DateTime.Now.ToString("yyyyMMMddmm"));

                        ExportDataGridToExcel(MapPath(filepath));
                        CECWebSrv.AuditLog_AddActivity(UserToken.userid, "enrollment export created");

                        Page.ClientScript.RegisterStartupScript(GetType(), "downloadExport",
                            String.Format("<script>window.open('{0}');</script>", filepath));

                        break;
                }

                return handled;
            }
            else
                return base.OnBubbleEvent(source, args);
        }

        protected void enrolTblSumGridView_RowDataBound(object source, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.ID = "summaryHeader";

                ///-----------------------------------------------
                /// the rest of the columns cohort columns, except the total
                /// 
                for (int _i=0; _i < e.Row.Cells.Count; _i++)
                {
                    System.Web.UI.WebControls.TableHeaderCell hc =
                        new TableHeaderCell();
                    hc.ID = dt_totals.Columns[_i].ColumnName.Replace(" ", string.Empty);

                    if (CECWebSrv.IsCohort(UserToken, e.Row.Cells[_i].Text))
                    {
                        System.Web.UI.WebControls.HyperLink cohortLnk =
                            new HyperLink();
                        cohortLnk.Text = e.Row.Cells[_i].Text;
                        //cohortLnk.Target = "_blank";
                        cohortLnk.NavigateUrl = String.Format("/cohortDetails.aspx?cohort_acronym={0}", dt_totals.Columns[_i].ColumnName);

                        hc.CssClass = "table-col-10perc ";
                        hc.Controls.Add(cohortLnk);
                    }
                    else
                    {
                        hc.CssClass = "table-col-20perc";
                        hc.Text = e.Row.Cells[_i].Text;
                    }

                    e.Row.Cells.RemoveAt(_i);
                    e.Row.Cells.AddAt(_i, hc);
                }

                ///----------------------------------------------
                /// current enrollment count label 
                /// 
                System.Web.UI.WebControls.TableHeaderCell hdrLb =
                    new TableHeaderCell();
                hdrLb.ID = "sumcount";
                hdrLb.CssClass = "table-col-20perc";
                hdrLb.Text = "Current Enrollment Numbers";

                e.Row.Cells.RemoveAt(0);
                e.Row.Cells.AddAt(0, hdrLb);

                ///----------------------------------------------
                /// and the total column
                /// 
                e.Row.Cells.RemoveAt(e.Row.Cells.Count - 1);

                System.Web.UI.WebControls.TableHeaderCell total =
                    new TableHeaderCell();
                total.ID = "total";
                total.CssClass = "table-col-20perc";
                total.Text = "Total";

                e.Row.Cells.Add(total);
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow[] dra = webfieldsTbl.Select(String.Format("data_field='{0}'", e.Row.Cells[0].Text));
                DataRow dr = dra[0];
                
                string orgColumnName = e.Row.Cells[0].Text;
                //_curSumLbl = orgColumnName.Split('.')[1];

                ///---------------------------------------------------
                /// change to tableheadercell
                /// 
                System.Web.UI.WebControls.TableHeaderCell nmHdr =
                    new TableHeaderCell();
                nmHdr.ID = _curSumLbl;
                nmHdr.CssClass = "h-race";
                nmHdr.Attributes["headers"] = String.Format("{0}", "sumcount");
                nmHdr.Text = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, orgColumnName);

                e.Row.Cells.RemoveAt(0);
                e.Row.Cells.AddAt(0, nmHdr);

                ///--------------------------------------------------
                /// the data cells: add header  cell ids.
                for (int _i = 1; _i < e.Row.Cells.Count; _i++)
                {
                    if (dt_totals.Columns[_i].ColumnName.ToLower() == "total")
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0}", "total");
                    else
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1}", _curSumLbl, dt_totals.Columns[_i].ColumnName);
                }
            }
        }

        protected void enrolTblMalesGridView_RowDataBound(object source, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.ID = "maleHeader";

                ///-----------------------------------------------
                /// the rest of the columns cohort columns, except the total
                /// 
                for (int _i = 1; _i < e.Row.Cells.Count - 1; _i++)
                {
                    System.Web.UI.WebControls.TableHeaderCell hc =
                        new TableHeaderCell();
                    hc.ID = dt_males.Columns[_i].ColumnName;

                    if (CECWebSrv.IsCohort(UserToken, e.Row.Cells[_i].Text))
                    {
                        DataRow dr_cohort = null;
                        using (DataTable dt = CECWebSrv.GetFilteredCohortRecords(UserToken, "cohort_id, cohort_name, cohort_acronym", String.Format("cohort_acronym='{0}'", dt_males.Columns[_i].ColumnName)).Tables[0])
                            dr_cohort = dt.Rows[0];

                        System.Web.UI.HtmlControls.HtmlAnchor cohortLnk =
                                new HtmlAnchor();
                        cohortLnk.HRef = String.Format("/cohortDetails.aspx?cohort_id={0}", dr_cohort["cohort_id"]);
                        cohortLnk.Attributes["tabindex"] = "0";
                        cohortLnk.Attributes["role"] = "button";
                        cohortLnk.Attributes["data-toggle"] = "popover";
                        cohortLnk.Attributes["data-trigger"] = "focus";
                        cohortLnk.Attributes["data-placement"] = "top";
                        cohortLnk.Attributes["data-trigger"] = "hover";
                        cohortLnk.Attributes["data-content"] = String.Format("{0}", dr_cohort["cohort_name"]);
                        cohortLnk.InnerHtml = helper.HTMLEncode(e.Row.Cells[_i].Text);

                        hc.CssClass = "table-col-10perc sortable";
                        hc.Controls.Add(cohortLnk);
                    }
                    else
                    {
                        hc.CssClass = "table-col-20perc";
                        hc.Text = e.Row.Cells[_i].Text;
                    }

                    e.Row.Cells.RemoveAt(_i);
                    e.Row.Cells.AddAt(_i, hc);
                }

                ///-----------------------------------------------
                /// add first column spacing as to accommidate the
                /// row spans
                /// 
                System.Web.UI.WebControls.TableHeaderCell ethnic =
                    new TableHeaderCell();
                ethnic.CssClass = "table-col-20perc";
                ethnic.Text = "Ethnicity";
                ethnic.ID = "ethnic";

                e.Row.Cells.AddAt(0, ethnic);

                ///-----------------------------------------------
                /// now add header information to Race column
                /// 
                e.Row.Cells.RemoveAt(1);

                System.Web.UI.WebControls.TableHeaderCell race =
                    new TableHeaderCell();
                race.CssClass = "table-col-20perc";
                race.ID = "race";
                race.Text = "Race";

                e.Row.Cells.AddAt(1, race);

                ///----------------------------------------------
                /// and the total column
                /// 
                e.Row.Cells.RemoveAt(e.Row.Cells.Count - 1);

                System.Web.UI.WebControls.TableHeaderCell total =
                    new TableHeaderCell();
                total.ID = "total";
                total.CssClass = "table-col-20perc";
                total.Text = "Total";

                e.Row.Cells.Add(total);

                e.Row.TableSection = TableRowSection.TableHeader;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow[] dra = webfieldsTbl.Select(String.Format("data_field='{0}'", e.Row.Cells[0].Text));
                DataRow dr = dra[0];

                _isfirstrow = false;

                string orgColumnName = e.Row.Cells[0].Text;
                //_curRace = orgColumnName.Split('.')[1];
                ///----------------------------------------------------------------------------
                /// add ethnicity row spans
                /// 
                if ((orgColumnName.Split('_').Length >= 3) && orgColumnName.Split('_')[2] != _ethnicity)
                {
                    _ethnicity = orgColumnName.Split('_')[2];

                    System.Web.UI.WebControls.TableHeaderCell tcRs =
                        new TableHeaderCell();
                    tcRs.CssClass = "comRowHeader";
                    tcRs.Attributes["headers"] = "ethnic";

                    switch (_ethnicity)
                    {
                        case "nonhispanic":
                            tcRs.Text = "Not Hispanic or Latino";
                            tcRs.ID = "NH";
                            tcRs.RowSpan = Races.Length;
                            break;
                        case "hispanic":
                            tcRs.Text = "Hispanic or Latino";
                            tcRs.ID = "H";
                            tcRs.RowSpan = Races.Length;
                            break;
                        case "unknown":
                            tcRs.Text = "Unknown/Not Reported Ethnicity";
                            tcRs.ID = "UE";
                            tcRs.RowSpan = Races.Length;
                            break;
                        default:
                            tcRs.Text = "";
                            break;
                    }
                    _curEthnic = tcRs.ID;

                    e.Row.Cells.AddAt(0, tcRs);
                    e.Row.CssClass = "first-row";

                    _isfirstrow = true;
                }

                ///---------------------------------------------------
                /// change to tableheadercell
                /// 
                System.Web.UI.WebControls.TableHeaderCell nmHdr =
                    new TableHeaderCell();
                nmHdr.ID = _curRace;
                nmHdr.CssClass = "h-race";
                nmHdr.Attributes["headers"] = String.Format("{0} {1}", _curEthnic, "race");
                nmHdr.Text = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, orgColumnName);

                if (_isfirstrow)
                {
                    e.Row.Cells.RemoveAt(1);
                    e.Row.Cells.AddAt(1, nmHdr);
                }
                else
                {
                    e.Row.Cells.RemoveAt(0);
                    e.Row.Cells.AddAt(0, nmHdr);
                }

                ///--------------------------------------------------
                /// the data cells: add header  cell ids.
                for (int _i = (_isfirstrow ? 2 : 1); _i < e.Row.Cells.Count; _i++)
                {
                    if (_i == (e.Row.Cells.Count - 1))
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, "total");
                    else if (_isfirstrow)
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, dt_males.Columns[(_i - 1)].ColumnName);
                    else
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, dt_males.Columns[_i].ColumnName);
                }

                ///--------------------------------------------------------------
                /// tables are grouped by gender, no need for the extra section
                ///  rows
                ///  
                //int wholeNumber = Convert.ToInt32(dr["demographics_position"]);
                //if (_webfieldPosition != wholeNumber)
                //{
                //    _webfieldPosition = wholeNumber;

                //    // add merged cells above this position to display as groupings
                //    System.Web.UI.WebControls.TableHeaderCell tc1 =
                //        new TableHeaderCell();
                //    tc1.CssClass = "enrollmentGroupTitle";
                //    tc1.ColumnSpan = e.Row.Cells.Count;

                //    tc1.Text = webfieldsTbl.Select("demographics_position=" + _webfieldPosition)[0]["field_label"].ToString();

                //    System.Web.UI.WebControls.GridViewRow gvr =
                //        new GridViewRow(e.Row.RowIndex, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                //    gvr.Cells.Add(tc1);

                //    int lastIndex = enrollTblMales.Controls[0].Controls.Count;
                //    enrollTblMales.Controls[0].Controls.AddAt(lastIndex - 1, gvr);
                //}
            }
        }

        protected void enrolTblMalesGridView_DataBound(object source, EventArgs e)
        {
            ResetDataBindingVariables();
        }

        protected void enrolTblFemalesGridView_RowDataBound(object source, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                ///-----------------------------------------------
                /// the rest of the columns cohort columns, except the total
                /// 
                for (int _i = 1; _i < e.Row.Cells.Count - 1; _i++)
                {
                    System.Web.UI.WebControls.TableHeaderCell hc =
                        new TableHeaderCell();
                    hc.ID = dt_females.Columns[_i].ColumnName;

                    if (CECWebSrv.IsCohort(UserToken, e.Row.Cells[_i].Text))
                    {
                        DataRow dr_cohort = null;
                        using (DataTable dt = CECWebSrv.GetFilteredCohortRecords(UserToken, "cohort_id, cohort_name, cohort_acronym", String.Format("cohort_acronym='{0}'", dt_females.Columns[_i].ColumnName)).Tables[0])
                            dr_cohort = dt.Rows[0];

                        System.Web.UI.HtmlControls.HtmlAnchor cohortLnk =
                                new HtmlAnchor();
                        cohortLnk.HRef = String.Format("/cohortDetails.aspx?cohort_id={0}", dr_cohort["cohort_id"]);
                        cohortLnk.Attributes["tabindex"] = "0";
                        cohortLnk.Attributes["role"] = "button";
                        cohortLnk.Attributes["data-toggle"] = "popover";
                        cohortLnk.Attributes["data-trigger"] = "focus";
                        cohortLnk.Attributes["data-placement"] = "top";
                        cohortLnk.Attributes["data-trigger"] = "hover";
                        cohortLnk.Attributes["data-content"] = String.Format("{0}", dr_cohort["cohort_name"]);
                        cohortLnk.InnerHtml = helper.HTMLEncode(e.Row.Cells[_i].Text);

                        hc.CssClass = "table-col-10perc sortable";
                        hc.Controls.Add(cohortLnk);
                    }
                    else
                    {
                        hc.CssClass = "table-col-20perc";
                        hc.Text = e.Row.Cells[_i].Text;
                    }

                    e.Row.Cells.RemoveAt(_i);
                    e.Row.Cells.AddAt(_i, hc);
                }

                ///-----------------------------------------------
                /// add first column spacing as to accommidate the
                /// row spans
                /// 
                System.Web.UI.WebControls.TableHeaderCell ethnic =
                    new TableHeaderCell();
                ethnic.CssClass = "table-col-20perc";
                ethnic.Text = "Ethnicity";
                ethnic.ID = "ethnic";

                e.Row.Cells.AddAt(0, ethnic);

                ///-----------------------------------------------
                /// now add header information to Race column
                /// 
                e.Row.Cells.RemoveAt(1);

                System.Web.UI.WebControls.TableHeaderCell race =
                    new TableHeaderCell();
                race.CssClass = "table-col-20perc";
                race.ID = "race";
                race.Text = "Race";

                e.Row.Cells.AddAt(1, race);

                ///----------------------------------------------
                /// and the total column
                /// 
                e.Row.Cells.RemoveAt(e.Row.Cells.Count - 1);

                System.Web.UI.WebControls.TableHeaderCell total =
                    new TableHeaderCell();
                total.ID = "total";
                total.CssClass = "table-col-20perc";
                total.Text = "Total";

                e.Row.Cells.Add(total);

                e.Row.TableSection = TableRowSection.TableHeader;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow[] dra = webfieldsTbl.Select(String.Format("data_field='{0}'", e.Row.Cells[0].Text));
                DataRow dr = dra[0];

                _isfirstrow = false;

                string orgColumnName = e.Row.Cells[0].Text;
                //_curRace = orgColumnName.Split('.')[1];
                ///----------------------------------------------------------------------------
                /// add ethnicity row spans
                /// 
                if ((orgColumnName.Split('_').Length >= 3) && orgColumnName.Split('_')[2] != _ethnicity)
                {
                    _ethnicity = orgColumnName.Split('_')[2];

                    System.Web.UI.WebControls.TableHeaderCell tcRs =
                        new TableHeaderCell();
                    tcRs.CssClass = "comRowHeader";
                    tcRs.Attributes["headers"] = "ethnic";

                    switch (_ethnicity)
                    {
                        case "nonhispanic":
                            tcRs.Text = "Not Hispanic or Latino";
                            tcRs.ID = "NH";
                            tcRs.RowSpan = Races.Length;
                            break;
                        case "hispanic":
                            tcRs.Text = "Hispanic or Latino";
                            tcRs.ID = "H";
                            tcRs.RowSpan = Races.Length;
                            break;
                        case "unknown":
                            tcRs.Text = "Unknown/Not Reported Ethnicity";
                            tcRs.ID = "UE";
                            tcRs.RowSpan = Races.Length;
                            break;
                        default:
                            tcRs.Text = "";
                            break;
                    }
                    _curEthnic = tcRs.ID;

                    e.Row.Cells.AddAt(0, tcRs);
                    e.Row.CssClass = "first-row";

                    _isfirstrow = true;
                }

                ///---------------------------------------------------
                /// change to tableheadercell
                /// 
                System.Web.UI.WebControls.TableHeaderCell nmHdr =
                    new TableHeaderCell();
                nmHdr.ID = _curRace;
                nmHdr.CssClass = "h-race";
                nmHdr.Attributes["headers"] = String.Format("{0} {1}", _curEthnic, "race");
                nmHdr.Text = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, orgColumnName);

                if (_isfirstrow)
                {
                    e.Row.Cells.RemoveAt(1);
                    e.Row.Cells.AddAt(1, nmHdr);
                }
                else
                {
                    e.Row.Cells.RemoveAt(0);
                    e.Row.Cells.AddAt(0, nmHdr);
                }

                ///--------------------------------------------------
                /// the data cells: add header  cell ids.
                for (int _i = (_isfirstrow ? 2 : 1); _i < e.Row.Cells.Count; _i++)
                {
                    if (_i == (e.Row.Cells.Count - 1))
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, "total");
                    else if (_isfirstrow)
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, dt_females.Columns[(_i - 1)].ColumnName);
                    else
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, dt_females.Columns[_i].ColumnName);
                }

                ///--------------------------------------------------------------
                /// tables are grouped by gender, no need for the extra section
                ///  rows
                ///  
                //int wholeNumber = Convert.ToInt32(dr["demographics_position"]);
                //if (_webfieldPosition != wholeNumber)
                //{
                //    _webfieldPosition = wholeNumber;

                //    // add merged cells above this position to display as groupings
                //    System.Web.UI.WebControls.TableHeaderCell tc1 =
                //        new TableHeaderCell();
                //    tc1.CssClass = "enrollmentGroupTitle";
                //    tc1.ColumnSpan = e.Row.Cells.Count;

                //    tc1.Text = webfieldsTbl.Select("demographics_position=" + _webfieldPosition)[0]["field_label"].ToString();

                //    System.Web.UI.WebControls.GridViewRow gvr =
                //        new GridViewRow(e.Row.RowIndex, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                //    gvr.Cells.Add(tc1);

                //    int lastIndex = enrollTblMales.Controls[0].Controls.Count;
                //    enrollTblMales.Controls[0].Controls.AddAt(lastIndex - 1, gvr);
                //}
            }
        }

        protected void enrolTblFemalesGridView_DataBound(object source, EventArgs e)
        {
            ResetDataBindingVariables();
        }

        protected void enrolTblUnknownGridView_RowDataBound(object source, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                ///-----------------------------------------------
                /// the rest of the columns cohort columns, except the total
                /// 
                for (int _i = 1; _i < e.Row.Cells.Count - 1; _i++)
                {
                    System.Web.UI.WebControls.TableHeaderCell hc =
                        new TableHeaderCell();
                    hc.ID = dt_unknown.Columns[_i].ColumnName;

                    if (CECWebSrv.IsCohort(UserToken, e.Row.Cells[_i].Text))
                    {
                        DataRow dr_cohort = null;
                        using (DataTable dt = CECWebSrv.GetFilteredCohortRecords(UserToken, "cohort_id, cohort_name, cohort_acronym", String.Format("cohort_acronym='{0}'", dt_unknown.Columns[_i].ColumnName)).Tables[0])
                            dr_cohort = dt.Rows[0];

                        System.Web.UI.HtmlControls.HtmlAnchor cohortLnk =
                                new HtmlAnchor();
                        cohortLnk.HRef = String.Format("/cohortDetails.aspx?cohort_id={0}", dr_cohort["cohort_id"]);
                        cohortLnk.Attributes["tabindex"] = "0";
                        cohortLnk.Attributes["role"] = "button";
                        cohortLnk.Attributes["data-toggle"] = "popover";
                        cohortLnk.Attributes["data-trigger"] = "focus";
                        cohortLnk.Attributes["data-placement"] = "top";
                        cohortLnk.Attributes["data-trigger"] = "hover";
                        cohortLnk.Attributes["data-content"] = String.Format("{0}", dr_cohort["cohort_name"]);
                        cohortLnk.InnerHtml = helper.HTMLEncode(e.Row.Cells[_i].Text);

                        hc.CssClass = "table-col-10perc sortable";
                        hc.Controls.Add(cohortLnk);
                    }
                    else
                    {
                        hc.CssClass = "table-col-20perc";
                        hc.Text = e.Row.Cells[_i].Text;
                    }

                    e.Row.Cells.RemoveAt(_i);
                    e.Row.Cells.AddAt(_i, hc);
                }

                ///-----------------------------------------------
                /// add first column spacing as to accommidate the
                /// row spans
                /// 
                System.Web.UI.WebControls.TableHeaderCell ethnic =
                    new TableHeaderCell();
                ethnic.CssClass = "table-col-20perc";
                ethnic.Text = "Ethnicity";
                ethnic.ID = "ethnic";

                e.Row.Cells.AddAt(0, ethnic);

                ///-----------------------------------------------
                /// now add header information to Race column
                /// 
                e.Row.Cells.RemoveAt(1);

                System.Web.UI.WebControls.TableHeaderCell race =
                    new TableHeaderCell();
                race.CssClass = "table-col-20perc";
                race.ID = "race";
                race.Text = "Race";

                e.Row.Cells.AddAt(1, race);

                ///----------------------------------------------
                /// and the total column
                /// 
                e.Row.Cells.RemoveAt(e.Row.Cells.Count - 1);

                System.Web.UI.WebControls.TableHeaderCell total =
                    new TableHeaderCell();
                total.ID = "total";
                total.CssClass = "table-col-20perc";
                total.Text = "Total";

                e.Row.Cells.Add(total);
                e.Row.TableSection = TableRowSection.TableHeader;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow[] dra = webfieldsTbl.Select(String.Format("data_field='{0}'", e.Row.Cells[0].Text));
                DataRow dr = dra[0];

                _isfirstrow = false;

                string orgColumnName = e.Row.Cells[0].Text;
                //_curRace = orgColumnName.Split('.')[1];
                ///----------------------------------------------------------------------------
                /// add ethnicity row spans
                /// 
                if ((orgColumnName.Split('_').Length >= 3) && orgColumnName.Split('_')[2] != _ethnicity)
                {
                    _ethnicity = orgColumnName.Split('_')[2];

                    System.Web.UI.WebControls.TableHeaderCell tcRs =
                        new TableHeaderCell();
                    tcRs.CssClass = "comRowHeader"; //"h-ethnicity";
                    tcRs.Attributes["headers"] = "ethnic";

                    switch (_ethnicity)
                    {
                        case "nonhispanic":
                            tcRs.Text = "Not Hispanic or Latino";
                            tcRs.ID = "NH";
                            tcRs.RowSpan = Races.Length;
                            break;
                        case "hispanic":
                            tcRs.Text = "Hispanic or Latino";
                            tcRs.ID = "H";
                            tcRs.RowSpan = Races.Length;
                            break;
                        case "unknown":
                            tcRs.Text = "Unknown/Not Reported Ethnicity";
                            tcRs.ID = "UE";
                            tcRs.RowSpan = Races.Length;
                            break;
                        default:
                            tcRs.Text = "";
                            break;
                    }
                    _curEthnic = tcRs.ID;

                    e.Row.Cells.AddAt(0, tcRs);
                    e.Row.CssClass = "first-row";

                    _isfirstrow = true;
                }

                ///---------------------------------------------------
                /// change to tableheadercell
                /// 
                System.Web.UI.WebControls.TableHeaderCell nmHdr =
                    new TableHeaderCell();
                nmHdr.ID = _curRace;
                nmHdr.CssClass = "h-race";
                nmHdr.Attributes["headers"] = String.Format("{0} {1}", _curEthnic, "race");
                nmHdr.Text = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, orgColumnName);

                if (_isfirstrow)
                {
                    e.Row.Cells.RemoveAt(1);
                    e.Row.Cells.AddAt(1, nmHdr);
                }
                else
                {
                    e.Row.Cells.RemoveAt(0);
                    e.Row.Cells.AddAt(0, nmHdr);
                }

                ///--------------------------------------------------
                /// the data cells: add header  cell ids.
                for (int _i = (_isfirstrow ? 2 : 1); _i < e.Row.Cells.Count; _i++)
                {
                    if (_i == (e.Row.Cells.Count - 1))
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, "total");
                    else if (_isfirstrow)
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, dt_unknown.Columns[(_i - 1)].ColumnName);
                    else
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curEthnic, _curRace, dt_unknown.Columns[_i].ColumnName);
                }

                ///--------------------------------------------------------------
                /// tables are grouped by gender, no need for the extra section
                ///  rows
                ///  
                //int wholeNumber = Convert.ToInt32(dr["demographics_position"]);
                //if (_webfieldPosition != wholeNumber)
                //{
                //    _webfieldPosition = wholeNumber;

                //    // add merged cells above this position to display as groupings
                //    System.Web.UI.WebControls.TableHeaderCell tc1 =
                //        new TableHeaderCell();
                //    tc1.CssClass = "enrollmentGroupTitle";
                //    tc1.ColumnSpan = e.Row.Cells.Count;

                //    tc1.Text = webfieldsTbl.Select("demographics_position=" + _webfieldPosition)[0]["field_label"].ToString();

                //    System.Web.UI.WebControls.GridViewRow gvr =
                //        new GridViewRow(e.Row.RowIndex, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                //    gvr.Cells.Add(tc1);

                //    int lastIndex = enrollTblMales.Controls[0].Controls.Count;
                //    enrollTblMales.Controls[0].Controls.AddAt(lastIndex - 1, gvr);
                //}
            }
        }

        protected void enrolTblUnknownGridView_DataBound(object source, EventArgs e)
        {
            ResetDataBindingVariables();
        }

        protected void ClearOptions_Clicked(object sender, EventArgs e)
        {
            CohortIDsToCompare = new string[0];
            Ethnicities = new string[0];
            Races = new string[0];

            select_cohort.ClearSelectedCohortIDs();
            gender_list.ClearSelectedOptions();
            race_list.ClearSelectedOptions();
            ethnicity_list.ClearSelectedOptions();

            //cb_gmale.Checked = false;
            //cb_gfemale.Checked = false;
            //cb_gunknown.Checked = false;
            //cb_ehis.Checked = false;
            //cb_enonhis.Checked = false;
            //cb_eunknown.Checked = false;
            //cb_rasian.Checked = false;
            //cb_rblack.Checked = false;
            //cb_rmulti.Checked = false;
            //cb_rnative.Checked = false;
            //cb_rpi.Checked = false;
            //cb_runknown.Checked = false;
            //cb_rwhite.Checked = false;

            EnsureChildControls();

            PopulateEnrollmentGrids();
        }

        protected void Submit_Clicked(object source, EventArgs e)
        {
            EnsureChildControls();

            CohortIDsToCompare = null;
            Ethnicities = null;
            Races = null;
                       
            // cohorts
            CohortIDsToCompare = select_cohort.GetSelectedCohortIDs();
            // races
            Races = race_list.GetSelectedOptions();
            // ethnicities
            Ethnicities = ethnicity_list.GetSelectedOptions();

            //System.Collections.ArrayList tArray = new ArrayList();
            //// races
            //tArray.Clear();
            //if (cb_rnative.Checked)
            //    tArray.Add("ai");
            //if (cb_rasian.Checked)
            //    tArray.Add("asian");
            //if (cb_rblack.Checked)
            //    tArray.Add("black");
            //if (cb_rpi.Checked)
            //    tArray.Add("pi");
            //if (cb_rwhite.Checked)
            //    tArray.Add("white");
            //if (cb_runknown.Checked)
            //    tArray.Add("unknown");
            //if (cb_rmulti.Checked)
            //    tArray.Add("multiple");

            //Races = (string[])tArray.ToArray(typeof(string));

            // ethnicities
            //tArray.Clear();
            //if (cb_enonhis.Checked)
            //    tArray.Add("nonhispanic");
            //if (cb_ehis.Checked)
            //    tArray.Add("hispanic");
            //if (cb_eunknown.Checked)
            //    tArray.Add("unknown");
            //Ethnicities = (string[])tArray.ToArray(typeof(string));

            string missingFields = string.Empty;
            if (gender_list.GetSelectedOptions().Length == 0)
            {
                missingFields += "&bull;Error: <b>Gender</b> is required<br/>";
                gender.Attributes["class"] += " filter-component-required";
            }
            else
                gender.Attributes["class"] = " filter-component";

            if (CohortIDsToCompare.Length == 0)
            {
                missingFields += "&bull;Error: <b>Select Cohorts</b> is required<br/>";
                select_cohort.Attributes["class"] += " filter-component-required";
            }

            if (Races.Length == 0)
            {
                missingFields += "&bull;Error: <b>Race</b> is required<br/>";
                race.Attributes["class"] += " filter-component-required";
            }
            else
                race.Attributes["class"] = " filter-component";

            if (Ethnicities.Length == 0)
            {
                missingFields += "&bull;Error: <b>Ethnicity</b> is required<br/>";
                ethnicity.Attributes["class"] += " filter-component-required";
            }
            else
                ethnicity.Attributes["class"] = " filter-component";

            if (!String.IsNullOrWhiteSpace(missingFields))
                RegisterJSError(String.Format("<p>The form is missing required fields. Please check the following fields then try again.<p>{0}</p>", missingFields));
        }
        #endregion

        private void ResetDataBindingVariables()
        {
            _isfirstrow = false;
            _curEthnic = string.Empty;
            _curRace = string.Empty;
            _curSumLbl = string.Empty;
            _ethnicity = string.Empty;
        }

        private void PopulateEnrollmentGrids()
        {
            bool othersOkay = true;
            if (CohortIDsToCompare.Length == 0 || Races.Length == 0 || Ethnicities.Length == 0)
                othersOkay = false;
            ///-----
            /// retreive field display settings
            /// 
            webfieldsTbl = CECWebSrv.GetCohortWebFieldsForEnrollmentGrid(UserToken).Tables[0];

            //PopulateSumEnrollmentGrid();

            bool gender_male = false,
                gender_female = false,
                gender_unknown = false;
            foreach (string s in gender_list.GetSelectedOptions())
            {
                if (s == "male")
                    gender_male = true;
                else if (s == "female")
                    gender_female = true;
                else if (s == "unknown")
                    gender_unknown = true;
            }

            dt_males = new DataTable();
            dt_males.TableName = "dt_males";
            if (gender_male && othersOkay)
            {
                enrollTblMales.Visible = true;
                enrollTblMales.Attributes["has_results"] = "true";

                PopulateMaleEnrollmentGrid();
            }
            else
                enrollTblMales.Visible = false;
            enrollTblMales.DataSource = dt_males;
            enrollTblMales.DataBind();

            dt_females = new DataTable();
            dt_females.TableName = "dt_females";
            if (gender_female && othersOkay)
            {
                enrollTblFemales.Visible = true;
                enrollTblFemales.Attributes["has_results"] = "true";

                PopulateFemaleEnrollmentGrid();
            }
            else
                enrollTblFemales.Visible = false;
            enrollTblFemales.DataSource = dt_females;
            enrollTblFemales.DataBind();
            
            dt_unknown = new DataTable();
            dt_unknown.TableName = "dt_unknown";
            if (gender_unknown && othersOkay)
            {
                enrollTblUnknown.Visible = true;
                enrollTblUnknown.Attributes["has_results"] = "true";

                PopulateUnknownEnrollmentGrid();
            }
            else
                enrollTblUnknown.Visible = false;
            enrollTblUnknown.DataSource = dt_unknown;
            enrollTblUnknown.DataBind();
        }

        private void PopulateSumEnrollmentGrid()
        {
            //-----
            // bind enrollment grid data
            System.Data.DataSet sg;
            //if ((Session["CohortIDsToCompare"] != null) && ((string[])Session["CohortIDsToCompare"]).Length > 0)
            //    sg = CECWebSrv.GetCohortForEnrollmentGrid_SummaryTable(UserToken, CohortIDsToCompare);
            //else
            //{
                sg = new DataSet();
                sg.Tables.Add("tbl_enrollment");
            //}

            DataTable t = sg.Tables["tbl_enrollment"];

            dt_totals = new DataTable();
            dt_totals.TableName = "dt_totals";

            dt_totals.Columns.Add(new DataColumn("Current Enrollment Counts"));

            // find acronym position and build the pivot table column names
            int acronymPosition = t.Columns.IndexOf("cohort_acronym");
            for (int _p = 0; _p < t.Rows.Count; _p++)
            {
                if (acronymPosition > -1 && dt_totals.Columns.IndexOf(t.Rows[_p][acronymPosition].ToString()) == -1)
                    dt_totals.Columns.Add(new DataColumn(t.Rows[_p][t.Columns[acronymPosition]].ToString()));

            }
            dt_totals.Columns.Add(new DataColumn("Total"));

            // fill the pivot table
            for (int _col = 0; _col < t.Columns.Count; _col++)
            {
                int sumCount = 0;
                string colName = t.Columns[_col].ColumnName;
                DataRow dr = dt_totals.NewRow();

                // columns that have a position of 0 should not be displayed
                if (Convert.ToInt32(CECWebSrv.GetCohortWebFieldByColumnName(UserToken, colName).Rows[0]["demographics_position"]) == 0)
                    continue;

                for (int _row = 0; _row < t.Rows.Count; _row++)
                {
                    if (_row < dt_totals.Columns.Count)
                    {
                        /// --------------------------------------------------
                        ///  for version 3, unanswered data points will be -1, transform -1 to N/P (not provided)
                        ///  
                        dr[0] = colName;
                        if (helper.IsNumerical(t.Rows[_row][_col]) && (int)t.Rows[_row][_col] == -1)
                        {
                            dr[_row + 1] = "N/P";
                            continue;
                        }
                        else if (helper.IsNumerical(t.Rows[_row][_col]))
                            dr[_row + 1] = helper.FormatCount((int)t.Rows[_row][_col]);
                        else if (t.Columns[_col].DataType == typeof(DateTime))
                            dr[_row + 1] = DateTime.Parse(t.Rows[_row][_col].ToString()).ToString("MM/dd/yyyy");
                        else
                            dr[_row + 1] = t.Rows[_row][_col];

                        if (helper.IsNumerical(t.Rows[_row][_col]))
                            sumCount += (int)t.Rows[_row][_col];
                    }
                }

                if (t.Columns[_col].DataType == typeof(int))
                    dr[dt_totals.Columns.Count - 1] = helper.FormatCount(sumCount);

                dt_totals.Rows.Add(dr);
            }

            ///---------------------------------------------------------------------------
            /// bind summary totals
            /// 
            enrollTblSum.DataSource = dt_totals;
            enrollTblSum.DataBind();
        }

        private void PopulateUnknownEnrollmentGrid()
        {
            _curRace = string.Empty;
            _curEthnic = string.Empty;

            if (CohortIDsToCompare.Length == 0 || Ethnicities.Length == 0 || Races.Length == 0)
                return;

            //-----
            // bind enrollment grid data
            System.Data.DataSet sg;
            if ((CohortIDsToCompare != null) && (CohortIDsToCompare.Length > 0))
                sg = CECWebSrv.GetCohortForEnrollmentGrid(UserToken, CohortIDsToCompare, "unknown", Races, Ethnicities);
            else
            {
                sg = new DataSet();
                sg.Tables.Add("tbl_enrollment");
            }

            DataTable t = sg.Tables["tbl_enrollment"];

            // dt_unknown is instantiated in OnInit()
            //dt_unknown = new DataTable();
            //dt_unknown.TableName = "dt_unknown";

            // add empty column at index 0
            dt_unknown.Columns.Add(new DataColumn());
            dt_unknown.Columns[0].ColumnName = "Race";

            // find acronym position and build the pivot table column names
            int acronymPosition = t.Columns.IndexOf("cohort_acronym");
            for (int _p = 0; _p < t.Rows.Count; _p++)
            {
                if (acronymPosition > -1 && dt_unknown.Columns.IndexOf(t.Rows[_p][acronymPosition].ToString()) == -1)
                    dt_unknown.Columns.Add(new DataColumn(t.Rows[_p][t.Columns[acronymPosition]].ToString()));

            }
            dt_unknown.Columns.Add(new DataColumn("Total"));

            // fill the pivot table
            for (int _col = 0; _col < t.Columns.Count; _col++)
            {
                int sumCount = 0;
                string colName = t.Columns[_col].ColumnName;
                DataRow dr = dt_unknown.NewRow();

                //---
                // columns that have a position of 0 should not be displayed
                if (Convert.ToInt32(CECWebSrv.GetCohortWebFieldByColumnName(UserToken, colName).Rows[0]["demographics_position"]) == 0)
                    continue;

                dr[0] = colName;
                for (int _row = 0; _row < t.Rows.Count; _row++)
                {
                    //if (_row < (dt_unknown.Columns.Count))
                    //{
                        /// --------------------------------------------------
                        ///  for version 3, unanswered data points will be -1, transform -1 to N/P (not provided)
                        ///  
                        if (helper.IsNumerical(t.Rows[_row][_col]) && (int)t.Rows[_row][_col] == -1)
                        {
                            dr[_row + 1] = "N/P";
                            continue;
                        }
                        else if (helper.IsNumerical(t.Rows[_row][_col]))
                            dr[_row + 1] = helper.FormatCount((int)t.Rows[_row][_col]);
                        else if (t.Columns[_col].DataType == typeof(DateTime))
                            dr[_row + 1] = DateTime.Parse(t.Rows[_row][_col].ToString()).ToString("MM/dd/yyyy");
                        else
                            dr[_row + 1] = t.Rows[_row][_col];

                        if (helper.IsNumerical(t.Rows[_row][_col]))
                            sumCount += (int)t.Rows[_row][_col];
                    //}
                }

                if (t.Columns[_col].DataType == typeof(int))
                    dr[dt_unknown.Columns.Count - 1] = helper.FormatCount(sumCount);

                dt_unknown.Rows.Add(dr);
            }
        }

        private void PopulateFemaleEnrollmentGrid()
        {
            _curRace = string.Empty;
            _curEthnic = string.Empty;

            if (CohortIDsToCompare.Length == 0 || Ethnicities.Length == 0 || Races.Length == 0)
                return;

            //-----
            // bind enrollment grid data
            System.Data.DataSet sg;
            if ((CohortIDsToCompare != null) && (CohortIDsToCompare.Length > 0))
                sg = CECWebSrv.GetCohortForEnrollmentGrid(UserToken, CohortIDsToCompare, "females", Races, Ethnicities);
            else
            {
                sg = new DataSet();
                sg.Tables.Add("tbl_enrollment");
            }

            DataTable t = sg.Tables["tbl_enrollment"];

            // dt_females instantiated in OnInit()
            //dt_females = new DataTable();
            //dt_females.TableName = "dt_females";

            // add empty column at index 0
            dt_females.Columns.Add(new DataColumn());
            dt_females.Columns[0].ColumnName = "Race";

            // find acronym position and build the pivot table column names
            int acronymPosition = t.Columns.IndexOf("cohort_acronym");
            for (int _p = 0; _p < t.Rows.Count; _p++)
            {
                if (acronymPosition > -1 && dt_females.Columns.IndexOf(t.Rows[_p][acronymPosition].ToString()) == -1)
                    dt_females.Columns.Add(new DataColumn(t.Rows[_p][t.Columns[acronymPosition]].ToString()));

            }
            dt_females.Columns.Add(new DataColumn("Total"));

            // fill the pivot table
            for (int _col = 0; _col < t.Columns.Count; _col++)
            {
                int sumCount = 0;
                string colName = t.Columns[_col].ColumnName;
                DataRow dr = dt_females.NewRow();

                //---
                // columns that have a position of 0 should not be displayed
                if (Convert.ToInt32(CECWebSrv.GetCohortWebFieldByColumnName(UserToken, colName).Rows[0]["demographics_position"]) == 0)
                    continue;

                dr[0] = colName;
                for (int _row = 0; _row < t.Rows.Count; _row++)
                {
                    //if (_row < dt_females.Columns.Count)
                    //{
                        /// --------------------------------------------------
                        ///  for version 3, unanswered data points will be -1, transform -1 to N/P (not provided)
                        ///  
                        
                        if (helper.IsNumerical(t.Rows[_row][_col]) && (int)t.Rows[_row][_col] == -1)
                        {
                            dr[_row + 1] = "N/P";
                            continue;
                        }
                        else if (helper.IsNumerical(t.Rows[_row][_col]))
                            dr[_row + 1] = helper.FormatCount((int)t.Rows[_row][_col]);
                        else if (t.Columns[_col].DataType == typeof(DateTime))
                            dr[_row + 1] = DateTime.Parse(t.Rows[_row][_col].ToString()).ToString("MM/dd/yyyy");
                        else
                            dr[_row + 1] = t.Rows[_row][_col];

                        if (helper.IsNumerical(t.Rows[_row][_col]))
                            sumCount += (int)t.Rows[_row][_col];
                   // }
                }

                if (t.Columns[_col].DataType == typeof(int))
                    dr[dt_females.Columns.Count - 1] = helper.FormatCount(sumCount);

                dt_females.Rows.Add(dr);
            }
        }

        private void PopulateMaleEnrollmentGrid()
        {
            _curRace = string.Empty;
            _curEthnic = string.Empty;

            if (CohortIDsToCompare.Length == 0 || Ethnicities.Length == 0 || Races.Length == 0)
                return;

            //-----
            // bind enrollment grid data
            System.Data.DataSet sg;
            if ((CohortIDsToCompare != null) && CohortIDsToCompare.Length > 0)
                sg = CECWebSrv.GetCohortForEnrollmentGrid(UserToken, CohortIDsToCompare, "males", Races, Ethnicities);
            else
            {
                sg = new DataSet();
                sg.Tables.Add("tbl_enrollment");
            }

            DataTable t = sg.Tables["tbl_enrollment"];

            // dt_males is instantiated in OnInit()
            //dt_males = new DataTable();
            //dt_males.TableName = "dt_males";

            // add empty column at index 0
            dt_males.Columns.Add(new DataColumn());
            dt_males.Columns[0].ColumnName = "Race";

            // find acronym position and build the pivot table column names
            int acronymPosition = t.Columns.IndexOf("cohort_acronym");
            for (int _p = 0; _p < t.Rows.Count; _p++)
            {
                if (acronymPosition > -1 && dt_males.Columns.IndexOf(t.Rows[_p][acronymPosition].ToString()) == -1)
                    dt_males.Columns.Add(new DataColumn(t.Rows[_p][t.Columns[acronymPosition]].ToString()));

            }
            dt_males.Columns.Add(new DataColumn("Total"));

            // fill the pivot table
            for (int _col = 0; _col < t.Columns.Count; _col++)
            {
                int sumCount = 0;
                string colName = t.Columns[_col].ColumnName;
                DataRow dr = dt_males.NewRow();

                //---
                // columns that have a position of 0 should not be displayed
                if (Convert.ToInt32(CECWebSrv.GetCohortWebFieldByColumnName(UserToken, colName).Rows[0]["demographics_position"]) == 0)
                    continue;

                dr[0] = colName;
                for (int _row = 0; _row < t.Rows.Count; _row++)
                {
                    //if (_row < dt_males.Columns.Count)
                    //{

                        /// --------------------------------------------------
                        ///  for version 3, unanswered data points will be -1, transform -1 to N/P (not provided)
                        ///  
                        
                        if (helper.IsNumerical(t.Rows[_row][_col]) && (int)t.Rows[_row][_col] == -1)
                        {
                            dr[_row + 1] = "N/P";
                            continue;
                        }
                        else if (helper.IsNumerical(t.Rows[_row][_col]))
                            dr[_row + 1] = helper.FormatCount((int)t.Rows[_row][_col]);
                        else if (t.Columns[_col].DataType == typeof(DateTime))
                            dr[_row + 1] = DateTime.Parse(t.Rows[_row][_col].ToString()).ToString("MM/dd/yyyy");
                        else
                            dr[_row + 1] = t.Rows[_row][_col];

                        if (helper.IsNumerical(t.Rows[_row][_col]))
                            sumCount += (int)t.Rows[_row][_col];
                    //}
                }

                if (t.Columns[_col].DataType == typeof(int))
                    dr[dt_males.Columns.Count - 1] = helper.FormatCount(sumCount);

                dt_males.Rows.Add(dr);
            }
        }

        private string ExportDataGridToExcel(string savePath)
        {
            /// excel writer row index
            int rowIndex = 0;

            try 
            {
                CECHarmPublicService ps =
                    new CECHarmPublicService();

                NPOI.XSSF.UserModel.XSSFWorkbook wkbk =
                        new XSSFWorkbook();
                NPOI.SS.UserModel.ISheet wkst = wkbk.CreateSheet();
                wkbk.SetActiveSheet(0);

                /// write header to excel
                NPOI.SS.UserModel.IRow dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue(String.Format("Cohort Data Export Generated from the CEDCD Website ({0})", Request.Url.Authority));

                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Table Name:");
                dataRow.CreateCell(1).SetCellValue("Enrollment Counts");

                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Export Date:");
                dataRow.CreateCell(1).SetCellValue(DateTime.Now.ToString("MM/dd/yyyy"));

                rowIndex += 2;

                bool gender_male = false,
                    gender_female = false,
                    gender_unknown = false;
                foreach (string s in gender_list.GetSelectedOptions())
                {
                    if (s == "male")
                        gender_male = true;
                    else if (s == "female")
                        gender_female = true;
                    else if (s == "unknown")
                        gender_unknown = true;
                }

                bool doneAll = false;
                int tblCount = 1;
                string tableName = string.Empty;
                while (!doneAll)
                {
                    System.Data.DataTable toExport = new DataTable();
                    switch (tblCount)
                    {
                        //case 0:
                        //    if (dt_totals == null)
                        //        PopulateSumEnrollmentGrid();

                        //    toExport = dt_totals;
                        //    tableName = "Enrollment: Totals";
                        //    break;
                        case 1:
                            if (!gender_male)
                                break;

                            PopulateMaleEnrollmentGrid();

                            toExport = dt_males;
                            tableName = "Enrollment: Males";
                            break;
                        case 2:
                            if (!gender_female)
                                break;

                            PopulateFemaleEnrollmentGrid();

                            toExport = dt_females;
                            tableName = "Enrollment: Females";
                            break;
                        case 3:
                            if (!gender_unknown)
                                break;

                            PopulateUnknownEnrollmentGrid();

                            toExport = dt_unknown;
                            tableName = "Enrollment: Unknown";
                            break;
                        default:
                            doneAll = true;
                            break;
                    }
                    if (doneAll)
                        break;

                    if(tblCount > 1)
                        rowIndex += 2;
                    tblCount++;

                    if (toExport.Rows.Count == 0)
                        continue;

                    ///--------------------------------------------------------
                    /// special for enrollment tables: table titles
                    /// 
                    NPOI.SS.UserModel.IRow titleRow = wkst.CreateRow(rowIndex++);
                    titleRow.CreateCell(0).SetCellValue(tableName);
 
                    ///--------------------------------------------------------
                    /// column headers
                    /// 
                    NPOI.SS.UserModel.IRow headerRow = wkst.CreateRow(rowIndex++);
                    if (tblCount > 0 && !tableName.ToLower().Contains("totals"))
                    {
                        ICell eHdr = headerRow.CreateCell(0);
                        eHdr.SetCellValue("Ethnicity");
                    }

                    for (int _c = 0; _c < toExport.Columns.Count; _c++)
                    {
                        ICell c = headerRow.GetCell((_c + 1));
                        if (c == null)
                            c = headerRow.CreateCell((_c + 1));

                        c.SetCellValue(toExport.Columns[_c].ColumnName);
                    }

                    /// data rows
                    for (int _i = 0; _i < toExport.Rows.Count; _i++)
                    {
                        int colPos = 0;

                        /// create data row object then step through each cell to populate the excel row
                        dataRow = wkst.CreateRow(rowIndex++);

                        ///-----------------------------------------------
                        /// ethnic cell
                        /// 
                        ICell eCell = dataRow.CreateCell(colPos++);

                        string _ethnic = toExport.Rows[_i][0].ToString();
                        _ethnic = _ethnic.Split('_')[2];
                        switch (_ethnic)
                        {
                            case "nonhispanic":
                                eCell.SetCellValue("Not Hispanic or Latino");
                                break;
                            case "hispanic":
                                eCell.SetCellValue("Hispanic or Latino");
                                break;
                            case "unknown":
                                eCell.SetCellValue("Unknown/Not Reported Ethnicity");
                                break;
                            default:
                                colPos--;
                                break;
                        }

                        for (int _p = 0; _p < toExport.Columns.Count; _p++)
                        {
                            /// get first cell and check for null, if null create cell
                            ICell c = dataRow.GetCell(colPos);
                            if (c == null)
                                c = dataRow.CreateCell(colPos);

                            string cellVal = toExport.Rows[_i][_p].ToString();
                            if (helper.IsStringEmptyWhiteSpace(cellVal) || cellVal == "&nbsp;")
                                c.SetCellValue(" ");
                            else if (!helper.IsNumerical(cellVal) && CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, cellVal) != string.Empty)
                                c.SetCellValue(CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, cellVal));
                            else
                                c.SetCellValue(cellVal);

                            colPos++;
                        }
                    }

                    ///-----------------------
                    /// was to handle merge fields
                    ///                             
                    /// handle column spans
                    //if (t.Rows[_i].Cells[_p].ColumnSpan > 0)
                    //{
                    //    int colmerge = colPos + t.Rows[_i].Cells[_p].ColumnSpan - 1;
                    //    wkst.AddMergedRegion(new CellRangeAddress(rowIndex - 1, rowIndex - 1, colPos, colmerge));

                    //    colPos = colmerge;
                    //}
                }

                /// write output
                FileStream fs = new FileStream(savePath, FileMode.Create);
                wkbk.Write(fs);
                fs.Close();

                return savePath;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}