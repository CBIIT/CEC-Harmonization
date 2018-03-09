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

    public partial class sidebarFilter : System.Web.UI.UserControl
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
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool handled = false;
            if (args is CommandEventArgs)
            {
                EnsureChildControls();

                CommandEventArgs cea = (args as CommandEventArgs);

                return handled;
            }
            else
                return base.OnBubbleEvent(source, args);
        }

    }
}