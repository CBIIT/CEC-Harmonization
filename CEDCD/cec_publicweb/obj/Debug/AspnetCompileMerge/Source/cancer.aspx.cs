using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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


    public partial class cancer : CECPage
    {
        private System.Data.DataTable cancerTbl,
                webfieldsTbl;

        /// <summary>
        /// create the necessary childcontrols for filtering
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            cancerGridView.ClientIDMode = ClientIDMode.Static;

            select_cohort.UserToken = UserToken;
            select_cancer.UserToken = UserToken;
        }

        #region Properties

        /// <summary>
        /// get or set string array of cohort ids
        /// </summary>
        protected string[] CohortIDsToCompare
        {
            get
            {
                if ((Session["CancerCohortIDsToCompare"] != null) && ((string[])Session["CancerCohortIDsToCompare"]).Length > 0)
                    return (string[])Session["CancerCohortIDsToCompare"];
                else
                    return new string[0];
            }
            set
            {
                Session["CancerCohortIDsToCompare"] = value;
            }
        }

        /// <summary>
        /// get the selected cancer category
        /// </summary>
        //protected string CancerCategory
        //{
        //    get
        //    {
        //       // if (rb_prevalent.Checked)
        //       //     return "prevalent";
        //       // else
        //            return "incident";
        //    }
        //}

        protected string[] CancerTypes
        {
            get
            {
                if ((Session["pgCancerTypes"] != null) && ((string[])Session["pgCancerTypes"]).Length > 0)
                    return (string[])Session["pgCancerTypes"];
                else
                    return new string[0];
            }
            set
            {
                Session["pgCancerTypes"] = value;
            }
        }

        protected string[] Genders
        {
            get
            {
                if ((Session["pgCancerGenders"] != null) && ((string[])Session["pgCancerGenders"]).Length > 0)
                    return (string[])Session["pgCancerGenders"];
                else
                    return new string[0];
            }
            set
            {
                Session["pgCancerGenders"] = value;
            }
        }
        #endregion

        #region Event Handling
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // instantiate data tables
            cancerTbl = new DataTable();
            cancerTbl.TableName = "cancerTable";
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                CohortIDsToCompare = new string[0];
                CancerTypes = new string[0];
                Genders = new string[0];

                CECWebSrv.AuditLog_AddActivity(UserToken.userid, "cancer page");
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            PopulateCancerGrid();

            submitBtn.Attributes["disabled"] = "disabled";

            base.OnLoadComplete(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            exportTblBtn.Visible = (cancerTbl.Rows.Count > 0 ? true : false);
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

                        string filepath = String.Format("./user_files/{0}/cancer_{1}.xlsx", UserToken.userid, DateTime.Now.ToString("yyyyMMMddmm"));

                        PopulateCancerGrid();

                        ExportDataGridToExcel(cancerTbl, MapPath(filepath));
                        CECWebSrv.AuditLog_AddActivity(UserToken.userid, "cancer export created");

                        Page.ClientScript.RegisterStartupScript(GetType(), "downloadExport",
                            String.Format("<script>window.open('{0}');</script>", filepath));

                        break;
                }

                return handled;
            }
            else
                return base.OnBubbleEvent(source, args);
        }

        protected void cancerGridView_RowDataBound(object source, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int _i = 0; _i < e.Row.Cells.Count; _i++)
                {
                    System.Web.UI.WebControls.TableHeaderCell hc =
                        new TableHeaderCell();
                    hc.ID = cancerTbl.Columns[_i].ColumnName.Replace(" ", string.Empty);

                    if (CECWebSrv.IsCohort(UserToken, e.Row.Cells[_i].Text))
                    {
                        DataRow dr_cohort = null;
                        using (DataTable dt = CECWebSrv.GetFilteredCohortRecords(UserToken, "cohort_id, cohort_name, cohort_acronym", String.Format("cohort_acronym='{0}'", cancerTbl.Columns[_i].ColumnName)).Tables[0])
                            dr_cohort = dt.Rows[0];

                        System.Web.UI.HtmlControls.HtmlAnchor cohortLnk =
                                new HtmlAnchor();
                        cohortLnk.HRef = String.Format("./cohortDetails.aspx?cohort_id={0}", dr_cohort["cohort_id"]);
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
                e.Row.TableSection = TableRowSection.TableHeader;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                /// if the cell is just "Select Cancer Site..."
                if (e.Row.Cells[0] != null && (e.Row.Cells[0].Text == "Select Cancer Type To Continue" || e.Row.Cells[0].Text.Contains("None of the select cohorts reported any")))
                    return;

                ///---------------------------------------------------
                /// change to tableheadercell the gender
                /// 
                string orgColumnName = e.Row.Cells[1].Text;
                //string _curSumLbl = orgColumnName.Split('.')[1];

                System.Web.UI.WebControls.TableHeaderCell nmHdr =
                    new TableHeaderCell();
                nmHdr.Attributes["headers"] = cancerTbl.Columns[0].ColumnName.Replace(" ", string.Empty);
                nmHdr.ID = orgColumnName;
                nmHdr.Text = orgColumnName;
                //nmHdr.Controls.Add(new LiteralControl(orgColumnName)); //CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, orgColumnName)

                e.Row.Cells.RemoveAt(1);
                e.Row.Cells.AddAt(1, nmHdr);

                ///---------------------------------------------------
                /// change to tableheadercell the cancer
                /// 
                string _curCanLbl = e.Row.Cells[0].Text;
                if (Genders.Length == 2)
                {
                    if (e.Row.Cells[1].Text == "Males")
                    {
                        System.Web.UI.WebControls.TableHeaderCell cnHdr =
                         new TableHeaderCell();

                        cnHdr.CssClass = "comRowHeader";
                        cnHdr.ID = _curCanLbl;
                        cnHdr.RowSpan = 2;
                        cnHdr.Text = select_cancer.cancer_list[e.Row.Cells[0].Text];  ///cnHdr.Controls.Add(new LiteralControl()); //CECWebSrv.GetCancerLabel(UserToken, int.Parse(e.Row.Cells[0].Text)

                        e.Row.Cells.RemoveAt(0);
                        e.Row.Cells.AddAt(0, cnHdr);
                    }
                    else
                        e.Row.Cells.RemoveAt(0);
                }
                else
                {
                    System.Web.UI.WebControls.TableHeaderCell cnHdr =
                     new TableHeaderCell();

                    cnHdr.CssClass = "comRowHeader";
                    cnHdr.ID = _curCanLbl;
                    cnHdr.Text = select_cancer.cancer_list[e.Row.Cells[0].Text];
                    //cnHdr.Controls.Add(new LiteralControl()); //CECWebSrv.GetCancerLabel(UserToken, int.Parse(e.Row.Cells[0].Text)

                    e.Row.Cells.RemoveAt(0);
                    e.Row.Cells.AddAt(0, cnHdr);
                }

                ///-----------------------------------------------------
                /// go through the cells starting at a specific index to write
                ///  in attributes for screen readers
                for (int _i = 1; _i < e.Row.Cells.Count; _i++)
                {
                    string _cohort = string.Empty;
                    if (e.Row.Cells.Count < cancerTbl.Columns.Count)
                        _cohort = cancerTbl.Columns[_i + 1].ColumnName;
                    else
                        _cohort = cancerTbl.Columns[_i].ColumnName;

                    e.Row.Cells[_i].Attributes["headers"] = String.Format("{0} {1} {2}", _curCanLbl, orgColumnName, _cohort);

                    /// ---------------------------------------
                    ///  was N/P, per Amy 23-April, 0 is preferred
                    ///  per Amy, 4-May, 0 should appear if the cohort has incident cancers--even for prevelant;
                    ///   show N/P if incident and prevelant is not provided
                    if (String.IsNullOrWhiteSpace(e.Row.Cells[_i].Text) || e.Row.Cells[_i].Text == "-1" || e.Row.Cells[_i].Text == "&nbsp;")
                    {
                        e.Row.Cells[_i].Text = "N/P";
                        //if (CECWebSrv.CohortHasCancerIndicator(UserToken, (CancerCategory.ToLower() == "incident" ? "prevalent" : "incident"), _cohort))
                        //e.Row.Cells[_i].Text = "0";
                        //else
                        //    e.Row.Cells[_i].Text = "N/P";
                    }
                }
            }
        }

        protected void ClearOptions_Clicked(object sender, EventArgs e)
        {
            CohortIDsToCompare = new string[0];
            CancerTypes = new string[0];
            Genders = new string[0];

            select_cohort.ClearSelectedCohortIDs();
            select_cancer.ClearSelectedCancers();

            cb_gmale.Checked = false;
            cb_gfemale.Checked = false;

            EnsureChildControls();

            PopulateCancerGrid();
        }

        protected void Submit_Clicked(object source, EventArgs e)
        {
            EnsureChildControls();

            CancerTypes = null;
            CohortIDsToCompare = null;
            Genders = null;

            // cohorts
            CohortIDsToCompare = select_cohort.GetSelectedCohortIDs();
            // cancer types
            CancerTypes = select_cancer.GetSelectedCancers(); 
          
            // genders
            Genders = gender.GetSelectedOptions();

            string missingFields = string.Empty;
            if (Genders.Length == 0) // && !cb_gunknown.Checked
            {
                missingFields += "&bull;Error: <b>Gender</b> is required<br/>";
                gender_area.Attributes["class"] += " filter-component-required";
            }
            /*else
            {
                gender_area.Attributes["class"] = " filter-component";

                System.Collections.ArrayList g = new ArrayList();
                if (cb_gfemale.Checked)
                    g.Add("female");
                if (cb_gmale.Checked)
                    g.Add("male");

                Genders = (string[])g.ToArray(typeof(string));
            }*/

            if (CohortIDsToCompare.Length == 0)
            {
                missingFields += "&bull;Error: <b>Select Cohorts</b> is required<br/>";
                select_cohort.Attributes["class"] += " filter-component-required";
            }

            //if (!rb_incident.Checked && !rb_prevalent.Checked)
            //{
            //    missingFields += "&bull;Error: <b>Incident or Prevalent</b> is required<br/>";
            //    cancer_incident_area.Attributes["class"] += " filter-component-required";
            //}
            //else
            //    cancer_incident_area.Attributes["class"] = " filter-component";

            if (CancerTypes.Length == 0)
            {
                missingFields += "&bull;Error: <b>Cancer Type</b> is required<br/>";
                select_cancer.Attributes["class"] += " filter-component-required";
            }

            if (!String.IsNullOrWhiteSpace(missingFields))
                RegisterJSError(String.Format("<p>The form is missing required fields. Please check the following fields then try again.<p>{0}</p>", missingFields));
        }

        #endregion
    
        private void PopulateCancerGrid()
        {
            Genders = gender.GetSelectedOptions(); 

            cancerTbl = new DataTable();
            cancerTbl.TableName = "cancerTable";

            if (CohortIDsToCompare.Length == 0 || CancerTypes.Length == 0 || Genders.Length == 0) //|| CancerCategory.Length == 0
            {
                cancerGridView.DataSource = cancerTbl;
                cancerGridView.DataBind();
                cancerGridView.Visible = false;
                return;
            }
            else
            {
                cancerGridView.Visible = true;
                cancerGridView.Attributes["has_results"] = "true";
            }

            //-----
            // retreive field display settings
            webfieldsTbl = CECWebSrv.GetCohortWebFieldsForCancerGrid(UserToken).Tables[0];

            /*ArrayList tGenders = new ArrayList();
            if (cb_gmale.Checked)
                tGenders.Add("male");
            if (cb_gfemale.Checked)
                tGenders.Add("female"); */

            // (string[])tGenders.ToArray(typeof(string));

            //-----
            // bind cancer grid data
            System.Data.DataSet sg;
            if (CohortIDsToCompare.Length > 0 && CancerTypes.Length > 0)
                sg = CECWebSrv.GetCohortForCancerGrid(UserToken, CancerTypes, Genders, CohortIDsToCompare);
            else
            {
                sg = new DataSet();
                sg.Tables.Add("tbl_cancer");
            }

            DataTable t = sg.Tables["tbl_cancer"];

            // cancerTbl instantiated in OnInit()
            //cancerTbl = new DataTable();
            //cancerTbl.TableName = "cancerTable";

            // add empty column at index 0 and 1
            cancerTbl.Columns.Add(new DataColumn());
            cancerTbl.Columns[0].ColumnName = "Cancer";
            cancerTbl.Columns.Add(new DataColumn());
            cancerTbl.Columns[1].ColumnName = "Gender";
            cancerTbl.PrimaryKey = new DataColumn[] { cancerTbl.Columns[0], cancerTbl.Columns[1] };

            // if nothing selected to query, end routine here
            if (CohortIDsToCompare.Length == 0 || CancerTypes.Length == 0 || Genders.Length == 0)
            {
                cancerTbl.Rows.Add((new object[] { "Select options to continue" }));
                return;
            }

            // find acronym position and build the pivot table column names
            int acronymPosition = t.Columns.IndexOf("cohort_acronym");
            for (int _p = 0; _p < t.Rows.Count; _p++)
            {
                if (acronymPosition > -1 && cancerTbl.Columns.IndexOf(t.Rows[_p][acronymPosition].ToString()) == -1)
                    cancerTbl.Columns.Add(new DataColumn(t.Rows[_p][acronymPosition].ToString()));
            }

            ///---------------------------------------------
            /// fill pivot table of reported cancer counts
            string _curCancer = string.Empty;
            int _curCancerIndx = 0;
            DataRow dr_m = null,
                    dr_f = null;

            bool male = false,
                female = false;
            foreach(string s in Genders)
            {
                if(s == "male")
                    male = true;
                else if(s == "female")
                    female = true;
            }
            ///--------------------------------------------
            /// any cancer sites selected
            if (CancerTypes.Length > 0 && CohortIDsToCompare.Length > 0)
            {
                for (int _col = 2; _col < t.Columns.Count; _col++)
                {
                    if ((CancerTypes.Length > _curCancerIndx) && _curCancer != CancerTypes[_curCancerIndx])
                    {
                        _curCancer = CancerTypes[_curCancerIndx];
                        if (!cancerTbl.Rows.Contains(new string[] { _curCancer, "Males" }))  //t.Rows[_row]["cancer_type_id"].ToString())     !t.Columns[_col].ColumnName.ToLower().Contains(CancerTypes[_curCancerIndx])
                        {
                            //_curCancer = CancerTypes[_curCancerIndx++]; //t.Rows[_row]["cancer_type_id"].ToString();
                            dr_m = cancerTbl.NewRow();
                            dr_m[0] = _curCancer;
                            dr_m[1] = string.Format("{0}", "Males");

                            if (male)
                                cancerTbl.Rows.Add(dr_m);
                        }

                        if (!cancerTbl.Rows.Contains(new string[] { _curCancer, "Females" }))
                        {
                            dr_f = cancerTbl.NewRow();
                            dr_f[0] = _curCancer;
                            dr_f[1] = string.Format("{0}", "Females");

                            if (female)
                                cancerTbl.Rows.Add(dr_f);
                        }
                    }

                    for (int _row = 0; _row < t.Rows.Count; _row++)
                    {
                        //if ((CancerTypes.Length > _curCancerIndx) && !t.Columns[_col].ColumnName.ToLower().Contains(CancerTypes[_curCancerIndx]))
                        //    _curCancerIndx++;

                        //if (_curCancerIndx >= CancerTypes.Length)
                        //    break;


                        string _curCohort = (string)t.Rows[_row]["cohort_acronym"];
                        int aCol = cancerTbl.Columns.IndexOf(_curCohort);

                        if (t.Columns[_col].ColumnName.ToLower().Contains("_male"))
                            dr_m[aCol] = t.Rows[_row][_col];

                        if (t.Columns[_col].ColumnName.ToLower().Contains("_female"))
                            dr_f[aCol] = t.Rows[_row][_col];

                    }

                    if ((t.Columns.Count > _col + 1) && !t.Columns[_col + 1].ColumnName.ToLower().Contains(CancerTypes[_curCancerIndx]))
                        _curCancerIndx++;
                }

                for (int _col = 0; _col < cancerTbl.Columns.Count; _col++)
                {
                    for (int _row = 0; _row < cancerTbl.Rows.Count; _row++)
                    {
                        if (helper.IsNumerical(cancerTbl.Rows[_row][_col]))
                            cancerTbl.Rows[_row][_col] = helper.FormatCount(int.Parse(cancerTbl.Rows[_row][_col].ToString()));
                    }
                }
            }

            cancerGridView.DataSource = cancerTbl;
            cancerGridView.DataBind();
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
                dataRow.CreateCell(1).SetCellValue("Cancer Counts");

                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Export Date:");
                dataRow.CreateCell(1).SetCellValue(DateTime.Now.ToString("MM/dd/yyyy"));
                
                // gender selections
                string tAss = string.Empty;
                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Selected Gender(s):");
                foreach (string s in Genders)
                    tAss += String.Format("{0}, ", s);
                tAss = (new CultureInfo("en-US").TextInfo.ToTitleCase(tAss.TrimEnd(new char[] { ' ', ',' })));
                dataRow.CreateCell(1).SetCellValue(tAss);
                // cancer category
                tAss = string.Empty;
                //dataRow = wkst.CreateRow(rowIndex++);
                //dataRow.CreateCell(0).SetCellValue("Cancer Category:");
                //dataRow.CreateCell(1).SetCellValue((new CultureInfo("en-US").TextInfo.ToTitleCase(CancerCategory)));
                // cancer selections
                tAss = string.Empty;
                dataRow = wkst.CreateRow(rowIndex++);
                dataRow.CreateCell(0).SetCellValue("Selected Cancer Type(s):");
                foreach (string c in CancerTypes)
                    tAss += String.Format("{0}, ", select_cancer.GetCancerLabel(c));
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

                if (toExport.Rows.Count > 1)
                {
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
                            /// position 0 is cancer type, position 1 is male/female
                            if (_p == 0)
                                c.SetCellValue(select_cancer.GetCancerLabel(cellVal));
                            else if (helper.IsStringEmptyWhiteSpace(cellVal) || cellVal == "&nbsp;" || cellVal == "-1")
                            {
                                /// ---------------------------------------
                                ///  was N/P, per Amy 23-April, 0 is preferred
                                ///  per Amy, 4-May, 0 should appear if the cohort has incident cancers--even for prevelant;
                                ///   show N/P if incident and prevelant is not provided
                                if (String.IsNullOrWhiteSpace(cellVal) || cellVal == "&nbsp;")
                                {
                                    //if (CECWebSrv.CohortHasCancerIndicator(UserToken, toExport.Columns[_p].ColumnName))
                                        cellVal = "0";
                                    //else
                                    //    cellVal = "N/P";
                                }
                                else
                                    cellVal = "N/P";

                                c.SetCellValue(cellVal);
                            }
                            else
                                c.SetCellValue(cellVal);

                            colPos++;
                        }
                    }
                }
                else
                {
                    ICell c = wkst.CreateRow(rowIndex++).GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    c.SetCellValue(String.Format("None of the select cohorts reported any 'todo' cancers"));
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