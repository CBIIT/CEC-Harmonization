using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
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



    public partial class biospecimen : CECPage
    {
        private System.Data.DataTable bioTbl;
        private System.Data.DataTable webfieldsTbl;

        // following variables track certain aspects
        //  of the bioTbl as the dataset is being bound to
        //  the table
        private bool _isFirstRow;
        private string _curSpecimen;
        
        /// <summary>
        /// create the necessary childcontrols for filtering
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            select_cancer.UserToken = UserToken;
            select_cohort.UserToken = UserToken;
            select_specimenTypes.UserToken = UserToken;

            bioGridView.ClientIDMode = ClientIDMode.Static;

            DataTable csDT = CECWebSrv.GetCancerSites(UserToken);
        }

        #region Properties

        /// <summary>
        /// get or set string array of cohort ids
        /// </summary>
        protected string[] CohortIDsToCompare
        {
            get
            {
                if ((Session["BioCohortIDsToCompare"] != null) && ((string[])Session["BioCohortIDsToCompare"]).Length > 0)
                    return (string[])Session["BioCohortIDsToCompare"];
                else
                    return new string[0];
            }
            set
            {
                Session["BioCohortIDsToCompare"] = value;
            }
        }

        protected string[] SelectedCancers
        {
            get
            {
                if ((Session["BioCancers"] != null) && ((string[])Session["BioCancers"]).Length > 0)
                    return (string[])Session["BioCancers"];
                else
                    return new string[0];
            }
            set
            {
                Session["BioCancers"] = value;
            }
        }

        protected string[] SelectedSpecimenTypes
        {
            get
            {
                if ((Session["BioSpecimenTypes"] != null) && ((string[])Session["BioSpecimenTypes"]).Length > 0)
                    return (string[])Session["BioSpecimenTypes"];
                else
                    return new string[0];
            }
            set
            {
                Session["BioSpecimenTypes"] = value;
            }
        }
        #endregion

        #region EventHandling

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // instantiate data tables
            bioTbl = new DataTable();
            bioTbl.TableName = "bioTable";

            _isFirstRow = false;
            _curSpecimen = string.Empty;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                CohortIDsToCompare = new string[0];
                SelectedCancers = new string[0];
                SelectedSpecimenTypes = new string[0];

                CECWebSrv.AuditLog_AddActivity(UserToken.userid, "biospecimen page");
            }

        }

        protected override void OnLoadComplete(EventArgs e)
        {
            PopulateBioCountsGrid();

            submitBtn.Attributes["disabled"] = "disabled";

            base.OnLoadComplete(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            exportTblBtn.Visible = (bioTbl.Rows.Count > 0 ? true : false);

            base.OnPreRender(e);
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

                        string filepath = String.Format("./user_files/{0}/biospecimen_{1}.xlsx", UserToken.userid, DateTime.Now.ToString("yyyyMMMddmm"));

                        PopulateBioCountsGrid();

                        ExportDataGridToExcel(bioTbl, Server.MapPath(filepath));
                        CECWebSrv.AuditLog_AddActivity(UserToken.userid, "biospecimen export created");

                        Page.ClientScript.RegisterStartupScript(GetType(), "downloadExport",
                            String.Format("<script>window.open('{0}');</script>", filepath));

                        break;
                }

                return handled;
            }
            else
                return base.OnBubbleEvent(source, args);
        }

        protected void bioGridView_RowDataBound(object source, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int _i = 0; _i < e.Row.Cells.Count; _i++)
                {
                    System.Web.UI.WebControls.TableHeaderCell hc =
                        new TableHeaderCell();
                    hc.ID = bioTbl.Columns[_i].ColumnName.Replace(" ", string.Empty).Replace(":", string.Empty);

                    if (CECWebSrv.IsCohort(UserToken, e.Row.Cells[_i].Text))
                    {
                        DataRow dr_cohort = null;
                        using (DataTable dt = CECWebSrv.GetFilteredCohortRecords(UserToken, "cohort_id, cohort_name, cohort_acronym", String.Format("cohort_acronym='{0}'", bioTbl.Columns[_i].ColumnName)).Tables[0])
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
                        hc.Text = e.Row.Cells[_i].Text;
                        hc.CssClass = "table-col-20perc";
                    }

                    e.Row.Cells.RemoveAt(_i);
                    e.Row.Cells.AddAt(_i, hc);
                }

                e.Row.TableSection = TableRowSection.TableHeader;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                _isFirstRow = false;

               // DataRow[] dra = webfieldsTbl.Select(String.Format("data_field='{0}'", e.Row.Cells[0].Text));
               // DataRow dr = dra[0];

                string orgColumnName = e.Row.Cells[0].Text;

                ///---------------------------------------------------
                /// change to tableheadercell
                /// 
                System.Web.UI.WebControls.TableHeaderCell nmHdr =
                    new TableHeaderCell();
                nmHdr.Attributes["headers"] = bioTbl.Columns[0].ColumnName.Replace(" ", string.Empty).Replace(":", string.Empty);
                nmHdr.ID = orgColumnName;
                nmHdr.Text = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, orgColumnName);
                if (_curSpecimen != orgColumnName)
                {
                    nmHdr.RowSpan = SelectedCancers.Length;

                    _isFirstRow = true;
                    _curSpecimen = orgColumnName;

                    e.Row.Cells.RemoveAt(0);
                    e.Row.Cells.AddAt(0, nmHdr);
                }
                else
                    e.Row.Cells.RemoveAt(0);

                System.Web.UI.WebControls.TableHeaderCell cnHdr =
                    new TableHeaderCell();
                cnHdr.Attributes["headers"] = String.Format("{0} CancerTypes", nmHdr.ID);
                if (_isFirstRow)
                {
                    cnHdr.ID = e.Row.Cells[1].Text.ToLower();
                    cnHdr.Text = e.Row.Cells[1].Text;

                    e.Row.Cells.RemoveAt(1);
                    e.Row.Cells.AddAt(1, cnHdr);

                    e.Row.Cells[1].Text = select_cancer.GetCancerLabel(e.Row.Cells[1].Text);
                }
                else
                {
                    cnHdr.ID = e.Row.Cells[0].Text.ToLower();
                    cnHdr.Text = e.Row.Cells[0].Text;

                    e.Row.Cells.RemoveAt(0);
                    e.Row.Cells.AddAt(0, cnHdr);

                    e.Row.Cells[0].Text = select_cancer.GetCancerLabel(e.Row.Cells[0].Text);
                }

                for (int _i = (_isFirstRow ? 2 : 1); _i < e.Row.Cells.Count; _i++)
                {
                    string headers = String.Format("{0} {1}", nmHdr.ID, cnHdr.ID);
                    if (_i > 1)
                        headers += String.Format(" {0}", bioTbl.Columns[_i].ColumnName.Replace(" ", string.Empty));

                    e.Row.Cells[_i].Attributes["headers"] = headers;
                    if (String.IsNullOrWhiteSpace(e.Row.Cells[_i].Text) || e.Row.Cells[_i].Text == "-1" || e.Row.Cells[_i].Text == "&nbsp;")
                        e.Row.Cells[_i].Text = "N/P";
                }
            }
        }

        protected void ClearOptions_Clicked(object sender, EventArgs e)
        {
            CohortIDsToCompare = new string[0];
            SelectedCancers = new string[0];
            SelectedSpecimenTypes = new string[0];

            select_cohort.ClearSelectedCohortIDs();
            select_cancer.ClearSelectedCancers();
            select_specimenTypes.ClearSelectedSpecimenTypes();

            EnsureChildControls();

            PopulateBioCountsGrid();
        }

        protected void Submit_Clicked(object source, EventArgs e)
        {
            EnsureChildControls();

            CohortIDsToCompare = null;
            SelectedCancers = null;
            SelectedSpecimenTypes = null;

            // cohorts
            CohortIDsToCompare = select_cohort.GetSelectedCohortIDs();
            // cancer types
            SelectedCancers = select_cancer.GetSelectedCancers();
            // specimen types
            SelectedSpecimenTypes = select_specimenTypes.GetSelectedSpecimenTypes();

            string missingFields = string.Empty;
            if (CohortIDsToCompare.Length == 0)
            {
                missingFields += "&bull;Error: <b>Select Cohorts</b> is required<br/>";
                select_cohort.Attributes["class"] += " filter-component-required";
            }

            if (SelectedCancers.Length == 0)
            {
                missingFields += "&bull;Error: <b>Cancer Type</b> is required<br/>";
                select_cancer.Attributes["class"] += " filter-component-required";
            }

            if (SelectedSpecimenTypes.Length == 0)
            {
                missingFields += "&bull;Error: <b>Specimen Types</b> is required<br/>";
                select_specimenTypes.Attributes["class"] += " filter-component-required";
            }

            if (!String.IsNullOrWhiteSpace(missingFields))
                RegisterJSError(String.Format("<p>The form is missing required fields. Please check the following fields then try again.<p>{0}</p>", missingFields));
        }
        #endregion

        private void PopulateBioCountsGrid()
        {
            bioTbl = new DataTable();
            bioTbl.TableName = "bioTable";

            if (CohortIDsToCompare.Length == 0 || SelectedCancers.Length == 0 || SelectedSpecimenTypes.Length == 0)
            {
                bioGridView.DataSource = bioTbl;
                bioGridView.DataBind();
                bioGridView.Visible = false;
                return;
            }
            else
            {
                bioGridView.Visible = true;
                bioGridView.Attributes["has_results"] = "true";
            }

            //-----
            // reset the position integer
            //_webfieldPosition = 0;
            //-----
            // retreive field display settings
            webfieldsTbl = CECWebSrv.GetCohortWebFieldsForBiospecimenGrid(UserToken).Tables[0];

            //-----
            // bind enrollment grid data
            System.Data.DataSet sg;
            if (CohortIDsToCompare.Length > 0 && SelectedCancers.Length > 0)
                sg = CECWebSrv.GetCohortForBiospecimenGrid(UserToken, SelectedCancers, CohortIDsToCompare, SelectedSpecimenTypes);
            else
            {
                sg = new DataSet();
                sg.Tables.Add("tbl_biospecimen");
            }

            System.Data.DataTable t = sg.Tables["tbl_biospecimen"];

            // add empty column at index 0
            bioTbl.Columns.Add(new DataColumn());
            bioTbl.Columns[0].ColumnName = "Specimens Type";
            bioTbl.Columns.Add(new DataColumn());
            bioTbl.Columns[1].ColumnName = "Cancer";
            bioTbl.PrimaryKey = new DataColumn[] { bioTbl.Columns[0], bioTbl.Columns[1] };

            // find acronym position and build the pivot table column names
            int acronymPosition = t.Columns.IndexOf("cohort_acronym");
            for (int _p = 0; _p < t.Rows.Count; _p++)
            {
                if (acronymPosition > -1 && bioTbl.Columns.IndexOf(t.Rows[_p][acronymPosition].ToString()) == -1)
                    bioTbl.Columns.Add(new DataColumn(t.Rows[_p][acronymPosition].ToString()));
            }
           // bioTbl.Columns.Add(new DataColumn("Total"));

            string _curSpecimen = string.Empty,
                _curCancer = string.Empty;
            int _curSpecimenIndx = 0,
                _curCancerIndx = -1;
            DataRow dr = null;
            // fill the pivot table
            for (int _col = 2; _col < t.Columns.Count; _col++)
            {
                if ((SelectedCancers.Length > _curCancerIndx + 1) && !bioTbl.Rows.Contains(new string[] { SelectedSpecimenTypes[_curSpecimenIndx], SelectedCancers[_curCancerIndx + 1] }))
                    _curCancerIndx++;
                else if (SelectedCancers.Length <= (_curCancerIndx + 1))
                {
                    _curCancerIndx = 0;
                    _curSpecimenIndx++;
                }

                if (!bioTbl.Rows.Contains(new string[] { SelectedSpecimenTypes[_curSpecimenIndx], SelectedCancers[_curCancerIndx] }))
                {
                    dr = bioTbl.NewRow();
                    dr[0] = SelectedSpecimenTypes[_curSpecimenIndx];
                    dr[1] = SelectedCancers[_curCancerIndx];

                    bioTbl.Rows.Add(dr);
                }

                for (int _row = 0; _row < t.Rows.Count; _row++)
                {

                    string _curCohort = (string)t.Rows[_row]["cohort_acronym"];
                    int aCol = bioTbl.Columns.IndexOf(_curCohort);

                    if (helper.IsNumerical(t.Rows[_row][_col]))
                        dr[aCol] = (int)t.Rows[_row][_col];
                    else
                        dr[aCol] = t.Rows[_row][_col];
                }
            }

            for (int _col = 1; _col < bioTbl.Columns.Count; _col++)
            {
                for (int _row = 0; _row < bioTbl.Rows.Count; _row++)
                {
                    if(helper.IsNumerical(bioTbl.Rows[_row][_col]))
                        bioTbl.Rows[_row][_col] = helper.FormatCount(int.Parse(bioTbl.Rows[_row][_col].ToString()));
                }
            }

            bioGridView.DataSource = bioTbl;
            bioGridView.DataBind();
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
                dataRow.CreateCell(1).SetCellValue("Biospecimen Counts");

                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Export Date:");
                dataRow.CreateCell(1).SetCellValue(DateTime.Now.ToString("MM/dd/yyyy"));

                // specimen type selections
                string tAss = string.Empty;
                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Specimen Type(s):");
                foreach (string s in SelectedSpecimenTypes)
                    tAss += String.Format("{0}, ", select_specimenTypes.GetSpecimenTypeLabel(s));
                tAss = (new CultureInfo("en-US").TextInfo.ToTitleCase(tAss.TrimEnd(new char[] { ' ', ',' })));
                dataRow.CreateCell(1).SetCellValue(tAss);
                // cancer type selections
                tAss = string.Empty;
                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Cancer Type(s):");
                foreach (string s in SelectedCancers)
                    tAss += String.Format("{0}, ", select_cancer.GetCancerLabel(s));
                tAss = (new CultureInfo("en-US").TextInfo.ToTitleCase(tAss.TrimEnd(new char[] { ' ', ',' })));
                dataRow.CreateCell(1).SetCellValue(tAss);
                // cohort selections
                tAss = string.Empty;
                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Selected Cohort(s):");
                foreach (string s in CohortIDsToCompare)
                    tAss += String.Format("{0}, ", select_cohort.GetCohortAcronym(int.Parse(s)));
                tAss = (new CultureInfo("en-US").TextInfo.ToTitleCase(tAss.TrimEnd(new char[] { ' ', ',' })));
                dataRow.CreateCell(1).SetCellValue(tAss);

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
                        if (_p == 0)
                            c.SetCellValue(CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, cellVal));
                        else if (_p == 1)
                            c.SetCellValue(select_cancer.GetCancerLabel(cellVal));
                        else if (helper.IsStringEmptyWhiteSpace(cellVal) || cellVal == "&nbsp;" || cellVal == "-1")
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
    }
}