using System.Web.Mvc;

namespace ADESCOM.Areas.Directorio
{
    public class DirectorioAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Directorio";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Directorio_default",
                "Directorio/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}