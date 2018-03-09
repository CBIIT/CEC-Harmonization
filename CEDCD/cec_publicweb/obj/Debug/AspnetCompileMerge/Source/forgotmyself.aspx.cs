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
    public partial class forgotmyself : CECPage
    {
        private helper help;
        private cec_publicservice.CECInputFormService ps;


        #region Event Handling

        protected override void  OnLoad(EventArgs e)
        {
 	         base.OnLoad(e);

            if(!IsPostBack)
                CECWebSrv.AuditLog_AddActivity(0, "[anonymous] forgot password page");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            help =
                new helper();

            ps = new CECInputFormService();
        }

        protected void forgotPassword_SendBtnClicked(object sender, EventArgs e)
        {
            if (helper.IsStringEmptyWhiteSpace(fg_email.Text))
            {
                fg_errorMsg.InnerText = "Email address cannot be left blank";

                RegisterJSAlert(fg_errorMsg.InnerText);
                return;
            }
            else if (!helper.IsEmailAddress(fg_email.Text))
            {
                fg_errorMsg.InnerText = "Email address not in expected format";

                RegisterJSAlert(fg_errorMsg.InnerText);
                return;
            }

            try
            {
                UserData ud = ps.GetUserInformationByEmail(fg_email.Text);
                CECMembershipProvider prov = (Membership.Providers["CECProvider"] as CECMembershipProvider);
                string newPass = prov.ResetPassword(ud.email, string.Empty);

                System.Collections.Specialized.NameValueCollection data =
                    new NameValueCollection();
                data.Add("password", newPass);
                data.Add("to", ud.email);

                DataRow[] dr_users;
                using (DataTable dt_users = ps.GetUsers(helper.CreateTemporaryToken(), "uid, username, email"))
                {
                    dr_users = dt_users.Select(String.Format("email='{0}'", ud.email));
                }

                if (dr_users.Length > 1)
                {
                    string additional_accounts = string.Empty;
                    foreach (DataRow dr in dr_users)
                        additional_accounts += String.Format("\t{0}\n", dr["username"]);

                    data.Add("additional_accounts", String.Format("<p>The following accounts were updated with the password above because they are associated with this email address:<pre>{0}</pre></p>", additional_accounts));
                }
                else
                    data.Add("additional_accounts", string.Empty);

                ps.CreateEmailAndSend(helper.CreateTemporaryToken(), "lost_password", data);

                CECWebSrv.AuditLog_AddActivity(ud.userid, "password reset; email sent");

                fg_errorMsg.Attributes["class"] = "bg-success text-sucess";
                fg_errorMsg.InnerText = "Email successfully sent";
                //Response.Redirect("/select.aspx", false);
            }
            catch (Exception ex)
            {
                fg_errorMsg.InnerText = String.Format("Failed to email the password to {0}.", fg_email.Text);
                LogError(fg_errorMsg.InnerText, ex);
            }
        }

        protected void forgotPassword_CancelBtnClicked(object sender, EventArgs e)
        {
            Response.Redirect("/select.aspx", false);
        }

        #endregion
    }
}