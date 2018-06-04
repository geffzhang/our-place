using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness CON_CuentasProxy;
        protected ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas OBJCON_Cuentas = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas();

        public ActionResult Index()
        {
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

        [Logger]
        public JsonResult GetIngresosVsEgresos()
        {
            ADESCOMBUSINESS.Models.IngresosVsEgresos MesesIngresos = new ADESCOMBUSINESS.Models.IngresosVsEgresos();
            MesesIngresos = ADESCOMBUSINESS.GlobalBusiness.GetIngresosVsEgresos((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            return Json(MesesIngresos, JsonRequestBehavior.AllowGet);
        }
    }
}