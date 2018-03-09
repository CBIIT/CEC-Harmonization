using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;



namespace cec_publicweb.input
{
    using cec_publicservice;


    public partial class Edit_Cohort : System.Web.UI.Page
    {
        private string field_highlights;
        private System.Data.DataTable dt_input, dt_cohort;
        private cec_publicservice.CECInputFormService CECWebSrv;

        private System.Collections.Specialized.NameValueCollection ethnicity, gender, race, cancers, biospecimens;

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
        #endregion

        protected void RegisterJSAlert(string text)
        {
            string literalStr =
                    String.Format("<div class='modal' tabindex='-1' id='alertModal' role='alertdialog' aria-labeledby='alertTitle' aria-describedby='alertMsg'><div class='modal-dialog' role=\"document\"><div class='modal-content'><div class='modal-header'><h4 id='alertTitle' class='modal-title'>Alert</h4></div> " +
                    "<div id='alertMsg' class='modal-body' aria-atomic='true'>{0}</div><div class='modal-footer'><div class='pull-right'><button type='button' id='modalAlertClose' class='btn btn-default' data-dismiss='modal'>Close</button></div></div><div class='modal-footer'></div></div></div></div>", text);

            ClientScript.RegisterClientScriptBlock(GetType(), "alert", literalStr + " <script type='text/javascript'>$('#alertModal').modal({backdrop:'static', keyboard:true, show:true}); $('#modalAlertClose').focus();</script>");
        }

        protected void RegisterJSError(string text)
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
            return Regex.Replace(value_code, @"[\W|\s]+", string.Empty);
        }

        private string FindCancerKeyByValue(string value)
        {
            for (int _i = 0; _i < cancers.Count; _i++)
            {
                if (cancers[_i] == value)
                    return cancers.Keys[_i];
            }

            return string.Empty;
        }

        private void CreateEditSection(ref Control current_container, DataRow dr)
        {
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    section.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    section.Attributes["style"] = dr["container_style_css"].ToString();
            }

            System.Web.UI.HtmlControls.HtmlGenericControl cnt = new HtmlGenericControl("div");
            section.Controls.Add(cnt);
            if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
                    cnt.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
                else
                    cnt.Attributes["style"] = dr["input_style_css"].ToString();
            }
            cnt.Controls.Add(new LiteralControl(String.Format("{0}", dr["help_text"])));

            current_container = section;
        }

        private void CreateEditGroupPanel(ref Control current_container, DataRow dr)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl panel = new HtmlGenericControl("div");
            current_container.Controls.Add(panel);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    panel.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    panel.Attributes["style"] = dr["container_style_css"].ToString();
            }

            if (!String.IsNullOrWhiteSpace(dr["label_text"].ToString()))
            {
                System.Web.UI.HtmlControls.HtmlGenericControl panel_header = new HtmlGenericControl("div");
                panel_header.Controls.Add(new LiteralControl(String.Format("<h3>{0}</h3>", dr["label_text"])));
                panel.Controls.Add(panel_header);

                if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
                {
                    if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
                        panel_header.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
                    else
                        panel_header.Attributes["style"] = dr["label_style_css"].ToString();
                }
            }

            System.Web.UI.HtmlControls.HtmlGenericControl cnt = new HtmlGenericControl("div");
            if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
                    cnt.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
                else
                    cnt.Attributes["style"] = dr["input_style_css"].ToString();
            }
            panel.Controls.Add(cnt);

            current_container = cnt;
        }

        private void CreateEditLabel(ref Control current_container, DataRow dr)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl coldiv = new HtmlGenericControl("div");
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    coldiv.Attributes["class"] += dr["container_style_css"].ToString().TrimStart('.');
                else
                    coldiv.Attributes["style"] = dr["container_style_css"].ToString();
            }
            current_container.Controls.Add(coldiv);

            System.Web.UI.HtmlControls.HtmlGenericControl h4 = new HtmlGenericControl("h4");
            if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
                    h4.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
                else
                    h4.Attributes["style"] = dr["label_style_css"].ToString();
            }

            if (dr["label_text"].ToString().Contains("{{"))
            {
                if (dt_cohort.Rows.Count == 0)
                    throw new Exception("unable to render label text substitutions without cohort data");

                int last_start_index = 0;
                string label_text = dr["label_text"].ToString();
                bool done = false;
                while (!done)
                {
                    if (dr["label_text"].ToString().IndexOf("}}", last_start_index) == -1)
                        throw new Exception(String.Format("label with id {0} does not have a matching closing bracket", dr["id"]));

                    string data_field = dr["label_text"].ToString();
                    int start = data_field.IndexOf("{{", last_start_index) + 2,
                        end = data_field.IndexOf("}}", start);
                    data_field = data_field.Substring(start, end - start);

                    if (dt_cohort.Columns[data_field] == null)
                        throw new Exception(String.Format("unrecognized data field substitution for label: {0}", data_field));

                    string data_value = dt_cohort.Rows[0][data_field].ToString();
                    label_text = label_text.Replace("{{" + data_field + "}}", data_value);

                    last_start_index = end;
                    if (dr["label_text"].ToString().IndexOf("{{", last_start_index) == -1)
                        done = true;
                }
                h4.InnerHtml = label_text;
            }
            else
                h4.InnerHtml = dr["label_text"].ToString();

            coldiv.Controls.Add(h4);
        }

        private void CreateEditCheckbox(ref Control current_container, DataRow dr)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl colDiv =
                new HtmlGenericControl("div");
            current_container.Controls.Add(colDiv);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    colDiv.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    colDiv.Attributes["style"] = dr["container_style_css"].ToString();
            }

            if (UserToken.access_level >= 200 && field_highlights.Contains(dr["data_field"].ToString()))
                colDiv.Attributes["class"] += " field-has-changes bg-info text-info";

            System.Web.UI.HtmlControls.HtmlGenericControl cntDiv = new HtmlGenericControl("div");
            colDiv.Controls.Add(cntDiv);
            if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
                    cntDiv.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
                else
                    cntDiv.Attributes["style"] = dr["input_style_css"].ToString();
            }

            System.Web.UI.WebControls.CheckBox ckbx = new CheckBox();
            ckbx.ID = String.Format("{0}", dr["data_field"]);

            if (dr["attributes"] != DBNull.Value && !String.IsNullOrWhiteSpace(dr["attributes"].ToString()))
            {
                string[] elAttribs = dr["attributes"].ToString().Split('|');
                foreach (string attribs in elAttribs)
                {
                    string[] attribPair = attribs.Split('=');
                    if (attribPair.Length != 2)
                        throw new Exception(String.Format("Input with id {0} has unparsable attributes", dr["id"]));
                    else
                        ckbx.Attributes[attribPair[0]] = attribPair[1];
                }
            }

            System.Web.UI.HtmlControls.HtmlGenericControl label = new HtmlGenericControl("label");
            if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
                    label.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
                else
                    label.Attributes["style"] = dr["label_style_css"].ToString();
            }
            label.Controls.Add(ckbx);
            label.Controls.Add(new LiteralControl(String.Format(" {0}", dr["label_text"])));

            cntDiv.Controls.Add(label);
        }

        private void CreateEditCheckboxText(ref Control current_container, DataRow dr)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl colDiv =
                new HtmlGenericControl("div");
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    colDiv.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    colDiv.Attributes["style"] = dr["container_style_css"].ToString();
            }

            if (UserToken.access_level >= 200 && field_highlights.Contains(dr["data_field"].ToString()))
                colDiv.Attributes["class"] += " field-has-changes  bg-info text-info";

            current_container.Controls.Add(colDiv);

            bool include_text = false;
            int input_field_id = 0;

            System.Web.UI.HtmlControls.HtmlGenericControl cntDiv = new HtmlGenericControl("div");
            colDiv.Controls.Add(cntDiv);
            if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
                    cntDiv.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
                else
                    cntDiv.Attributes["style"] = dr["input_style_css"].ToString();
            }

            System.Web.UI.WebControls.CheckBox ckbx = new CheckBox();
            ckbx.ID = String.Format("{0}", dr["data_field"]);

            if (dr["attributes"] != DBNull.Value && !String.IsNullOrWhiteSpace(dr["attributes"].ToString()))
            {
                string[] elAttribs = dr["attributes"].ToString().Split('|');
                foreach (string attribs in elAttribs)
                {
                    if (attribs.Contains("[") && attribs.Contains("]"))
                    {
                        int start_pos = attribs.IndexOf('[') + 1,
                            len = attribs.IndexOf(']') - start_pos;

                        input_field_id = int.Parse(attribs.Substring(start_pos, len));
                        include_text = true;

                    }
                }
                /*string[] elAttribs = dr["attributes"].ToString().Split('|');
                foreach (string attribs in elAttribs)
                {
                    string[] attribPair = attribs.Split('=');
                    if (attribPair.Length != 2)
                        throw new Exception(String.Format("Input with id {0} has unparsable attributes", dr["id"]));
                    else
                        ckbx.Attributes[attribPair[0]] = attribPair[1];
                }*/
            }

            System.Web.UI.HtmlControls.HtmlGenericControl label = new HtmlGenericControl("label");
            if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
                    label.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
                else
                    label.Attributes["style"] = dr["label_style_css"].ToString();
            }
            label.Controls.Add(ckbx);
            label.Controls.Add(new LiteralControl(String.Format(" {0}", dr["label_text"])));
            cntDiv.Controls.Add(label);

            if (include_text)
            {
                System.Web.UI.Control tmp_container = current_container;
                current_container = colDiv;

                System.Data.DataRow[] childCtrl_dr = dt_input.Select(String.Format("id={0}", input_field_id));
                if (childCtrl_dr.Length == 1)
                {
                    colDiv.Attributes["onchange"] = String.Format("input_clear_associate_text_field(this);");
                    CreateEditTextBox(ref current_container, childCtrl_dr[0], true);
                }
                else
                    throw new Exception(String.Format("error encountered while searching for control with id:{0}", input_field_id));

                current_container = tmp_container;
            }
        }

        private void CreateEditRadioButton(ref Control current_container, DataRow dr)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl colDiv =
                new HtmlGenericControl("div");
            current_container.Controls.Add(colDiv);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    colDiv.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    colDiv.Attributes["style"] = dr["container_style_css"].ToString();
            }

            if (UserToken.access_level >= 200 && field_highlights.Contains(dr["data_field"].ToString()))
                colDiv.Attributes["class"] += " field-has-changes bg-info text-info";

            bool alternate = false;
            if ((dr["section"].ToString() == "17" || dr["section"].ToString() == "30") && dr["input_style_css"].ToString().ToLower() == ".radio-inline")
                alternate = true;

            System.Web.UI.HtmlControls.HtmlGenericControl label =
                new HtmlGenericControl((alternate ? "legend" : "h4"));
            if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
                    label.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
                else
                    label.Attributes["style"] = dr["label_style_css"].ToString();
            }

            if (!String.IsNullOrWhiteSpace(dr["label_text"].ToString()))
            {
                label.InnerHtml = dr["label_text"].ToString();
                colDiv.Controls.Add(label);
            }

            string[] radio_options = dr["attributes"].ToString().Split('|');
            foreach (string radio_option in radio_options)
            {
                string option = string.Empty,
                    option_label = string.Empty;
                if (radio_option.Contains(":"))
                {
                    string[] split = radio_option.Split(':');
                    option_label = split[0];
                    option = split[1];
                }
                else
                    option_label = radio_option;

                System.Web.UI.HtmlControls.HtmlGenericControl cntDiv = new HtmlGenericControl("div");
                colDiv.Controls.Add(cntDiv);
                if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
                {
                    if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
                        cntDiv.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
                    else
                        cntDiv.Attributes["style"] = dr["input_style_css"].ToString();
                }

                System.Web.UI.WebControls.RadioButton radio = new RadioButton();
                radio.ID = String.Format("{0}_{1}", dr["data_field"], (!String.IsNullOrWhiteSpace(option) ? SteralizeValueCode(option) : option_label).ToLower());
                radio.GroupName = dr["data_field"].ToString();
                radio.Attributes["value"] = (!String.IsNullOrWhiteSpace(option) ? option : option_label);
                string ts = radio.Attributes["value"];
                bool tb = !String.IsNullOrWhiteSpace(option);

                System.Web.UI.HtmlControls.HtmlGenericControl labelctrl =
                    new HtmlGenericControl("label");
                labelctrl.Controls.Add(radio);
                labelctrl.Controls.Add(new LiteralControl(option_label));

                cntDiv.Controls.Add(labelctrl);
            }
        }

        private void CreateEditRadioText(ref Control current_container, DataRow dr)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl colDiv =
                new HtmlGenericControl("div");
            current_container.Controls.Add(colDiv);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    colDiv.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    colDiv.Attributes["style"] = dr["container_style_css"].ToString();
            }

            if (UserToken.access_level >= 200 && field_highlights.Contains(dr["data_field"].ToString()))
                colDiv.Attributes["class"] += " field-has-changes bg-info text-info";

            bool alternate = false;
            if ((dr["section"].ToString() == "17" || dr["section"].ToString() == "30") && dr["input_style_css"].ToString().ToLower() == ".radio-inline")
                alternate = true;

            System.Web.UI.HtmlControls.HtmlGenericControl label =
                new HtmlGenericControl((alternate ? "legend" : "h4"));
            if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
                    label.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
                else
                    label.Attributes["style"] = dr["label_style_css"].ToString();
            }

            if (!String.IsNullOrWhiteSpace(dr["label_text"].ToString()))
            {
                label.InnerHtml = dr["label_text"].ToString();
                colDiv.Controls.Add(label);
            }

            string[] radio_options = dr["attributes"].ToString().Split('|');
            foreach (string radio_option in radio_options)
            {
                bool include_text = false;
                int input_field_id = 0;

                string option = string.Empty,
                    option_label = string.Empty;
                if (radio_option.Contains(":"))
                {
                    string[] split = radio_option.Split(':');
                    option_label = split[0];
                    option = split[1];
                }
                else
                    option_label = radio_option;

                if (option.Contains("[") && option.Contains("]"))
                {
                    int start_pos = option.IndexOf('[') + 1,
                        len = option.IndexOf(']') - start_pos;

                    input_field_id = int.Parse(option.Substring(start_pos, len));
                    include_text = true;

                    option = option.Substring(0, start_pos - 1);
                }

                System.Web.UI.HtmlControls.HtmlGenericControl cntDiv = new HtmlGenericControl("div");
                colDiv.Controls.Add(cntDiv);
                if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
                {
                    if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
                        cntDiv.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
                    else
                        cntDiv.Attributes["style"] = dr["input_style_css"].ToString();
                }

                System.Web.UI.WebControls.RadioButton radio = new RadioButton();
                radio.ID = String.Format("{0}_{1}", dr["data_field"], (!String.IsNullOrWhiteSpace(option) ? SteralizeValueCode(option) : option_label).ToLower());
                radio.GroupName = dr["data_field"].ToString();
                radio.Attributes["value"] = (!String.IsNullOrWhiteSpace(option) ? option : option_label);

                System.Web.UI.HtmlControls.HtmlGenericControl labelctrl =
                    new HtmlGenericControl("label");
                labelctrl.Controls.Add(radio);
                labelctrl.Controls.Add(new LiteralControl(option_label));
                cntDiv.Controls.Add(labelctrl);

                if (include_text)
                {
                    System.Web.UI.Control tmp_container = current_container;
                    current_container = colDiv;

                    System.Data.DataRow[] childCtrl_dr = dt_input.Select(String.Format("id={0}", input_field_id));
                    if (childCtrl_dr.Length == 1)
                    {
                        colDiv.Attributes["onchange"] = String.Format("input_clear_associate_text_field(this);");
                        CreateEditTextBox(ref current_container, childCtrl_dr[0], true);
                    }
                    else
                        throw new Exception(String.Format("error encountered while searching for control with id:{0}", input_field_id));

                    current_container = tmp_container;
                }
            }
        }

        private void CreateEditTextBox(ref Control current_container, DataRow dr, bool textarea)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl cnt = new HtmlGenericControl("div");
            current_container.Controls.Add(cnt);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    cnt.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    cnt.Attributes["style"] = dr["container_style_css"].ToString();
            }

            if (UserToken.access_level >= 200 && field_highlights.Contains(dr["data_field"].ToString()))
                cnt.Attributes["class"] += " field-has-changes bg-info text-info";

            System.Web.UI.WebControls.TextBox itx = new TextBox();
            itx.ID = String.Format("{0}", dr["data_field"]);
            if (textarea)
                itx.TextMode = TextBoxMode.MultiLine;
            else
                itx.TextMode = TextBoxMode.SingleLine;

            if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
                    itx.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
                else
                    itx.Attributes["style"] = dr["input_style_css"].ToString();
            }

            if (dr["attributes"] != DBNull.Value && !String.IsNullOrWhiteSpace(dr["attributes"].ToString()))
            {
                string[] elAttribs = dr["attributes"].ToString().Split('|');
                foreach (string attribs in elAttribs)
                {
                    string[] attribPair = attribs.Split('=');
                    if (attribPair.Length != 2)
                        throw new Exception(String.Format("Input with id {0} has unparsable attributes", dr["id"]));
                    else
                    {
                        switch (attribPair[0])
                        {
                            case "rows":
                                itx.Rows = int.Parse(attribPair[1]);
                                break;
                            case "cols":
                                itx.Columns = int.Parse(attribPair[1]);
                                break;
                            default:
                                itx.Attributes[attribPair[0]] = attribPair[1];
                                break;
                        }
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(dr["help_text"].ToString()))
                itx.ToolTip = dr["help_text"].ToString();

            string validationStr = "";
            if (dr["validation_check"] != DBNull.Value && !String.IsNullOrWhiteSpace(dr["validation_check"].ToString()))
                validationStr = dr["validation_check"].ToString();

            string requiredFieldStr = "false";
            if ((bool)dr["required_field"])
                requiredFieldStr = "true";

            if (validationStr != "false" || requiredFieldStr != "false")
                itx.Attributes["onchange"] = String.Format("input_evaluate_field('{0}', {1}, {2}); ", itx.ID, (String.IsNullOrWhiteSpace(validationStr) ? "''" : String.Format("/{0}/g", validationStr)),
                                        requiredFieldStr);

            if (dr["label_text"].ToString() != ".")
            {
                System.Web.UI.HtmlControls.HtmlGenericControl label = new HtmlGenericControl("label");
                if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
                {
                    if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
                        label.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
                    else
                        label.Attributes["style"] = dr["label_style_css"].ToString();
                }

                if ((bool)dr["required_field"])
                    label.Attributes["class"] += " require-input ";

                label.Attributes["for"] = itx.ID;
                label.InnerHtml = String.Format("{0} ", dr["label_text"]);

                cnt.Controls.Add(label);
            }
            cnt.Controls.Add(itx);
        }

        private void CreateEditEnrollmentTable(ref Control current_container, DataRow dr)
        {
            //System.Web.UI.HtmlControls.HtmlGenericControl caption_label =
            //    new HtmlGenericControl("h4");
            //caption_label.InnerHtml = dr["label_text"].ToString();
            //current_container.Controls.Add(caption_label);
            //if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
            //{
            //    if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
            //        caption_label.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
            //    else
            //        caption_label.Attributes["style"] = dr["label_style_css"].ToString();
            //}

            System.Web.UI.WebControls.Table ctbl =
                new Table();
            ctbl.ID = "enrollment_table";
            current_container.Controls.Add(ctbl);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    ctbl.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    ctbl.Attributes["style"] = dr["container_style_css"].ToString();
            }
            //if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            //{
            //    if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
            //        ctbl.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
            //    else
            //        ctbl.Attributes["style"] = dr["input_style_css"].ToString();
            //}

            if (dr["attributes"] != DBNull.Value && !String.IsNullOrWhiteSpace(dr["attributes"].ToString()))
            {
                string[] elAttribs = dr["attributes"].ToString().Split('|');
                foreach (string attribs in elAttribs)
                {
                    string[] attribPair = attribs.Split('=');
                    if (attribPair.Length != 2)
                        throw new Exception(String.Format("Input with id {0} has unparsable attributes", dr["id"]));
                    else
                        ctbl.Attributes[attribPair[0]] = attribPair[1];
                }
            }

            ctbl.Caption = dr["label_text"].ToString();

            System.Web.UI.WebControls.TableHeaderRow hrI_hdr =
                new TableHeaderRow();
            hrI_hdr.TableSection = TableRowSection.TableHeader;
            ctbl.Rows.Add(hrI_hdr);
            System.Web.UI.WebControls.TableHeaderCell fhdr =
                new TableHeaderCell();
            fhdr.RowSpan = 2;
            fhdr.Text = "Racial Categories";
            hrI_hdr.Cells.Add(fhdr);

            for (int ethno_i = 0; ethno_i < ethnicity.Count; ethno_i++)
            {
                System.Web.UI.WebControls.TableHeaderCell ehdr =
                    new TableHeaderCell();
                ehdr.Text = ethnicity[ethno_i];

                hrI_hdr.Cells.Add(ehdr);
            }
            System.Web.UI.WebControls.TableHeaderCell lhdr =
                new TableHeaderCell();
            lhdr.RowSpan = 2;
            lhdr.Text = "Total";
            hrI_hdr.Cells.Add(lhdr);

            System.Web.UI.WebControls.TableHeaderRow hrII_hdr =
                new TableHeaderRow();
            hrII_hdr.TableSection = TableRowSection.TableHeader;
            ctbl.Rows.Add(hrII_hdr);
            for (int ethno_i = 0; ethno_i < ethnicity.Count; ethno_i++)
            {
                if (hrI_hdr.Cells[(ethno_i + 1)].ColumnSpan != gender.Count)
                    hrI_hdr.Cells[(ethno_i + 1)].ColumnSpan = gender.Count;

                for (int gender_i = 0; gender_i < gender.Count; gender_i++)
                {
                    System.Web.UI.WebControls.TableHeaderCell rhdr =
                        new TableHeaderCell();
                    rhdr.Text = gender[gender_i];
                    rhdr.CssClass = "table-sub-head";

                    hrII_hdr.Cells.Add(rhdr);
                }
            }

            for (int race_i = 0; race_i < race.Count; race_i++)
            {
                System.Web.UI.WebControls.TableRow dtblRow =
                    new TableRow();
                ctbl.Rows.Add(dtblRow);

                System.Web.UI.WebControls.TableHeaderCell dc =
                    new TableHeaderCell();
                dc.Text = race[race_i];
                dtblRow.Cells.Add(dc);

                for (int ethno_i = 0; ethno_i < ethnicity.Count; ethno_i++)
                {
                    for (int gender_i = 0; gender_i < gender.Count; gender_i++)
                    {
                        System.Web.UI.WebControls.TableCell tc = new TableCell();
                        dtblRow.Cells.Add(tc);

                        System.Web.UI.WebControls.TextBox itx = new TextBox();
                        itx.ID = String.Format("race_{0}_{1}_{2}", race.Keys[race_i], ethnicity.Keys[ethno_i], gender.Keys[gender_i]);
                        itx.TextMode = TextBoxMode.Number;
                        itx.Width = Unit.Pixel(75);

                        tc.Controls.Add(itx);
                    }
                }

                System.Web.UI.WebControls.TextBox total = new TextBox();
                total.ID = String.Format("race_{0}_total", race.Keys[race_i]);
                total.TextMode = TextBoxMode.Number;
                total.Width = Unit.Pixel(75);

                System.Web.UI.WebControls.TableCell tctotal =
                    new TableCell();
                dtblRow.Cells.Add(tctotal);
                tctotal.Controls.Add(total);
            }

            System.Web.UI.WebControls.TableRow totalRow =
                new TableRow();
            ctbl.Rows.Add(totalRow);

            System.Web.UI.WebControls.TableHeaderCell tfC =
                new TableHeaderCell();
            tfC.Text = "Total";
            totalRow.Cells.Add(tfC);

            for (int ethno_i = 0; ethno_i < ethnicity.Count; ethno_i++)
            {
                for (int gender_i = 0; gender_i < gender.Count; gender_i++)
                {
                    System.Web.UI.WebControls.TableCell tc = new TableCell();
                    totalRow.Cells.Add(tc);

                    System.Web.UI.WebControls.TextBox totalcol = new TextBox();
                    totalcol.ID = String.Format("{0}_{1}_total", ethnicity.Keys[ethno_i], gender.Keys[gender_i]);
                    totalcol.TextMode = TextBoxMode.Number;
                    totalcol.Width = Unit.Pixel(75);

                    tc.Controls.Add(totalcol);
                }
            }

            System.Web.UI.WebControls.TableCell tlC =
                new TableCell();
            totalRow.Cells.Add(tlC);

            System.Web.UI.WebControls.TextBox totaltx = new TextBox();
            totaltx.ID = "race_total_total";
            totaltx.TextMode = TextBoxMode.Number;
            totaltx.Width = Unit.Pixel(75);

            tlC.Controls.Add(totaltx);
        }

        private void CreateEditCancerTable(ref Control current_container, DataRow dr)
        {
            //System.Web.UI.HtmlControls.HtmlGenericControl caption_label =
            //    new HtmlGenericControl("h4");
            //caption_label.InnerHtml = dr["label_text"].ToString();
            //current_container.Controls.Add(caption_label);
            //if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
            //{
            //    if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
            //        caption_label.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
            //    else
            //        caption_label.Attributes["style"] = dr["label_style_css"].ToString();
            //}

            System.Web.UI.WebControls.Table ctbl =
                new Table();
            current_container.Controls.Add(ctbl);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    ctbl.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    ctbl.Attributes["style"] = dr["container_style_css"].ToString();
            }
            //if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            //{
            //    if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
            //        ctbl.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
            //    else
            //        ctbl.Attributes["style"] = dr["input_style_css"].ToString();
            //}

            if (dr["attributes"] != DBNull.Value && !String.IsNullOrWhiteSpace(dr["attributes"].ToString()))
            {
                string[] elAttribs = dr["attributes"].ToString().Split('|');
                foreach (string attribs in elAttribs)
                {
                    string[] attribPair = attribs.Split('=');
                    if (attribPair.Length != 2)
                        throw new Exception(String.Format("Input with id {0} has unparsable attributes", dr["id"]));
                    else
                    {
                        ctbl.Attributes[attribPair[0]] = attribPair[1];
                    }
                }
            }

            ctbl.Caption = dr["label_text"].ToString();

            System.Web.UI.WebControls.TableHeaderRow hdrRow3 =
                new TableHeaderRow();
            hdrRow3.TableSection = TableRowSection.TableHeader;
            ctbl.Rows.Add(hdrRow3);
            foreach (string label in new string[] { "ICD-9", "ICD-10/O", "Cancer Type", "Males", "Females" })
            {
                System.Web.UI.WebControls.TableHeaderCell hdrCell =
                    new TableHeaderCell();
                hdrCell.Text = label;
                hdrRow3.Cells.Add(hdrCell);
            }

            using (DataTable cancer_dt = CECWebSrv.GetCancerTypes(UserToken))
            {
                foreach (DataRow cancer_dr in cancer_dt.Rows)
                {
                    if ((int)cancer_dr["cancer_type_id"] == 0)
                        continue;

                    System.Web.UI.WebControls.TableRow dataRow =
                        new TableRow();
                    ctbl.Rows.Add(dataRow);

                    System.Web.UI.WebControls.TableHeaderCell dc1 =
                        new TableHeaderCell();
                    dc1.Text = cancer_dr["icd_9"].ToString();

                    System.Web.UI.WebControls.TableHeaderCell dc2 =
                        new TableHeaderCell();
                    dc2.Text = cancer_dr["icd_10"].ToString();

                    System.Web.UI.WebControls.TableHeaderCell dc3 =
                        new TableHeaderCell();
                    dc3.Text = cancer_dr["cancer_type"].ToString();

                    dataRow.Cells.Add(dc1);
                    dataRow.Cells.Add(dc2);
                    dataRow.Cells.Add(dc3);

                    System.Web.UI.WebControls.TextBox male =
                        new TextBox();
                    male.ID = String.Format("ci_{0}_male", FindCancerKeyByValue(dc3.Text));
                    male.TextMode = TextBoxMode.Number;
                    male.Width = Unit.Pixel(75);

                    System.Web.UI.WebControls.TableCell dc4 =
                        new TableCell();
                    dc4.Controls.Add(male);

                    dataRow.Cells.Add(dc4);

                    System.Web.UI.WebControls.TextBox female =
                        new TextBox();
                    female.ID = String.Format("ci_{0}_female", FindCancerKeyByValue(dc3.Text));
                    female.TextMode = TextBoxMode.Number;
                    female.Width = Unit.Pixel(75);

                    System.Web.UI.WebControls.TableCell dc5 =
                        new TableCell();
                    dc5.Controls.Add(female);

                    dataRow.Cells.Add(dc5);
                }
            }
        }

        private void CreateEditBiospecimenTable(ref Control current_container, DataRow dr)
        {
            //System.Web.UI.HtmlControls.HtmlGenericControl caption_label =
            //    new HtmlGenericControl("h4");
            //caption_label.InnerHtml = dr["label_text"].ToString();
            //current_container.Controls.Add(caption_label);
            //if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["label_style_css"].ToString()))
            //{
            //    if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
            //        caption_label.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
            //    else
            //        caption_label.Attributes["style"] = dr["label_style_css"].ToString();
            //}

            System.Web.UI.WebControls.Table ctbl =
                  new Table();
            current_container.Controls.Add(ctbl);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    ctbl.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    ctbl.Attributes["style"] = dr["container_style_css"].ToString();
            }
            //if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            //{
            //    if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
            //        ctbl.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
            //    else
            //        ctbl.Attributes["style"] = dr["input_style_css"].ToString();
            //}

            if (dr["attributes"] != DBNull.Value && !String.IsNullOrWhiteSpace(dr["attributes"].ToString()))
            {
                string[] elAttribs = dr["attributes"].ToString().Split('|');
                foreach (string attribs in elAttribs)
                {
                    string[] attribPair = attribs.Split('=');
                    if (attribPair.Length != 2)
                        throw new Exception(String.Format("Input with id {0} has unparsable attributes", dr["id"]));
                    else
                    {
                        ctbl.Attributes[attribPair[0]] = attribPair[1];
                    }
                }
            }

            ctbl.Caption = dr["label_text"].ToString();

            System.Web.UI.WebControls.TableHeaderRow hdrRow1 =
                new TableHeaderRow();
            hdrRow1.TableSection = TableRowSection.TableHeader;
            ctbl.Rows.Add(hdrRow1);
            foreach (string label in new string[] { "ICD-9", "ICD-10/O", "Cancer Type" })
            {
                System.Web.UI.WebControls.TableHeaderCell hdrCell =
                    new TableHeaderCell();
                hdrCell.Text = label;
                hdrRow1.Cells.Add(hdrCell);
            }

            DataTable biospecimen_dt = CECWebSrv.GetBiospecimenTypes(UserToken);
            foreach (DataRow bio_dr in biospecimen_dt.Rows)
            {
                System.Web.UI.WebControls.TableHeaderCell dc1 =
                    new TableHeaderCell();
                dc1.Text = bio_dr["biospecimen_type"].ToString();

                hdrRow1.Cells.Add(dc1);
            }

            DataTable cancer_dt = CECWebSrv.GetCancerTypes(UserToken);
            foreach (DataRow cancer_dr in cancer_dt.Rows)
            {
                System.Web.UI.WebControls.TableHeaderRow dataRow =
                    new TableHeaderRow();
                ctbl.Rows.Add(dataRow);

                System.Web.UI.WebControls.TableHeaderCell dc1 =
                    new TableHeaderCell();
                dc1.Text = cancer_dr["icd_9"].ToString();

                System.Web.UI.WebControls.TableHeaderCell dc2 =
                    new TableHeaderCell();
                dc2.Text = cancer_dr["icd_10"].ToString();

                System.Web.UI.WebControls.TableHeaderCell dc3 =
                    new TableHeaderCell();
                dc3.Text = cancer_dr["cancer_type"].ToString();

                dataRow.Cells.Add(dc1);
                dataRow.Cells.Add(dc2);
                dataRow.Cells.Add(dc3);

                foreach (DataRow bio_dr in biospecimen_dt.Rows)
                {
                    System.Web.UI.WebControls.TextBox cancer_input =
                        new TextBox();
                    cancer_input.ID = String.Format("{0}{1}", cancer_dr["data_field_prefix"], bio_dr["data_field_crumb"]);
                    cancer_input.TextMode = TextBoxMode.Number;
                    cancer_input.Width = Unit.Pixel(75);

                    System.Web.UI.WebControls.TableCell dc4 =
                        new TableCell();
                    dc4.Controls.Add(cancer_input);

                    dataRow.Cells.Add(dc4);
                }
            }
        }

        private void CreateEditAttachmentArea(ref Control current_container, DataRow dr)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl cnt = 
                new HtmlGenericControl("div");
            current_container.Controls.Add(cnt);
            if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                    cnt.Attributes["class"] = dr["container_style_css"].ToString().TrimStart('.');
                else
                    cnt.Attributes["style"] = dr["container_style_css"].ToString();
            }

            if (!String.IsNullOrWhiteSpace(dr["label_text"].ToString()))
            {
                System.Web.UI.HtmlControls.HtmlGenericControl caption = 
                    new HtmlGenericControl("div");
                caption.Controls.Add(new LiteralControl(String.Format("<h4>{0}</h4>", dr["label_text"])));
                cnt.Controls.Add(caption);

                if ((dr["label_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
                {
                    if (String.Format("{0}", dr["label_style_css"]).StartsWith("."))
                        caption.Attributes["class"] = dr["label_style_css"].ToString().TrimStart('.');
                    else
                        caption.Attributes["style"] = dr["label_style_css"].ToString();
                }
            }

            cnt.Controls.Add(new LiteralControl("<button type=\"button\" action=\"add\" id=\"add\" class=\"btn btn-primary glyphicon glyphicon-plus\"> </button>"));
            System.Web.UI.HtmlControls.HtmlTable tbl =
                new HtmlTable();
            cnt.Attributes["id"] = "attachments_table";
            cnt.Controls.Add(tbl);
            if ((dr["input_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["input_style_css"].ToString()))
            {
                if (String.Format("{0}", dr["input_style_css"]).StartsWith("."))
                    tbl.Attributes["class"] = dr["input_style_css"].ToString().TrimStart('.');
                else
                    tbl.Attributes["style"] = dr["input_style_css"].ToString();
            }

            tbl.Rows.Add(new HtmlTableRow());
            string[] table_headers = new string[] { "Category", "Type", "Attachment", "Options" };
            foreach (string s in table_headers)
            {
                System.Web.UI.HtmlControls.HtmlTableCell th =
                    new HtmlTableCell("th");
                th.InnerText = s;
                tbl.Rows[0].Cells.Add(th);
            }

            System.Data.DataTable dt_attachments = CECWebSrv.GetCohortAttachments(UserToken, int.Parse(draftCohortId.Value));
            foreach(DataRow attachment in dt_attachments.Rows)
            {
                System.Web.UI.HtmlControls.HtmlTableRow tr =
                    new HtmlTableRow();

                System.Web.UI.HtmlControls.HtmlTableCell tc_i =
                    new HtmlTableCell();
                tc_i.InnerText = attachment["document_type"].ToString();

                System.Web.UI.HtmlControls.HtmlTableCell tc_ii =
                    new HtmlTableCell();
                tc_ii.InnerText = attachment["attachment_type"].ToString();

                System.Web.UI.HtmlControls.HtmlTableCell tc_iii =
                    new HtmlTableCell();
                if (attachment["attachment_type"].ToString() == "url")
                    tc_iii.InnerText = attachment["url"].ToString();
                else
                    tc_iii.InnerText = attachment["file_name"].ToString();

                System.Web.UI.HtmlControls.HtmlTableCell options =
                    new HtmlTableCell();
                options.Controls.Add(new LiteralControl(String.Format("<button type=\"button\" action=\"delete\" id=\"{0}\" class=\"btn btn-primary glyphicon glyphicon-trash\"> </button>", attachment["id"])));
                //if (attachment["attachment_type"].ToString() == "url")
                    options.Controls.Add(new LiteralControl(String.Format("<button type=\"button\" action=\"edit\" id=\"{0}\" class=\"btn btn-primary glyphicon glyphicon-pencil\"> </button>", attachment["id"])));

                tr.Cells.Add(tc_i);
                tr.Cells.Add(tc_ii);
                tr.Cells.Add(tc_iii);
                tr.Cells.Add(options);
                tbl.Rows.Add(tr);
            }
        }

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
                    Response.Redirect("/select.aspx", false);
                }
            }

            ethnicity = new NameValueCollection();
            ethnicity.Add("nonhispanic", "Not Hispanic or Latino");
            ethnicity.Add("hispanic", "Hispanic or Latino");
            ethnicity.Add("unknown", "Unknown/Not Reported Ethnicity");

            gender = new NameValueCollection();
            gender.Add("females", "Female");
            gender.Add("males", "Male");
            gender.Add("unknown", "Unknown/Not Reported");

            race = new NameValueCollection();
            race.Add("ai", "American Indian/Alaska Native");
            race.Add("asian", "Asian");
            race.Add("pi", "Native Hawaiian or Other Pacific Islander");
            race.Add("black", "Black or African American");
            race.Add("white", "White");
            race.Add("multiple", "More Than One Race");
            race.Add("unknown", "Unknown or Not Reported");

            cancers = new NameValueCollection();
            using (DataTable dt = CECWebSrv.GetCancerTypes(UserToken))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if ((int)dr["cancer_type_id"] == 0)
                        continue;

                    cancers.Add(dr["data_field_crumb"].ToString(), dr["cancer_type"].ToString());
                }
            }

            biospecimens = new NameValueCollection();
            using (DataTable dt = CECWebSrv.GetBiospecimenTypes(UserToken))
            {
                foreach (DataRow dr in dt.Rows)
                    biospecimens.Add(dr["data_field_crumb"].ToString(), dr["biospecimen_type"].ToString());
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            string proxy_location = (Request.Url.Host == "localhost" ? "{0}://{1}/cec_service/cec_inputform.ashx?proxy" : "{0}://{1}/cec_inputform.ashx?proxy");
            Page.ClientScript.RegisterClientScriptInclude("webproxy", String.Format(proxy_location, Request.Url.Scheme, Request.Url.Host));

            int section_id = 1;
            if (Request.QueryString["section"] != null)
                section_id = int.Parse(Request.QueryString["section"]);

            dt_input = CECWebSrv.GetInputFieldsBySection(UserToken, section_id);
            if (dt_input.Rows.Count == 0)
            {
                Controls.Add(new LiteralControl("<h1>Nothing to render</h1>"));
                return;
            }

            int cohort_id = 0;
            draftCohortId.Value = "0";
            if (UserToken.TokenSet && UserToken.access_level == 100)
            {
                UserData ud = CECWebSrv.GetUserInformationByUserID(UserToken, UserToken.userid);
                cohort_id = (int)ud.cohort_id;
                draftCohortId.Value = CECWebSrv.GetDraftWebCohortId(UserToken, cohort_id).ToString();

                if (!ud.help_shown)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "help_tour", "tour.init(); tour.restart();", true);

                    ud.help_shown = true;
                    CECWebSrv.SetUserSecurityAttributes(UserToken, ud);
                }
            }
            else if (UserToken.TokenSet && UserToken.access_level >= 200 && Request.QueryString["id"] != null)
                draftCohortId.Value = Request.QueryString["id"];
            else
                throw new Exception("cannot complete request");

            dt_cohort = CECWebSrv.GetCohortRecordByWebId(UserToken, int.Parse(draftCohortId.Value));
            
            field_highlights = string.Empty;
            edit_intro.dt_cohort = dt_cohort;
            edit_intro.data_field_changes = string.Empty;

            string cohort_record_status = dt_cohort.Rows[0]["status"].ToString().ToLower();
            if((cohort_record_status == "published" || cohort_record_status == "unpublished") && UserToken.access_level >= 200)
            {
                edit_toolbar.draftCohortID = CECWebSrv.GetDraftWebCohortId(UserToken, (int)dt_cohort.Rows[0]["cohort_id"]);
                edit_toolbar.ShowPublishToolBar = true;
            }
            else if (UserToken.access_level == 100) {
                if (cohort_record_status == "pending")
                    edit_toolbar.Visible = false;
                else
                    edit_toolbar.ShowEditToolBar = true;                        
            }
            else if (UserToken.access_level >= 200)
            {
                if (cohort_record_status == "pending")
                {
                    edit_toolbar.ShowReviewerToolBar = true;

                    field_highlights = CECWebSrv.IdentifyChangesInCohortRecord(UserToken, (int)dt_cohort.Rows[0]["cohort_id"], dt_cohort.Rows[0]["status"].ToString());
                    edit_intro.dt_cohort = dt_cohort;
                    edit_intro.data_field_changes = field_highlights;
                }
                else
                    edit_toolbar.ShowEditToolBar = true;
            }

            save_timestamp.InnerHtml = String.Format("Last saved on {0}", ((DateTime)dt_cohort.Rows[0]["status_timestamp"]).ToString("dd-MMM-yyyy HH:mm"));

            bool groupOpen = false,
                rowOpen = false;
            int currentGrouping = 0,
                currentRow = 0;
            Control current_container = section, section_container = section;
            foreach (DataRow dr in dt_input.Rows)
            {
                if (dr["use_with_input_id"] != DBNull.Value && (double)dr["position"] == 0)
                    continue;

                if (rowOpen && (dr["use_with_input_id"] == DBNull.Value || currentRow != (int)dr["use_with_input_id"]))
                {
                    rowOpen = false;
                    currentRow = 0;
                    current_container = (dr["section"].ToString() == "17" || dr["section"].ToString() == "30" ? current_container.Parent.Parent : current_container.Parent);
                }

                if (groupOpen && currentGrouping != (int)Math.Truncate((double)dr["position"]))
                {
                    groupOpen = false;
                    currentGrouping = 0;
                    current_container = section_container;
                }

                string control_type = dr["type"].ToString().ToLower().Trim();

                if (control_type == "section")
                {
                    CreateEditSection(ref section_container, dr);
                    current_container = section_container;
                }
                else if (control_type == "group")
                {
                    current_container = section_container;

                    groupOpen = true;
                    currentGrouping = (int)Math.Truncate((double)dr["position"]);
                    CreateEditGroupPanel(ref current_container, dr);
                }
                else if (control_type == "row")
                {
                    System.Web.UI.HtmlControls.HtmlGenericControl coldiv = new HtmlGenericControl("div");
                    coldiv.Attributes["class"] = "row ";
                    if ((dr["container_style_css"] != DBNull.Value) && !String.IsNullOrWhiteSpace(dr["container_style_css"].ToString()))
                    {
                        if (String.Format("{0}", dr["container_style_css"]).StartsWith("."))
                            coldiv.Attributes["class"] += dr["container_style_css"].ToString().TrimStart('.');
                        else
                            coldiv.Attributes["style"] = dr["container_style_css"].ToString();
                    }
                    current_container.Controls.Add(coldiv);

                    if (dr["section"].ToString() == "17" || dr["section"].ToString() == "30")
                    {
                        System.Web.UI.HtmlControls.HtmlGenericControl fieldset =
                            new HtmlGenericControl("fieldset");
                        coldiv.Controls.Add(fieldset);

                        current_container = fieldset;
                    }
                    else
                        current_container = coldiv;

                    rowOpen = true;
                    currentRow = (int)dr["id"];
                }
                else if (control_type == "label")
                    CreateEditLabel(ref current_container, dr);
                else if (control_type == "checkbox")
                    CreateEditCheckbox(ref current_container, dr);
                else if (control_type == "radio")
                    CreateEditRadioButton(ref current_container, dr);
                else if (control_type == "text")
                    CreateEditTextBox(ref current_container, dr, false);
                else if (control_type == "radio_text")
                    CreateEditRadioText(ref current_container, dr);
                else if (control_type == "checkbox_text")
                    CreateEditCheckboxText(ref current_container, dr);
                else if (control_type == "enrollment_table")
                    CreateEditEnrollmentTable(ref current_container, dr);
                else if (control_type == "cancer_table")
                    CreateEditCancerTable(ref current_container, dr);
                else if (control_type == "biospecimen_table")
                    CreateEditBiospecimenTable(ref current_container, dr);
                else if (control_type == "attachment_area")
                    CreateEditAttachmentArea(ref current_container, dr);
                else if (control_type == "textarea")
                    CreateEditTextBox(ref current_container, dr, true);
                else
                {
                    current_container.Controls.Add(new LiteralControl(String.Format("<div class=\"col-md-12\"><p class=\"bg-danger\"><b>{0}</b> of type <i>{1}</i> not implemented</p></div>", dr["label_text"], dr["type"])));
                }
            }

            base.OnLoad(e);
        }
    }
}