using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cec_publicservice
{

    public class SecurityToken
    {
        public int userid;

        public string email;
        public string session;
        public int access_level;

        public bool TokenSet
        {
            get
            {
                if (session != null && session != string.Empty)
                    return true;
                else
                    return false;
            }
        }
    }
}