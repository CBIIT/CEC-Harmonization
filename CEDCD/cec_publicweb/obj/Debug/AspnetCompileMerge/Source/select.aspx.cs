using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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


    public partial class summary : CECPage
    {
        private bool redirect;
        private string redirectionUrl,
            checkboxToGetFocus;

        private helper help;

        private System.Data.DataTable summaryTable;
        private System.Web.UI.WebControls.CheckBox select_all;

        /// <summary>
        /// create the necessary childcontrols for filtering
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            summaryGridView.ClientIDMode = ClientIDMode.Static;

            /// DEPRECATED CODE
            //wf_include.SelectedNodeChanged +=
            //    new EventHandler(SelectedNodeChanged);
            //wf_collect.SelectedNodeChanged +=
            //    new EventHandler(SelectedNodeChanged);
            //wf_design.SelectedNodeChanged +=
            //    new EventHandler(SelectedNodeChanged);
            //wf_cancerCollect.SelectedNodeChanged +=
            //    new EventHandler(SelectedNodeChanged);
            //wf_member.SelectedNodeChanged +=
            //    new EventHandler(SelectedNodeChanged);

            ///-------------------------------------------------------------------------
            /// all check toggle
            select_all = new CheckBox();
            select_all.CssClass = "select-checkbox";
            select_all.ID = "compare_all";
            select_all.AutoPostBack = true;


            //if (cohortVisibleCount.Items.Count == 0)
            //{
            //    cohortVisibleCount.Items.Add(new ListItem("15", "15"));

            //    for (int _i = 25; _i <= 75; _i += 25)
            //        cohortVisibleCount.Items.Add(new ListItem(_i.ToString(), _i.ToString()));

            //    cohortVisibleCount.Items.Add(new ListItem("All", "1000"));
            //}
        }

        #region Properties

        protected string[] Session_FilterTreeState
        {
            get
            {
                if (Session["Session_FilterTreeState"] != null)
                    return (string[])Session["Session_FilterTreeState"];
                else
                    return new string[0];
            }
            set
            {
                Session["Session_FilterTreeState"] = (string[])value;
            }
        }

        /// <summary>
        /// get array of CECFilteringOptions controls on the
        /// select page--static array is returned
        /// </summary>
        protected CECFilteringOptions[] FilterControls
        {
            get
            {
                return new CECFilteringOptions[0]; //{ wf_include , wf_collect, wf_design, wf_cancerCollect, wf_member };
            }
        }

        protected string CurrentFilterString
        {
            get
            {
                if (Session["select_Session_FilterString"] != null)
                    return Session["select_Session_FilterString"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                Session["select_Session_FilterString"] = value;
            }
        }

        /// <summary>
        /// get value indicating if select_all was checked...sort of
        /// </summary>
        protected bool CompareAllCohortsCheckBox_Checked
        {
            get
            {
                if (Session["select_CompareAllCohortsCheckBox_Checked"] != null)
                    return (bool)Session["select_CompareAllCohortsCheckBox_Checked"];
                else
                    return false;
            }
            set
            {
                Session["select_CompareAllCohortsCheckBox_Checked"] = value;
            }
        }

        /// <summary>
        /// get grid sort direction
        /// </summary>
        protected SortDirection SummaryGridSortDirection
        {
            get
            {
                if (Session["summaryGridSortDirection"] != null)
                    return (SortDirection)Session["summaryGridSortDirection"];
                else
                {
                    SummaryGridSortDirection = SortDirection.Ascending;
                    return SummaryGridSortDirection;
                }
            }
            set
            {
                Session["summaryGridSortDirection"] = (SortDirection)value;
            }
        }

        /// <summary>
        /// get grid sort column
        /// </summary>
        protected string SummaryGridSortColumn
        {
            get
            {
                return (string)Session["summaryGridSortColumn"] ?? "cohort_acronym";
            }
            set
            {
                Session["summaryGridSortColumn"] = value;
            }
        }

        /// <summary>
        /// get or set string array of cohort ids from the database to
        /// compare/view data for throughout the site
        /// </summary>
        protected string[] CohortIDsToCompare
        {
            get
            {
                if ((Session["select_CohortIDsToCompare"] != null) && ((string[])Session["select_CohortIDsToCompare"]).Length > 0)
                    return (string[])Session["select_CohortIDsToCompare"];
                else
                    return new string[0];
            }
            set
            {
                Session["select_CohortIDsToCompare"] = value;
            }
        }

        /// <summary>
        /// get or set the number of rows to display on the summary grid; default is 15
        /// </summary>
        protected int SummaryGridRowSize
        {
            get
            {
                if (Session["summaryGridRowSize"] == null)
                    SummaryGridRowSize = 15; // int.Parse(cohortVisibleCount.SelectedValue);

                return (int)Session["summaryGridRowSize"];
            }
            set
            {
                Session["summaryGridRowSize"] = value;
            }
        }

        /// <summary>
        /// get or set whether the cohort list is using filtering or
        /// keyword searching
        /// </summary>
        protected bool UsingFilterOptions
        {
            get
            {
                if (ViewState["listUsingFilterOptions"] == null)
                    UsingFilterOptions = true;

                return (bool)ViewState["listUsingFilterOptions"];
            }
            set
            {
                ViewState["listUsingFilterOptions"] = value;
            }
        }

        protected int CurrentPageIndex
        {
            get
            {
                if (ViewState["gridCurrentPageIndex"] == null)
                    CurrentPageIndex = 0;

                return (int)ViewState["gridCurrentPageIndex"];
            }
            set
            {
                ViewState["gridCurrentPageIndex"] = value;
            }
        }

        #region Derilict Properties

        /// <summary>
        /// get or set string array of cohort ids to track as selections while 
        /// users navigate the summary grid
        /// </summary>
        //protected string[] CohortIDsViewState
        //{
        //    get
        //    {
        //        if ((Session["CohortIDsSession"] != null) && ((string[])Session["CohortIDsSession"]).Length > 0)
        //            return (string[])Session["CohortIDsSession"];
        //        else
        //            return new string[0];
        //    }
        //    set
        //    {
        //        Session["CohortIDsSession"] = value;
        //    }
        //}


        /// <summary>
        /// get or set the alphabetical pagenation block for cohort acronym to filter the summary grid by; default is string.Empty meaning no filtering
        /// examples: a-c; d-f; g-i; w-z
        /// </summary>
        //public string AlphaPagenationBlock
        //{
        //    get
        //    {
        //        if (Session["alphaPagenation"] == null)
        //            AlphaPagenationBlock = string.Empty;

        //        return (string)Session["alphaPagenation"];
        //    }
        //    set
        //    {
        //        Session["alphaPagenation"] = value;
        //    }
        //}
        #endregion

        #endregion

        #region EventHandling

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            redirect = false;
            redirectionUrl = string.Empty;

            checkboxToGetFocus = string.Empty;

            help = new helper();

            EnsureChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PopulateSummaryTable();

            if (Request.Params.HasKeys() && Request.Params["__EVENTTARGET"] != null)
            {
                if (Request.Params["__EVENTTARGET"].Contains("cohortCompare"))
                {
                    string id = Request.Params["__EVENTARGUMENT"];
                    SaveCohortIDToSession(id);

                    /// return focus to clicked checkbox
                    checkboxToGetFocus = "compare_" + id;
                }
            }

           // not sure why this does not consistently work.  
           if ((Request.UrlReferrer != null) && !(Request.UrlReferrer.PathAndQuery.Contains("/select.aspx") || Request.UrlReferrer.PathAndQuery.EndsWith("/")))
            RestoreFilterTreeSessionStates();

            PopulateSummaryTable();
            SetSummaryGridForDisplay();

            //UserData ud = CECWebSrv.GetUserInformation(UserToken.email);
            //if (!ud.help_shown)
            //{
            //    Page.ClientScript.RegisterStartupScript(typeof(CECPage), "HelpTour", "tour.init(); tour.restart();", true);

            //    ud.help_shown = true;
            //    CECWebSrv.SetUserSecurityAttributes(UserToken, ud);
            //}

            if(!IsPostBack)
                CECWebSrv.AuditLog_AddActivity(UserToken.userid, "select cohort page");
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            if (redirect)
                Page.Response.Redirect(redirectionUrl);

            if (IsPostBack && select_all.Checked != CompareAllCohortsCheckBox_Checked)
                OnSelectAll_Toggle(select_all, EventArgs.Empty);

            //if (shouldRefresh)
            //    PopulateSummaryGrid();

            /// ----------------------------------------------
            /// set the index for the table size in the drop down menu
            //if (int.Parse(cohortVisibleCount.SelectedValue) != SummaryGridRowSize)
            //{
            //    System.Web.UI.WebControls.ListItem li = cohortVisibleCount.Items.FindByValue(SummaryGridRowSize.ToString());
            //    cohortVisibleCount.SelectedIndex = cohortVisibleCount.Items.IndexOf(li);

            //    summaryGridView.PageSize = SummaryGridRowSize;
            //}
            //SummaryGridRowSize = summaryGridView.PageSize;

            ///--------------------------------
            /// css active
            string pg = String.Format("pg_{0}", summaryGridView.PageIndex);
            if (summaryPager.FindControl(pg) != null)
                (summaryPager.FindControl(pg) as LinkButton).CssClass = "active";

            ///-----------------------------------------
            /// must have more than one page to merit display
            if (summaryGridView.PageCount <= 1)
                summaryPager.Controls.Clear();

            ///--------------------------------
            /// set record count text
            /// 
            int a = (SummaryGridRowSize * (summaryGridView.PageIndex + 1) - SummaryGridRowSize) + 1,
                b = SummaryGridRowSize * (summaryGridView.PageIndex + 1);
            b = (b >= summaryTable.Rows.Count ? summaryTable.Rows.Count : b);
            summaryCount.Text = String.Format("{0}-{1} of {2}", a, b, summaryTable.Rows.Count);

            //if (checkboxToGetFocus != string.Empty)
            //{
            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "getFocus",
            //       String.Format("$(document).ready(function(){{ if($('#{0}').attr('checked') == 'checked') $('#{0}').focus() }});", checkboxToGetFocus), true);
            //}

            string sortCtrl = Request.Form["__EVENTARGUMENT"];
            if (!String.IsNullOrWhiteSpace(sortCtrl) && sortCtrl.ToLower().Contains("sort"))
            {
                sortCtrl = sortCtrl.Substring((sortCtrl.IndexOf("$") + 1));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "getFocus",
                    String.Format("$(document).ready(function(){{ $('#{0} > a').focus(); }});", sortCtrl), true);
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool handled = false;
            if (args is CommandEventArgs)
            {
                EnsureChildControls();

                CommandEventArgs cea = (args as CommandEventArgs);
                switch (cea.CommandName)
                {
                    case "viewDetails":
                        handled = true;
                        redirect = true;

                        redirectionUrl = (String.Format("./cohortDetails.aspx?cohort_id={0}", cea.CommandArgument));
                        break;
                    case "export":
                        handled = true;

                        string filepath = String.Format("./user_files/{0}/cohortselect_{1}.xlsx", UserToken.userid, DateTime.Now.ToString("yyyyMMMddmm"));

                        PopulateSummaryTable();

                        ExportDataGridToExcel(summaryTable, MapPath(filepath));
                        CECWebSrv.AuditLog_AddActivity(UserToken.userid, "select cohort export created");

                        Page.ClientScript.RegisterStartupScript(GetType(), "downloadExport",
                            String.Format("<script>window.open('{0}');</script>", filepath));

                        break;
                }

                return handled;
            }
            else
                return base.OnBubbleEvent(source, args);
        }

        /// <summary>
        /// called when the summarygridview is bound to data source
        /// </summary>
        protected void summaryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.ID = "summaryHeader";

                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.Controls[0] is LinkButton)
                    {
                        string colName = (tc.Controls[0] as LinkButton).Text;
                        tc.ID = (tc.Controls[0] as LinkButton).Text;
                        (tc.Controls[0] as LinkButton).Controls.Add(new LiteralControl(CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, colName)));

                        System.Web.UI.HtmlControls.HtmlGenericControl span =
                            new HtmlGenericControl("span");
                        span.Attributes["class"] = "glyphicon glyphicon-sort";
                        (tc.Controls[0] as LinkButton).Controls.Add(span);
                    }

                    tc.CssClass = "sortable ";

                    if(tc.ID == SummaryGridSortColumn)
                        tc.CssClass += String.Format("columnSorting_{0}", GetSortString(SummaryGridSortDirection));
                }

                //System.Web.UI.WebControls.TableHeaderCell hc =
                //    new TableHeaderCell();
                //hc.ID = "table-select-col";
                //hc.ClientIDMode = ClientIDMode.Static;
                //hc.ToolTip = "Toggle Select All";

                //hc.Controls.Add(new LiteralControl(String.Format("<label for=\"{0}\" class=\"invisibleLabel\">Toggle Select All</label>", select_all.ID)));

                //hc.Controls.Add(select_all);

                //checkbox
                //e.Row.Cells.AddAt(0, hc);

                e.Row.TableSection = TableRowSection.TableHeader;

                /*//contact
                TableHeaderCell con = new TableHeaderCell();
                con.ID = "contactCol";
                con.ClientIDMode = ClientIDMode.Static;
                con.Attributes.Add("scope", "col");
                con.Text = "Contact";
                e.Row.Cells.Add(con);

                //policies
                TableHeaderCell pol = new TableHeaderCell();
                pol.ID = "policyCol";
                pol.ClientIDMode = ClientIDMode.Static;
                pol.Attributes.Add("scope", "col");
                pol.Text = "Policies";
                e.Row.Cells.Add(pol);

                //more
                TableHeaderCell more = new TableHeaderCell();
                more.ID = "moreCol";
                more.ClientIDMode = ClientIDMode.Static;
                more.Attributes.Add("scope", "col");
                more.Text = "More";
                e.Row.Cells.Add(more);*/
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // get cohort id
                int acronymIndex = summaryTable.Columns.IndexOf("cohort_acronym");
                string acronym = (e.Row.DataItem as DataRowView)[acronymIndex].ToString();
                int cId = (int)CECWebSrv.GetCohortDetails(UserToken, acronym).Tables[0].Rows[0]["cohort_id"];

                // get cohort name index
                int nameIndex = summaryTable.Columns.IndexOf("cohort_name");

                /// commented out March 5 by SC. Accessibility report did not like
                ///  multiple labels for one input control
                //----------------------
                // create label control for each field
                //foreach (TableCell tc in e.Row.Cells)
                //{
                //    System.Web.UI.WebControls.Label lb =
                //        new Label();
                //    lb.AssociatedControlID = "compare_" + cId.ToString();
                //    lb.Text = tc.Text;

                //    tc.Controls.Add(lb);
                //}

                // get the cohort name to link
                System.Web.UI.WebControls.HyperLink nm =
                    new HyperLink();
                nm.Text = e.Row.Cells[nameIndex].Text;
                nm.NavigateUrl = String.Format("./cohortDetails.aspx?cohort_id={0}", cId);
                //nm.Target = "_blank";

                e.Row.Cells[nameIndex].Controls.Add(nm);
                //------------------------------------------

                System.Web.UI.WebControls.HyperLink ac =
                    new HyperLink();
                ac.Text = e.Row.Cells[acronymIndex].Text;
                ac.NavigateUrl = String.Format("./cohortDetails.aspx?cohort_id={0}", cId);

                e.Row.Cells[acronymIndex].Controls.Add(ac);
                //------------------------------------------


                //---
                // now lets add the custom columns:
                // checkbox control column
                //System.Web.UI.WebControls.CheckBox ckbx =
                //    new CheckBox();
                //ckbx.CssClass = "select-checkbox";
                //ckbx.ID = "compare_" + cId.ToString();
                //ckbx.ClientIDMode = ClientIDMode.Static;
                //if (CohortIDsToCompare.Contains(cId.ToString()))
                //    ckbx.Checked = true;

                //ckbx.Attributes.Add("onclick", String.Format("javascript:__doPostBack('cohortCompare', '{0}');", cId));
                
                //System.Web.UI.WebControls.TableCell tc1 =
                //    new TableCell();
                //tc1.Attributes["headers"] = String.Format("{0} {1}", acronym, "table-select-col");
                
                //tc1.Controls.Add(new LiteralControl(String.Format("<label for=\"{0}\" class=\"invisibleLabel\">Select {1} Cohort</label>", ckbx.ID, acronym)));
                //tc1.Controls.Add(ckbx);
                //e.Row.Cells.AddAt(0, tc1);

                ////--
                //// contact link column
                //System.Web.UI.WebControls.HyperLink con =
                //    new HyperLink();
                //con.Text = String.Format("contact {0}", acronym);
                ////con.Target = "_blank";
                //con.CssClass = "tableIcons contact";
                //con.NavigateUrl = String.Format("/cohortDetails.aspx?cohort_id={0}", cId);

                ////con.Controls.Add((new Image() { ImageUrl = "/images/ico_contact.png" }));

                //System.Web.UI.WebControls.TableCell tc2 =
                //    new TableCell();
                //tc2.Attributes["headers"] = String.Format("{0} {1}", acronym, "contactCol");
                //tc2.Controls.Add(con);
                //e.Row.Cells.Add(tc2);

                ////--
                //// policies
                //System.Web.UI.WebControls.HyperLink pro =
                //    new HyperLink();
                //pro.Text = String.Format("policies {0}", acronym);
                ////pro.Target = "_blank";
                //pro.CssClass = "tableIcons policies";
                //pro.NavigateUrl = String.Format("/cohortDetails.aspx?cohort_id={0}&tab=policies", cId);

                ////pro.Controls.Add((new Image() { ImageUrl="/images/ico_policies.png" }));

                //System.Web.UI.WebControls.TableCell tc3 =
                //    new TableCell();
                //tc3.Attributes["headers"] = String.Format("{0} {1}", acronym, "policyCol");
                //tc3.Controls.Add(pro);
                //e.Row.Cells.Add(tc3);

                ////---
                //// more
                //System.Web.UI.WebControls.HyperLink more =
                //    new HyperLink();
                //more.Text = String.Format("more {0}", acronym);
                ////more.Target = "_blank";
                //more.CssClass = "tableIcons more";
                //more.NavigateUrl = String.Format("/cohortDetails.aspx?cohort_id={0}&tab=more", cId);

                ////more.Controls.Add((new Image() { ImageUrl = "/images/ico_more.png" }));

                //System.Web.UI.WebControls.TableCell tc4 =
                //    new TableCell();
                //tc4.Attributes["headers"] = String.Format("{0} {1}", acronym, "moreCol");
                //tc4.Controls.Add(more);
                //e.Row.Cells.Add(tc4);

                ///--------------------------------------------------
                /// the data cells: add header cell ids.
                for (int _i = 0; _i < e.Row.Cells.Count; _i++)
                {
                    if(e.Row.Cells[_i].Attributes["headers"] == null && _i <= summaryTable.Columns.Count)
                        e.Row.Cells[_i].Attributes["headers"] = String.Format("{0}", summaryTable.Columns[_i].ColumnName);
                        
                }
            }
        }

        /// <summary>
        /// called after the summaryGridView has been bound to a data source;
        ///  time to call anything that relies on the gridview being ready
        /// </summary>
        protected void summaryGridView_Bound(object sender, EventArgs args)
        {
            PopulatePagerControls();
        }

        /// <summary>
        /// handles the filtering control postbacks
        /// </summary>
        protected void SelectedNodeChanged(object sender, EventArgs e)
        {
            UsingFilterOptions = true;

            if (inKeyword.Text.ToLower() != "find a cohort by name" && !helper.IsStringEmptyWhiteSpace(inKeyword.Text))
                inKeyword.Text = string.Empty;
        }

        /// <summary>
        /// handles sorts for the summaryGrid
        /// </summary>
        protected void summaryGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (e.SortExpression == SummaryGridSortColumn)
            {
                if (e.SortDirection == SummaryGridSortDirection)
                {
                    if (SummaryGridSortDirection == SortDirection.Ascending)
                        SummaryGridSortDirection = SortDirection.Descending;
                    else
                        SummaryGridSortDirection = SortDirection.Ascending;
                }
                else
                    SummaryGridSortDirection = e.SortDirection;
            }
            else
            {
                SummaryGridSortColumn = e.SortExpression;
                SummaryGridSortDirection = e.SortDirection;
            }

            SetSummaryGridForDisplay();
        }

        /// <summary>
        /// handled the event when the cohort visible count changes
        /// </summary>
        protected void cohortVisibleCount_Changed(object sender, EventArgs e)
        {
            SummaryGridRowSize = 15; // Convert.ToInt32(cohortVisibleCount.SelectedValue);
            summaryGridView.PageSize = SummaryGridRowSize;
            SetSummaryGridForDisplay();
        }

        /// <summary>
        ///  handles the check all control
        /// </summary>
        protected void OnSelectAll_Toggle(object sender, EventArgs e)
        {
            System.Collections.ArrayList arr =
                new ArrayList(CohortIDsToCompare);

            CompareAllCohortsCheckBox_Checked = select_all.Checked;

            if (select_all.Checked)
            {
                //-----
                // bind summary grid data
                System.Data.DataSet sg = CECWebSrv.GetCohortForSummaryGrid(UserToken, SqlizeFilterCriteria());
                System.Data.DataTable t = sg.Tables["tbl_web_cohorts_v4_0"];

                foreach (DataRow dr in t.Rows)
                {
                    int acronymIndex = t.Columns.IndexOf("cohort_acronym");
                    int cId = (int)CECWebSrv.GetCohortDetails(UserToken, dr["cohort_acronym"].ToString()).Tables[0].Rows[0]["cohort_id"];

                    if (!arr.Contains(cId.ToString()))
                        arr.Add(cId.ToString());
                }
            }
            else
                arr.Clear();

            CohortIDsToCompare = (string[])arr.ToArray(typeof(string));

            SetSummaryGridForDisplay();

            /// return focus to clicked checkbox
            checkboxToGetFocus = select_all.ID;
        }

        /// <summary>
        /// custom pager controls, previous group button
        /// </summary>
        protected void PagePrevious_Clicked(object sender, EventArgs e)
        {
            SaveCohortIDArrayToSession();

            if (summaryGridView.PageIndex - 1 <= 0)
                summaryGridView.SetPageIndex(0);
            else
                summaryGridView.SetPageIndex(CurrentPageIndex - 1);
        }

        /// <summary>
        /// custom pager controls, next group button
        /// </summary>
        protected void PageNext_Clicked(object sender, EventArgs e)
        {
            SaveCohortIDArrayToSession();

            summaryGridView.SetPageIndex(CurrentPageIndex + 1);
        }

        protected void PageViewAll_Clicked(object sender, EventArgs e)
        {
            SaveCohortIDArrayToSession();

            SummaryGridRowSize = summaryTable.Rows.Count;
            summaryGridView.PageSize = SummaryGridRowSize;
            SetSummaryGridForDisplay();
        }

        /// <summary>
        /// custom pager controls, page number buttons
        /// </summary>
        protected void PageNumber_Clicked(object sender, EventArgs e)
        {
            SaveCohortIDArrayToSession();

            if (sender is LinkButton)
            {
                string id = ((LinkButton)sender).ID;
                id = id.Split('_')[1];
                summaryGridView.SetPageIndex(Convert.ToInt32(id));
            }
        }

        /// <summary>
        /// handles paging for the summaryGrid
        /// </summary>
        protected void summaryGridView_PageChanging(object sender, GridViewPageEventArgs e)
        {
            CurrentPageIndex = e.NewPageIndex;
            SetSummaryGridForDisplay();
        }

        /// <summary>
        /// handles the filter options "clear all" event and deselects all
        /// checked nodes in the tree
        /// </summary>
        protected void FilterClear_Clicked(object sender, EventArgs e)
        {
            CohortIDsToCompare = new string[0];
            Session_FilterTreeState = new string[0];
            CurrentFilterString = string.Empty;

            foreach (CECFilteringOptions tree in FilterControls)
                ClearFilterTree(tree);

            PopulateSummaryTable();
            SetSummaryGridForDisplay();
        }

        protected void FilterEngage_Clicked(object sender, EventArgs e)
        {
            EnsureChildControls();

            UsingFilterOptions = true;

            if (inKeyword.Text.ToLower() != "find a cohort by name" && !helper.IsStringEmptyWhiteSpace(inKeyword.Text))
                inKeyword.Text = string.Empty;

            CohortIDsToCompare = new string[0];

            ArrayList filters = new ArrayList();

            foreach (CECFilteringOptions fo in FilterControls)
            {
                CheckBox[] cklist = fo.GetCheckedBoxes();
                foreach (CheckBox ck in cklist)
                    filters.Add(ck.ID);
            }

            Session_FilterTreeState = (string[])filters.ToArray(typeof(string));

            CurrentFilterString = SqlizeFilterCriteria();

            CECWebSrv.AuditLog_AddActivity(UserToken.userid, "select cohort page:search engaged:: " + CurrentFilterString);

            PopulateSummaryTable();
            SetSummaryGridForDisplay();
        }

        protected void KeywordSearch_Engage(object sender, EventArgs e)
        {
            if (!helper.IsStringEmptyWhiteSpace(inKeyword.Text) && inKeyword.Text != "search cohorts")
            {
                UsingFilterOptions = false;

                CohortIDsToCompare = new string[0];
                Session_FilterTreeState = new string[0];

                foreach (CECFilteringOptions tree in FilterControls)
                    ClearFilterTree(tree);

                PopulateSummaryTable();
                SetSummaryGridForDisplay();
            }
        }
        #endregion

        private void PopulateCriteriaFilterTree(CECFilteringOptions tree)
        {
            //-----
            // filtering tree
            System.Data.DataSet wff = CECWebSrv.GetWebFilterFields(UserToken, tree.RootCategoryID);

            //TreeNode ptr = new TreeNode();
            ///// add the child controls
            //foreach (DataRow dr in wff.Tables["tbl_web_filter_fields"].Rows)
            //{
            //    string nodePath = String.Format("{1}{0}{2}", tree.PathSeparator, dr["category_id"], dr["id"]);
            //    if (tree.FindNode(nodePath) != null)
            //        continue;

            //    if (ptr.Text != (string)dr["category_name"])
            //    {
            //        ptr = new TreeNode((string)dr["category_name"]);
            //        ptr.Value = dr["category_id"].ToString();
            //        ptr.Expanded = ((bool)dr["default_expand"] ? true : false);

            //        tree.Nodes.Add(ptr);
            //    }

            //    TreeNode child =
            //        new TreeNode((string)dr["filter_label"], dr["id"].ToString());
            //    ptr.ChildNodes.Add(child);
            //}
        }

        private void RestoreFilterTreeSessionStates()
        {
            foreach (CECFilteringOptions tree in FilterControls)
            {
                tree.SetActiveCheckBoxes(Session_FilterTreeState);

                ///-----------------------------------------------
                /// reset the filter options based on the last session state
                ///// 
                //for (int _i = 0; _i < Session_FilterTreeState.Length; _i++)
                //{
                //    TreeNode tn = tree.FindNode(Session_FilterTreeState[_i]);
                //    if (tn != null)
                //    {
                //        if (tn.ChildNodes.Count > 0)
                //            tn.ToggleExpandState();
                //        else
                //            tn.Checked = true;
                //    }
                //}
            }

            /// Feb 5: maybe retreive the checkbox control and give it focus? I will have to parse out the id from session_filtertreestate
            ///  but tree control does not create "controls" until render
            //if (Session_FilterTreeState.Length > 0)
            //{
            //    Control ckbx = FindControlRecursive(this, Session_FilterTreeState[Session_FilterTreeState.Length - 1]);
            //}
        }

        private void ClearFilterTree(CECFilteringOptions tree)
        {
            tree.ClearAllCheckboxes();
            //TreeNode[] tnc = new TreeNode[tree.CheckedNodes.Count];
            //tree.CheckedNodes.CopyTo(tnc, 0);
            //foreach (TreeNode tn in tnc)
            //{
            //    tree.FindNode(tn.ValuePath).Checked = false;
            //}
        }

        private void SaveCohortIDToSession(string cohortID)
        {
            ArrayList arr = new ArrayList(CohortIDsToCompare);
            if (!arr.Contains(cohortID))
                arr.Add(cohortID);
            else
                arr.Remove(cohortID);

            CohortIDsToCompare = (string[])arr.ToArray(typeof(string));
        }

        private void SaveCohortIDArrayToSession()
        {
            ArrayList arr = new ArrayList(CohortIDsToCompare);

            foreach (GridViewRow row in summaryGridView.Rows)
            {
                if ((row.Cells[0] != null) && row.Cells[0].HasControls() && row.Cells[0].Controls[0] is CheckBox)
                {
                    CheckBox ckbx = (row.Cells[0].Controls[0] as CheckBox);

                    string justID = ckbx.ID;
                    justID = justID.Substring(8, justID.Length - 8);

                    if (!arr.Contains(justID) && ckbx.Checked)
                        arr.Add(justID);
                    else if (arr.Contains(justID) && !ckbx.Checked)
                        arr.Remove(justID);
                }
            }

            CohortIDsToCompare = (string[])arr.ToArray(typeof(string));
        }

        private string GetSortString(SortDirection sortDir)
        {
            if (sortDir == SortDirection.Descending)
                return "DESC";
            else
                return "ASC";
        }

        private string SqlizeFilterCriteria()
        {
            EnsureChildControls();
            
            string filter = string.Empty;
            foreach (CECFilteringOptions fo in FilterControls)
            {
                string interim = string.Empty;

                CheckBox[] ck_list = fo.GetCheckedBoxes();
                foreach (CheckBox ck in ck_list)
                {
                    interim = String.Format(" {0} or", CECWebSrv.GetWebFilterByFilterId(UserToken, int.Parse(ck.ID.Split('_')[1])).Rows[0]["filter_criteria"].ToString());

                    if (interim.EndsWith("or"))
                        interim = interim.Remove(interim.Length - 2, 2);
                }

                if (interim != string.Empty)
                    filter += String.Format(" ({0}) and", interim);
            }

            ///----------------------------------------------
            /// For the auto postback version of the tree control
            /// 
            //foreach (CECFilteringOptions tree in FilterControls)
            //{
            //    // sidebar filtering
            //    string interim = string.Empty;

            //    for (int _p = 0; _p < tree.CheckedNodes.Count; _p++)
            //    {
            //        interim += String.Format(" {0} or", CECWebSrv.GetWebFilterByFilterId(UserToken, int.Parse(tree.CheckedNodes[_p].Value)).Rows[0]["filter_criteria"].ToString());

            //        if (((tree.CheckedNodes.Count == _p + 1) || tree.CheckedNodes[_p].Parent.Value != tree.CheckedNodes[_p + 1].Parent.Value) && interim != String.Empty)
            //        {
            //            if (interim != string.Empty)
            //            {
            //                if (interim.EndsWith("or"))
            //                    interim = interim.Remove(interim.Length - 2, 2);

            //                filter += String.Format(" ({0}) and", interim);
            //                interim = string.Empty;
            //            }
            //        }
            //    }
            //}

            #region Derilict Code: Alpha Pagenation
            // pagination filtering --- not used anymore
            //if (AlphaPagenationBlock != string.Empty)
            //{
            //    string interim = string.Empty;

            //    switch (AlphaPagenationBlock)
            //    {
            //        case "a-c":
            //            interim = " (lower(a.cohort_acronym) like 'a%' or lower(a.cohort_acronym) like 'b%' or lower(a.cohort_acronym) like 'c%')";
            //            break;
            //        case "d-f":
            //            interim = " (lower(a.cohort_acronym) like 'd%' or lower(a.cohort_acronym) like 'e%' or lower(a.cohort_acronym) like 'f%')";
            //            break;
            //        case "g-i":
            //            interim = " (lower(a.cohort_acronym) like 'g%' or lower(a.cohort_acronym) like 'h%' or lower(a.cohort_acronym) like 'i%')";
            //            break;
            //        case "j-l":
            //            interim = " (lower(a.cohort_acronym) like 'j%' or lower(a.cohort_acronym) like 'k%' or lower(a.cohort_acronym) like 'l%')";
            //            break;
            //        case "m-o":
            //            interim = " (lower(a.cohort_acronym) like 'm%' or lower(a.cohort_acronym) like 'n%' or lower(a.cohort_acronym) like 'o%')";
            //            break;
            //        case "p-r":
            //            interim = " (lower(a.cohort_acronym) like 'p%' or lower(a.cohort_acronym) like 'q%' or lower(a.cohort_acronym) like 'r%')";
            //            break;
            //        case "s-u":
            //            interim = " (lower(a.cohort_acronym) like 's%' or lower(a.cohort_acronym) like 't%' or lower(a.cohort_acronym) like 'u%')";
            //            break;
            //        case "w-z":
            //            interim = " (lower(a.cohort_acronym) like 'w%' or lower(a.cohort_acronym) like 'x%' or lower(a.cohort_acronym) like 'y%' or lower(a.cohort_acronym) like 'z%')";
            //            break;
            //    }

            //    if (filter != string.Empty)
            //        filter += " and " + interim;
            //    else
            //        filter = interim;
            //}
            #endregion

            if (filter.EndsWith("and"))
                filter = filter.Remove(filter.Length - 3, 3);

            return filter;
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
                NPOI.SS.UserModel.IRow dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue(String.Format("Cohort Data Export Generated from the CEDCD Website ({0})", Request.Url.Authority));

                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Table Name:");
                dataRow.CreateCell(1).SetCellValue("Cohort Selection");

                //string filter = String.Empty;
                //foreach (CECFilteringOptions tree in FilterControls)
                //{
                //    foreach (CheckBox ck in tree.GetCheckedBoxes())
                //        filter += String.Format("{0}; ", tree.GetCheckBoxLabel(ck.ID));
                //}
                //filter = filter.TrimEnd(new char[] { ';', ' ' });
                //dataRow = wkst.CreateRow(rowIndex++);
                //dataRow.CreateCell(0).SetCellValue("Web Filter Options:");
                //dataRow.CreateCell(1).SetCellValue(filter);

                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Export Date:");
                dataRow.CreateCell(1).SetCellValue(DateTime.Now.ToString("MM/dd/yyyy"));

                rowIndex += 2;


                /// handle header row
                dataRow = wkst.CreateRow(rowIndex++);
                for (int _c = 0; _c < toExport.Columns.Count; _c++)
                {
                    ICell c = dataRow.GetCell(_c);
                    if (c == null)
                        c = dataRow.CreateCell(_c);

                    c.SetCellValue(CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, toExport.Columns[_c].ColumnName));
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
                        if (helper.IsStringEmptyWhiteSpace(cellVal) || cellVal == "&nbsp;")
                            c.SetCellValue(" ");
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

        private DataTable PopulateSummaryTable()
        {
            //-----
            // bind summary grid data
            System.Data.DataSet sg;
            if (UsingFilterOptions)
                sg = CECWebSrv.GetCohortForSummaryGrid(UserToken, CurrentFilterString);
            else
                sg = CECWebSrv.GetCohortForSummaryGrid(UserToken, String.Format(" (lower(cohort_name) like '%{0}%') OR (lower(cohort_acronym) like '%{0}%') ", helper.SterilizeDBText(inKeyword.Text.ToLower())));

            System.Data.DataTable t = sg.Tables["tbl_web_cohorts_v4_0"];

            summaryTable = new DataTable();

            foreach (DataColumn dc in t.Columns)
            {
                // columns that have a position of 0 should not be displayed
                if (Convert.ToInt32(CECWebSrv.GetCohortWebFieldByColumnName(UserToken, dc.ColumnName).Rows[0]["summary_position"]) == 0)
                    continue;

                summaryTable.Columns.Add(dc.ColumnName);
            }

            foreach (DataRow dr in t.Rows)
            {
                DataRow ndr = summaryTable.NewRow();

                for (int _col = 0; _col < t.Columns.Count; _col++)
                {
                    //---
                    // columns that have a position of 0 should not be displayed
                    if (Convert.ToInt32(CECWebSrv.GetCohortWebFieldByColumnName(UserToken, t.Columns[_col].ColumnName).Rows[0]["summary_position"]) == 0)
                        continue;

                    if (t.Columns[_col].DataType == typeof(DateTime))
                        ndr[_col - 1] = DateTime.Parse(dr[_col].ToString()).ToString("MM/dd/yyyy");
                    else if (helper.IsNumerical(dr[_col]) && !(t.Columns[_col].ColumnName.ToLower().Contains("date") || t.Columns[_col].ColumnName.ToLower().Contains("year")))
                        ndr[_col - 1] = helper.FormatCount((int)dr[_col]);
                    else
                        ndr[_col - 1] = dr[_col].ToString();
                }
                summaryTable.Rows.Add(ndr);
            }

            summaryGridView.DataSource = summaryTable;

            return summaryTable;
        }

        private void SetSummaryGridForDisplay()
        {
            summaryGridView.PageSize = SummaryGridRowSize;
            summaryTable.DefaultView.Sort = String.Format("[{0}] {1}", SummaryGridSortColumn, GetSortString(SummaryGridSortDirection));
            summaryGridView.PageIndex = CurrentPageIndex;

            summaryGridView.DataBind();
        }

        private void PopulatePagerControls()
        {
            summaryPager.Controls.Clear();

            /// ---------------------------------------
            /// the in-between, zero-based...except for the page number rendered
            /// 
            int iRuns = 0,
                iStart = (summaryGridView.PageIndex > 4 || summaryGridView.PageIndex < (summaryGridView.PageCount - 4)) ? summaryGridView.PageIndex : 0;
            for (int _i = iStart; _i < summaryGridView.PageCount; _i++)
            {
                if (iRuns == 4)
                    break;

                System.Web.UI.WebControls.LinkButton page =
                    new LinkButton();
                page.ID = String.Format("pg_{0}", _i);
                page.Text = (_i + 1).ToString();
                page.Click +=
                    new EventHandler(PageNumber_Clicked);

                summaryPager.Controls.Add(page);

                iRuns++;
            }

            ///-----------------------------------------
            /// always a previous arrow
            /// 
            System.Web.UI.WebControls.LinkButton prev =
                new LinkButton();
            prev.Click +=
                new EventHandler(PagePrevious_Clicked);

            prev.ID = "previous";
            prev.CssClass = "arrow prev";
            prev.Text = "previous";

            summaryPager.Controls.AddAt(0, prev);
            ///---------------------------------------
            /// always a next arrow
            /// 
            System.Web.UI.WebControls.LinkButton next =
                new LinkButton();

            next.ID = "next";
            next.CssClass = "arrow next";
            next.Text = "next";

            next.Click +=
                new EventHandler(PageNext_Clicked);

            summaryPager.Controls.Add(next);

            ///-----------------------------------------
            /// add view-all
            /// 
            System.Web.UI.WebControls.LinkButton viewAll =
                new LinkButton();
            viewAll.ID = "view_all";
            viewAll.CssClass = "";
            viewAll.Text = "View All";

            viewAll.Click +=
                new EventHandler(PageViewAll_Clicked);

            summaryPager.Controls.Add(viewAll);
        }
    }
}