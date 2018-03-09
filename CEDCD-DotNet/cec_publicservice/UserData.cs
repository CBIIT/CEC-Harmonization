using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cec_publicservice
{

    public class UserData
    {
        public int userid;
        public int access_level;
        public int? cohort_id;

        public string user_name;
        public string email;
        public string display_name;

        public bool account_lockout;
        public bool help_shown;
        public bool password_expired;
        public bool password_reset_required;

        public DateTime last_login;
        public DateTime account_lockout_date;
        public DateTime password_change_date;
    }
}