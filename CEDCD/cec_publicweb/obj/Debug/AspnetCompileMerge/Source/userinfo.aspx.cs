using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using cec_publicservice;


namespace cec_publicweb
{
    public partial class userinfo : CECPage
    {
        private cec_publicweb.helper help;
        private cec_publicservice.CECInputFormService websrv;

        #region Properties

        public bool ForPasswordReset
        {
            get
            {
                if (Page.ClientQueryString.Contains("resetPassword"))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// get or set the captcha answer
        /// </summary>
        public int CaptchaAnswer
        {
            get
            {
                if (ViewState["CaptchaAnswer"] == null)
                    CaptchaAnswer = 0;

                return (int)ViewState["CaptchaAnswer"];
            }
            set
            {
                ViewState["CaptchaAnswer"] = value;
            }
        }
        #endregion

        #region Event Handling

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && UserToken.TokenSet)
            {
                UserData ud = websrv.GetUserInformationByUserID(UserToken, UserToken.userid);
                rg_emailAddress.Text = ud.email;
                rg_displayName.Text = ud.display_name;
            }

            if (ForPasswordReset && !IsPostBack)
            {
                CECWebSrv.AuditLog_AddActivity(UserToken.userid, "user profile page; for password reset");

                userInformation.Visible = false;

                fg_cancelBtn.Visible = false;

                rg_errorMsg.InnerText = "Account password has expired, please set a new password";
                RegisterJSAlert(rg_errorMsg.InnerText);
            }
            else if(!ForPasswordReset)
            {
                CECWebSrv.AuditLog_AddActivity(UserToken.userid, "user profile page");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            help =
                new helper();

            websrv =
                new CECInputFormService();
        }

        protected override void OnPreRender(EventArgs e)
        {
            // create captcha
            Random rand =
                new Random();
            int c1 = rand.Next(1, 9);
            int c2 = rand.Next(1, 9);
            CaptchaAnswer = c1 + c2;

            captchaLabel.InnerText = String.Format("{0} + {1} = ", c1, c2);
        }

        protected void registrationBtnClicked(object sender, EventArgs e)
        {
            string captcha = rg_captcha.Text;
            rg_captcha.Text = "";

            if (captcha != CaptchaAnswer.ToString())
            {
                rg_errorMsg.InnerText = "CAPTCHA answer is incorrect";

                RegisterJSAlert(rg_errorMsg.InnerText);
                return;
            }

            if (!helper.IsStringEmptyWhiteSpace(rg_password1.Text) && rg_password2.Text != rg_password1.Text)
            {
                rg_errorMsg.InnerText = "Password does not match confirm password";

                RegisterJSAlert(rg_errorMsg.InnerText);
                return;
            }

            try
            {
                UserData ud = websrv.GetUserInformationByUserID(UserToken, UserToken.userid);

                if (!helper.IsStringEmptyWhiteSpace(rg_password1.Text))
                {
                    CECMembershipProvider cecMp = (Membership.Providers["CECProvider"] as CECMembershipProvider);
                    cecMp.UserToken = UserToken;
                    if (cecMp.ValidatePasswordStrength(rg_password1.Text))
                        cecMp.ChangePassword(rg_password1.Text);

                        ud.password_expired = false;
                        ud.password_change_date = DateTime.Today;
                        if (ForPasswordReset)
                            ud.password_reset_required = false;
                        
                        websrv.SetUserSecurityAttributes(UserToken, ud);
                }

                if (!helper.IsStringEmptyWhiteSpace(rg_displayName.Text) && ud.display_name != rg_displayName.Text)
                    ud.display_name = rg_displayName.Text;

                if (!helper.IsStringEmptyWhiteSpace(rg_emailAddress.Text) && ud.email != rg_emailAddress.Text)
                    ud.email = rg_emailAddress.Text;

                websrv.SetUserInformation(UserToken, ud);

                CECWebSrv.AuditLog_AddActivity(UserToken.userid, "user profile information updated");

                SendEmailConfirmation();
                Response.Redirect("/input/bouncer.aspx", false);
            }
            catch (Exception ex)
            {
                if (ex is MembershipPasswordException)
                    rg_errorMsg.InnerText = ex.Message;
                else
                    rg_errorMsg.InnerText = "User account was not saved";

#if (DEBUG || DEBUGDEV)
                rg_errorMsg.InnerText += String.Format(" ({0})", ex.Message);
#endif
                LogError(rg_errorMsg.InnerText, ex);

                RegisterJSAlert(String.Format("ERROR: {0}", rg_errorMsg.InnerText));
            }

        }

        protected void CancelBtnClicked(object sender, EventArgs e)
        {
            Response.Redirect("/input/bouncer.aspx", false);
        }
        #endregion

        private bool ValidateRegistrationFields()
        {
            bool valid = true;

            try
            {
                if (ForPasswordReset || (!helper.IsStringEmptyWhiteSpace(rg_password1.Text) || (!helper.IsStringEmptyWhiteSpace(rg_password2.Text))))
                {
                    if (helper.IsStringEmptyWhiteSpace(rg_password1.Text))
                        valid = false;

                    if (helper.IsStringEmptyWhiteSpace(rg_password2.Text))
                        valid = false;
                }
            }
            catch
            {
                valid = false;
            }

            return valid;
        }

        private void SendEmailConfirmation()
        {
            using (cec_publicservice.CECInputFormService websrv = new CECInputFormService())
            {
                UserData ud = websrv.GetUserInformationByUserID(UserToken, UserToken.userid);
                System.Collections.Specialized.NameValueCollection data =
                    new NameValueCollection();
                data.Add("name", ud.display_name);
                data.Add("to", ud.email);

                websrv.CreateEmailAndSend(UserToken, "user_info_update", data);
            }
        }
    }
}