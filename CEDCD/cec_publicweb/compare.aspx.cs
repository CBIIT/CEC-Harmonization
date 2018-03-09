using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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



    public partial class compare : CECPage
    {
        private helper help;

        private System.Data.DataTable compareTbl;
        private System.Data.DataTable webfieldsTbl;
        private System.Data.DataTable tabsTbl;

        private int _webfieldPosition;

        private bool shouldRefreshGrid;
        
        #region Properties

        protected int SelectedTabID
        {
            get
            {
                if (ViewState["SelectedTabID"] == null)
                    SelectedTabID = 1;

                return (int)ViewState["SelectedTabID"];
            }
            set
            {
                ViewState["SelectedTabID"] = value;
            }
        }

        protected List<int> ExpandedFieldIds
        {
            get
            {
                if (ViewState["ExpandedFieldIds"] == null)
                    ExpandedFieldIds = new List<int>();

                return (List<int>)ViewState["ExpandedFieldIds"];
            }
            set
            {
                ViewState["ExpandedFieldIds"] = value;
            }
        }

        protected string[] CompareTabNames
        {
            get
            {
                return new string[] { "Basic Cohort Information", "Data Collected at Baseline", "Data Collected at Followup", "Cancer Information",
                           "Mortality", "Linkages and Technology", "Specimen Overview" };
            }
        }

        protected string[] RaceTypeOfParticipantFilters
        {
            get
            {
                if (Session["wf_typeRace_filter_selections"] != null)
                    return (string[])Session["wf_typeRace_filter_selections"];
                else
                    return new string[0];
            }
        }

        protected string[] GenderTypeOfParticipantFilters
        {
            get
            {
                if (Session["wf_typeGender_filter_selections"] != null)
                    return (string[])Session["wf_typeGender_filter_selections"];
                else
                    return new string[0];
            }
        }


        protected string[] EthnicityTypeOfParticipantFilters
        {
            get
            {
                if (Session["wf_typeEthnic_filter_selections"] != null)
                    return (string[])Session["wf_typeEthnic_filter_selections"];
                else
                    return new string[0];
            }
        }

        protected string[] AgeOfParticipantFilters
        {
            get
            {
                if (Session["wf_age_filter_selections"] != null)
                    return (string[])Session["wf_age_filter_selections"];
                else
                    return new string[0];
            }
        }

        protected string[] DataCollectedFilters
        {
            get
            {
                if (Session["wf_collectData_filter_selections"] != null)
                    return (string[])Session["wf_collectData_filter_selections"];
                else
                    return new string[0];
            }
        }

        protected string[] SpecimenCollectedFilters
        {
            get
            {
                if (Session["wf_collectSpecimen_filter_selections"] != null)
                    return (string[])Session["wf_collectSpecimen_filter_selections"];
                else
                    return new string[0];
            }
        }

        protected string[] CancerCollectedFilters
        {
            get
            {
                if (Session["wf_cancerCollect_filter_selections"] != null)
                    return (string[])Session["wf_cancerCollect_filter_selections"];
                else
                    return new string[0];
            }
        }

        protected string[] StudyDesignFilters
        {
            get
            {
                if (Session["wf_design_filter_selections"] != null)
                    return (string[])Session["wf_design_filter_selections"];
                else
                    return new string[0];
            }
        }

        protected string[] MembershipFilters
        {
            get
            {
                if (Session["wf_member_filter_selections"] != null)
                    return (string[])Session["wf_member_filter_selections"];
                else
                    return new string[0];
            }
        }
        #endregion

        /// <summary>
        /// create the necessary childcontrols for filtering
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            compareGridView.ClientIDMode = ClientIDMode.Static;
        }

        #region EventHandling

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (Request.QueryString != null && Request.QueryString.AllKeys.Contains("tab"))
                SelectedTabID = int.Parse(Request.QueryString["tab"]);

            if(!IsPostBack)
                CECWebSrv.AuditLog_AddActivity(UserToken.userid, String.Format("compare page::tab {0}", SelectedTabID));

            // populate filter selections
            PopulateFilterLabels();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            shouldRefreshGrid = false;
            help = new helper();

            compareTbl = new DataTable();
            compareTbl.TableName = "compareTable";            
            
            ///---------------
            ///retreive tabs
            ///
            tabsTbl = CECWebSrv.GetTabs(UserToken);
        }

        protected override void OnPreRender(EventArgs e)
        {
            //base.OnPreRender(e);

            //TODO: remove the commented code below. now that navigation is handled as a user control
            //for (int _i = 1; _i <= 7; _i++)
            //{
            //    System.Web.UI.HtmlControls.HtmlGenericControl item = (HtmlGenericControl)FindControl(String.Format("sub{0}", _i));
            //    if (item.Attributes["class"] == null)
            //        item.Attributes.Add("class", "");

            //    string css = "active";
            //    if ((SelectedTabID != _i) && item.Attributes["class"].Contains(css))
            //        item.Attributes["class"] = item.Attributes["class"].Replace(css, "");
            //    else if (SelectedTabID == _i)
            //        item.Attributes["class"] += String.Format(" {0} ", css);
            //}

            DataRow[] drs = tabsTbl.Select(String.Format("tab_id={0}", SelectedTabID));
            if (drs.Length > 0)
                tabLabel.InnerText = drs[0]["tab_label"].ToString();
            
            PopulateCompareGrid();

            exportTblBtn.Visible = (compareTbl.Rows.Count > 0 ? true : false);

            if ((Session["CohortIDsToCompare"] != null) && ((string[])Session["CohortIDsToCompare"]).Length > 0)
                ClientScript.RegisterStartupScript(GetType(), "expandFirstSection", "<script type='text/javascript'>expandFirstSection();</script>");
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool handled = false;
            if (args is CommandEventArgs)
            {
                CommandEventArgs cea = (args as CommandEventArgs);
                switch (cea.CommandName)
                {
                    //case "tabNav":
                        //handled = true;

                        //SelectedTabID = Convert.ToInt32(cea.CommandArgument);
                        //PopulateCompareGrid();
                        //break;
                    case "export":
                        handled = true;

                        //if (compareTbl == null)
                            PopulateCompareGrid();

                        string filepath = String.Format("./user_files/{0}/compare{1}_{2}.xlsx", UserToken.userid, SelectedTabID, DateTime.Now.ToString("yyyyMMMddmm"));

                        ExportDataGridToExcel(compareTbl, MapPath(filepath));
                        CECWebSrv.AuditLog_AddActivity(UserToken.userid, "compare export created");

                        Page.ClientScript.RegisterStartupScript(GetType(), "downloadExport",
                            String.Format("<script>window.open('{0}');</script>", filepath));

                        break;
                }

                return handled;
            }
            else
                return base.OnBubbleEvent(source, args);
        }

        protected void compareGridView_RowDataBound(object source, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int _i = 0; _i < e.Row.Cells.Count; _i++)
                {
                    System.Web.UI.WebControls.TableHeaderCell hc =
                        new TableHeaderCell();
                    hc.ID = compareTbl.Columns[_i].ColumnName.Replace(" ", string.Empty);

                    if (CECWebSrv.IsCohort(UserToken, e.Row.Cells[_i].Text))
                    {
                        DataRow dr_cohort = null;
                        using(DataTable dt = CECWebSrv.GetFilteredCohortRecords(UserToken, "cohort_id, cohort_name, cohort_acronym", String.Format("cohort_acronym='{0}'", compareTbl.Columns[_i].ColumnName)).Tables[0])
                            dr_cohort = dt.Rows[0];

                        System.Web.UI.HtmlControls.HtmlAnchor cohortLnk =
                            new HtmlAnchor();
                        cohortLnk.HRef = String.Format("./cohortDetails.aspx?cohort_id={0}", dr_cohort["cohort_id"]); ;
                        cohortLnk.Attributes["tabindex"] = "0";
                        cohortLnk.Attributes["role"] = "button";
                        cohortLnk.Attributes["data-toggle"] = "popover";
                        cohortLnk.Attributes["data-trigger"] = "focus";
                        cohortLnk.Attributes["data-placement"] = "top";
                        cohortLnk.Attributes["data-trigger"] = "hover";
                        cohortLnk.Attributes["data-content"] = String.Format("{0}", dr_cohort["cohort_name"]);
                        cohortLnk.InnerHtml = helper.HTMLEncode(e.Row.Cells[_i].Text);

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

                e.Row.ID = "sticker";
                e.Row.TableSection = TableRowSection.TableHeader;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow[] dra = webfieldsTbl.Select(String.Format("data_field='{0}'", e.Row.Cells[0].Text));
                DataRow dr = dra[0];

                int wholeNumber = (int)double.Parse(dr["compare_position"].ToString());
                if (_webfieldPosition != wholeNumber)
                {
                    _webfieldPosition = wholeNumber;

                    // add merged cells above this position to display as groupings
                    System.Web.UI.WebControls.TableHeaderCell tc1 =
                        new TableHeaderCell();
                    tc1.ColumnSpan = e.Row.Cells.Count;
                    tc1.ID = webfieldsTbl.Select("compare_position=" + _webfieldPosition)[0]["field_label"].ToString().Replace(" ", String.Empty);
                    tc1.Attributes["headers"] = compareTbl.Columns[0].ColumnName.Replace(" ", string.Empty);
                    tc1.Attributes["tabindex"] = "0";
                    tc1.CssClass = "compareGroup-header";
                    tc1.Controls.Add(new LiteralControl(webfieldsTbl.Select("compare_position=" + _webfieldPosition)[0]["field_label"].ToString()));

                    // expandable/collapsable sections
                    int rows = webfieldsTbl.Select(String.Format("compare_position > {0} and compare_position < ({0} + 1) and compare_parent_id=0", wholeNumber)).Length;
                    System.Web.UI.HtmlControls.HtmlAnchor expand =
                        new HtmlAnchor();
                    expand.ID = dr["id"].ToString();
                    expand.Attributes["class"] = "section-expand";
                    expand.Attributes["target-rows"] = rows.ToString();
                    tc1.Controls.Add(expand);

                    System.Web.UI.WebControls.GridViewRow gvr =
                        new GridViewRow(e.Row.RowIndex, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                    gvr.Cells.AddAt(0, tc1);

                    int lastIndex = compareGridView.Controls[0].Controls.Count - 1;
                    compareGridView.Controls[0].Controls.AddAt(lastIndex, gvr);
                }

                string columnName = e.Row.Cells[0].Text;
                //string _curSumLbl = orgColumnName.Split('.')[1];

                // current grouping
                string _curGroupLbl = string.Empty;
                if (_webfieldPosition > 0)
                    _curGroupLbl = webfieldsTbl.Select("compare_position=" + _webfieldPosition)[0]["field_label"].ToString().Replace(" ", string.Empty);

                ///---------------------------------------------------
                /// change to tableheadercell
                /// 
                System.Web.UI.WebControls.TableHeaderCell nmHdr =
                    new TableHeaderCell();
                nmHdr.ID = columnName;
                nmHdr.Controls.Add(new LiteralControl(CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, columnName)));

                ///---------------------------------------------------
                /// add control for field description
                /// 
                if (IsFieldDescriptionEnabled(columnName))
                {
                    nmHdr.Controls.Add(FieldDescriptionShowButton(columnName));
                    Page.Controls.Add(FieldDescriptionModalDialog(columnName));
                }

                if ((dr["compare_parent_id"] == DBNull.Value || (int)dr["compare_parent_id"] == 0) && (dr["compare_expandable"] == DBNull.Value || (bool)dr["compare_expandable"] == false))
                    e.Row.CssClass = "compare-row compare-section-hidden";

                if ((int)dr["compare_parent_id"] != 0)
                {
                    nmHdr.CssClass = "compareChildRecord";
                    e.Row.CssClass = "compare-child-row-hidden compare-child-row";
                }

                if (dr["compare_expandable"] != DBNull.Value && Convert.ToBoolean(dr["compare_expandable"]))
                {
                    int rows = webfieldsTbl.Select(String.Format("compare_parent_id={0}", dr["id"])).Length;

                    System.Web.UI.HtmlControls.HtmlAnchor expand =
                        new HtmlAnchor();
                    expand.ID = dr["id"].ToString();
                    expand.Attributes["class"] = "row-expand";
                    expand.Attributes["target-rows"] = rows.ToString();
                    //expand.Click +=
                    //    new EventHandler(Expandable_Clicked);


                    if (ExpandedFieldIds.IndexOf(Convert.ToInt32(dr["id"])) > -1)
                    {
                        //expand.CssClass += " active";
                        //expand.ImageUrl = "/images/minus.gif";

                        ///------------------------------
                        /// tab ids are added in a first in/last out fashion
                        /// 
                        if (expand.ID == ExpandedFieldIds[ExpandedFieldIds.Count - 1].ToString())
                            SetFocus(nmHdr);
                    }
                    // else
                    //     expand.ImageUrl = "/images/plus.gif";

                    nmHdr.Controls.Add(expand);

                    e.Row.CssClass = "compare-parent-row compare-section-hidden";
                }

                e.Row.Cells.RemoveAt(0);
                e.Row.Cells.AddAt(0, nmHdr);

                ///--------------------------------------------------
                /// the data cells: add header cell ids.
                for (int _i = 0; _i < e.Row.Cells.Count; _i++)
                {
                    if (_i == 0)
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1}", _curGroupLbl, compareTbl.Columns[_i].ColumnName.Replace(" ", string.Empty));
                    else
                    {
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curGroupLbl, columnName, compareTbl.Columns[_i].ColumnName.Replace(" ", string.Empty));

                        e.Row.Cells[_i].Text = Server.HtmlDecode(e.Row.Cells[_i].Text);
                    }
                }

                //if (dr["compare_parent_id"] != DBNull.Value)
                //{
                //e.Row.Cells[1].CssClass = "childTextLabel";

                //if (Convert.ToInt32(dr["field_order"]) % 2 == 0)
                //    e.Row.CssClass = " childText_AltEven";
                //else
                //    e.Row.CssClass = " childText_AltOdd";
                //}
                //else
                //    e.Row.Cells[1].CssClass = "textLabel";
                // e.Row.Cells[1].Text = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, colName);

                //for (int _i = 2; _i < e.Row.Cells.Count; _i++)
                //    e.Row.Cells[_i].CssClass = "numberLabel";

            }
        }
        #endregion

        private void PopulateFilterLabels()
        {
            bool anyFilters = false;

            if (AgeOfParticipantFilters.Length > 0 || EthnicityTypeOfParticipantFilters.Length > 0 || RaceTypeOfParticipantFilters.Length > 0 || GenderTypeOfParticipantFilters.Length > 0)
            {
                anyFilters = true;

                fl_include.Visible = true;
                fl_include.Controls.Add(new LiteralControl("<li class=\"text-nowrap\"><b>Type of Participant</b></li>"));
                foreach (string s in GenderTypeOfParticipantFilters)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    //li.Attributes["class"] = "text-nowrap";
                    li.InnerText = s;
                    fl_include.Controls.Add(li);
                }
                foreach (string s in EthnicityTypeOfParticipantFilters)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    //li.Attributes["class"] = "text-nowrap";
                    li.InnerText = s;
                    fl_include.Controls.Add(li);
                }

                foreach (string s in RaceTypeOfParticipantFilters)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    //li.Attributes["class"] = "text-nowrap";
                    li.InnerText = s;
                    fl_include.Controls.Add(li);
                }

                foreach (string s in AgeOfParticipantFilters)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    //li.Attributes["class"] = "text-nowrap";
                    li.InnerText = s;
                    fl_include.Controls.Add(li);
                }
            }

            if (DataCollectedFilters.Length > 0)
            {
                anyFilters = true;

                fl_collectData.Visible = true;
                fl_collectData.Controls.Add(new LiteralControl("<li class=\"text-nowrap\"><b>Data Collected</b></li>"));
                foreach (string s in DataCollectedFilters)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    //li.Attributes["class"] = "text-nowrap";
                    li.InnerText = s;
                    fl_collectData.Controls.Add(li);
                }
            }

            if (SpecimenCollectedFilters.Length > 0)
            {
                anyFilters = true;

                fl_collectSpecimen.Visible = true;
                fl_collectSpecimen.Controls.Add(new LiteralControl("<li class=\"text-nowrap\"><b>Specimens Collected</b></li>"));
                foreach (string s in SpecimenCollectedFilters)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    //li.Attributes["class"] = "text-nowrap";
                    li.InnerText = s;
                    fl_collectSpecimen.Controls.Add(li);
                }
            }

            if (CancerCollectedFilters.Length > 0)
            {
                anyFilters = true;

                fl_cancer.Visible = true;
                fl_cancer.Controls.Add(new LiteralControl("<li class=\"text-nowrap\"><b>Cancer Collected</b></li>"));
                foreach (string s in CancerCollectedFilters)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    //li.Attributes["class"] = "text-nowrap";
                    li.InnerText = s;
                    fl_cancer.Controls.Add(li);
                }
            }

            if (StudyDesignFilters.Length > 0)
            {
                anyFilters = true;

                fl_design.Visible = true;
                fl_design.Controls.Add(new LiteralControl("<li class=\"text-nowrap\"><b>Study Design</b></li>"));
                foreach (string s in StudyDesignFilters)
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    //li.Attributes["class"] = "text-nowrap";
                    li.InnerText = s;
                    fl_design.Controls.Add(li);
                }
            }

            if (!anyFilters)
            {
                fl_include.Visible = true;
                fl_include.Controls.Add(new LiteralControl("<li class=\"text-nowrap\">No Filters Selected</li>"));
            }
        }

        private void PopulateCompareGridWithChildren(int parentFieldId)
        {
            if (Session["CohortIDsToCompare"] == null)
                throw new Exception("never would have thought you could get here without having Session.CohortIDsToCompare already defined");

            //-----
            // bind summary grid data
            System.Data.DataSet sg = CECWebSrv.GetCohortForCompareGridByParentID(UserToken, (string[])Session["CohortIDsToCompare"], parentFieldId);

            DataTable t = sg.Tables["tbl_web_cohorts_v4_0"];

            // fill the pivot table
            for (int _col = 0; _col < t.Columns.Count; _col++)
            {
                /// support for virtual fields
                DataTable cohortFieldsTbl = CECWebSrv.GetCohortWebFieldByColumnName(UserToken, t.Columns[_col].ColumnName);
                //---
                // columns that have a position of 0 should not be displayed
                if ((DBNull.Value == cohortFieldsTbl.Rows[0]["compare_position"]) || Convert.ToInt32(cohortFieldsTbl.Rows[0]["compare_position"]) == 0)
                    continue;

                DataRow dr = compareTbl.NewRow();
                dr[0] = t.Columns[_col].ColumnName;
                for (int _row = 0; _row < t.Rows.Count; _row++)
                {
                    /// --------------------------------------------------
                    ///  for version 3, unanswered data points will be blank (''), null or -1, transform to N/P (not provided)
                    ///---------------------------------------------------
                    /// (BUGNET: CEC-5278)  [SUPERSEDED]
                    /// 
                    if (DBNull.Value == t.Rows[_row][_col] || helper.IsStringEmptyWhiteSpace(t.Rows[_row][_col].ToString()) || t.Rows[_row][_col].ToString() == "-1")
                        dr[_row + 1] = "N/P";
                    //else if ((SelectedTabID == 6) && t.Columns[_col].ColumnName == "tech_use_of_mobile" || t.Columns[_col].ColumnName == "tech_use_of_cloud")
                    //{
                    //    string v = t.Rows[_row][_col].ToString();
                    //    if (Convert.ToInt32(v) == 0)
                    //        dr[_row + 1] = "**Yes**";
                    //    else if (Convert.ToInt32(v) == 1)
                    //        dr[_row + 1] = "**No but considering it**";
                    //    else
                    //        dr[_row + 1] = "**No and no plans to do so**";
                    //}
                    else if (helper.IsNumerical(t.Rows[_row][_col]) && !(t.Columns[_col].ColumnName.ToLower().Contains("date") || t.Columns[_col].ColumnName.ToLower().Contains("year")))
                    {
                        int num = Convert.ToInt32(t.Rows[_row][_col]);
                        dr[_row + 1] = helper.FormatCount(num);
                    }
                    else if (t.Columns[_col].DataType == typeof(DateTime))
                        dr[_row + 1] = DateTime.Parse(t.Rows[_row][_col].ToString()).ToString("MM/dd/yyyy");
                    //else if (t.Columns[_col].DataType == typeof(Boolean))
                    //{
                    //    if (Convert.ToBoolean(t.Rows[_row][_col]))
                    //        dr[_row + 1] = "**Yes**";
                    //    else
                    //        dr[_row + 1] = "**No**";
                    //}
                    //else if (help.IsStringEmptyWhiteSpace(t.Rows[_row][_col].ToString()))
                    //    dr[_row + 1] = "**N/A**";
                    else
                        dr[_row + 1] = t.Rows[_row][_col];
                }
                compareTbl.Rows.Add(dr);
            }
        }

        private void PopulateCompareGrid()
        {
            if ((Session["CohortIDsToCompare"] == null) || ((string[])Session["CohortIDsToCompare"]).Length <= 0)
                Response.Redirect("./cohortSelect.aspx");

            compareGridView.Attributes["has_results"] = "true";

            //-----
            // reset the position integer
            _webfieldPosition = 0;

            //-----
            // retreive field display settings
            webfieldsTbl = CECWebSrv.GetCohortWebFieldsForCompareGrid(UserToken, SelectedTabID).Tables[0];

            //-----
            // bind summary grid data
            System.Data.DataSet sg;
            if ((Session["CohortIDsToCompare"] != null) && ((string[])Session["CohortIDsToCompare"]).Length > 0)
                sg = CECWebSrv.GetCohortForCompareGridByTabId(UserToken, (string[])Session["CohortIDsToCompare"], SelectedTabID);
            else
            {
                sg = new DataSet();
                sg.Tables.Add("tbl_web_cohorts_v4_0");
            }

            DataTable t = sg.Tables["tbl_web_cohorts_v4_0"];
            
            // compareTbl instantiated in OnInit()
            //compareTbl = new DataTable();
            //compareTbl.TableName = "compareTable";

            // add empty column at index 0
            compareTbl.Columns.Add(new DataColumn());

            if (SelectedTabID == 2)
                compareTbl.Columns[0].ColumnName = "Data Collected at Baseline";
            else if (SelectedTabID == 3)
                compareTbl.Columns[0].ColumnName = "Data Collected at Followup";
            else if (SelectedTabID == 4)
                compareTbl.Columns[0].ColumnName = "Cancer Information";
            else if (SelectedTabID == 7)
                compareTbl.Columns[0].ColumnName = "Specimens Collected";
            else
                compareTbl.Columns[0].ColumnName = "Data Collected";

            // find acronym position and build the pivot table column names
            int acronymPosition = t.Columns.IndexOf("cohort_acronym");
            for (int _p = 0; _p < t.Rows.Count; _p++)
            {
                if (acronymPosition > -1 && compareTbl.Columns.IndexOf(t.Rows[_p][acronymPosition].ToString()) == -1)
                    compareTbl.Columns.Add(new DataColumn(t.Rows[_p][acronymPosition].ToString()));
            }

            // fill the pivot table
            for (int _col = 0; _col < t.Columns.Count; _col++)
            {
                DataTable cohortFieldsTbl = CECWebSrv.GetCohortWebFieldByColumnName(UserToken, t.Columns[_col].ColumnName);
                //---
                // columns that have a position of 0 should not be displayed
                if (cohortFieldsTbl.Rows.Count == 0 || DBNull.Value == cohortFieldsTbl.Rows[0]["compare_position"] || Convert.ToInt32(cohortFieldsTbl.Rows[0]["compare_position"]) == 0)
                    continue;

                //--- 
                // child fields should not be displayed unless explicitly called on in the routine
                // further down
                //if (cohortFieldsTbl.Rows[0]["compare_parent_id"] != DBNull.Value && (int)cohortFieldsTbl.Rows[0]["compare_parent_id"] != 0)
                //    continue;

                DataRow dr = compareTbl.NewRow();
                dr[0] = t.Columns[_col].ColumnName;
                for (int _row = 0; _row < t.Rows.Count; _row++)
                {
                    /// --------------------------------------------------
                    ///  for version 3, unanswered data points will be blank (''), null or -1, transform to N/P (not provided)
                    ///---------------------------------------------------
                    /// (BUGNET: CEC-5278)  [SUPERSEDED]
                    /// 
                    if (DBNull.Value == t.Rows[_row][_col] || helper.IsStringEmptyWhiteSpace(t.Rows[_row][_col].ToString()) || t.Rows[_row][_col].ToString() == "-1")
                        dr[_row + 1] = "N/P";
                    //else if ((SelectedTabID == 6) && t.Columns[_col].ColumnName == "tech_use_of_mobile" || t.Columns[_col].ColumnName == "tech_use_of_cloud")
                    //{
                    //    string v = t.Rows[_row][_col].ToString();
                    //    if (Convert.ToInt32(v) == 0)
                    //        dr[_row + 1] = "**Yes**";
                    //    else if (Convert.ToInt32(v) == 1)
                    //        dr[_row + 1] = "**No but considering it**";
                    //    else
                    //        dr[_row + 1] = "**No and no plans to do so**";
                    //}
                    else if (helper.IsNumerical(t.Rows[_row][_col]) && !(t.Columns[_col].ColumnName.ToLower().Contains("date") || t.Columns[_col].ColumnName.ToLower().Contains("year")))
                    {
                        /// was checking for -1s here, is this necessary?
                        int num = Convert.ToInt32(t.Rows[_row][_col]);
                        dr[_row + 1] = helper.FormatCount(num);
                    }
                    else if (t.Columns[_col].DataType == typeof(DateTime))
                        dr[_row + 1] = DateTime.Parse(t.Rows[_row][_col].ToString()).ToString("MM/dd/yyyy");
                    //else if (t.Columns[_col].DataType == typeof(Boolean))
                    //{
                    //    if (Convert.ToBoolean(t.Rows[_row][_col]))
                    //        dr[_row + 1] = "**Yes**";
                    //    else
                    //        dr[_row + 1] = "**No**";
                    //}
                    else
                        dr[_row + 1] = t.Rows[_row][_col];
                }
                compareTbl.Rows.Add(dr);

                //if (ExpandedFieldIds.IndexOf(Convert.ToInt32(cohortFieldsTbl.Rows[0]["id"])) > -1)
                    PopulateCompareGridWithChildren((int)cohortFieldsTbl.Rows[0]["id"]);
            }

            compareGridView.DataSource = compareTbl;
            compareGridView.DataBind();
        }

        private string ExportDataGridToExcel(DataTable toExport, string savePath)
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
                ///                
                NPOI.SS.UserModel.IRow dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue(String.Format("Cohort Data Export Generated from the CEDCD Website ({0})", Request.Url.Authority));

                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Table Name:");
                dataRow.CreateCell(1).SetCellValue(String.Format("Cohort Overview Tab: {0}", CompareTabNames[SelectedTabID - 1]));

                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Export Date:");
                dataRow.CreateCell(1).SetCellValue(DateTime.Now.ToString("MM/dd/yyyy"));

                rowIndex += 2;

                ///--------------------------------------------------------
                /// column headers
                /// 
                NPOI.SS.UserModel.IRow headerRow = wkst.CreateRow(rowIndex++);
                for (int _c = 0; _c < toExport.Columns.Count; _c++)
                {
                    ICell c = headerRow.GetCell(_c);
                    if (c == null)
                        c = headerRow.CreateCell(_c);

                    c.SetCellValue(toExport.Columns[_c].ColumnName);
                }

                /// data rows
                for (int _i = 0; _i < toExport.Rows.Count; _i++)
                {
                    int colPos = 0;

                    /// create data row object then step through each cell to populate the excel row
                    dataRow = wkst.CreateRow(rowIndex++);
                    for (int _p = 0; _p < toExport.Columns.Count; _p++)
                    {
                        /// get first cell and check for null, if null create cell
                        ICell c = dataRow.GetCell(colPos);
                        if (c == null)
                            c = dataRow.CreateCell(colPos);

                        string cellVal = toExport.Rows[_i][_p].ToString();
                        if (helper.IsStringEmptyWhiteSpace(cellVal) || cellVal == "&nbsp;" || cellVal == "-1")
                            c.SetCellValue("N/P");
                        else if (!helper.IsNumerical(cellVal) && CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, cellVal) != string.Empty)
                            c.SetCellValue(CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, cellVal));
                        else
                            c.SetCellValue(cellVal);

                        colPos++;
                    }
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

        #region Derelict Code

        //private void Expandable_Clicked(object sender, EventArgs ea)
        //{
        //    shouldRefreshGrid = true;

        //    if (sender is LinkButton)
        //    {
        //        int id = Convert.ToInt32((sender as LinkButton).ID);
        //        if (ExpandedFieldIds.IndexOf(id) > -1)
        //            ExpandedFieldIds.Remove(id);
        //        else
        //            ExpandedFieldIds.Add(id);
        //    }
        //}
        #endregion
    }
}