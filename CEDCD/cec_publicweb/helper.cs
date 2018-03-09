using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using cec_publicservice;

//using NPOI.SS.UserModel;
//using NPOI.SS.Util;
//using NPOI.XSSF.UserModel;
//using NPOI.XWPF.UserModel;



namespace cec_publicweb
{
    #region CEDCD Site Exceptions

    public class NotLoggedInException : WebException
    {
        #region Constructor

        public NotLoggedInException(string msg)
            : base(msg)
        {
        }

        public NotLoggedInException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }
        #endregion
    }

    public class AccountLockedOutException : WebException
    {
        public string User
        {
            get;
            set;
        }
        public DateTime LockoutDate
        {
            get;
            set;
        }

        #region Constructor

        public AccountLockedOutException(string user)
            : base("AccountLockedOutException")
        {
            User = user;
        }

        public AccountLockedOutException(string user, DateTime lockout)
            : base("AccountLockedOutException")
        {
            User = user;
            LockoutDate = lockout;
        }
        #endregion
    }
    #endregion

    public class CECCohortCheckBoxList : System.Web.UI.WebControls.WebControl
    {
        private cec_publicservice.CECHarmPublicService CECWebSrv;

        private System.Collections.ArrayList checkbox_ids;

        private System.Data.DataTable dt_cohorts;

        private System.Web.UI.HtmlControls.HtmlGenericControl list_i; //list_ii, list_iii, list_iv;
        private System.Web.UI.HtmlControls.HtmlGenericControl cohort_count;

        #region Properties
        /// <summary>
        /// get the security token for the given user
        /// </summary>
        public SecurityToken UserToken
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public CECCohortCheckBoxList() : base("div") { }
        #endregion

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            CECWebSrv = new CECHarmPublicService();

            checkbox_ids = new ArrayList();

            System.Web.UI.HtmlControls.HtmlGenericControl mainDiv = new HtmlGenericControl("div");
            mainDiv.Attributes["class"] += "dropdown filter-component btn-group";
            Controls.Add(mainDiv);

            cohort_count = new HtmlGenericControl("span");
            cohort_count.Attributes["class"] = "badge";
            cohort_count.ID = "cohort_list_count";
            cohort_count.InnerText = "0";

            System.Web.UI.HtmlControls.HtmlGenericControl btnShow =
                new HtmlGenericControl("button");
            btnShow.ID = "show_cohort_list_btn";
            btnShow.Attributes["class"] = "btn btn-default dropdown-toggle";
            btnShow.Attributes["type"] = "button";
            btnShow.Attributes["data-toggle"] = "dropdown";
            btnShow.Attributes["aria-haspopup"] = "true";
            btnShow.Attributes["aria-expanded"] = "false";
            btnShow.Controls.Add(new LiteralControl("Select Cohorts&nbsp;"));
            btnShow.Controls.Add(cohort_count);
            mainDiv.Controls.Add(btnShow);

            System.Web.UI.HtmlControls.HtmlGenericControl div_i =
                new HtmlGenericControl("div");
            div_i.ID = "cohort_list_container";
            div_i.Attributes["class"] = "dropdown-menu filter-component-dropdown";
            mainDiv.Controls.Add(div_i);

            div_i.Controls.Add(new LiteralControl("<h4>Select Cohort(s)</h4>"));

            System.Web.UI.HtmlControls.HtmlGenericControl btnClose =
                new HtmlGenericControl("button");
            btnClose.ID = "cohort_list_container_close";
            btnClose.Attributes["type"] = "button";
            btnClose.Attributes["class"] = "btn btn-primary pull-right";
            btnClose.InnerHtml = "X";
            div_i.Controls.Add(btnClose);

            //div_i.Controls.Add(new LiteralControl("<hr />"));

            list_i = new HtmlGenericControl("ul");
            div_i.Controls.Add(list_i);

            System.Web.UI.WebControls.CheckBox cb_allc =
                new CheckBox();
            cb_allc.ID = "cohort_all";
            cb_allc.Attributes["OnClick"] = "cohorts_allCohortsChecked(this);";

            System.Web.UI.HtmlControls.HtmlGenericControl sp_allc =
                new HtmlGenericControl("span");
            sp_allc.Attributes["class"] = "filter-component-input";
            sp_allc.Controls.Add(cb_allc);

            System.Web.UI.HtmlControls.HtmlGenericControl lb_allc =
                new HtmlGenericControl("label");
            lb_allc.Attributes["for"] = cb_allc.ID;
            lb_allc.Attributes["title"] = "Select/Deselect All Cohorts";
            lb_allc.Controls.Add(sp_allc);
            lb_allc.Controls.Add(new LiteralControl("All Cohorts"));

            System.Web.UI.HtmlControls.HtmlGenericControl li =
                new HtmlGenericControl("li");
            li.Controls.Add(lb_allc);
            list_i.Controls.Add(li);
            checkbox_ids.Add("cohort_all");

            // populate dl_cohorts of cohorts
            dt_cohorts = CECWebSrv.GetCohortForSummaryGrid(UserToken, string.Empty).Tables["tbl_web_cohorts_v4_0"];
            for (int _i = 0; _i < dt_cohorts.Rows.Count; _i++)
            {
                // first list column
                System.Data.DataRow dr = dt_cohorts.Rows[_i];
                System.Web.UI.WebControls.CheckBox cb_i =
                    new CheckBox();
                cb_i.ID = dr["cohort_id"].ToString();
                cb_i.InputAttributes["cohort_name"] = dr["cohort_acronym"].ToString();
                //(dr["cohort_name"].ToString().Length > 32 ? String.Format("{0}...", dr["cohort_name"].ToString().Substring(0, 29)) : dr["cohort_name"].ToString());

                System.Web.UI.HtmlControls.HtmlGenericControl sp_i =
                    new HtmlGenericControl("span");
                sp_i.Attributes["class"] = "filter-component-input";
                sp_i.Controls.Add(cb_i);

                System.Web.UI.HtmlControls.HtmlGenericControl lb_i =
                    new HtmlGenericControl("label");
                lb_i.Attributes["for"] = cb_i.ID;
                lb_i.Attributes["title"] = dr["cohort_name"].ToString();
                lb_i.Controls.Add(sp_i);
                lb_i.Controls.Add(new LiteralControl(dr["cohort_acronym"].ToString()));

                System.Web.UI.HtmlControls.HtmlGenericControl li_i =
                    new HtmlGenericControl("li");
                li_i.Controls.Add(lb_i);
                list_i.Controls.Add(li_i);
                checkbox_ids.Add(cb_i.ID);
            }

            System.Web.UI.HtmlControls.HtmlGenericControl selected =
                    new HtmlGenericControl("ul");
            selected.ID = "selected_cohort_list";
            selected.Attributes["class"] = "picked-options";
            Controls.Add(selected);
        }
            
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            cohort_count.InnerText = GetSelectedCohortIDs().Length.ToString();

            base.OnPreRender(e);
        }

        protected Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control c in root.Controls)
            {
                if (c.ID == id)
                    return c;

                Control t = FindControlRecursive(c, id);

                if (t != null && t.ID == id)
                    return t;
            }
            return null;
        }

        public string[] GetSelectedCohortIDs()
        {
            System.Collections.ArrayList list =
                new ArrayList();

            foreach (String ck_id in checkbox_ids)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, ck_id) as CheckBox);

                if ((cb != null) && cb.Checked && cb.ID != "cohort_all")
                    list.Add(ck_id);
            }

            return (string[])list.ToArray(typeof(string));
        }

        public void ClearSelectedCohortIDs()
        {
            foreach (String ck_id in checkbox_ids)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, ck_id) as CheckBox);

                if (cb != null)
                    cb.Checked = false;
            }
        }

        public string GetCohortAcronym(int cohort_id)
        {
            DataRow[] dr_r = dt_cohorts.Select(String.Format("cohort_id={0}", cohort_id));
            if (dr_r.Length <= 0)
                return string.Empty;

            return dr_r[0]["cohort_acronym"].ToString();
        }

        public string GetCohortName(int cohort_id)
        {
            DataRow[] dr_r = dt_cohorts.Select(String.Format("cohort_id={0}", cohort_id));
            if (dr_r.Length <= 0)
                return string.Empty;

            return dr_r[0]["cohort_name"].ToString();
        }
    }

    public class CECCancerCheckBoxList : System.Web.UI.WebControls.WebControl
    {
        private cec_publicservice.CECHarmPublicService CECWebSrv;

        public System.Collections.Specialized.NameValueCollection cancer_list;

        private System.Collections.ArrayList checkbox_ids;

        private System.Web.UI.HtmlControls.HtmlGenericControl list;
        private System.Web.UI.HtmlControls.HtmlGenericControl cancer_count;
        

        #region Properties
        /// <summary>
        /// get the security token for the given user
        /// </summary>
        public SecurityToken UserToken
        {
            get;
            set;
        }

        public bool ShowAllCancerOption
        {
            get;
            set;
        }

        public bool ShowNoCancerOption
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public CECCancerCheckBoxList() 
            : base("div") 
        {
            ShowAllCancerOption = false;
            ShowNoCancerOption = false;
        }
        #endregion

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            CECWebSrv = new CECHarmPublicService();

            cancer_list = new NameValueCollection();
            using (DataTable dt = CECWebSrv.GetCancerTypes(UserToken))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if ((int)dr["cancer_type_id"] == 0 && Page.Request.Path.Contains("cancer.aspx"))
                        continue;

                    cancer_list.Add(dr["data_field_crumb"].ToString(), dr["cancer_type"].ToString());
                }
            }

            checkbox_ids = new ArrayList();

            System.Web.UI.HtmlControls.HtmlGenericControl container = new HtmlGenericControl("div");
            container.Attributes["class"] += "dropdown filter-component btn-group";
            Controls.Add(container);

            cancer_count = new HtmlGenericControl("span");
            cancer_count.Attributes["class"] = "badge";
            cancer_count.ID = "cancer_list_count";
            cancer_count.InnerText = "0";

            System.Web.UI.HtmlControls.HtmlGenericControl btnShow =
                new HtmlGenericControl("button");
            btnShow.ID = "cancer_dropdown";
            btnShow.Attributes["class"] = "btn btn-default dropdown-toggle";
            btnShow.Attributes["data-toggle"] = "dropdown";
            btnShow.Attributes["aria-haspopup"] = "true";
            btnShow.Attributes["aria-expanded"] = "false";
            btnShow.Attributes["type"] = "button";
            btnShow.Controls.Add(new LiteralControl("Cancer Type&nbsp;"));
            btnShow.Controls.Add(cancer_count);
            container.Controls.Add(btnShow);

            System.Web.UI.HtmlControls.HtmlGenericControl div_i =
                new HtmlGenericControl("div");
            div_i.ID = "cancer_options_list";
            div_i.Attributes["class"] = "dropdown-menu filter-component-dropdown";
            container.Controls.Add(div_i);

            div_i.Controls.Add(new LiteralControl("<h4>Select Cancer(s)</h4>"));

            System.Web.UI.HtmlControls.HtmlGenericControl btnClose =
                    new HtmlGenericControl("button");
            btnClose.ID = "cancer_type_close_btn";
            btnClose.Attributes["class"] = "btn btn-primary cancer_type_close_btn pull-right";
            btnClose.Attributes["type"] = "button";
            btnClose.InnerHtml = "X";
            div_i.Controls.Add(btnClose);

            //div_i.Controls.Add(new LiteralControl("<hr />"));

            list = new HtmlGenericControl("ul");
            div_i.Controls.Add(list);

            if (ShowAllCancerOption)
            {
                System.Web.UI.WebControls.CheckBox cb_allcancer =
                    new CheckBox();
                cb_allcancer.ID = "cancer_all";
                //cb_allcancer.Text = "All Cancer";
                cb_allcancer.Attributes["OnClick"] = "cancer_allCancerChecked(this);";

                System.Web.UI.HtmlControls.HtmlGenericControl span =
                    new HtmlGenericControl("span");
                span.Attributes["class"] = "filter-component-input";
                span.Controls.Add(cb_allcancer);

                System.Web.UI.HtmlControls.HtmlGenericControl label =
                    new HtmlGenericControl("label");
                label.Attributes["for"] = cb_allcancer.ID;
                label.Controls.Add(span);
                label.Controls.Add(new LiteralControl("All Cancer"));

                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");
                li.Controls.Add(label);
                list.Controls.Add(li);
                checkbox_ids.Add("cancer_all");
            }

            // populate list of cancers
            for (int _i = 0; _i < cancer_list.Count; _i++)
            {
                System.Web.UI.WebControls.CheckBox cb_i =
                    new CheckBox();
                cb_i.ID = cancer_list.Keys[_i];
                cb_i.InputAttributes["cancer_name"] = cancer_list[_i];
                //"cancer_" + dr["cancer_type_id"].ToString();
                //cb_i.Text = cancer_list[_i]; //dr["cancer_type"].ToString();

                //if (cb_i.Text.Length > 30)
                //    cb_i.LabelAttributes["class"] += " wrap-label";

                System.Web.UI.HtmlControls.HtmlGenericControl span =
                    new HtmlGenericControl("span");
                span.Attributes["class"] = "filter-component-input";
                span.Controls.Add(cb_i);

                System.Web.UI.HtmlControls.HtmlGenericControl label =
                    new HtmlGenericControl("label");
                label.Attributes["for"] = cb_i.ID;
                label.Controls.Add(span);
                label.Controls.Add(new LiteralControl(cancer_list[_i]));
                
                System.Web.UI.HtmlControls.HtmlGenericControl li_i =
                    new HtmlGenericControl("li");
                li_i.Controls.Add(label);
                // insert no-cancer at first position
                //if ((int)dr["cancer_type_id"] == 0)
                //    list.Controls.AddAt(0, li_i);
                //else
                list.Controls.Add(li_i);
                checkbox_ids.Add(cb_i.ID);
            }


            System.Web.UI.HtmlControls.HtmlGenericControl selected =
                new HtmlGenericControl("ul");
            selected.ID = "selected_cancer_list";
            selected.Attributes["class"] = "picked-options";
            Controls.Add(selected);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            cancer_count.InnerText = GetSelectedCancers().Length.ToString();

            base.OnPreRender(e);
        }

        protected Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control c in root.Controls)
            {
                if (c.ID == id)
                    return c;

                Control t = FindControlRecursive(c, id);

                if (t != null && t.ID == id)
                    return t;
            }
            return null;
        }
        
        public string[] GetSelectedCancers()
        {
            System.Collections.ArrayList tlist =
                new ArrayList();

            foreach (String ck_id in checkbox_ids)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, ck_id) as CheckBox);

                if ((cb != null) && cb.Checked && cb.ID != "cancer_all")
                    tlist.Add(cb.ID);
            }

            return (string[])tlist.ToArray(typeof(string));
        }

        public void ClearSelectedCancers()
        {
            foreach (String ck_id in checkbox_ids)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, ck_id) as CheckBox);

                if (cb != null)
                    cb.Checked = false;
            }
        }

        public string GetCancerLabel(string key)
        {
            return cancer_list[key];
        }
    }

    public class CECSpecimenCheckBoxList : System.Web.UI.WebControls.WebControl
    {
        private cec_publicservice.CECHarmPublicService CECWebSrv;

        private System.Collections.Specialized.NameValueCollection specimen_types;

        private System.Web.UI.HtmlControls.HtmlGenericControl list;
        private System.Web.UI.HtmlControls.HtmlGenericControl specimen_count;

        #region Properties
        /// <summary>
        /// get the security token for the given user
        /// </summary>
        public SecurityToken UserToken
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public CECSpecimenCheckBoxList() : base("div") { }
        #endregion

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            System.Web.UI.HtmlControls.HtmlGenericControl container =
                new HtmlGenericControl("div");
            container.Attributes["class"] += "dropdown filter-component btn-group";
            Controls.Add(container);

            specimen_count = new HtmlGenericControl("span");
            specimen_count.Attributes["class"] = "badge";
            specimen_count.ID = "specimen_list_count";
            specimen_count.InnerText = "0";

            System.Web.UI.HtmlControls.HtmlGenericControl btnShow =
                new HtmlGenericControl("button");
            btnShow.ID = "specimen_dropdown";
            btnShow.Attributes["class"] = "btn btn-default dropdown-toggle";
            btnShow.Attributes["data-toggle"] = "dropdown";
            btnShow.Attributes["aria-haspopup"] = "true";
            btnShow.Attributes["aria-expanded"] = "false";
            btnShow.Attributes["type"] = "button";
            btnShow.Controls.Add(new LiteralControl("Specimen Type&nbsp;"));
            btnShow.Controls.Add(specimen_count);
            container.Controls.Add(btnShow);

            System.Web.UI.HtmlControls.HtmlGenericControl div_i =
                new HtmlGenericControl("div");
            div_i.ID = "specimen_options_list";
            div_i.Attributes["class"] = "dropdown-menu filter-component-dropdown";
            container.Controls.Add(div_i);

            div_i.Controls.Add(new LiteralControl("<h4>Select Specimen Type(s)</h4>"));

            System.Web.UI.HtmlControls.HtmlGenericControl btnClose =
                    new HtmlGenericControl("button");
            btnClose.ID = "specimen_type_close_btn";
            btnClose.Attributes["class"] = "btn btn-primary specimen_type_close_btn pull-right";
            btnClose.Attributes["type"] = "button";
            btnClose.InnerHtml = "X";
            div_i.Controls.Add(btnClose);

            //div_i.Controls.Add(new LiteralControl("<hr />"));

            list = new HtmlGenericControl("ul");
            div_i.Controls.Add(list);

            // populate list of specimen types
            for(int i=0; i<specimen_types.Count; i++)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    new CheckBox();
                cb.ID = specimen_types.Keys[i];
                cb.InputAttributes["specimen_name"] = specimen_types[i];
                //cb.Text = specimen_types[i];

                System.Web.UI.HtmlControls.HtmlGenericControl sp =
                    new HtmlGenericControl("span");
                sp.Attributes["class"] = "filter-component-input";
                sp.Controls.Add(cb);

                System.Web.UI.HtmlControls.HtmlGenericControl lb =
                    new HtmlGenericControl("label");
                lb.Attributes["for"] = cb.ID;
                lb.Controls.Add(sp);
                lb.Controls.Add(new LiteralControl(specimen_types[i]));

                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");
                li.Controls.Add(lb);
                list.Controls.Add(li);
            }

            System.Web.UI.HtmlControls.HtmlGenericControl selected =
                new HtmlGenericControl("ul");
            selected.ID = "selected_specimen_list";
            selected.Attributes["class"] = "picked-options";
            Controls.Add(selected);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            specimen_types = new NameValueCollection();
            CECWebSrv = new CECHarmPublicService();
            using (DataTable dt = CECWebSrv.GetBiospecimenTypes(UserToken))
            {
                foreach (DataRow dr in dt.Rows)
                    specimen_types.Add(dr["data_field_crumb"].ToString(), dr["biospecimen_type"].ToString());
            }

            EnsureChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            specimen_count.InnerText = GetSelectedSpecimenTypes().Length.ToString();

            base.OnPreRender(e);
        }

        protected Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control c in root.Controls)
            {
                if (c.ID == id)
                    return c;

                Control t = FindControlRecursive(c, id);

                if (t != null && t.ID == id)
                    return t;
            }
            return null;
        }

        public string[] GetSelectedSpecimenTypes()
        {
            System.Collections.ArrayList tlist =
                new ArrayList();

            foreach(string s in specimen_types.AllKeys)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, s) as CheckBox);

                if ((cb != null) && cb.Checked)
                    tlist.Add(s);
            }

            return (string[])tlist.ToArray(typeof(string));
        }

        public void ClearSelectedSpecimenTypes()
        {
            foreach (String ck_id in specimen_types.Keys)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, ck_id) as CheckBox);

                if (cb != null)
                    cb.Checked = false;
            }
        }

        public string GetSpecimenTypeLabel(string key)
        {
            return specimen_types[key];
        }
    }

    public class CECGenderCheckBoxList : System.Web.UI.WebControls.WebControl
    {
        private cec_publicservice.CECHarmPublicService CECWebSrv;

        private System.Collections.Specialized.NameValueCollection list;

        private System.Web.UI.HtmlControls.HtmlGenericControl ctrl_list;
        private System.Web.UI.HtmlControls.HtmlGenericControl ctrl_list_count;

        #region Properties
        /// <summary>
        /// get the security token for the given user
        /// </summary>
        public SecurityToken UserToken
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public CECGenderCheckBoxList() : base("div") { }
        #endregion

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            System.Web.UI.HtmlControls.HtmlGenericControl container =
                new HtmlGenericControl("div");
            container.Attributes["class"] += "dropdown filter-component btn-group";
            Controls.Add(container);

            ctrl_list_count = new HtmlGenericControl("span");
            ctrl_list_count.Attributes["class"] = "badge";
            ctrl_list_count.ID = "gender_list_count";
            ctrl_list_count.InnerText = "0";

            System.Web.UI.HtmlControls.HtmlGenericControl btnShow =
                new HtmlGenericControl("button");
            btnShow.ID = "gender_dropdown";
            btnShow.Attributes["class"] = "btn btn-default dropdown-toggle";
            btnShow.Attributes["data-toggle"] = "dropdown";
            btnShow.Attributes["aria-haspopup"] = "true";
            btnShow.Attributes["aria-expanded"] = "false";
            btnShow.Attributes["type"] = "button";
            btnShow.Controls.Add(new LiteralControl("Gender&nbsp;"));
            btnShow.Controls.Add(ctrl_list_count);
            container.Controls.Add(btnShow);

            System.Web.UI.HtmlControls.HtmlGenericControl div_i =
                new HtmlGenericControl("div");
            div_i.ID = "list_gender";
            div_i.Attributes["class"] = "dropdown-menu filter-component-dropdown";
            container.Controls.Add(div_i);

            div_i.Controls.Add(new LiteralControl("<h4>Select Gender</h4>"));

            System.Web.UI.HtmlControls.HtmlGenericControl btnClose =
                    new HtmlGenericControl("button");
            btnClose.ID = "gender_close_btn";
            btnClose.Attributes["class"] = "btn btn-primary pull-right";
            btnClose.Attributes["type"] = "button";
            btnClose.InnerHtml = "X";
            div_i.Controls.Add(btnClose);

            //div_i.Controls.Add(new LiteralControl("<hr />"));

            ctrl_list = new HtmlGenericControl("ul");
            div_i.Controls.Add(ctrl_list);

            // populate list of specimen types
            for (int i = 0; i < list.Count; i++)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    new CheckBox();
                cb.ID = String.Format("{0}_{1}", this.ID, list.Keys[i]);
                cb.InputAttributes["option_name"] = list[i];
                //cb.Text = specimen_types[i];

                System.Web.UI.HtmlControls.HtmlGenericControl sp =
                    new HtmlGenericControl("span");
                sp.Attributes["class"] = "filter-component-input";
                sp.Controls.Add(cb);

                System.Web.UI.HtmlControls.HtmlGenericControl lb =
                    new HtmlGenericControl("label");
                lb.Attributes["for"] = cb.ID;
                lb.Controls.Add(sp);
                lb.Controls.Add(new LiteralControl(list[i]));

                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");
                li.Controls.Add(lb);
                ctrl_list.Controls.Add(li);
            }

            System.Web.UI.HtmlControls.HtmlGenericControl selected =
                new HtmlGenericControl("ul");
            selected.ID = "selected_list_gender";
            selected.Attributes["class"] = "picked-options";
            Controls.Add(selected);

            string script = String.Format("<script type=\"text/javascript\"> " +
                "$('#list_{0}').click(function (event) {{ if($('#gender_dropdown').attr('aria-expanded') == 'true' && event.target.nodeName != 'BUTTON') {{ event.stopPropagation(); }} }}); " + 
                "function e{0}_populateSelectList() {{ var selected = $('#list_{0} input:checked').length; " +
                " $('#gender_list_count').text(selected); " +
                " $('#selected_list_{0}').empty(); var labelCount = 0; " +
                " $('#list_{0} input:checked').each(function () {{ if (this.id == '') {{ return true; }} " +
                " if (labelCount < 4) {{ $('#selected_list_{0}').append(\"<li>\" + $(this).attr(\"option_name\") + \"</li>\"); }} " +
                " else {{ $('#selected_list_{0}').append(\"<li>and \" + (selected - 4) + \" more...</li>\"); return false; }} " +
                " labelCount++; }}); }} " +
                "$('#list_{0}').change(function (event) {{ e{0}_populateSelectList(); }}); " +
                "$('#list_{0}').ready(function (event) {{ e{0}_populateSelectList(); }}); " +
                "</script>", "gender");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "gender", script);

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            list = new NameValueCollection();
            list.Add("male", "Males");
            list.Add("female", "Females");

            if (Page.Request.Path.Contains("enrollment"))
                list.Add("unknown", "Unknown");
            
            CECWebSrv = new CECHarmPublicService();
            
            EnsureChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            ctrl_list_count.InnerText = GetSelectedOptions().Length.ToString();

            base.OnPreRender(e);
        }

        protected Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control c in root.Controls)
            {
                if (c.ID == id)
                    return c;

                Control t = FindControlRecursive(c, id);

                if (t != null && t.ID == id)
                    return t;
            }
            return null;
        }

        public string[] GetSelectedOptions()
        {
            System.Collections.ArrayList tlist =
                new ArrayList();

            foreach (string s in list.AllKeys)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, String.Format("{0}_{1}", this.ID, s)) as CheckBox);

                if ((cb != null) && cb.Checked)
                    tlist.Add(s);
            }

            return (string[])tlist.ToArray(typeof(string));
        }

        public void ClearSelectedOptions()
        {
            foreach (String ck_id in list.Keys)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, String.Format("{0}_{1}", this.ID, ck_id)) as CheckBox);

                if (cb != null)
                    cb.Checked = false;
            }
        }

        public string GetOptionLabel(string key)
        {
            return list[key];
        }
    }

    public class CECRaceCheckBoxList : System.Web.UI.WebControls.WebControl
    {
        private cec_publicservice.CECHarmPublicService CECWebSrv;

        private System.Collections.Specialized.NameValueCollection list;

        private System.Web.UI.HtmlControls.HtmlGenericControl ctrl_list;
        private System.Web.UI.HtmlControls.HtmlGenericControl ctrl_list_count;

        #region Properties
        /// <summary>
        /// get the security token for the given user
        /// </summary>
        public SecurityToken UserToken
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public CECRaceCheckBoxList() : base("div") { }
        #endregion

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            System.Web.UI.HtmlControls.HtmlGenericControl container =
                new HtmlGenericControl("div");
            container.Attributes["class"] += "dropdown filter-component btn-group";
            Controls.Add(container);

            ctrl_list_count = new HtmlGenericControl("span");
            ctrl_list_count.Attributes["class"] = "badge";
            ctrl_list_count.ID = "race_list_count";
            ctrl_list_count.InnerText = "0";

            System.Web.UI.HtmlControls.HtmlGenericControl btnShow =
                new HtmlGenericControl("button");
            btnShow.ID = "race_dropdown";
            btnShow.Attributes["class"] = "btn btn-default dropdown-toggle";
            btnShow.Attributes["data-toggle"] = "dropdown";
            btnShow.Attributes["aria-haspopup"] = "true";
            btnShow.Attributes["aria-expanded"] = "false";
            btnShow.Attributes["type"] = "button";
            btnShow.Controls.Add(new LiteralControl("Race&nbsp;"));
            btnShow.Controls.Add(ctrl_list_count);
            container.Controls.Add(btnShow);

            System.Web.UI.HtmlControls.HtmlGenericControl div_i =
                new HtmlGenericControl("div");
            div_i.ID = "list_race";
            div_i.Attributes["class"] = "dropdown-menu filter-component-dropdown";
            container.Controls.Add(div_i);

            div_i.Controls.Add(new LiteralControl("<h4>Select Race</h4>"));

            System.Web.UI.HtmlControls.HtmlGenericControl btnClose =
                    new HtmlGenericControl("button");
            btnClose.ID = "race_close_btn";
            btnClose.Attributes["class"] = "btn btn-primary pull-right";
            btnClose.Attributes["type"] = "button";
            btnClose.InnerHtml = "X";
            div_i.Controls.Add(btnClose);

            //div_i.Controls.Add(new LiteralControl("<hr />"));

            ctrl_list = new HtmlGenericControl("ul");
            div_i.Controls.Add(ctrl_list);

            // populate list of specimen types
            for (int i = 0; i < list.Count; i++)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    new CheckBox();
                cb.ID = String.Format("{0}_{1}", this.ID, list.Keys[i]);
                cb.InputAttributes["option_name"] = list[i];
                //cb.Text = specimen_types[i];

                System.Web.UI.HtmlControls.HtmlGenericControl sp =
                    new HtmlGenericControl("span");
                sp.Attributes["class"] = "filter-component-input";
                sp.Controls.Add(cb);

                System.Web.UI.HtmlControls.HtmlGenericControl lb =
                    new HtmlGenericControl("label");
                lb.Attributes["for"] = cb.ID;
                lb.Controls.Add(sp);
                lb.Controls.Add(new LiteralControl(list[i]));

                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");
                li.Controls.Add(lb);
                ctrl_list.Controls.Add(li);
            }

            System.Web.UI.HtmlControls.HtmlGenericControl selected =
                new HtmlGenericControl("ul");
            selected.ID = "selected_list_race";
            selected.Attributes["class"] = "picked-options";
            Controls.Add(selected);

            string script = String.Format("<script type=\"text/javascript\"> " +
                "$('#list_{0}').click(function (event) {{ if($('#race_dropdown').attr('aria-expanded') == 'true' && event.target.nodeName != 'BUTTON') {{ event.stopPropagation(); }} }}); " +
                "function e{0}_populateSelectList() {{ var selected = $('#list_{0} input:checked').length; " +
                " $('#race_list_count').text(selected); " +
                " $('#selected_list_{0}').empty(); var labelCount = 0; " +
                " $('#list_{0} input:checked').each(function () {{ if (this.id == '') {{ return true; }} " +
                " if (labelCount < 4) {{ $('#selected_list_{0}').append(\"<li>\" + $(this).attr(\"option_name\") + \"</li>\"); }} " +
                " else {{ $('#selected_list_{0}').append(\"<li>and \" + (selected - 4) + \" more...</li>\"); return false; }} " +
                " labelCount++; }}); }} " +
                "$('#list_{0}').change(function (event) {{ e{0}_populateSelectList(); }}); " +
                "$('#list_{0}').ready(function (event) {{ e{0}_populateSelectList(); }}); " +
                "</script>", "race");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "race", script);

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            list = new NameValueCollection();
            list.Add("ai", "American Indian/Alaskan Native");
            list.Add("asian", "Asian");
            list.Add("black", "Black or African American");
            list.Add("pi", "Native Hawaiian or Other Pacific Islander");
            list.Add("white", "White");
            list.Add("unknown", "Unknown");
            list.Add("multiple", "More Than One Race");

            CECWebSrv = new CECHarmPublicService();

            EnsureChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            ctrl_list_count.InnerText = GetSelectedOptions().Length.ToString();

            base.OnPreRender(e);
        }

        protected Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control c in root.Controls)
            {
                if (c.ID == id)
                    return c;

                Control t = FindControlRecursive(c, id);

                if (t != null && t.ID == id)
                    return t;
            }
            return null;
        }

        public string[] GetSelectedOptions()
        {
            System.Collections.ArrayList tlist =
                new ArrayList();

            foreach (string s in list.AllKeys)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, String.Format("{0}_{1}", this.ID, s)) as CheckBox);

                if ((cb != null) && cb.Checked)
                    tlist.Add(s);
            }

            return (string[])tlist.ToArray(typeof(string));
        }

        public void ClearSelectedOptions()
        {
            foreach (String ck_id in list.Keys)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, String.Format("{0}_{1}", this.ID, ck_id)) as CheckBox);

                if (cb != null)
                    cb.Checked = false;
            }
        }

        public string GetOptionLabel(string key)
        {
            return list[key];
        }
    }

    public class CECEthnicityCheckBoxList : System.Web.UI.WebControls.WebControl
    {
        private cec_publicservice.CECHarmPublicService CECWebSrv;

        private System.Collections.Specialized.NameValueCollection list;

        private System.Web.UI.HtmlControls.HtmlGenericControl ctrl_list;
        private System.Web.UI.HtmlControls.HtmlGenericControl ctrl_list_count;

        #region Properties
        /// <summary>
        /// get the security token for the given user
        /// </summary>
        public SecurityToken UserToken
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public CECEthnicityCheckBoxList() : base("div") { }
        #endregion

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            System.Web.UI.HtmlControls.HtmlGenericControl container =
                new HtmlGenericControl("div");
            container.Attributes["class"] += "dropdown filter-component btn-group";
            Controls.Add(container);

            ctrl_list_count = new HtmlGenericControl("span");
            ctrl_list_count.Attributes["class"] = "badge";
            ctrl_list_count.ID = "ethn_list_count";
            ctrl_list_count.InnerText = "0";

            System.Web.UI.HtmlControls.HtmlGenericControl btnShow =
                new HtmlGenericControl("button");
            btnShow.ID = "ethn_dropdown";
            btnShow.Attributes["class"] = "btn btn-default dropdown-toggle";
            btnShow.Attributes["data-toggle"] = "dropdown";
            btnShow.Attributes["aria-haspopup"] = "true";
            btnShow.Attributes["aria-expanded"] = "false";
            btnShow.Attributes["type"] = "button";
            btnShow.Controls.Add(new LiteralControl("Ethnicity&nbsp;"));
            btnShow.Controls.Add(ctrl_list_count);
            container.Controls.Add(btnShow);

            System.Web.UI.HtmlControls.HtmlGenericControl div_i =
                new HtmlGenericControl("div");
            div_i.ID = "list_ethn";
            div_i.Attributes["class"] = "dropdown-menu filter-component-dropdown";
            container.Controls.Add(div_i);

            div_i.Controls.Add(new LiteralControl("<h4>Select Ethnicity</h4>"));

            System.Web.UI.HtmlControls.HtmlGenericControl btnClose =
                    new HtmlGenericControl("button");
            btnClose.ID = "ethn_close_btn";
            btnClose.Attributes["class"] = "btn btn-primary pull-right";
            btnClose.Attributes["type"] = "button";
            btnClose.InnerHtml = "X";
            div_i.Controls.Add(btnClose);

            //div_i.Controls.Add(new LiteralControl("<hr />"));

            ctrl_list = new HtmlGenericControl("ul");
            div_i.Controls.Add(ctrl_list);

            // populate list of specimen types
            for (int i = 0; i < list.Count; i++)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    new CheckBox();
                cb.ID = String.Format("{0}_{1}", this.ID, list.Keys[i]);
                cb.InputAttributes["option_name"] = list[i];
                //cb.Text = specimen_types[i];

                System.Web.UI.HtmlControls.HtmlGenericControl sp =
                    new HtmlGenericControl("span");
                sp.Attributes["class"] = "filter-component-input";
                sp.Controls.Add(cb);

                System.Web.UI.HtmlControls.HtmlGenericControl lb =
                    new HtmlGenericControl("label");
                lb.Attributes["for"] = cb.ID;
                lb.Controls.Add(sp);
                lb.Controls.Add(new LiteralControl(list[i]));

                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");
                li.Controls.Add(lb);
                ctrl_list.Controls.Add(li);
            }

            System.Web.UI.HtmlControls.HtmlGenericControl selected =
                new HtmlGenericControl("ul");
            selected.ID = "selected_list_ethn";
            selected.Attributes["class"] = "picked-options";
            Controls.Add(selected);

            string script = String.Format("<script type=\"text/javascript\"> " +
                "$('#list_{0}').click(function (event) {{ if($('#ethn_dropdown').attr('aria-expanded') == 'true' && event.target.nodeName != 'BUTTON') {{ event.stopPropagation(); }} }}); " +
                "function e{0}_populateSelectList() {{ var selected = $('#list_{0} input:checked').length; " +
                " $('#ethn_list_count').text(selected); " +
                " $('#selected_list_{0}').empty(); var labelCount = 0; " +
                " $('#list_{0} input:checked').each(function () {{ if (this.id == '') {{ return true; }} " +
                " if (labelCount < 4) {{ $('#selected_list_{0}').append(\"<li>\" + $(this).attr(\"option_name\") + \"</li>\"); }} " +
                " else {{ $('#selected_list_{0}').append(\"<li>and \" + (selected - 4) + \" more...</li>\"); return false; }} " +
                " labelCount++; }}); }} " +
                "$('#list_{0}').change(function (event) {{ e{0}_populateSelectList(); }}); " +
                "$('#list_{0}').ready(function (event) {{ e{0}_populateSelectList(); }}); " +
                "</script>", "ethn");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ethn", script);

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            list = new NameValueCollection();
            list.Add("nonhispanic", "Non-Hispanic/Latino");
            list.Add("hispanic", "Hispanic/Latino");
            list.Add("unknown", "Unknown");

            CECWebSrv = new CECHarmPublicService();

            EnsureChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            ctrl_list_count.InnerText = GetSelectedOptions().Length.ToString();

            base.OnPreRender(e);
        }

        protected Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control c in root.Controls)
            {
                if (c.ID == id)
                    return c;

                Control t = FindControlRecursive(c, id);

                if (t != null && t.ID == id)
                    return t;
            }
            return null;
        }

        public string[] GetSelectedOptions()
        {
            System.Collections.ArrayList tlist =
                new ArrayList();

            foreach (string s in list.AllKeys)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, String.Format("{0}_{1}", this.ID, s)) as CheckBox);

                if ((cb != null) && cb.Checked)
                    tlist.Add(s);
            }

            return (string[])tlist.ToArray(typeof(string));
        }

        public void ClearSelectedOptions()
        {
            foreach (String ck_id in list.Keys)
            {
                System.Web.UI.WebControls.CheckBox cb =
                    (FindControlRecursive(this, String.Format("{0}_{1}", this.ID, ck_id)) as CheckBox);

                if (cb != null)
                    cb.Checked = false;
            }
        }

        public string GetOptionLabel(string key)
        {
            return list[key];
        }
    }


    public class CECFilteringOptions : System.Web.UI.WebControls.WebControl
    {
        private System.Collections.ArrayList categories;
        private System.Collections.ArrayList nodes;
        private System.Collections.ArrayList ck_nodes;

        #region Constructor

        public CECFilteringOptions()
            : base("div") { }
        #endregion

        #region Properties

        public int RootCategoryID
        {
            get
            {
                if (ViewState["rootCategory"] == null)
                    RootCategoryID = 0;

                return (int)ViewState["rootCategory"];
            }
            set
            {
                ViewState["rootCategory"] = value;
            }
        }

        public bool RenderChildrenVertically
        {
            get
            {
                if (ViewState["renderChildrenStacked"] == null)
                    RenderChildrenVertically = true;

                return (bool)ViewState["renderChildrenStacked"];
            }
            set
            {
                ViewState["renderChildrenStacked"] = value;
            }
        }

        public bool RenderAsDropDown
        {
            get
            {
                if (ViewState["renderAsDropDown"] == null)
                    RenderAsDropDown = false;

                return (bool)ViewState["renderAsDropDown"];
            }
            set
            {
                ViewState["renderAsDropDown"] = value;
            }
        }

        /// <summary>
        /// get the security token for the given user
        /// </summary>
        protected SecurityToken UserToken
        {
            get
            {
                if (this.Page.Session["UserSecurityToken"] == null)
                    return new SecurityToken();
                else
                    return (SecurityToken)this.Page.Session["UserSecurityToken"];
            }
        }
        #endregion

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "Execution")]
        protected override void CreateChildControls()
        {
            try
            {
                cec_publicservice.CECHarmPublicService ps =
                    new CECHarmPublicService();

                System.Data.DataTable tbl = ps.GetWebFilterFields(UserToken, RootCategoryID).Tables["tbl_web_filter_fields"];

                if (RenderAsDropDown)
                    CreateDropDownList(tbl);
                else
                {
                    System.Web.UI.HtmlControls.HtmlGenericControl h3 =
                        new HtmlGenericControl();

                    System.Web.UI.HtmlControls.HtmlGenericControl lst =
                        new HtmlGenericControl();

                    foreach (DataRow dr in tbl.Rows)
                    {
                        if (h3.ID != String.Format("{0}", dr["category_id"]))
                        {
                            System.Web.UI.HtmlControls.HtmlGenericControl dv =
                                new HtmlGenericControl("div");

                            h3 = new HtmlGenericControl("h3");
                            h3.ID = String.Format("{0}", dr["category_id"]);
                            h3.InnerText = dr["category_name"].ToString();

                            // allows category state toggling
                            //h3.Attributes["onClick"] = String.Format("javascript:toggleCategoryState('{0}');return false;", dr["category_id"]);

                            // allows element focus
                            h3.Attributes["tabindex"] = "0";

                            dv.Controls.Add(h3);

                            lst = new HtmlGenericControl("ul");
                            lst.ID = String.Format("list_{0}", dr["category_id"]);
                            dv.Controls.Add(lst);

                            Controls.Add(dv);
                            categories.Add(h3.ID);


                            /// field to track if section has been expanded/collapsed
                            System.Web.UI.WebControls.HiddenField catSt =
                                new HiddenField();
                            catSt.ID = String.Format("{0}_state", dr["category_id"]);
                            dv.Controls.Add(catSt);

                            System.Web.UI.HtmlControls.HtmlGenericControl selected =
                                new HtmlGenericControl("ul");
                            selected.ID = String.Format("selected_list_{0}", dr["category_id"]);
                            selected.Attributes["class"] = "picked-options";
                            Controls.Add(selected);

                            string script = String.Format("<script type=\"text/javascript\"> " +
                                "function e{0}_populateSelectList() {{ var selected = $('#list_{0} input:checked').length; " +
                                " $('#selected_list_{0}').empty(); var labelCount = 0; " +
                                " $('#list_{0} input:checked').each(function () {{ if (this.id == '') {{ return true; }} " +
                                " if (labelCount < 4) {{ $('#selected_list_{0}').append(\"<li>\" + $(this).attr(\"option_name\") + \"</li>\"); }} " +
                                " else {{ $('#selected_list_{0}').append(\"<li>and \" + (selected - 4) + \" more...</li>\"); return false; }} " +
                                " labelCount++; }}); }} " +
                                "$('#list_{0}').change(function (event) {{ e{0}_populateSelectList(); }}); " +
                                "$('#list_{0}').ready(function (event) {{ e{0}_populateSelectList(); }}); " +
                                "</script>", dr["category_id"]);

                            Page.ClientScript.RegisterStartupScript(this.GetType(), dr["category_id"].ToString(), script);
                        }

                        System.Web.UI.WebControls.CheckBox ck =
                            new CheckBox();
                        ck.InputAttributes["option_name"] = dr["filter_label"].ToString();
                        ck.ID = String.Format("{0}_{1}", dr["category_id"], dr["id"]);

                        System.Web.UI.WebControls.Label ck_lb =
                            new Label();
                        ck_lb.ID = String.Format("{0}_{1}_label", dr["category_id"], dr["id"]);
                        ck_lb.AssociatedControlID = ck.ID;
                        ck_lb.Text = dr["filter_label"].ToString();

                        System.Web.UI.HtmlControls.HtmlGenericControl li =
                            new HtmlGenericControl("li");
                        li.Controls.Add(ck);
                        li.Controls.Add(ck_lb);

                        lst.Controls.Add(li);
                        nodes.Add(ck.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            categories = new ArrayList();
            nodes = new ArrayList();
            ck_nodes = new ArrayList();

            ClientIDMode = ClientIDMode.Static;

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (string s in categories)
            {
                System.Web.UI.Control dv = FindControlRecursive(this, String.Format("{0}_list", s));

                if (!RenderChildrenVertically)
                    (dv as HtmlGenericControl).Attributes["class"] = "wf_renderBlocks";

                for (int _i = 0; _i < nodes.Count; _i++)
                {
                    if (!nodes[_i].ToString().Contains(String.Format("{0}_", s)))
                        continue;

                    System.Web.UI.Control ck = FindControlRecursive(dv, nodes[_i].ToString());

                    if ((ck is CheckBox) && ck_nodes.Contains(ck.ID))
                        (ck as CheckBox).Checked = true;

                    // uncomment to have expanded sections when a child checkbox is checked
                    //if ((ck is CheckBox) && (ck as CheckBox).Checked)
                    //{
                    //    System.Web.UI.Control h3 = FindControlRecursive(this, s);
                    //    (h3 as HtmlGenericControl).Attributes["class"] += " active";
                    //}
                }
            }
        }

        protected Control FindControlRecursive(Control root, string id)
        {
            if (root == null)
                return null;

            if (root.ID == id)
                return root;

            foreach (Control c in root.Controls)
            {
                if (c.ID == id)
                    return c;

                Control t = FindControlRecursive(c, id);

                if (t != null && t.ID == id)
                    return t;
            }
            return null;
        }

        protected void CreateDropDownList(DataTable dt_options)
        {
            if (dt_options.Rows.Count == 0)
                return;

            System.Web.UI.HtmlControls.HtmlGenericControl list =
                new HtmlGenericControl("ul");

            int current_category = -1;
            for (int i = 0; i < dt_options.Rows.Count; i++)
            {

                if (current_category != (int)dt_options.Rows[i]["category_id"])
                {
                    current_category = (int)dt_options.Rows[i]["category_id"];
                    string control_id = String.Format("{0}{1}", ID, current_category);

                    System.Web.UI.HtmlControls.HtmlGenericControl container =
                        new HtmlGenericControl("div");
                    container.ID = control_id;
                    container.Attributes["class"] += "dropdown filter-component btn-group";
                    Controls.Add(container);

                    System.Web.UI.HtmlControls.HtmlGenericControl specimen_count =
                        new HtmlGenericControl("span");
                    specimen_count.Attributes["class"] = "badge";
                    specimen_count.ID = String.Format("{0}_list_count", control_id);
                    specimen_count.InnerText = "0";

                    System.Web.UI.HtmlControls.HtmlGenericControl btnShow =
                        new HtmlGenericControl("button");
                    btnShow.ID = String.Format("{0}_dropdown", control_id);
                    btnShow.Attributes["class"] = "btn btn-default dropdown-toggle";
                    btnShow.Attributes["data-toggle"] = "dropdown";
                    btnShow.Attributes["aria-haspopup"] = "true";
                    btnShow.Attributes["aria-expanded"] = "false";
                    btnShow.Attributes["type"] = "button";
                    btnShow.Controls.Add(new LiteralControl(String.Format("{0}&nbsp;", dt_options.Rows[i]["category_name"])));
                    btnShow.Controls.Add(specimen_count);
                    container.Controls.Add(btnShow);

                    System.Web.UI.HtmlControls.HtmlGenericControl div_i =
                        new HtmlGenericControl("div");
                    div_i.ID = String.Format("{0}_options_list", control_id);
                    div_i.Attributes["class"] = "dropdown-menu filter-component-dropdown";
                    container.Controls.Add(div_i);
                    div_i.Controls.Add(new LiteralControl(String.Format("<h4>{0}</h4>", dt_options.Rows[i]["category_name"])));

                    System.Web.UI.HtmlControls.HtmlGenericControl btnClose =
                            new HtmlGenericControl("button");
                    btnClose.ID = String.Format("{0}_type_close_btn", control_id);
                    btnClose.Attributes["class"] = "btn btn-primary pull-right";
                    btnClose.Attributes["type"] = "button";
                    btnClose.InnerHtml = "X";
                    div_i.Controls.Add(btnClose);

                    //div_i.Controls.Add(new LiteralControl("<hr />"));

                    list =
                        new HtmlGenericControl("ul");
                    div_i.Controls.Add(list);

                    System.Web.UI.HtmlControls.HtmlGenericControl selected =
                        new HtmlGenericControl("ul");
                    selected.ID = String.Format("{0}_selected_list", control_id);
                    selected.Attributes["class"] = "picked-options";
                    Controls.Add(selected);

                    string script = String.Format("<script type=\"text/javascript\"> " +
                        "function {0}_populateSelectList() {{ var selected = $('#{0}_options_list input:checked').length; $('#{0}_list_count').text(selected); " +
                        " $('#{0}_selected_list').empty(); var labelCount = 0; " +
                        " $('#{0}_options_list input:checked').each(function () {{ if (this.id == 'cohort_all') {{ return true; }} " +
                        " if (labelCount < 4) {{ $('#{0}_selected_list').append(\"<li>\" + $(this).attr(\"option_name\") + \"</li>\"); }} " +
                        " else {{ $('#{0}_selected_list').append(\"<li>and \" + (selected - 4) + \" more...</li>\"); return false; }} " +
                        " labelCount++; }}); }} " +
                        "$('#{0}_options_list').click(function (event) {{ if ($('#{0}_dropdown').attr('aria-expanded') == 'true' && event.target.nodeName != 'BUTTON') {{ event.stopPropagation(); }} }}); " +
                        "$('#{0}_options_list').change(function (event) {{ {0}_populateSelectList(); }}); " +
                        "$('#{0}_options_list').ready(function (event) {{ {0}_populateSelectList(); }}); " +
                        "</script>", control_id);

                    Page.ClientScript.RegisterStartupScript(this.GetType(), control_id, script);
                }

                // populate list of specimen types
                //            ck_lb.ID = String.Format("{0}_{1}_label", dr["category_id"], dr["id"]);

                System.Web.UI.WebControls.CheckBox cb =
                    new CheckBox();
                cb.ID = String.Format("{0}_{1}", dt_options.Rows[i]["category_id"], dt_options.Rows[i]["id"]);
                cb.InputAttributes["option_name"] = dt_options.Rows[i]["filter_label"].ToString();

                System.Web.UI.HtmlControls.HtmlGenericControl sp =
                    new HtmlGenericControl("span");
                sp.Attributes["class"] = "filter-component-input";
                sp.Controls.Add(cb);

                System.Web.UI.HtmlControls.HtmlGenericControl lb =
                    new HtmlGenericControl("label");
                lb.Attributes["for"] = cb.ID;
                lb.ID = String.Format("{0}_label", cb.ID);
                lb.Controls.Add(sp);
                lb.Controls.Add(new LiteralControl(dt_options.Rows[i]["filter_label"].ToString()));

                System.Web.UI.HtmlControls.HtmlGenericControl li =
                    new HtmlGenericControl("li");
                li.Controls.Add(lb);
                list.Controls.Add(li);

                nodes.Add(cb.ID);
            }
        }

        public CheckBox[] GetCheckedBoxes()
        {
            EnsureChildControls();

            System.Collections.ArrayList lst =
                new ArrayList();

            foreach (string n in nodes)
            {
                System.Web.UI.Control ck = FindControlRecursive(this, n);
                if ((ck is CheckBox) && (ck as CheckBox).Checked)
                    lst.Add((ck as CheckBox));
            }

            return (CheckBox[])lst.ToArray(typeof(CheckBox));
        }

        public string GetCheckBoxLabel(string checkboxID)
        {
            EnsureChildControls();

            System.Web.UI.Control lb = FindControlRecursive(this, String.Format("{0}_label", checkboxID));
            if ((lb != null) && (lb is Label))
                return (lb as Label).Text;
            else
                return string.Empty;
        }

        public void ClearAllCheckboxes()
        {
            EnsureChildControls();

            ck_nodes = new ArrayList();
            
            CheckBox[] ck_list = GetCheckedBoxes();
            for (int _i = 0; _i < ck_list.Length; _i++)
            {
                System.Web.UI.WebControls.CheckBox ck = (CheckBox)FindControlRecursive(this, ck_list[_i].ID);
                ck.Checked = false;
            }

            foreach (string s in categories)
            {
                System.Web.UI.HtmlControls.HtmlGenericControl h3 = (HtmlGenericControl)FindControlRecursive(this, s);
                if (h3 != null)
                {
                    if ((h3.Attributes["class"] != null) && h3.Attributes["class"].Contains("active"))
                        h3.Attributes["class"] = h3.Attributes["class"].Replace("active", string.Empty);
                }
            }
        }

        public void SetActiveCheckBoxes(string[] ckIDs)
        {
            EnsureChildControls();

            foreach (string s in ckIDs)
                ck_nodes.Add(s);
        }
    }

    public class CECPage : System.Web.UI.Page
    {
        private cec_publicservice.CECHarmPublicService ps;

        #region Properties

        /// <summary>
        /// get the cec harmoney public web service
        /// </summary>
        protected CECHarmPublicService CECWebSrv
        {
            get
            {
                return ps;
            }
        }

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

        protected void RegisterJSAlert(string text)
        { 
            string literalStr =
                    String.Format("<div class='modal' tabindex='-1' id='alertModal' role='alertdialog' aria-labeledby='alertTitle' aria-describedby='alertMsg'><div class='modal-dialog' role=\"document\"><div class='modal-content'><div class='modal-header'><h4 id='alertTitle' class='modal-title'>Alert</h4></div> " +
                    "<div id='alertMsg' class='modal-body bg-warning' aria-atomic='true'>{0}</div><div class='modal-footer'><div class='pull-right'><button type='button' id='modalAlertClose' class='btn btn-default' data-dismiss='modal'>Close</button></div></div><div class='modal-footer'></div></div></div></div>", text);

            ClientScript.RegisterClientScriptBlock(GetType(), "alert", literalStr + " <script type='text/javascript'>$('#alertModal').modal({backdrop:'static', keyboard:true, show:true}); $('#modalAlertClose').focus();</script>");
        }

        protected void RegisterJSError(string text)
        {
            string literalStr =
                    String.Format("<div class='modal' tabindex='-1' id='alertModal' role='alertdialog' aria-labeledby='alertTitle' aria-describedby='alertMsg'><div class='modal-dialog' role=\"document\"><div class='modal-content'><div class='modal-header'><h4 id='alertTitle' class='modal-title'>Alert</h4></div> " +
                    "<div id='alertMsg' class='modal-body bg-danger' aria-atomic='true'>{0}<div class='pull-right'><button type='button' id='modalAlertClose' class='btn btn-default' data-dismiss='modal'>Close</button></div></div><div class='modal-footer'></div></div></div></div>", text);

            ClientScript.RegisterClientScriptBlock(GetType(), "error", literalStr + " <script type='text/javascript'>$('#alertModal').modal({backdrop:'static', keyboard:true, show:true}); $('#modalAlertClose').focus();</script>");
        }

        protected override void OnInit(EventArgs e)
        {
            MaintainScrollPositionOnPostBack = true;
            
            if (ps == null)
                ps = new CECHarmPublicService();

            Error +=
                new EventHandler(OnPageError);

            using(cec_publicservice.CECInputFormService websrv = new CECInputFormService())
            {
                if (!websrv.ValidateSecurityToken(UserToken))
                    FormsAuthentication.SignOut();
            }
            
            base.OnInit(e);
        }

        protected void OnPageError(object sender, EventArgs e)
        {
            Session["PageException"] = Server.GetLastError();

            Response.Redirect("/error.aspx", true);
        }

        protected Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control c in root.Controls)
            {
                if (c.ID == id)
                    return c;

                Control t = FindControlRecursive(c, id);

                if (t != null && t.ID == id) 
                    return t;
            }
            return null;
        }

        protected bool IsFieldDescriptionEnabled(string dataField)
        {
            return (bool)CECWebSrv.IsFieldDescriptionEnabled(UserToken, dataField);
        }

        protected Control FieldDescriptionShowButton(string dataField)
        {
            string field = dataField;
            if (field.Contains("."))
                field = field.Split('.')[1];

            System.Web.UI.HtmlControls.HtmlAnchor lk =
                new HtmlAnchor();

            lk.InnerHtml = "&nbsp;";
            lk.HRef = "#";
            lk.Attributes.Add("class", "icon_modal");
            lk.Attributes.Add("data-toggle", "modal");
            lk.Attributes.Add("data-target", String.Format("#{0}Modal", field));

            return lk;
        }

        protected Control FieldDescriptionModalDialog(string dataField)
        {
            string field = dataField;
            if (field.Contains("."))
                field = field.Split('.')[1];

            string literalStr =
                String.Format("<div class='modal fade' id='{0}Modal'><div class='modal-dialog'><div class='modal-content'><div class='modal-header'> " +
                "<h4 class='modal-title'>Field Description</h4></div> " +
                "<div class='modal-body'>{1}</div><div class='modal-footer'><button type='button' class='btn btn-default' data-dismiss='modal'>Close</button></div></div></div></div>",
                    field, CECWebSrv.GetDataFieldDescription(UserToken, dataField));

            System.Web.UI.LiteralControl lc =
                new LiteralControl(literalStr);

            return lc;
        }

        protected void LogError(string text)
        {
            string logName = String.Format("messages_{0}.log", DateTime.Today.ToString("dd-MM-yyyy"));

            helper h = new helper();
            h.WriteToLog(text, MapPath(String.Format("/tmp/{0}", logName)));
        }

        protected void LogError(string text, Exception ex)
        {
            string logName = String.Format("messages_{0}.log", DateTime.Today.ToString("dd-MM-yyyy"));
            string fullText = String.Format("{1}{0}{2}{0}", Environment.NewLine, text, ex.Message);
            fullText += ex.StackTrace + Environment.NewLine + Environment.NewLine;

            helper h = new helper();
            h.WriteToLog(MapPath(String.Format("/tmp/{0}", logName)), fullText);
        }
    }

    public class helper
    {
        public void WriteToLog(string path, string text)
        {
            if (text == string.Empty)
                text = "writting nothing";

            using (System.IO.StreamWriter log = new System.IO.StreamWriter(path, true))
                log.WriteLine(text);
        }

        #region Public Static

        /// <summary>
        /// check if string is null, string.empty or whitespace (non-printing characters)
        /// </summary>
        public static bool IsStringEmptyWhiteSpace(string str)
        {
            // trim spacing from beginning and end of string
            if (str != null)
                str = str.Trim();

            if (String.IsNullOrEmpty(str))
                return true;

            // carriage return, bell character, or new line
            if (Regex.IsMatch(str, @"^[\a\r\n]+"))
                return true;

            return false;
        }

        public static bool IsLogicalBoolQuestion(object raw)
        {
            bool isTrue = false;

            if (IsNumerical(raw))
            {
                if (int.Parse(raw.ToString()) <= 1 && int.Parse(raw.ToString()) >= 0)
                    isTrue = true;
            }
            else if (raw is bool)
                isTrue = true;
            else
            {
                switch (raw.ToString().ToLower())
                {
                    case "yes":
                        isTrue = true;
                        break;
                    case "no":
                        isTrue = true;
                        break;
                }
            }

            return isTrue;
        }

        public static bool IsLogicalTrue(object raw)
        {
            bool isTrue = false;

            if ((IsNumerical(raw)) && int.Parse(raw.ToString()) > 0)
                isTrue = true;
            else if ((raw is bool) && (bool)raw)
                isTrue = true;
            else
            {
                switch (raw.ToString().ToLower())
                {
                    case "yes":
                        isTrue = true;
                        break;
                }
            }

            return isTrue;
        }

        public static bool IsNumerical(object raw)
        {
            string t = raw.ToString();
            if (IsStringEmptyWhiteSpace(t))
                return false;

            int val;
            if (int.TryParse(raw.ToString(), out val))
                return true;
            else
                return false;

            if (Regex.IsMatch(t, @"\D+"))
                return false;
            else
                return true;
        }

        public static bool IsEmailAddress(string email)
        {
            if (IsStringEmptyWhiteSpace(email))
                return false;

            if (Regex.IsMatch(email, @"\S+@\w+\.[a-z]{2,3}"))
                return true;
            else
                return false;
        }

        public static bool IsPhoneNumber(string number)
        {
            // phone numbers on contact us can be blank. 
            //  create a new override if I need to distingish its use
            if (IsStringEmptyWhiteSpace(number))
                return true;

            if (Regex.IsMatch(number, @"[^\+\(\)\-\.\d ext]", RegexOptions.IgnoreCase))
                return false;
            else
                return true;
        }

        public static string FormatCount(int count)
        {
            ///TODO: perhaps a localized object can handle this better
            string sc = count.ToString();

            /// is the string too short
            if (sc.Length == 3)
                return sc;

            for (int _p = count.ToString().Length; _p >= 0; _p= _p - 3)
            {
                if (_p == count.ToString().Length || _p == 0)
                    continue;

                    sc = sc.Insert(_p, ",");
            }

            return sc;
        }

        public static string ComputeSHA256(string rawStr)
        {
            byte[] braw = Encoding.ASCII.GetBytes(rawStr);
            byte[] boutput = (new SHA256Managed()).ComputeHash(braw);

            string hashed = string.Empty;
            foreach (byte b in boutput)
                hashed += b.ToString("x2");

            return hashed;
        }

        public static string GetCSSClassForFileExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            string cssClass = string.Empty;
            FileInfo fi = new FileInfo(fileName);
            switch (fi.Extension)
            {
                case ".doc":
                    cssClass = "link-docx";
                    break;
                case ".docx":
                    cssClass = "link-docx";
                    break;
                case ".pdf":
                    cssClass = "link-pdf";
                    break;
                case ".xls":
                    cssClass = "link-excel";
                    break;
                case ".xlsx":
                    cssClass = "link-excel";
                    break;
            }

            return cssClass;
        }

        internal static SecurityToken CreateTemporaryToken()
        {
            SecurityToken tok = new SecurityToken();
            tok.userid = 0;
            tok.session = "NEW";

            return tok;
        }

        /// <summary>
        /// strip html tags from string
        /// </summary>
        public static string StripHTML(string raw)
        {
            System.Text.RegularExpressions.Regex reg =
                new Regex(@"<.*?>");

            return reg.Replace(raw, string.Empty);
        }

        /// <summary>
        /// html encode string
        /// </summary>
        public static string HTMLEncode(string raw)
        {
            string _medium = raw.Replace("'", "&apos;");
            _medium = _medium.Replace("’", "&apos;");
            _medium = _medium.Replace("<", "&lt;");
            _medium = _medium.Replace(">", "&gt;");
            _medium = _medium.Replace(" & ", " &amp; ");
            _medium = _medium.Replace("\"", "&quot;");
            _medium = _medium.Replace("“", "&quot;");
            _medium = _medium.Replace("”", "&quot;");
            _medium = _medium.Replace("!", "&#33;");
            _medium = _medium.Replace("$", "&#38;");
            _medium = _medium.Replace("(", "&#40;");
            _medium = _medium.Replace(")", "&#41;");
            _medium = _medium.Replace("*", "&#42;");

            return _medium;
        }

        /// <summary>
        /// html decode string
        /// </summary>
        public static string HTMLDecode(string raw)
        {
            string _medium = raw.Replace("&lt;", "<");
            _medium = _medium.Replace("&gt;", ">");
            _medium = _medium.Replace("&amp;", "&");
            _medium = _medium.Replace("&apos;", "'");
            _medium = _medium.Replace("&#39;", "'");
            _medium = _medium.Replace("&quot;", "\"");
            _medium = _medium.Replace("&#33;", "!");
            _medium = _medium.Replace("&#38;", "$");
            _medium = _medium.Replace("&#40;", "(");
            _medium = _medium.Replace("&#41;", ")");
            _medium = _medium.Replace("&#42;", "*");

            return _medium;
        }

        /// <summary>
        /// sterilize text for database use
        /// </summary>
        public static string SterilizeDBText(string raw)
        {
            string _medium = raw.Replace("'", "''");

            return _medium;
        }
        #endregion
    }

    internal sealed class CECLoginAttemptsCollection : System.Collections.Specialized.NameObjectCollectionBase
    {
        private int[] IndexesOf(string email)
        {
            ArrayList list = new ArrayList();

            string[] keys = BaseGetAllKeys();
            for (int _i = 0; _i < keys.Length; _i++)
            {
                if (keys[_i].ToLower() == email.ToLower())
                    list.Add(_i);
            }

            return (int[])list.ToArray(typeof(int));
        }
        
        public void Add(string email, DateTime attemptTime)
        {
            this.BaseAdd(email, attemptTime);
        }
        
        public void DeleteAttempts(string email)
        {
            int[] positions = IndexesOf(email);
            foreach (int pos in positions)
            {
                BaseRemoveAt(pos);
            }
        }

        public int Attempts(string email)
        {
            int _c = 0;
            foreach (string key in BaseGetAllKeys())
            {
                if (key.ToLower() == email.ToLower())
                    _c++;
            }

            return _c;
        }

        public DateTime GetFirstAttemptTime(string email)
        {
            int[] attempts = IndexesOf(email);
            if (attempts.Length > 0)
                return (DateTime)BaseGet(attempts[0]);
            else
                return DateTime.MinValue;
        }

        #region Operators

        //public int operator +(int val1, int val2)
        //{
        //    return val1 + val2;
        //}
        #endregion
    }

    public sealed class CECMembershipProvider : System.Web.Security.MembershipProvider
    {
        private static CECLoginAttemptsCollection loginAttempts = new CECLoginAttemptsCollection();

        private cec_publicservice.CECInputFormService ps;

        private int _lockoutAttempts;
        private int _lockoutAttemptReset;
        private int _passwordMaxDaysValid;
        private int _passwordMinLength;
        private int _passwordHistoryCount;

        private string _passwordSalt;
        private string _passwordComplexityRegEx;

        #region Properties

        internal SecurityToken UserToken
        {
            get;
            set;
        }

        public override string ApplicationName
        {
            get;
            set;
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _lockoutAttempts; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _passwordMinLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _lockoutAttemptReset; }
        }
        
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordComplexityRegEx; }
        }
        #endregion

        private string ComputeSHA256(string rawStr)
        {
            byte[] braw = Encoding.ASCII.GetBytes(rawStr + _passwordSalt);
            byte[] boutput = (new SHA256Managed()).ComputeHash(braw);

            string hashed = string.Empty;
            foreach (byte b in boutput)
                hashed += b.ToString("x2");

            return hashed;
        }

        public bool ValidatePasswordStrength(string passcode)
        {
            bool strong = false;

            /// meets minimum length
            if (passcode.Length < _passwordMinLength)
                throw new MembershipPasswordException(String.Format("password length must be at least {0}", _passwordMinLength));

            string[] history = ps.GetUserPasswordHistory(UserToken);
            if(Array.IndexOf(history, passcode) != -1) 
                throw new MembershipPasswordException("password was previously used, please select a new password");
             
            /// meets complexity requirements
            if (!Regex.IsMatch(passcode, _passwordComplexityRegEx))
                throw new MembershipPasswordException("password is not complex enough");

            strong = true;
            return strong;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            SecurityToken sec = new SecurityToken();
            sec.userid = 0;
            sec.session = "NEW";      

            ps = new CECInputFormService();

            /// checks the configuration object for the necessary account policies
            ///  but these can be overridden below when the database is queried for
            ///  policies
            if (config["maxInvalidPasswordAttempts"] != null)
                _lockoutAttempts = int.Parse(config["maxInvalidPasswordAttempts"]);

            if (config["passwordAttemptWindow"] != null)
                _lockoutAttemptReset = int.Parse(config["passwordAttemptWindow"]);

            if (config["passwordComplexity"] != null)
                _passwordComplexityRegEx = config["passwordComplexity"];

            if (config["passwordHistory"] != null)
                _passwordHistoryCount = int.Parse(config["passwordHistory"]);

            if (config["passwordLength"] != null)
                _passwordMinLength = int.Parse(config["passwordLength"]);

            if (config["passwordMaximumDaysValid"] != null)
                _passwordMaxDaysValid = int.Parse(config["passwordMaximumDaysValid"]);

            DataTable dt_policies = ps.GetUserPolicies(sec);
            for (int _i = 0; _i < dt_policies.Rows.Count; _i++)
            {
                switch (dt_policies.Rows[_i]["policy"].ToString())
                {
                    case "maxInvalidPasswordAttempts":
                        _lockoutAttempts = int.Parse(dt_policies.Rows[_i]["configuration"].ToString());
                        break;
                    case "passwordAttemptWindow":
                        _lockoutAttemptReset = int.Parse(dt_policies.Rows[_i]["configuration"].ToString());
                        break;
                    case "passwordComplexity":
                        _passwordComplexityRegEx = dt_policies.Rows[_i]["configuration"].ToString();
                        break;
                    case "passwordHistory":
                        _passwordHistoryCount = int.Parse(dt_policies.Rows[_i]["configuration"].ToString());
                        break;
                    case "passwordLength":
                        _passwordMinLength = int.Parse(dt_policies.Rows[_i]["configuration"].ToString());
                        break;
                    case "passwordMaximumDaysValid":
                        _passwordMaxDaysValid = int.Parse(dt_policies.Rows[_i]["configuration"].ToString());
                        break;
                    case "passwordSalt":
                        _passwordSalt = dt_policies.Rows[_i]["configuration"].ToString();
                        break;
                }
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
                throw new ArgumentNullException("providerUserKey");
            else if (!(providerUserKey is int))
                throw new ArgumentException("ProviderUserKey must be an integer correspond to user id");

            UserData ud = ps.GetUserInformationByUserID(UserToken, (int)providerUserKey);

            if (userIsOnline)
            {
                ud.last_login = DateTime.Now;

                SecurityToken sec = new SecurityToken();
                sec.userid = 0;
                sec.session = "NEW";
                ps.SetUserSecurityAttributes(sec, ud);
            }

            return new MembershipUser(this.Name, ud.email, ud.userid, ud.email, string.Empty, string.Empty, true, ud.account_lockout, DateTime.MinValue, ud.last_login, ud.last_login, ud.password_change_date, ud.account_lockout_date);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");

            UserData ud = ps.GetUserInformationByUsername(UserToken, username);

            if (userIsOnline)
            {
                ud.last_login = DateTime.Now;

                SecurityToken sec = new SecurityToken();
                sec.userid = 0;
                sec.session = "NEW";
                ps.SetUserSecurityAttributes(sec, ud);
            }

            return new MembershipUser(this.Name, ud.email, ud.userid, ud.email, string.Empty, string.Empty, true, ud.account_lockout, DateTime.MinValue, ud.last_login, ud.last_login, ud.password_change_date, ud.account_lockout_date);
        }

        public override int GetNumberOfUsersOnline()
        {
            return 1;
        }

        public bool ChangePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException("ChangePassword requires username, newpassword arguments to be specified");

            try
            {
                // validate strength
                ValidatePasswordStrength(newPassword);

                ps.SetUserPassword(UserToken, UserToken.userid, ComputeSHA256(newPassword));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            string _passMedium = string.Empty;
            string[] asciiRanges = new string[] { "48:57", "65:90", "97:122" };
            Random ran = new Random(DateTime.Now.Millisecond);
            for (int _i = 0; _i < _passwordMinLength; _i++)
            {

                int asciiIndex = ran.Next(0, 3);

                int min = int.Parse(asciiRanges[asciiIndex].Split(':')[0]);
                int max = int.Parse(asciiRanges[asciiIndex].Split(':')[1]);
                int charPos = ran.Next(min, max);

                _passMedium += (char)charPos;
            }

            string password = _passMedium;

            DataRow[] dr_users;
            using (DataTable dt_users = ps.GetUsers(helper.CreateTemporaryToken(), "uid, username, email"))
            {
                dr_users = dt_users.Select(String.Format("email='{0}'", username));
            }

            foreach (DataRow user in dr_users)
            {
                UserData ud = ps.GetUserInformationByUserID(helper.CreateTemporaryToken(), (int)user["uid"]);

                SecurityToken std = new SecurityToken();
                std.session = "NEW";
                std.userid = ud.userid;
                std.email = ud.email;


                ps.SetUserPassword(std, ud.userid, ComputeSHA256(password));

                if (dr_users.Length == 1)
                {
                    ud.password_reset_required = true;
                    ps.SetUserSecurityAttributes(std, ud);
                }
            }

            return password;
        }

        public bool ValidateUser(string username, string password, out SecurityToken sec)
        {
            sec = new SecurityToken();
            sec.userid = 0;
            sec.session = "NEW";         
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Error: Username and password cannot be blank");

            UserData ud = ps.GetUserInformationByUsername(username);

            // increment login attempt counter for user
            loginAttempts.Add(username, DateTime.Now);
            // reached logout attempts
            if (loginAttempts.Attempts(username) > _lockoutAttempts)
            {
                // are the login attempts within the login attempt window,
                // if so, lock the account, otherwise, reset the attempts for user
                TimeSpan elapsed = DateTime.Now - loginAttempts.GetFirstAttemptTime(username);
                if (elapsed.TotalMinutes < _lockoutAttemptReset)
                {
                    ps.LockUserAccount(sec, ud.userid);

                    throw new AccountLockedOutException(username);
                }
                else
                {
                    loginAttempts.DeleteAttempts(username);
                    loginAttempts.Add(username, DateTime.Now);
                }
            }

            password = ComputeSHA256(password);
            string sessionID = ps.ProcessUserLogin(username, password);

            sec = ps.GetSecurityToken(ud.userid, sessionID);

            ud = ps.GetUserInformationByUserID(sec, ud.userid);
            ud.last_login = DateTime.Today;

            /// now that the user has been fully authenticated, check account conditions
            /// for any policy violations
            ///  -password expiration
            TimeSpan password_days = DateTime.Today - ud.password_change_date;
            if (password_days.TotalDays >= _passwordMaxDaysValid)
                ud.password_expired = true;
            ps.SetUserSecurityAttributes(sec, ud);

            OnValidatingPassword(new ValidatePasswordEventArgs(username, password, false));

            return true;
        }

        #region Not Implemented

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();

            //if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword) /*|| string.IsNullOrWhiteSpace(oldPassword))
            //    throw new ArgumentNullException("ChangePassword requires username, oldpassword, newpassword arguments to be specified");

            //try
            //{
            //    // test credentials for valid login data
            //    //string sessionID = CECWebSrvProcessUserLogin(username, ComputeSHA256(oldPassword));
            //    //SecurityTicket.session = sessionID;

            //    // validate strength
            //    //ValidatePasswordStrength(newPassword);

            //    // write password to database
            //    byte[] passcode = Encoding.Unicode.GetBytes(ComputeSHA256(newPassword));
            //    //CECWebSrvSetUserPassword(SecurityTicket, passcode);

            //    return false;
            //}
            //catch
            //{
            //    return false;
            //}
        }
        public override bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();

            //if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            //    throw new ArgumentNullException("username and password arguments cannot be null");

            //UserData ud = CECWebSrvGetUserInformation(username);

            //// increment login attempt counter for user
            //loginAttempts.Add(username, DateTime.Now);
            //// reached logout attempts
            //if (loginAttempts.Attempts(username) > _lockoutAttempts)
            //{
            //    // are the login attempts within the login attempt window,
            //    // if so, lock the account, otherwise, reset the attempts for user
            //    TimeSpan elapsed = DateTime.Now - loginAttempts.GetFirstAttemptTime(username);
            //    if (elapsed.TotalMinutes < _lockoutAttemptReset)
            //    {
            //        ud.account_lockout = true;
            //        ud.account_lockout_date = DateTime.Now;

            //        CECWebSrvSetUserSecurityAttributes((new SecurityToken() { session = "NEW" }), ud);

            //        throw new AccountLockedOutException(username);
            //    }
            //    else
            //    {
            //        loginAttempts.DeleteAttempts(username);
            //        loginAttempts.Add(username, DateTime.Now);
            //    }
            //}

            //password = ComputeSHA256(password);
            //string sessionID = CECWebSrvProcessUserLogin(username, password);

            ////if (SecurityTicket == null)
            ////    SecurityTicket = new SecurityToken();
            //SecurityToken sec = new SecurityToken();
            //sec.email = ud.email;
            //sec.session = sessionID;
            //sec.userid = ud.userid;

            ///// now that the user has been fully authenticated, check account conditions
            ///// for any policy violations
            /////  -password expiration
            //TimeSpan password_days = DateTime.Today - ud.password_change_date;
            //if (password_days.TotalDays >= _passwordMaxDaysValid)
            //    ud.password_expired = true;
            //CECWebSrvSetUserSecurityAttributes(sec, ud);

            //OnValidatingPassword(new ValidatePasswordEventArgs(username, password, false));

            //return true;
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}