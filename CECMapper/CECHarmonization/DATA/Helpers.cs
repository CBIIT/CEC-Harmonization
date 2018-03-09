using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CECHarmonization.DATA
{
    public class Helpers
    {
        public static List<SelectListItem> GetDropDownList<T>(
       string text, string value, string selected) where T : class
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "-Please select-", Value = string.Empty });
            //IQueryable<T> result = Db.Repository<T>();
            //var lisData = (from items in result
            //               select items).AsEnumerable().Select(m => new SelectListItem
            //               {
            //                   Text = (string)m.GetType().GetProperty(text).GetValue(m),
            //                   Value = (string)m.GetType().GetProperty(value).GetValue(m),
            //                   Selected = (selected != "") ? ((string)
            //                     m.GetType().GetProperty(value).GetValue(m) ==
            //                     selected ? true : false) : false,
            //               }).ToList();
            //list.AddRange(lisData);
            return list;
        }


        public class DB_Result
        {
            public object Value { get; set; }
            public int Error { get; set; }
            public string Message { get; set; }

        }
    }
}