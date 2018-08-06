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



namespace cec_publicweb.usrctrls
{

    public partial class detailTabs : System.Web.UI.UserControl
    {
        #region Properties

        /// <summary>
        /// get string array of cohort ids from the database to
        /// compare/view data for throughout the site
        /// </summary>
        protected string[] CohortIDsToCompare
        {
            get
            {
                if ((Session["CohortIDsToCompare"] != null) && ((string[])Session["CohortIDsToCompare"]).Length > 0)
                    return (string[])Session["CohortIDsToCompare"];
                else
                    return new string[0];
            }
        }
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.AppRelativeVirtualPath.Contains("compare"))
            {
                switch (Page.Request.QueryString["tab"])
                {
                    case "2":
                        (btnBaseline.Parent as HtmlControl).Attributes.Add("class", "active");
                        break;
                    case "3":
                        (btnFollowup.Parent as HtmlControl).Attributes.Add("class", "active");
                        break;
                    case "4":
                        (btnCancerInfo.Parent as HtmlControl).Attributes.Add("class", "active");
                        break;
                    case "5":
                        (btnMortality.Parent as HtmlControl).Attributes.Add("class", "active");
                        break;
                    case "6":
                        (btnLinkages.Parent as HtmlControl).Attributes.Add("class", "active");
                        break;
                    case "7":
                        (btnSpecimen.Parent as HtmlControl).Attributes["class"] += " active";
                        break;
                    default:
                        (btnBasic.Parent as HtmlControl).Attributes.Add("class", "active");
                        break;
                }
            }                
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool handled = false;
            if (args is CommandEventArgs)
            {
                EnsureChildControls();

                CommandEventArgs cea = (args as CommandEventArgs);
                if (cea.CommandName == "compare")
                {
                    handled = true;

                    switch (cea.CommandArgument.ToString())
                    {
                        case "baseline":
                            Response.Redirect("/compare.aspx?tab=2");
                            break;
                        case "followup":
                            Response.Redirect("/compare.aspx?tab=3");
                            break;
                        case "cancer":
                            Response.Redirect("/compare.aspx?tab=4");
                            break;
                        case "mortality":
                            Response.Redirect("/compare.aspx?tab=5");
                            break;
                        case "linkages":
                            Response.Redirect("/compare.aspx?tab=6");
                            break;
                        case "specimen":
                            Response.Redirect("/compare.aspx?tab=7");
                            break;
                        default:
                            Response.Redirect("/compare.aspx");
                            break;
                    }
                }
                return handled;
            }
            else
                return base.OnBubbleEvent(source, args);
        }

    }
}