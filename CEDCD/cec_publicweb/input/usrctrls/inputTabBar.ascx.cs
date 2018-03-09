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

using cec_publicservice;

using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;



namespace cec_publicweb.input.usrctrls
{
    using cec_publicservice;


    public partial class inputTabBar : System.Web.UI.UserControl
    {
        private cec_publicservice.CECInputFormService CECWebSrv;

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

        protected override void OnLoad(EventArgs e)
        {
            //inputTabs.ClientIDMode = ClientIDMode.Static;
            CECWebSrv = new CECInputFormService();

            System.Data.DataTable dt_sections = CECWebSrv.GetInputFieldsSections(UserToken);
            foreach (DataRow dr in dt_sections.Rows)
            {
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

                linkQueryStr += String.Format("section={0}", dr["id"]);
                linkQueryStr = linkQueryStr.TrimEnd('&');

                System.Web.UI.HtmlControls.HtmlAnchor link =
                    new HtmlAnchor();
                link.InnerHtml = String.Format("{0}", dr["label_text"]);
                link.HRef = String.Format("/input/edit.aspx?{0}", linkQueryStr);

                System.Web.UI.HtmlControls.HtmlGenericControl span =
                    new HtmlGenericControl("span");
                span.Attributes["class"] = "arrow down";
                
                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");
                li.Controls.Add(link);
                li.Controls.Add(span);

                if (Request.QueryString["section"] == dr["id"].ToString())
                    li.Attributes["class"] = "active";

                inputTabs.Controls.Add(li);
            }
        }
    }
}