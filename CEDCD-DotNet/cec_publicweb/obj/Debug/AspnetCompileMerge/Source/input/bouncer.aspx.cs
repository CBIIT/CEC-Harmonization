using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;


namespace cec_publicweb.input
{
    using cec_publicservice;

    public partial class bouncer : CECPage
    {
        protected override void OnLoad(EventArgs e)
        {
            if (UserToken.TokenSet && UserToken.access_level == 100)
                Response.Redirect("/input/edit.aspx?section=1", false);
            else if (UserToken.TokenSet && UserToken.access_level >= 200)
                Response.Redirect("/input/list.aspx?tab=pending", false);
        }
    }
}