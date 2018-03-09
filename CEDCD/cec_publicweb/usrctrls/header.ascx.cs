using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using cec_publicservice;

namespace cec_publicweb.usrctrls
{
    public partial class header : System.Web.UI.UserControl
    {
        private cec_publicservice.CECHarmPublicService ps;

        /// <summary>
        /// get the security token for the given user
        /// </summary>
        public SecurityToken UserToken
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
            base.OnInit(e);

            ps = new CECHarmPublicService();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.AppRelativeVirtualPath.Contains("home"))
                mastLogo.Src = "/images/logo_cedcd_large.png";

            //if (UserToken.TokenSet)
            //{
            //    userMenu.Visible = true;

            //    UserData ud = ps.GetUserInformation(UserToken.email);
                
            //    string welcome = string.Empty;
            //    if (ud.first_name != string.Empty)
            //        welcome = ud.first_name;

            //    if (welcome != string.Empty)
            //        welcomeText.InnerText = String.Format("Welcome, {0}", welcome);
            //}
            //else
            //    userMenu.Visible = false;


//#if (DEBUG)

//            userMenu.Visible = true;
//#endif

        }
    }
}