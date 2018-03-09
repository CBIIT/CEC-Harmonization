using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MicaData;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Omu.AwesomeMvc;
using NPOI.SS.Util;
using NPOI.HSSF.Util;

namespace CECHarmonization.Controllers
{
    public class GridExportController : Controller
    {

        DATA.MicaRepository ado = new DATA.MicaRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetItems(GridParams g)
        {
            return Json(BuildGridModel(g));
        }

        private GridModel<variable_vw> BuildGridModel(GridParams g)
        {

            // retreive Session info created when selections were made to mpre-map grid
            Dictionary<string, string> GroupedIds = Session["PreMap_GroupedIds"] as Dictionary<string, string>;

            var dt = ado.GetVariable_vw(GroupedIds);

            return new GridModelBuilder<variable_vw>(dt.AsQueryable(), g)
            {
                // same as pre-map setup
                Key = "nid",
                //Map = o => new { o.Id, o.Person, o.Food, o.Date, o.Price, o.Location, ChefName = o.Chef.FirstName + " " + o.Chef.LastName },
                MakeFooter = MakeFooter,
                MakeHeader = gr =>
                {
                    //get first item in the group
                    var first = gr.Items.First();

                    //get the grouped column value(s) for the first item
                    var val = string.Join(" ", AweUtil.GetColumnValue(gr.Column, first).Select(ToStr));

                    return new GroupHeader
                    {
                        Content = string.Format(" {0} : {1} ( Count = {2}, <input type='checkbox' name='id' value='{3}'/> )",
                                gr.Header,
                                val,
                                gr.Items.Count(),
                            //gr.Items.Max(o => o.delta)
                                null),
                        Collapsed = false
                    };
                }
            }.Build();
        }


        private object MakeFooter(GroupInfo<variable_vw> info)
        {
            //will add the word Total at the grid level footer (Level == 0)
            var pref = info.Level == 0 ? "Total " : "";

            return new
            {
                Title = pref + " count = " + info.Items.Count()   //,
                //Location = info.Items.Select(o => o.Location).Distinct().Count() + " distinct locations",
                //Date = pref + " max: " + info.Items.Max(o => o.Date).Date.ToShortDateString(),
                //Price = info.Items.Sum(o => o.Price),
                //ChefCount = info.Items.Select(o => o.Chef.Id).Distinct().Count() + " chefs"
            };
        }

        private string ToStr(object o)
        {
            return o is DateTime ? ((DateTime)o).ToShortDateString() : o.ToString();
        }

        [HttpPost]
        public ActionResult ExportGridToExcel(GridParams g)
        {
            var gridModel = BuildGridModel(g);

            var columns = new[] { "Id", "title", "Label", "Dataset", "Name", "Value", "Missing" };

            var workbook = GridExcelBuilder.Build(gridModel, columns);

            var stream = new MemoryStream();
            workbook.Write(stream);
            stream.Close();

            return File(stream.ToArray(), "application/vnd.ms-excel", "Metadata_based_on_Taxonomy.xls");
        }

        /// <summary>
        /// Demonstrates the simplest way of creating an excel workbook 
        /// it exports all the lunches records, without taking into count any sorting/paging that is done on the client side
        /// </summary>
        /// <returns></returns>
        //[HttpPost]

        public ActionResult ExportMetadataToExcel(string id)
        {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Metadata_based_on_Cohort");

            //// retreive Session info created when selections were made to mpre-map grid
            //Dictionary<string, string> GroupedIds = Session["PreMap_GroupedIds"] as Dictionary<string, string>;
            //string selectedText = Session["PreMap_selectedText"] as string;
            //string search = Session["PreMap_search"] as string;

            // title row
            var titleRow = sheet.CreateRow(0);
            titleRow.HeightInPoints = 45;
            var titlecell = titleRow.CreateCell(0);
            var datecell = titleRow.CreateCell(2);

            titlecell.CellStyle.WrapText = true;
            sheet.AddMergedRegion(CellRangeAddress.ValueOf("$A$1:$H$1"));
            sheet.SetColumnWidth(0, 5000);
            sheet.SetColumnWidth(1, 5000);
            sheet.SetColumnWidth(2, 5000);
            sheet.SetColumnWidth(3, 5000);
            sheet.SetColumnWidth(4, 6000);
            sheet.SetColumnWidth(5, 6000);
            sheet.SetColumnWidth(6, 5000);
            sheet.SetColumnWidth(7, 5000);

            // header row
            //var columns = new[] { "Id", "Study Name", "Dataset", "Title", "Variable Label", "Category Label", "Category Name", "Category Type", "Category Unit", "Category Missing", "Item Description" };
            var columns = new[] { "CMR Dataset Name", "Variable Name", "Variable Description", "Variable_Categories", "Variable_Categories_Label", "Variable_Categories_Missing", "Units", "Value_Type" };
            var headerRow = sheet.CreateRow(3);

            headerRow.HeightInPoints = 30;
            for (int i = 0; i < columns.Length; i++)
            {
                var headerCell = headerRow.CreateCell(i);
                headerCell.SetCellValue(columns[i]);
            }

            //fill content 
            var dt = ado.GetVariableValuesByStudy(id).Where(o => !o.dataset_name.ToUpper().Contains("HARMONIZED"));
            //).ToString() && OrderBy(o => o.dataset_name).ThenBy(p => p.title);

            string cohort = dt.First().study_name;
            titlecell.SetCellValue("Metadata for Study:  " + cohort + "       " + "Exported on: " + DateTime.Now.ToShortDateString().ToString());
            //datecell.SetCellValue("Exported on: " + DateTime.Now.ToShortDateString().ToString());
         

            int rowIndex = 4;   // start data at row 4
            foreach (variable_vw var_vw in dt.AsQueryable().ToList())
            {

                rowIndex = rowIndex + 1;
                var row = sheet.CreateRow(rowIndex);

                var DSName = row.CreateCell(0);
                DSName.SetCellValue(var_vw.dataset_name);

                var VarName = row.CreateCell(1);
                VarName.SetCellValue(var_vw.title);

                var VarDesc = row.CreateCell(2);
                VarDesc.SetCellValue(var_vw.field_label_value);

                var VarCat = row.CreateCell(3);
                VarCat.SetCellValue(var_vw.field_variable_categories_name);

                var VarCatL = row.CreateCell(4);
                VarCatL.SetCellValue(var_vw.field_variable_categories_label);

                var VarCatM = row.CreateCell(5);
                VarCatM.SetCellValue(var_vw.field_variable_categories_missing.ToString());

                var Units = row.CreateCell(6);
                Units.SetCellValue(var_vw.field_unit_value);

                var ValType = row.CreateCell(7);
                ValType.SetCellValue(var_vw.field_value_type_value);

                //var catunit = row.CreateCell(8);
                //catunit.SetCellValue(var_vw.field_unit_value);  // category unit

                //var catmissing = row.CreateCell(9);
                //catmissing.SetCellValue(var_vw.field_variable_categories_missing.Value);  //Category Missing

                //var itemdesc = row.CreateCell(10);
                //itemdesc.SetCellValue(var_vw.body_value); // item description

            }

            var stream = new MemoryStream();
            workbook.Write(stream);
            stream.Close();

            return File(stream.ToArray(), "application/vnd.ms-excel", "Metadata_based_on_Cohort.xls");
        }

        public ActionResult ExportAllToExcel()
        {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Metadata_based_on_Taxonomy");

            // retreive Session info created when selections were made to mpre-map grid
            Dictionary<string, string> GroupedIds = Session["PreMap_GroupedIds"] as Dictionary<string, string>;
            string selectedText = Session["PreMap_selectedText"] as string;
            string search = Session["PreMap_search"] as string;

            // title row
            var titleRow = sheet.CreateRow(0);
            titleRow.HeightInPoints = 45;
            var titlecell = titleRow.CreateCell(0);
            var datecell = titleRow.CreateCell(2);

            

            //if (search == "")
            //    titlecell.SetCellValue("Selected Items: " + selectedText );
            //else
            //    titlecell.SetCellValue("Selected Items: " + selectedText + "   Filtered on Study Name containing:  " + search);

            titlecell.CellStyle.WrapText = true;
            sheet.AddMergedRegion(CellRangeAddress.ValueOf("$A$1:$H$1"));
            sheet.SetColumnWidth(0, 5000);
            sheet.SetColumnWidth(1, 5000);
            sheet.SetColumnWidth(2, 5000);
            sheet.SetColumnWidth(3, 5000);
            sheet.SetColumnWidth(4, 6000);
            sheet.SetColumnWidth(5, 6000);
            sheet.SetColumnWidth(6, 5000);
            sheet.SetColumnWidth(7, 5000);

            // header row
            //var columns = new[] { "Id", "Study Name", "Dataset", "Title", "Variable Label", "Category Label", "Category Name", "Category Type", "Category Unit", "Category Missing", "Item Description" };
            var columns = new[] { "CMR Dataset Name", "Variable Name", "Variable Description", "Variable_Categories", "Variable_Categories_Label", "Variable_Categories_Missing", "Units", "Value_Type" };
            var headerRow = sheet.CreateRow(3);

            headerRow.HeightInPoints = 30;
            for (int i = 0; i < columns.Length; i++)
            {
                var headerCell = headerRow.CreateCell(i);
                headerCell.SetCellValue(columns[i]);
            }

            //fill content 
            var dt = ado.GetVariable_vw(GroupedIds).Where(o => o.study_name.ToLower().Contains(search)).OrderBy(o => o.dataset_name).ThenBy(p => p.title);

            string cohort = dt.First().study_name;
            titlecell.SetCellValue("Metadata for Taxonomies:  " + selectedText);
            datecell.SetCellValue("Exported on: " + DateTime.Now.ToShortDateString().ToString());

            int rowIndex = 4;   // start data at row 4
            foreach (variable_vw var_vw in dt.AsQueryable().ToList())
            {

                rowIndex = rowIndex + 1;
                var row = sheet.CreateRow(rowIndex);

                var DSName = row.CreateCell(0);
                DSName.SetCellValue(var_vw.dataset_name); 

                var VarName = row.CreateCell(1);
                VarName.SetCellValue(var_vw.title);

                var VarDesc = row.CreateCell(2);
                VarDesc.SetCellValue(var_vw.body_value);

                var VarCat = row.CreateCell(3);
                VarCat.SetCellValue(var_vw.field_variable_categories_name);

                var VarCatL = row.CreateCell(4);
                VarCatL.SetCellValue(var_vw.field_variable_categories_label);

                var VarCatM = row.CreateCell(5);
                VarCatM.SetCellValue(var_vw.field_variable_categories_missing.ToString());

                var Units = row.CreateCell(6);
                Units.SetCellValue(var_vw.field_unit_value);

                var ValType = row.CreateCell(7);
                ValType.SetCellValue(var_vw.field_value_type_value);

                //var catunit = row.CreateCell(8);
                //catunit.SetCellValue(var_vw.field_unit_value);  // category unit

                //var catmissing = row.CreateCell(9);
                //catmissing.SetCellValue(var_vw.field_variable_categories_missing.Value);  //Category Missing

                //var itemdesc = row.CreateCell(10);
                //itemdesc.SetCellValue(var_vw.body_value); // item description

            }

            var stream = new MemoryStream();
            workbook.Write(stream);
            stream.Close();

            return File(stream.ToArray(), "application/vnd.ms-excel", "Metadata_based_on_Taxonomy.xls");
        }
    }

    public static class GridExcelBuilder
    {
        public static HSSFWorkbook Build<T>(GridModel<T> gridModel, string[] columns)
        {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("sheet1");
            var headerRow = sheet.CreateRow(0);

            //create header
            for (int i = 0; i < columns.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(columns[i]);
            }

            var currentRow = 0;

            if (gridModel.Data.Groups != null)
            {
                foreach (var groupView in gridModel.Data.Groups)
                {
                    BuildGroup(sheet, columns, ref currentRow, groupView);
                }
            }
            else if (gridModel.Data.Items != null)
            {
                BuildItems(sheet, columns, ref currentRow, gridModel.Data.Items);
            }

            if (gridModel.Data.Footer != null)
            {
                BuildFooter(sheet, columns, ref currentRow, gridModel.Data.Footer);
            }

            return workbook;
        }

        private static void BuildGroup<T>(ISheet sheet, string[] columns, ref int currentRow, GroupView<T> groupView)
        {
            if (groupView.Header != null)
            {
                currentRow++;
                var row = sheet.CreateRow(currentRow);
                var cell = row.CreateCell(0);
                cell.SetCellValue(groupView.Header.Content);
            }

            if (groupView.Groups != null)
            {
                foreach (var groupViewx in groupView.Groups)
                {
                    BuildGroup(sheet, columns, ref currentRow, groupViewx);
                }
            }
            else if (groupView.Items != null)
            {
                BuildItems(sheet, columns, ref currentRow, groupView.Items);
            }

            if (groupView.Footer != null)
            {
                BuildFooter(sheet, columns, ref currentRow, groupView.Footer);
            }
        }

        private static void BuildItems(ISheet sheet, string[] columns, ref int currentRow, IList<object> items)
        {
            //fill content 
            foreach (var item in items)
            {
                currentRow++;
                var row = sheet.CreateRow(currentRow);

                for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
                {
                    var cell = row.CreateCell(columnIndex);
                    CellSetValue(cell, columns[columnIndex], item);
                }
            }
        }

        private static void BuildFooter(ISheet sheet, string[] columns, ref int currentRow, object footer)
        {
            currentRow++;
            var row = sheet.CreateRow(currentRow);
            for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
            {
                var cell = row.CreateCell(columnIndex);
                CellSetValue(cell, columns[columnIndex], footer);
            }
        }

        private static void CellSetValue(ICell cell, string column, object item)
        {
            var prop = item.GetType().GetProperty(column);

            if (prop != null)
            {
                var value = prop.GetValue(item, null);
                if (prop.PropertyType == typeof(DateTime))
                {
                    cell.SetCellValue(((DateTime)value).ToShortDateString());
                }
                else if (prop.PropertyType == typeof(int))
                {
                    cell.SetCellValue(Convert.ToDouble(value));
                }
                else
                {
                    cell.SetCellValue(value.ToString());
                }
            }
        }
    }

}