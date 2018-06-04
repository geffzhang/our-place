using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace ADESCOM.Helpers
{
    public static class AjaxExtensions
    {
        public static HtmlString GetHml(string Html)
        {
            return new HtmlString(Html.Replace("&lt;", "<").Replace("&#39;", "'").Replace("&gt;", ">"));
        }


    }
}