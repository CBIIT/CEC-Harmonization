using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class inputToolBar : System.Web.UI.UserControl
    {
        //private cec_publicservice.CECHarmPublicService CECWebSrv;

        internal bool ShowEditToolBar = false;
        internal bool ShowReviewerToolBar = false;
        internal bool ShowPublishToolBar = false;

        internal int draftCohortID = 0;

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
            enableEditBtn.Attributes["onclick"] = "enable_edit_input('toggle');evaluateSkips();set_associate_text_field_state();";
            if (UserToken.TokenSet && ShowEditToolBar)
            {
                toolbar.Controls.Remove(rejectListItem);
                toolbar.Controls.Remove(approveListItem);
                if(UserToken.access_level == 100)
                    toolbar.Controls.Remove(returnListItem);                
            }
            else if (UserToken.TokenSet && UserToken.access_level >= 200)
            {
                if(ShowReviewerToolBar)
                    toolbar.Controls.Remove(submitListItem);
                else if(ShowPublishToolBar) {
                    enableEditBtn.Attributes["onclick"] = String.Format("location.href = '/input/edit.aspx?id={0}';", draftCohortID);
                    enableEditBtn.Controls.Clear();
                    enableEditBtn.Controls.Add(new LiteralControl("<span class=\"glyphicon glyphicon-pencil\"></span><br><span class=\"button-text\">Start Draft</span>"));
                    toolbar.Controls.Remove(saveListItem);
                    toolbar.Controls.Remove(submitListItem);
                    toolbar.Controls.Remove(rejectListItem);
                    toolbar.Controls.Remove(approveListItem);
                }
            }
            else
            {
                toolbar.Controls.Clear();
            }
        }
    }
}