using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;


using cec_publicweb;
using cec_publicservice;



namespace cec_publicweb
{
    public partial class error : CECPage
    {

        private string GetReferrerUrl()
        {
            if (Request.UrlReferrer == null)
                return string.Empty;
            else
                return Request.UrlReferrer.OriginalString;
        }

        private string GetUserEmail()
        {
            if (Session["UserSecurityToken"] == null)
                return "anonymous";
            else
                return ((SecurityToken)Session["UserSecurityToken"]).email;
        }

        protected override void OnLoad(EventArgs e)
        {
            if(Session["PageException"] != null)
            {
                Exception ex = (Exception)Session["PageException"];

                if(!IsPostBack)
                    CECWebSrv.AuditLog_AddActivity((UserToken.userid == 0 ? 0 : UserToken.userid), String.Format("website error, email {0} :: {1}", GetUserEmail(), ex.Message));
                
                switch (ex.Message.ToLower())
                {
                    #region Session/Login Related Errors

                    case "session is invalid":
                        error_sessioninvalid.Visible = true;
                        break;
                    case "invalid session id":
                        error_sessioninvalid.Visible = true;
                        break;
                    case "session not valid":
                        error_sessioninvalid.Visible = true;
                        break;
                    case "accountlockedoutexception":
                        error_accountlockout.Visible = true;
                        break;
                    #endregion

                    default:

                        // ignore bad request url errors
                        if (ex.Message.ToLower().Contains("dangerous request.rawurl value"))
                        {
                            simpleError.InnerText = "bad url detected";
                            break;
                        }
                            
                        ///-----------------------------------
                        /// exception message to screen
                        /// 
                        simpleError.InnerHtml = helper.HTMLEncode(ex.Message);

#if (DEBUG || DEBUGDEV)
                        simpleError.InnerHtml += "<br /><br />" + ex.StackTrace.Replace(Environment.NewLine, "<br />");
#endif
                        LogError(simpleError.InnerText, ex);

                        try
                        {
                            Session.RemoveAll();
                            FormsAuthentication.SignOut();

                            string messageContent = String.Format("Error Was Encountered On {1} {0} {2} {0}{0} Error Origin: {3}{0} User: {4}{0} {0}Stack Trace: {5}",
                                (new object[] { Environment.NewLine, Request.Url.Authority, ex.Message, helper.HTMLEncode(GetReferrerUrl()), GetUserEmail(), ex.ToString() }));

                            if (ex is AccountLockedOutException)
                                messageContent = "Login Attempted With Locked Account " + (ex as AccountLockedOutException).User;

                            //System.Net.Mail.MailMessage msg =
                                //   new MailMessage("cedcdhelpdesk@westat.com", Configuration["EmailRecipient"], "CEDCD Website Error Encountered", messageContent);

                            //helper help = new helper();
                            //help.SendEmail(msg);

                            //simpleError.InnerHtml += String.Format("<br/><br/>Email Sent To Web Admins");
                        }
                        catch (Exception oops)
                        {
                            simpleError.InnerHtml += String.Format("<br/><br/>Error Sending Email: {0}", oops.Message);
                            
                            LogError(simpleError.InnerText, ex);
                        }
                        break;
                }
            }
        }
    }
}