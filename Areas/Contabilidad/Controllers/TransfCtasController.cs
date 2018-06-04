using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Contabilidad.Controllers
{
    public class TransfCtasController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_TransfCtasBusiness CON_TransfCtasProxy;
        protected ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas OBJVwCON_TransfCtas = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas();

        public ActionResult Index()
        {
            try { this.CON_TransfCtasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_TransfCtasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas>();
            return View(Lista);
        }

        public ActionResult RefreshData()
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas>();
            return View(Lista);
        }

        public ActionResult Detalle(int Transferencia_ID)
        {
            try { this.CON_TransfCtasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_TransfCtasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJVwCON_TransfCtas = CON_TransfCtasProxy.GetViewByID(Transferencia_ID);

            if (OBJVwCON_TransfCtas == null)
            {
                return HttpNotFound();
            }

            return View(OBJVwCON_TransfCtas);
        }

        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas());
        }

        public ActionResult RefreshDataSearchList(FormCollection form)
        {
            int CuentaOri_ID = 0;
            int CuentaDest_ID = 0;
            DateTime? FechaDesde = null;
            DateTime? FechaHasta = null;
            bool IncluirAnulados = false;
            bool IncluirCerrados = false;

            if (!string.IsNullOrEmpty(form["FindCuentaOri_ID"]))
                CuentaOri_ID = Convert.ToInt32(form["FindCuentaOri_ID"]);

            if (!string.IsNullOrEmpty(form["FindCuentaDest_ID"]))
                CuentaDest_ID = Convert.ToInt32(form["FindCuentaDest_ID"]);

            if (!string.IsNullOrEmpty(form["FindFechaDesde"]))
                FechaDesde = Convert.ToDateTime(form["FindFechaDesde"]);

            if (!string.IsNullOrEmpty(form["FindFechaHasta"]))
                FechaHasta = Convert.ToDateTime(form["FindFechaHasta"]);

            if (Convert.ToBoolean(form["FindAnulados"]))
                IncluirAnulados = true;

            if (Convert.ToBoolean(form["FindCerrados"]))
                IncluirCerrados = true;

            try { this.CON_TransfCtasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_TransfCtasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas>();
            Lista = CON_TransfCtasProxy.GetByFilter(CuentaOri_ID, CuentaDest_ID, FechaDesde, FechaHasta, IncluirAnulados, IncluirCerrados);
            return View("RefreshData", Lista);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_TransfCtas Registro)
        {
            try { this.CON_TransfCtasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_TransfCtasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.TRF_Mensaje))
            {
                ModelState.AddModelError("TRF_Mensaje", "Dato requerido");
            }

            if (Registro.TRF_Referencia == null)
                Registro.TRF_Referencia = String.Empty;

            if (Registro.CuentaOri_ID == 0)
            {
                ModelState.AddModelError("CuentaOri_ID", "Dato requerido");
            }

            if (Registro.CuentaDest_ID == 0)
            {
                ModelState.AddModelError("CuentaDest_ID", "Dato requerido");
            }

            if (Registro.TRF_Monto == 0)
            {
                ModelState.AddModelError("TRF_Monto", "Dato requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CON_TransfCtasProxy.InsTransferencia(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            return View(Registro);
        }
    }
}