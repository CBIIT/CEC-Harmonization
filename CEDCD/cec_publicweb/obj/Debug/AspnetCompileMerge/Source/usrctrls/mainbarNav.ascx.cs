using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

using cec_publicservice;

using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;



namespace cec_publicweb.usrctrls
{
    public partial class mainbarNav : System.Web.UI.UserControl
    {

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

        private void loginBtnClick(object sender, EventArgs e)
        {
            SecurityToken token = new SecurityToken();
            CECMembershipProvider prov = (Membership.Providers["CECProvider"] as CECMembershipProvider);
            try
            {
                string validation_errors = string.Empty;
                if (String.IsNullOrWhiteSpace(usernameIn.Value))
                    validation_errors += "<p>Username cannot be blank</p>";
                if (String.IsNullOrWhiteSpace(passwordIn.Value))
                    validation_errors += "<p>Password cannot be blank</p>";

                if (!String.IsNullOrEmpty(validation_errors))
                    throw new Exception(validation_errors);

                if (prov.ValidateUser(usernameIn.Value, passwordIn.Value, out token))
                {
                    string local_userfiles_path = MapPath(String.Format("/user_files/{0}", token.userid));
                    if (!Directory.Exists(local_userfiles_path))
                        Directory.CreateDirectory(local_userfiles_path);

                    using (cec_publicservice.CECInputFormService websrv = new CECInputFormService())
                    {
                        // verify/handle user account conditions
                        UserData ud = websrv.GetUserInformationByUserID(token, token.userid);

                        using (cec_publicservice.CECHarmPublicService pubsrv = new CECHarmPublicService())
                        {
                            if (ud.account_lockout)
                            {
                                pubsrv.AuditLog_AddActivity(ud.userid, "login attempted; failed due to lockout");
                                throw new AccountLockedOutException(ud.email, ud.account_lockout_date);
                            }

                            // user was successfully authenticated, so set the auth cookie
                            FormsAuthentication.SetAuthCookie(token.email, false);
                            Session["UserSecurityToken"] = token;
                            Response.SetCookie(new HttpCookie("sessionid", token.session));
                            Response.SetCookie(new HttpCookie("uid", token.userid.ToString()));
                            Response.SetCookie(new HttpCookie("edit_mode", ""));

                            pubsrv.AuditLog_AddActivity(ud.userid, "login");
                        }

                        if (ud.password_reset_required || ud.password_expired)
                            Page.Response.Redirect("/userinfo.aspx?resetPassword", false);
                        else
                            Page.Response.Redirect("/input/bouncer.aspx", false);
                            //FormsAuthentication.RedirectFromLoginPage(token.email, false);
                    }
                }
                else
                    throw new Exception("Unknown login failure");
            }
            catch (Exception ex)
            {
                string script = "<script type=\"text/javascript\"> $(function() { $('#invalidlogin_msg').html('" + ex.Message + "').show(); $('#login_dialog').modal('show'); }); </script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "login_errors", script);

                
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string proxy_location = (Request.Url.Host == "localhost" ? "{0}://{1}/cec_service/cec_inputform.ashx?proxy" : "{0}://{1}/input/cec_inputform.ashx?proxy");
            Page.ClientScript.RegisterClientScriptInclude("webproxy", String.Format(proxy_location, Request.Url.Scheme, Request.Url.Host));

            bool isLoggedIn = false;
            if(Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (!helper.IsStringEmptyWhiteSpace(authCookie.Value))
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (!ticket.Expired && (Request.Cookies["uid"] != null && Request.Cookies["uid"].Value != ""))
                        isLoggedIn = true;
                }
            }

            if (isLoggedIn)
            {
                if (UserToken.TokenSet)
                    userToken.Value = new JavaScriptSerializer().Serialize(UserToken);

                using (cec_publicservice.CECInputFormService websrv = new CECInputFormService())
                {
                    if (!websrv.ValidateSecurityToken(UserToken))
                        return;

                    liLogin.Controls.Clear();

                    UserData usrData = websrv.GetUserInformationByUserID(UserToken, int.Parse(Request.Cookies["uid"].Value));

                    System.Web.UI.HtmlControls.HtmlAnchor usrLink =
                        new HtmlAnchor();
                    usrLink.ID = "user_profile";
                    usrLink.HRef = "/userinfo.aspx";
                    usrLink.InnerHtml = String.Format("{0} &mdash;Account", usrData.display_name);
                    liLogin.Controls.Add(usrLink);

                    System.Web.UI.HtmlControls.HtmlAnchor logoutLink =
                        new HtmlAnchor();
                    logoutLink.Attributes["onclick"] = "logOut();";
                    logoutLink.ID = "user_logout";
                    logoutLink.InnerText = "Logout";
                    liLogin.Controls.Add(logoutLink);
                }
            }
            else
            {
                //set li active
                if (Page.AppRelativeVirtualPath.Contains("cohortselect"))
                    liSelect.Attributes["class"] += " active";
                else if (Page.AppRelativeVirtualPath.Contains("select"))
                    liHome.Attributes["class"] += " active";
                else if (Page.AppRelativeVirtualPath.Contains("about"))
                    liAbout.Attributes["class"] += " active";
                else if (Page.AppRelativeVirtualPath.Contains("biospecimen"))
                    liBiospecimen.Attributes["class"] += " active";
                else if (Page.AppRelativeVirtualPath.Contains("cancer"))
                    liCancer.Attributes["class"] += " active";
                else if (Page.AppRelativeVirtualPath.Contains("enrollment"))
                    liEnrollment.Attributes["class"] += " active";

                loginBtn.Click +=
                    new EventHandler(loginBtnClick);
            }

            // do logout
            if (Request.QueryString.ToString().Contains("logout"))
            {
                Session.RemoveAll();
                FormsAuthentication.SignOut();
            }
        }
    }
}