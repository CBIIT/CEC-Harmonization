using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using cec_publicservice;

namespace cec_publicweb.usrctrls
{
    public partial class contact : System.Web.UI.UserControl
    {
        private cec_publicweb.helper help;
        
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

        #region EventHandling

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            help = new helper();
        }

        protected void cancelBtnClicked(object sender, EventArgs e)
        {
            //if (UserToken.TokenSet)
            //    Response.Redirect("/select.aspx", false);
            //else
            //    Response.Redirect("/home.aspx", false);
        }

        protected void submitBtnClicked(object sender, EventArgs e)
        {
            string validationErrors = string.Empty;
            if (!ValidateFields(out validationErrors))
            {
                rg_errorMsg.InnerHtml = String.Format("Please complete the missing fields. <p>{0}</p>", validationErrors);
                rg_errorMsg.Attributes["role"] = "alert";

                string script = "<script type=\"text/javascript\"> $(function() { $('#contactOverlay').modal('show'); }); </script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "contact_validation_errors", script);

                //RegisterJSAlert(rg_errorMsg.InnerText);

                return;
            }

            try
            {
                using (cec_publicservice.CECInputFormService websrv = new CECInputFormService())
                {
                    System.Collections.Specialized.NameValueCollection data =
                        new NameValueCollection();
                    data.Add("first_name", cu_firstName.Text);
                    data.Add("last_name", cu_lastName.Text);
                    data.Add("institution_affiliation", cu_organization.Text);
                    data.Add("phone_number", cu_phone.Text);
                    data.Add("email_address", cu_email.Text);
                    data.Add("topic", cu_topic.SelectedItem.Text);
                    data.Add("email", cu_email.Text);
                    websrv.CreateEmailAndSend(helper.CreateTemporaryToken(), "helpdesk_inbound", data);

                    data.Clear();
                    data.Add("to", cu_email.Text);
                    websrv.CreateEmailAndSend(helper.CreateTemporaryToken(), "helpdesk_outbound", data);
                }
            }
            catch (Exception ex)
            {
                rg_errorMsg.InnerText = "Failed to send email";

#if (DEBUG || DEBUGDEV)
                rg_errorMsg.InnerText += String.Format(" ({0})", ex.Message);
#endif
                //LogError(rg_errorMsg.InnerText, ex);
            }
        }

        #endregion

        private bool ValidateFields(out string validationErrors)
        {
            bool valid = true;
            validationErrors = string.Empty;

            if (helper.IsStringEmptyWhiteSpace(cu_firstName.Text))
            {
                div_firstname.Attributes["class"] += " field-required";
                div_firstname.Attributes["aria-invalid"] = "true";
                valid = false;
                validationErrors += "&bull; <b>First Name</b> is required<br/>";
            }

            if (helper.IsStringEmptyWhiteSpace(cu_lastName.Text))
            {
                div_lastname.Attributes["class"] += " field-required";
                div_lastname.Attributes["aria-invalid"] = "true";
                valid = false;
                validationErrors += "&bull; <b>Last Name</b> is required<br />";
            }

            if (helper.IsStringEmptyWhiteSpace(cu_organization.Text))
            {
                div_organization.Attributes["class"] += " field-required";
                div_organization.Attributes["aria-invalid"] = "true";
                valid = false;
                validationErrors += "&bull; <b>Organization</b> is required<br />";
            }

            if (!helper.IsPhoneNumber(cu_phone.Text))
            {
                //div_phone.Attributes["class"] += " field-required";
                div_phone.Attributes["aria-invalid"] = "true";
                valid = false;
                validationErrors += "&bull; <b>Phone Number</b> is not a valid phone number, try entering only numbers<br />";
            }

            if (helper.IsStringEmptyWhiteSpace(cu_email.Text))
            {
                div_email.Attributes["class"] += " field-required";
                div_email.Attributes["aria-invalid"] = "true";
                valid = false;
                validationErrors += "&bull; <b>Email</b> is required<br />";
            }
            else if (!helper.IsEmailAddress(cu_email.Text))
            {
                div_email.Attributes["class"] += " field-required";
                div_email.Attributes["aria-invalid"] = "true";
                valid = false;
                validationErrors += "&bull; <b>Email</b> is not a valid email address<br />";
            }

            if (helper.IsStringEmptyWhiteSpace(cu_message.Text))
            {
                div_message.Attributes["class"] += " field-required";
                div_message.Attributes["aria-invalid"] = "true";
                valid = false;
                validationErrors += "&bull; <b>Message</b> is required<br />";
            }

            return valid;
        }
    }
}