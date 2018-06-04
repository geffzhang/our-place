using System.Web.Mvc;

namespace ADESCOM.Areas.ImpoExpo
{
    public class ImpoExpoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ImpoExpo";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ImpoExpo_default",
                "ImpoExpo/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}