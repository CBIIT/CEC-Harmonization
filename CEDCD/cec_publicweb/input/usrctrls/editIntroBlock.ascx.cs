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

using cec_publicservice;

namespace cec_publicweb.input.usrctrls
{
    public partial class editIntroBlock : System.Web.UI.UserControl
    {
        internal System.Data.DataTable dt_cohort;
        internal string data_field_changes;

        private System.Data.DataTable dt_input;

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

        protected override void OnInit(EventArgs e)
        {
            draft_intro.Visible = false;
            pending_intro.Visible = false;
            reviewer_intro.Visible = false;
            published_intro.Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            foreach(string k in Attributes.Keys)
                (Controls[0] as HtmlContainerControl).Attributes.Add(k, Attributes[k]);
            (Controls[0] as HtmlContainerControl).ID = this.ID;

            int section_id = 1;
            if (Request.QueryString["section"] != null)
                section_id = int.Parse(Request.QueryString["section"]);

            string linkQueryStr = "";
            if (Request.QueryString.Count > 0)
            {
                foreach (string s in Request.QueryString)
                {
                    if (s.Contains("section"))
                        continue;

                    linkQueryStr = String.Format("{0}={1}&", s, Request.QueryString[s]);
                }
            }

            using (cec_publicservice.CECInputFormService ps = new CECInputFormService())
            {
                dt_input = ps.GetInputFields(UserToken);

                string cohort_record_status = dt_cohort.Rows[0]["status"].ToString().ToLower();
                if (cohort_record_status == "published" || cohort_record_status == "unpublished")
                    published_intro.Visible = true;
                else if (cohort_record_status == "pending" && UserToken.access_level == 100)
                    pending_intro.Visible = true;
                else if (cohort_record_status == "pending" && UserToken.access_level >= 200)
                {
                    reviewer_intro.Visible = true;

                    string list_of_changes = String.Empty;
                    if (!String.IsNullOrWhiteSpace(data_field_changes))
                    {
                        foreach (string df in data_field_changes.Split(','))
                        {
                            string field = df;

                            if (field.EndsWith("_no") || field.EndsWith("_yes"))
                                field = field.Remove(df.LastIndexOf('_'), (field.Length - field.LastIndexOf('_')));

                            if (dt_input.Select(String.Format("data_field='{0}'", field)).Length == 0)
                                continue;

                            DataRow dr_input = dt_input.Select(String.Format("data_field='{0}'", field))[0];

                            string url = string.Empty;
                            if ((int)dr_input["section"] != section_id)
                                url = String.Format("/input/edit.aspx?{0}section={1}#{2}", linkQueryStr, dr_input["section"], df);
                            else
                                url = String.Format("#{0}", df);

                            string change_label = String.Format("<span class=\"change-label\">{0}</span> <a class=\"change-link\" href=\"{1}\">Go to</a>", helper.StripHTML(ps.GetInputFieldQuestionText(UserToken, field)), url);
                            list_of_changes += String.Format("<li>{0}</li>", change_label);
                        }

                        change_count.InnerText = data_field_changes.Split(',').Length.ToString();
                        list_changes.InnerHtml = String.Format("<ul>{0}</ul>", list_of_changes);
                    }
                    else
                    {
                        change_count.InnerText = "0";
                        list_changes.InnerHtml = "<u>Nothing has changed.</u>";
                    }
                }
                else
                    draft_intro.Visible = true;
            }
        }
    }
}