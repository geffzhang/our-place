using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ADESCOMUTILS;

namespace ADESCOM.Controllers
{
    public class InicioController : Controller
    {
        private string MensajeError = "";

        [Logger]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "none")]
        //[OutputCache(NoStore = false, Duration = 3600, VaryByParam = "none")]
        public ActionResult Index()
        {

            var sesion = Session["InfoUser"];
            if (sesion == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            int CantResidencias = DireccionesProxy.GetCantActivas();
            ViewBag.Residencias = CantResidencias;

            ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            int CantResidentes = ResidentesProxy.GetCantActivos();
            ViewBag.Residentes = CantResidentes;

            ADESCOMBUSINESS.GlobalBusiness GlobalProxy = new ADESCOMBUSINESS.GlobalBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            int _Recaudado = GlobalProxy.GetRecaudado();
            ViewBag.Recaudado = _Recaudado;

            GlobalProxy = new ADESCOMBUSINESS.GlobalBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            decimal Saldo = GlobalProxy.GetSaldoCuentas();
            ViewBag.SaldoCuentas = Saldo.ToString("C");

            return View();
        }

        [OutputCache(Duration = 3600)]
        [Logger]
        public ActionResult UnderConstruction()
        {
            return PartialView();
        }

        [OutputCache(Duration = 3600, VaryByParam = "Mensaje")]
        [Logger]
        public ActionResult SesionExpired(string Mensaje)
        {
            ViewBag.Mensaje = Mensaje;
            return View();
        }

        [OutputCache(Duration = 3600, VaryByParam = "Mensaje")]
        [Logger]
        public ActionResult Error(string Mensaje)
        {
            ViewBag.Mensaje = Mensaje;
            return View();
        }

        [OutputCache(Duration = 3600)]
        [Logger]
        public ActionResult Error400()
        {
            return PartialView();
        }

        [OutputCache(Duration = 3600)]
        [Logger]
        public ActionResult Error401()
        {
            return PartialView();
        }

        [OutputCache(Duration = 3600)]
        [Logger]
        public ActionResult Error403()
        {
            return PartialView();
        }

        [OutputCache(Duration = 3600)]
        [Logger]
        public ActionResult Error404()
        {
            return PartialView();
        }

        [OutputCache(Duration = 3600)]
        [Logger]
        public ActionResult Error500()
        {
            return PartialView();
        }

        [OutputCache(Duration = 3600)]
        [Logger]
        public ActionResult Error503()
        {
            return PartialView();
        }

        [OutputCache(Duration = 3600)]
        [Logger]
        public ActionResult Success()
        {
            return PartialView();
        }

        [Logger]
        public JsonResult GetBadgesInfo()
        {
            ADESCOMBUSINESS.Models.UPD_Badges BadgesInfo = new ADESCOMBUSINESS.Models.UPD_Badges();
            BadgesInfo = ADESCOMBUSINESS.GlobalBusiness.GetBadgesInfo((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            return Json(BadgesInfo, JsonRequestBehavior.AllowGet);
        }

        [Logger]
        public JsonResult GetIngresosVsEgresos()
        {
            ADESCOMBUSINESS.Models.IngresosVsEgresos MesesIngresos = new ADESCOMBUSINESS.Models.IngresosVsEgresos();
            MesesIngresos = ADESCOMBUSINESS.GlobalBusiness.GetIngresosVsEgresos((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            return Json(MesesIngresos, JsonRequestBehavior.AllowGet);
        }
    }
}