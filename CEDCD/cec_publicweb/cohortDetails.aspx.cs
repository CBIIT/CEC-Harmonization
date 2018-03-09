using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace cec_publicweb
{
    using cec_publicservice;


    public partial class cohortDetails : CECPage
    {
        private cec_publicservice.CECHarmPublicService ps;
        private cec_publicweb.helper help;

        private System.Collections.ArrayList clearedCohorts;
        private System.Collections.ArrayList listAttachments;
        private System.Data.DataTable dt_cohort;

        #region Properties

        #endregion

        #region Event Handling

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.Request.QueryString["cohort_id"] != null || Page.Request.QueryString["cohort_acronym"] != null)
            {     
                System.Data.DataSet ds1, ds2;
                if (Page.Request.QueryString["cohort_id"] != null)
                    ds1 = CECWebSrv.GetCohortDetails(UserToken, int.Parse(Page.Request.QueryString["cohort_id"]));
                else
                    ds1 = CECWebSrv.GetCohortDetails(UserToken, Page.Request.QueryString["cohort_acronym"]);

                System.Data.DataTable cohort = ds1.Tables["tbl_web_cohorts_v4_0"];

                if (cohort.Rows.Count == 0)
                {
                    cd_errorMsg.InnerText = "cohort not found";
                    return;
                }

                ///----------------------------------------
                /// stopped commenting, code has been hacked to handle changes from the client
                ///  in the time provided. first casualities are comments/documentation.... o.0
                ///  
                // grab all columns from the cohort_meta table
                System.Data.DataTable cohort_meta = CECWebSrv.GetFilteredCohortRecords(UserToken, "*", String.Format(" (cohort_id={0})", cohort.Rows[0]["cohort_id"])).Tables["tbl_web_cohorts_v4_0"];
                if (cohort_meta.Rows.Count > 0)
                {
                    //if ((cohort_meta.Rows[0]["attachmentQuestionnairePending"] != DBNull.Value) && (bool)cohort_meta.Rows[0]["attachmentQuestionnairePending"])
                    //    ChangeDefaultAttachmentEmptyText(quest_attachments.ID);

                    //if ((cohort_meta.Rows[0]["attachmentPoliciesPending"] != DBNull.Value) && (bool)cohort_meta.Rows[0]["attachmentPoliciesPending"])
                    //    ChangeDefaultAttachmentEmptyText(pol_attachments.ID);

                    //if ((cohort_meta.Rows[0]["attachmentPublicationsPending"] != DBNull.Value) && (bool)cohort_meta.Rows[0]["attachmentPublicationsPending"])
                    //    ChangeDefaultAttachmentEmptyText(pub_attachments.ID);

                    //if ((cohort_meta.Rows[0]["attachmentGrantsPending"] != DBNull.Value) && (bool)cohort_meta.Rows[0]["attachmentGrantsPending"])
                    //    ChangeDefaultAttachmentEmptyText(grant_attachments.ID);

                    //if ((cohort_meta.Rows[0]["attachmentProtocolsPending"] != DBNull.Value) && (bool)cohort_meta.Rows[0]["attachmentProtocolsPending"])
                    //    ChangeDefaultAttachmentEmptyText(prot_attachments.ID);
                }
                /// hacking ends here...spaghetti code!
                /// -------------------------------------------
                
                dt_cohort = cohort;

                PopulateWebFieldValues(cohort);

                PopulateOtherSection(cohort);
                
                ds2 = CECWebSrv.GetCohortAttachmentList(UserToken, (int)cohort.Rows[0]["id"]);
                PopulateAttachmentSection(ds2.Tables[0]);

                PopulateFormURL(cohort);

                if (Page.Request.QueryString["tab"] != null)
                {
                    string clientScr = String.Format("<script>toggleAccordion({0});</script>", helper.HTMLEncode(Page.Request.QueryString["tab"]));
                    Page.ClientScript.RegisterStartupScript(GetType(), "tab", clientScr);
                }

                if(!IsPostBack)
                    CECWebSrv.AuditLog_AddActivity(UserToken.userid, String.Format("cohort {0} details page", cohort.Rows[0]["cohort_acronym"]));
            }

            if (Page.Request.QueryString["download"] != null)
            {
                string savePath = CECWebSrv.GetCohortDocument(UserToken, Convert.ToInt32(Page.Request.QueryString["download"]));
                savePath = savePath.Replace("'", "\\'");
                Page.ClientScript.RegisterStartupScript(GetType(), "download", String.Format("<script>window.open('{0}');</script>", savePath));
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (ps == null)
                ps = new CECHarmPublicService();

            if (help == null)
                help = new helper();

            clearedCohorts = new ArrayList();
            listAttachments = new ArrayList();
        }
        #endregion

        private void CreateTitleListControls(HtmlControl parent, string colTitle, string colList, bool isFirst)
        {
            if (helper.IsStringEmptyWhiteSpace(colList))
                return;

            System.Web.UI.HtmlControls.HtmlGenericControl h3 =
                new HtmlGenericControl("h3");
            h3.InnerText = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, colTitle);
            if (isFirst)
                h3.Attributes["class"] = "first-item";

            parent.Controls.Add(h3);

            System.Web.UI.HtmlControls.HtmlGenericControl list =
                new HtmlGenericControl("ul");
            if (helper.IsLogicalTrue(dt_cohort.Rows[0][colList]))
                list.Controls.Add((new LiteralControl(String.Format("<li>{0}</li>", CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, colList)))));
            else if (dt_cohort.Rows[0][colList] == DBNull.Value || helper.IsStringEmptyWhiteSpace(dt_cohort.Rows[0][colList].ToString()))
                list.Controls.Add(new LiteralControl("<li>Not applicable</li>"));
            else
                list.Controls.Add((new LiteralControl(String.Format("<li>{0}</li>", dt_cohort.Rows[0][colList]))));

            parent.Controls.Add(list);
        }

        private void CreateTitleListControls(HtmlControl parent, string colTitle, string colList)
        {
            if (helper.IsStringEmptyWhiteSpace(colList))
                return;

            System.Web.UI.HtmlControls.HtmlGenericControl h3 =
                new HtmlGenericControl("h3");
            h3.InnerText = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, colTitle);
            parent.Controls.Add(h3);

            System.Web.UI.HtmlControls.HtmlGenericControl list =
                new HtmlGenericControl("ul");
            if (helper.IsLogicalTrue(dt_cohort.Rows[0][colList]))
                list.Controls.Add((new LiteralControl(String.Format("<li>{0}</li>", CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, colList)))));
            else if (dt_cohort.Rows[0][colList] == DBNull.Value || helper.IsStringEmptyWhiteSpace(dt_cohort.Rows[0][colList].ToString()))
                list.Controls.Add(new LiteralControl("<li>Not applicable</li>"));
            else
                list.Controls.Add((new LiteralControl(String.Format("<li>{0}</li>", dt_cohort.Rows[0][colList]))));

            parent.Controls.Add(list);
        }

        private void CreateTitleListControls(HtmlControl parent, string colTitle, bool literalTitle, string[] colList, bool isFirst)
        {
            if (colList.Length == 0)
                return;

            System.Web.UI.HtmlControls.HtmlGenericControl list =
                new HtmlGenericControl("ul");

            System.Web.UI.HtmlControls.HtmlGenericControl li =
                new HtmlGenericControl("li");
            li.InnerText = "Not applicable";

            foreach (string s in colList)
            {
                if ((dt_cohort.Rows[0][s] == DBNull.Value || helper.IsStringEmptyWhiteSpace(dt_cohort.Rows[0][s].ToString())) && !list.Controls.Contains(li))
                    list.Controls.Add(li);
                else if (helper.IsLogicalBoolQuestion(dt_cohort.Rows[0][s]))
                    list.Controls.Add((new LiteralControl(String.Format("<li>{0}</li>", CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, s)))));
                else if (!helper.IsLogicalBoolQuestion(dt_cohort.Rows[0][s]))
                {
                    if (helper.IsNumerical(dt_cohort.Rows[0][s]))
                    {
                        int count = int.Parse(dt_cohort.Rows[0][s].ToString());
                        list.Controls.Add((new LiteralControl(String.Format("<li>{0}: {1}</li>", CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, s), helper.FormatCount(count)))));
                    }
                    else
                        list.Controls.Add((new LiteralControl(String.Format("<li>{0}: {1}</li>", CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, s), dt_cohort.Rows[0][s]))));
                }
                else
                    list.Controls.Add((new LiteralControl(String.Format("<li>{0}</li>", dt_cohort.Rows[0][s]))));
            }

            ///----------------------------------
            /// remove any n_a controls if there are more than one list item
            /// 
            if (list.Controls.Contains(li) && list.Controls.Count > 1)
                list.Controls.Remove(li);

            ///------------------------------
            /// first add parent title
            System.Web.UI.HtmlControls.HtmlGenericControl h3 =
                new HtmlGenericControl("h3");
            if (literalTitle)
                h3.InnerText = colTitle;
            else
                h3.InnerText = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, colTitle);

            if (isFirst)
                h3.Attributes["class"] = "first-item";
            parent.Controls.Add(h3);

            ///------------------------------
            /// then add the UL list
            parent.Controls.Add(list);
        }

        private void CreateTitleListControls(HtmlControl parent, string colTitle, bool literalTitle, string[] colList)
        {
            if (colList.Length == 0)
                return;

            System.Web.UI.HtmlControls.HtmlGenericControl list =
                new HtmlGenericControl("ul");

            System.Web.UI.HtmlControls.HtmlGenericControl li =
                new HtmlGenericControl("li");
            li.InnerText = "Not applicable";

            foreach (string s in colList)
            {
                if ((dt_cohort.Rows[0][s] == DBNull.Value || helper.IsStringEmptyWhiteSpace(dt_cohort.Rows[0][s].ToString())) && !list.Controls.Contains(li))
                    list.Controls.Add(li);
                else if (helper.IsLogicalBoolQuestion(dt_cohort.Rows[0][s]))
                    list.Controls.Add((new LiteralControl(String.Format("<li>{0}</li>", CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, s)))));
                else if (!helper.IsLogicalBoolQuestion(dt_cohort.Rows[0][s]))
                {
                    if (helper.IsNumerical(dt_cohort.Rows[0][s]))
                    {
                        int count = int.Parse(dt_cohort.Rows[0][s].ToString());
                        list.Controls.Add((new LiteralControl(String.Format("<li>{0}: {1}</li>", CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, s), helper.FormatCount(count)))));
                    }
                    else
                        list.Controls.Add((new LiteralControl(String.Format("<li>{0}: {1}</li>", CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, s), dt_cohort.Rows[0][s]))));
                }
                else
                    list.Controls.Add((new LiteralControl(String.Format("<li>{0}</li>", dt_cohort.Rows[0][s]))));
            }

            ///----------------------------------
            /// remove any n_a controls if there are more than one list item
            /// 
            if (list.Controls.Contains(li) && list.Controls.Count > 1)
                list.Controls.Remove(li);

            ///------------------------------
            /// first add parent title
            System.Web.UI.HtmlControls.HtmlGenericControl h3 =
                new HtmlGenericControl("h3");
            if (literalTitle)
                h3.InnerText = colTitle;
            else
                h3.InnerText = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, colTitle);
            parent.Controls.Add(h3);

            ///------------------------------
            /// then add the UL list
            parent.Controls.Add(list);
        }

        private void AssignWebFieldLabel(HtmlGenericControl label, string dataColumn)
        {
            label.InnerText = CECWebSrv.GetCohortWebFieldLabelByColumnName(UserToken, dataColumn) + ": ";
        }

        private void PopulateWebFieldValues(DataTable dt)
        {
            cd_acronym.InnerText = (string)dt.Rows[0]["cohort_acronym"] ?? string.Empty;

            cd_name.InnerText = (string)dt.Rows[0]["cohort_name"] ?? string.Empty;

            cd_website.Attributes.Add("class", "link-url");
            cd_website.InnerText = "Cohort Website";
            if (dt.Rows[0]["cohort_web_site"] == DBNull.Value || helper.IsStringEmptyWhiteSpace(dt.Rows[0]["cohort_web_site"].ToString()))
                cd_website.Visible = false;
            if (dt.Rows[0]["cohort_web_site"] != DBNull.Value && dt.Rows[0]["cohort_web_site"].ToString().Trim().StartsWith("http"))
                cd_website.HRef = ((string)dt.Rows[0]["cohort_web_site"]).Trim();
            else
                cd_website.HRef = "http://" + ((string)dt.Rows[0]["cohort_web_site"]).Trim() ?? string.Empty;

            if (dt.Rows[0]["date_form_completed"] != null)
                cd_lastupdate.InnerText = DateTime.Parse(dt.Rows[0]["date_form_completed"].ToString()).ToString("MM/dd/yyyy");

            for (int _i = 1; _i <= 6; _i++)
            {
                string pName = String.Format("pi_name_{0}", _i);
                string iName = String.Format("pi_institution_{0}", _i);

                if ((dt.Rows[0][pName] != DBNull.Value) && !helper.IsStringEmptyWhiteSpace(dt.Rows[0][pName].ToString()))
                {
                    string html = String.Format("<li>{0} ({1})</li>", dt.Rows[0][pName], dt.Rows[0][iName]);
                    piList.Controls.Add(new LiteralControl(html));
                }
            }

            //if (CECWebSrv.CohortHasPIListAttachment(UserToken, (int)dt.Rows[0]["cohort_id"]))
            //{
            //    System.Data.DataTable piAtt = CECWebSrv.GetCohortPIListAttachment(UserToken, (int)dt.Rows[0]["cohort_id"]).Tables[0];

            //    System.Web.UI.WebControls.HyperLink attachment =
            //            new HyperLink();
            //    attachment.Text = "List of Additional Collaborators";
            //    attachment.NavigateUrl = String.Format("/cohortDetails.aspx?cohort_id={0}&download={1}", piAtt.Rows[0]["cohort_id"], piAtt.Rows[0]["id"]);

            //    System.Web.UI.HtmlControls.HtmlGenericControl li =
            //        new HtmlGenericControl("li");
            //    li.Attributes.Add("class", helper.GetCSSClassForFileExtension(piAtt.Rows[0]["file_name"].ToString()));
            //    li.Controls.Add(attachment);

            //    piList.Controls.Add(li);
            //}

            if (!helper.IsStringEmptyWhiteSpace(dt.Rows[0]["collab_contact_name"].ToString()))
            {
                cd_contact.Controls.Add(new LiteralControl(String.Format("<li>{0} ({1})</li>", dt.Rows[0]["collab_contact_name"], dt.Rows[0]["collab_contact_position"])));
                cd_contact.Controls.Add(new LiteralControl(String.Format("<li class=\"link-email\"> <a href='mailto:{0}'><span class=\"glyphicon glyphicon-envelope\"></span> {0}</a></li>", dt.Rows[0]["collab_contact_email"])));
                cd_contact.Controls.Add(new LiteralControl(String.Format("<li><span class=\"glyphicon glyphicon-phone-alt\"></span> {0}</li>", dt.Rows[0]["collab_contact_phone"])));
            }

            string c_desc = (string)dt.Rows[0]["cohort_description"] ?? string.Empty;
            c_desc = Regex.Replace(c_desc, @"\r", "<br />");
            cd_description.InnerHtml = String.Format("<p>{0}</p>", c_desc);

            for (int _i = 1; _i <= 10; _i++)
            {
                //string columnName = String.Format("participating_investigator_{0}", _i);

                //if ((dt.Rows[0][columnName] != DBNull.Value) && !help.IsStringEmptyWhiteSpace(dt.Rows[0][columnName].ToString()))
                //    cd_sites.Controls.Add(new LiteralControl(String.Format("<li>{0}</li>", dt.Rows[0][columnName])));
            }

            // Catchment area removed
            //if (!cd_sites.HasControls())
            //    cd_sites.Controls.Add(new LiteralControl("<li>No data available</li>"));
        }

        private void PopulateFormURL(DataTable dt)
        {
            ///-------------------------------------------------------------------
            /// add static urls from pdf form if available--work around for getting
            ///  urls into their respective categories
            ///  
            string[] columns = new string[] { "docs_questionnaires_url", "docs_cohort_protocol_url", "docs_data_sharing_policy_url", "docs_biospecimen_sharing_policy_url", "docs_publication_policy_url", "request_procedures_web_url" };
            for (int _i = 0; _i < columns.Length; _i++)
            {
                string col = columns[_i];
                int colIndex = (_i + 1);

                if (String.IsNullOrWhiteSpace(dt.Rows[0][col].ToString()))
                    continue;

                string test = dt.Rows[0][col].ToString().ToLower().Trim();
                if (!test.StartsWith("http") && !test.StartsWith("www"))
                    continue;

                //string _tc = dt.Rows[0][col].ToString();
                //if (!dt.Rows[0][col].ToString().ToLower().StartsWith("http") || !dt.Rows[0][col].ToString().ToLower().StartsWith("www"))
                //    continue;

                ///----------------------------------------
                /// stopped commenting, code has been hacked to handle changes from the client
                ///  in the time provided. first casualities are comments/documentation.... o.0
                ///  
                ///------------------------------------ 
                /// handles clearing the default text from
                ///  the various attachment sections
                /// 
                /// hacked here too
                if (colIndex == 1)
                    ClearDefaultsInSection(quest_attachments.ID);
                else if (colIndex == 2)
                    ClearDefaultsInSection(prot_attachments.ID);
                else if (colIndex == 3 || colIndex == 4 || colIndex == 5 || colIndex == 6)
                    ClearDefaultsInSection(pol_attachments.ID);

                /// hack here a bit, there a bit
                //System.Web.UI.HtmlControls.HtmlGenericControl section = (HtmlGenericControl)FindControlRecursive(attachments, colIndex.ToString());
                //if (section == null)
                //{
                //    section =
                //        new HtmlGenericControl("div");
                //    section.ID = colIndex.ToString();

                //    System.Web.UI.HtmlControls.HtmlGenericControl p =
                //        new HtmlGenericControl("h3");
                //    if (colIndex == 1)
                //        p.InnerText = "Questionnaires";
                //    else if (colIndex == 2)
                //        p.InnerText = "Main cohort protocol";
                //    else if (colIndex == 3)
                //        p.InnerText = "Data sharing policy";
                //    else if (colIndex == 4)
                //        p.InnerText = "Biospecimen sharing policy";
                //    else if (colIndex == 5)
                //        p.InnerText = "Publication (authorship) policy";

                //    section.Controls.Add(p);

                //    if (colIndex == 1)
                //        prot_attachments.Controls.Add(section);
                //    else if (colIndex == 2)
                //        prot_attachments.Controls.Add(section);
                //    else if (colIndex == 3)
                //        pol_attachments.Controls.Add(section);
                //    else if (colIndex == 4)
                //        pol_attachments.Controls.Add(section);
                //    else if (colIndex == 5)
                //        pol_attachments.Controls.Add(section);
                //}

                /// hack for you, and you, and you; hacks for everyone!
                string list_id = String.Format("{0}_ul", colIndex);
                if (colIndex == 1)
                    list_id = "quest_ul";
                else if (colIndex == 2)
                    list_id = "prot_ul";
                else
                    list_id = "pol_ul";

                System.Web.UI.HtmlControls.HtmlGenericControl ul = (HtmlGenericControl)FindControlRecursive(attachments, list_id);
                if (ul == null)
                {
                    ul =
                        new HtmlGenericControl("ul");
                    ul.Attributes["class"] = "links-list";
                    ul.ID = list_id;

                    if (colIndex == 1)
                        quest_attachments.Controls.Add(ul);
                    else if (colIndex == 2)
                        prot_attachments.Controls.Add(ul);
                    else
                        pol_attachments.Controls.Add(ul);

                    //section.Controls.Add(ul);
                }

                //if (ul != null && colIndex < 3)
                //    continue;
                
                System.Web.UI.WebControls.HyperLink priAtt = (HyperLink)FindControlRecursive(ul, String.Format("{0}_link", colIndex));
                if (priAtt != null)
                    continue;

                System.Web.UI.HtmlControls.HtmlGenericControl pli =
                    new HtmlGenericControl("li");
                pli.Attributes.Add("class", "link-url");

                priAtt =
                    new HyperLink();
                priAtt.ID = String.Format("{0}_link", colIndex);
                priAtt.Target = "_blank";

                string url = string.Empty;
                url = dt.Rows[0][col].ToString().Trim();

                if (!url.StartsWith("http"))
                    url = "http://" + url;

                priAtt.NavigateUrl = url;
                priAtt.Text = dt.Rows[0][col].ToString().Trim();

                /// and another hack
                string text_exists = String.Format("{0}:{1}", list_id, priAtt.Text.Substring(0, (priAtt.Text.Length >= 50 ? 50 : priAtt.Text.Length)));
                if (!listAttachments.Contains(text_exists))
                {
                    listAttachments.Add(text_exists);

                    pli.Controls.Add(priAtt);
                    ul.Controls.Add(pli);
                }
            }
        }

        private void PopulateAttachmentSection(DataTable dt)
        {
            string curAttachmentCategory = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                if ((int)dr["document_type_id"] == 7)
                    continue;

                ///----------------------------------------
                /// stopped commenting, code has been hacked to handle changes from the client
                ///  in the time provided. first casualities are comments/documentation.... o.0
                ///  
                int doc_type = (int)dr["document_type_id"];

                if (dr["attachment_type"].ToString() == "url")
                {
                    string test = dr["url"].ToString().ToLower().Trim();
                    if (!test.StartsWith("http") && !test.StartsWith("www"))
                        continue;
                }


                if (curAttachmentCategory != dr["document_type"].ToString())
                {
                    ///------------------------------------ 
                    /// handles clearing the default text from
                    ///  the various attachment sections
                    /// 
                    if ((int)dr["document_type_id"] == 1 && FindControl("quest_ul") == null)
                        ClearDefaultsInSection(quest_attachments.ID);
                    else if ((int)dr["document_type_id"] == 2 && FindControl("prot_ul") == null)
                        ClearDefaultsInSection(prot_attachments.ID);
                    else if (((int)dr["document_type_id"] == 3 || (int)dr["document_type_id"] == 4 || (int)dr["document_type_id"] == 5) && FindControl("pol_ul") == null)
                        ClearDefaultsInSection(pol_attachments.ID);
                    //else if ((int)dr["document_type_id"] == 10 && FindControl(String.Format("{0}_ul", dr["document_type_id"])) == null)
                    //    ClearDefaultsInSection(grant_attachments.ID);
                    //else if (FindControl(String.Format("{0}_ul", dr["document_type_id"])) == null)
                    //    ClearDefaultsInSection(pub_attachments.ID);

                    curAttachmentCategory = dr["document_type"].ToString();

                    System.Web.UI.HtmlControls.HtmlGenericControl section =
                        new HtmlGenericControl("div");
                    section.ID = doc_type.ToString();

                    System.Web.UI.HtmlControls.HtmlGenericControl p =
                        new HtmlGenericControl("h3");
                    p.InnerText = curAttachmentCategory;

                    section.Controls.Add(p);

                    System.Web.UI.HtmlControls.HtmlGenericControl ul =
                        new HtmlGenericControl("ul");
                    ul.Attributes["class"] = "links-list";

                    /// see, told you this was hacked, this is inelegant code
                    if (doc_type == 1 && !quest_attachments.HasControls())
                    {
                        ul.ID = "quest_ul";
                        quest_attachments.Controls.Add(ul);
                    }
                    else if (doc_type == 2 && !prot_attachments.HasControls())
                    {
                        ul.ID = "prot_ul";
                        prot_attachments.Controls.Add(ul);
                    }
                    else if ((doc_type == 3 || doc_type == 4 || doc_type == 5) && !pol_attachments.HasControls())
                    {
                        ul.ID = "pol_ul";
                        pol_attachments.Controls.Add(ul);
                    }
                    //else if (doc_type == 10 && !grant_attachments.HasControls())
                    //{
                    //    ul.ID = "10_ul";
                    //    grant_attachments.Controls.Add(ul);
                    //}
                    //else
                    //{
                    //    ul.ID = String.Format("{0}_ul", dr["document_type_id"]);
                    //    section.Controls.Add(ul);

                    //    pub_attachments.Controls.Add(section);
                    //}

                    ///------------------------------------------------------------------
                    /// since we do not want to display blank sections, these conditions
                    ///  will test if the section should be displayed
                    ///  
                    //if ((int)dr["document_type_id"] == 1)
                    //    prot_attachments.Controls.Add(section);
                    //else if ((int)dr["document_type_id"] == 2)
                    //    prot_attachments.Controls.Add(section);
                    //else if ((int)dr["document_type_id"] == 3)
                    //    pol_attachments.Controls.Add(section);
                    //else if ((int)dr["document_type_id"] == 4)
                    //    pol_attachments.Controls.Add(section);
                    //else if ((int)dr["document_type_id"] == 5)
                    //    pol_attachments.Controls.Add(section);
                    //if ((int)dr["document_type_id"] == 10)
                    //    grant_attachments.Controls.Add(section);
                    //else
                    //    pub_attachments.Controls.Add(section);
                }

                string list_id = String.Format("{0}_ul", doc_type);
                /// see, told you this was hacked, this is inelegant code
                if (doc_type == 3 || doc_type == 4 || doc_type == 5)
                    list_id = "pol_ul";
                else if (doc_type == 2)
                    list_id = "prot_ul";
                else if (doc_type == 1)
                    list_id = "quest_ul";
                else if (doc_type == 10)
                    list_id = "10_ul";

                System.Web.UI.HtmlControls.HtmlGenericControl list = (HtmlGenericControl)FindControl(list_id);
                if (list == null)
                    continue;

                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");

                System.Web.UI.WebControls.HyperLink attachment =
                    new HyperLink();
                
                switch (dr["attachment_type"].ToString())
                {
                    case "document":
                        attachment.ID = String.Format("doc_{0}", dr["id"]);
                        attachment.NavigateUrl = String.Format("./cohortDetails.aspx?cohort_id={0}&download={1}", dt_cohort.Rows[0]["cohort_id"], dr["id"]);
                        attachment.Text = dr["file_name"].ToString();

                        li.Attributes.Add("class", helper.GetCSSClassForFileExtension(attachment.Text));
                        break;
                    case "file":
                        attachment.ID = String.Format("file_{0}", dr["id"]);
                        attachment.NavigateUrl = String.Format("./cohortDetails.aspx?cohort_id={0}&download={1}", dt_cohort.Rows[0]["cohort_id"], dr["id"]);
                        attachment.Text = dr["file_name"].ToString();

                        li.Attributes.Add("class", helper.GetCSSClassForFileExtension(attachment.Text));
                        break;
                    case "url":
                        string url = dr["url"].ToString();
                        if (!url.StartsWith("http"))
                            url = "http://" + url;

                        //attachment.ID = String.Format("link_{0}", dr["document_type_id"]);
                        attachment.NavigateUrl = url;
                        attachment.Target = "_blank";
                        attachment.Text = url;

                        li.Attributes.Add("class", "link-url");
                        break;
                }

                /// more hacks not to add existing attachments to the ul list
                string text_exists = String.Format("{0}:{1}", list_id, attachment.Text.Substring(0, (attachment.Text.Length < 50 ? attachment.Text.Length : 50)));
                if (!listAttachments.Contains(text_exists))
                {
                    listAttachments.Add(text_exists);

                    li.Controls.Add(attachment);
                    list.Controls.Add(li);
                }
            } 
        }

        private void ClearDefaultsInSection(string section)
        {
            if (clearedCohorts.IndexOf(section) > -1)
                return;

            System.Web.UI.HtmlControls.HtmlGenericControl attachSect = (HtmlGenericControl)FindControlRecursive(attachments, section);
            if (attachSect != null)
            {
                attachSect.Controls.Clear();
                clearedCohorts.Add(section);
            }
        }

        private void ChangeDefaultAttachmentEmptyText(string section)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl attachSect = (HtmlGenericControl)FindControlRecursive(attachments, section);
            if (attachSect != null && attachSect.HasControls())
            {
                System.Web.UI.LiteralControl ul = (LiteralControl)attachSect.Controls[0];
                ul.Text = @"<ul><li>Pending</li></ul>";
            }          
        }

        #region Depricated Routines

        private void PopulateOtherSection(DataTable dt)
        {
            ///---------------------
            /// general other section
            /// 
            //if (helper.IsLogicalTrue(dt.Rows[0]["dlh_linked_to_existing_databases"]))
            //CreateTitleListControls(otherCohortInfo, "dlh_linked_to_existing_databases", "dlh_linked_to_existing_databases_specify", true);

            //if (helper.IsLogicalTrue(dt.Rows[0]["dlh_harmonization_projects"]))
            //CreateTitleListControls(otherCohortInfo, "dlh_harmonization_projects", "dlh_harmonization_projects_specify");

            //if (helper.IsLogicalTrue(dt.Rows[0]["tech_use_of_mobile"]))
            //CreateTitleListControls(otherCohortInfo, "tech_use_of_mobile", "tech_use_of_mobile_describe");

            //if (helper.IsLogicalTrue(dt.Rows[0]["tech_use_of_cloud"]))
            //CreateTitleListControls(otherCohortInfo, "tech_use_of_cloud", "tech_use_of_cloud_describe");

            ///---------------------
            /// bio other section
            /// 
            string[] bio1 = new string[] { "bio_samples_prepared_ffpe", "bio_samples_prepared_fff", "bio_samples_prepared_diag_slides", 
                "bio_samples_prepared_other_specify" };
            //for (int _i = 0; _i < bio1.Length; _i++)
            //{
            //    object elem = dt_cohort.Rows[0][bio1[_i]];
            //    if (!helper.IsLogicalTrue(elem) || help.IsStringEmptyWhiteSpace((elem as string)))
            //        bio1[_i] = string.Empty;
            //}
            //CreateTitleListControls(otherBioInfo, "Tumor Tissue Prepared/Stored (if applicable)", true, bio1, true);

            string[] bio2 = new string[] { "bio_samples_collected_by_core_biopsy", "bio_samples_collected_by_fna", "bio_samples_collected_by_surgery", "bio_samples_collected_by_other", 
                "bio_samples_collected_by_other_specify" };
            //for (int _i = 0; _i < bio2.Length; _i++)
            //{
            //    object elem = dt_cohort.Rows[0][bio2[_i]];
            //    if (!helper.IsLogicalTrue(elem) || help.IsStringEmptyWhiteSpace((elem as string)))
            //        bio2[_i] = string.Empty;
            //}
            //CreateTitleListControls(otherBioInfo, "Tumor Tissue Collected (if applicable)", true, bio2);

            string[] bio3 = new string[] { "bio_genotyping_data_count", "bio_genotyping_exome_count", "bio_genotyping_whole_genome_count", "bio_genotyping_epigenetic_count", 
                "bio_genotyping_other_data_count" };
            //for (int _i = 0; _i < bio3.Length; _i++)
            //{
            //    object elem = dt_cohort.Rows[0][bio3[_i]];
            //    if (help.IsStringEmptyWhiteSpace((elem as string)))
            //        bio3[_i] = string.Empty;
            //}
            //CreateTitleListControls(otherBioInfo, "Number of participants for Genotyping, Sequencing, Markers, and Other Omics Data", true, bio3);
        }
        #endregion
    }
}