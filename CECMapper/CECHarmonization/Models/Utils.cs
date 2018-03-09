using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CECHarmonization.Models
{
    public static class Utils
    {
        public static string EditFormat(string popupName)
        {
            return "<button type='button' class='awe-btn' onclick=\"awe.open('" + popupName + "', { params:{ id: .Id } })\"><span class='ico-edit'></span></button>";
        }

        public static string DeleteFormat(string popupName)
        {
            return "<button type='button' class='awe-btn' onclick=\"awe.open('" + popupName + "', { params:{ id: .Id } })\"><span class='ico-del'></span></button>";
        }



    }

    public class DeleteConfirmInput
    {
        public int Id { get; set; }
        public string GridId { get; set; }
        public string Message { get; set; }
    }
}