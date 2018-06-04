using System.Web.Mvc;

namespace ADESCOM.Areas.Egresos
{
    public class EgresosAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Egresos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Egresos_default",
                "Egresos/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}