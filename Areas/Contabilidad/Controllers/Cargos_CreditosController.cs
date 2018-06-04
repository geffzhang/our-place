using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Contabilidad.Controllers
{
    public class Cargos_CreditosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness CON_MovContablesProxy;

        public ActionResult Index()
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables>();
            return View(Lista);
        }

        public ActionResult RefreshData()
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables>();
            return View(Lista);
        }

        public ActionResult RefreshDataSearchList(FormCollection form)
        {
            int Cuenta_ID = 0;
            DateTime? FechaDesde = null;
            DateTime? FechaHasta = null;
            bool IncluirAnulados = false;
            bool IncluirCerrados = false;

            if (!string.IsNullOrEmpty(form["FindCuenta_ID"]))
                Cuenta_ID = Convert.ToInt32(form["FindCuenta_ID"]);

            if (!string.IsNullOrEmpty(form["FindFechaDesde"]))
                FechaDesde = Convert.ToDateTime(form["FindFechaDesde"]);

            if (!string.IsNullOrEmpty(form["FindFechaHasta"]))
                FechaHasta = Convert.ToDateTime(form["FindFechaHasta"]);

            if (Convert.ToBoolean(form["FindAnulados"]))
                IncluirAnulados = true;

            if (Convert.ToBoolean(form["FindCerrados"]))
                IncluirCerrados = true;

            try { this.CON_MovContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables>();
            Lista = CON_MovContablesProxy.GetByFilter(Cuenta_ID, FechaDesde, FechaHasta, IncluirAnulados, IncluirCerrados);
            return View("RefreshData", Lista);
        }

        public ActionResult Detalle(int MovContable_ID)
        {
            try { this.CON_MovContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables VwMovContable = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables();
            VwMovContable = CON_MovContablesProxy.GetViewByID(MovContable_ID);

            if (VwMovContable == null)
            {
                return HttpNotFound();
            }

            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness CON_CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas VwCuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas();
            VwCuenta = CON_CuentasProxy.GetViewByID(VwMovContable.Cuenta_ID);

            ADESCOMBUSINESS.Areas.Contabilidad.Models.MovContableCompuesto MovContableCompuesto = new ADESCOMBUSINESS.Areas.Contabilidad.Models.MovContableCompuesto();
            MovContableCompuesto.MovContable = VwMovContable;
            MovContableCompuesto.Cuenta = VwCuenta;

            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables> MovContablesLigados = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables>();
            MovContablesLigados = CON_MovContablesProxy.GetMovLigados((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], VwMovContable.MovContable_ID, VwMovContable.MCO_MovGrupo);
            ViewBag.MovLigados = MovContablesLigados;

            return View(MovContableCompuesto);
        }

        [Logger]
        public ActionResult CancelarMovimiento(int MovContable_ID)
        {
            ViewBag.MovContable_ID = MovContable_ID;
            return View();
        }

        [HttpPost, ActionName("CancelarMovimiento")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult CancelarMovimientoConfirmed(int MovContable_ID)
        {
            ViewBag.MovContable_ID = MovContable_ID;
            try { this.CON_MovContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            try
            {
                CON_MovContablesProxy.CancelarMovContable(MovContable_ID);
                ViewBag.Error = "OK";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View("CancelarMovimiento");
        }

        [Logger]
        public JsonResult GetConceptoDetails(int ConceptoContable_ID)
        {
            ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_ConceptosContables ConceptoContableInfo = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_ConceptosContables();

            if (ConceptoContable_ID <= 0)
            {
                return Json(ConceptoContableInfo, JsonRequestBehavior.AllowGet);
            }

            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness ConcContableProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_ConceptosContables OBJConceptoContable = ConcContableProxy.GetByID(ConceptoContable_ID);

            if (OBJConceptoContable != null)
            {
                if (OBJConceptoContable.CCO_TipoMovimiento.Equals("D"))
                    ConceptoContableInfo.CCO_TipoMovimiento = "DEBITO";
                if (OBJConceptoContable.CCO_TipoMovimiento.Equals("C"))
                    ConceptoContableInfo.CCO_TipoMovimiento = "CREDITO";
            }

            return Json(ConceptoContableInfo, JsonRequestBehavior.AllowGet);
        }

        [Logger]
        public ActionResult Crear()
        {
            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness ConcContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ViewBag.ConceptoContable_ID = new SelectList(ConcContablesProxy.GetActive(0), "ConceptoContable_ID", "CCO_Descripcion");

            ADESCOMBUSINESS.Areas.Contabilidad.Models.MovContableCompuesto MovContable = new ADESCOMBUSINESS.Areas.Contabilidad.Models.MovContableCompuesto();
            MovContable.MovContable = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables();
            MovContable.Cuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas();

            return View(MovContable);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Contabilidad.Models.MovContableCompuesto Registro)
        {
            try { this.CON_MovContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (Registro.MovContable.ConceptoContable_ID == 0)
            {
                ModelState.AddModelError("MovContable.ConceptoContable_ID", "Dato requerido");
            }

            if (String.IsNullOrEmpty(Registro.MovContable.MCO_Mensaje))
            {
                ModelState.AddModelError("MovContable.MCO_Mensaje", "Dato requerido");
            }

            if (Registro.MovContable.MCO_Referencia == null)
                Registro.MovContable.MCO_Referencia = String.Empty;

            if (Registro.Cuenta.Cuenta_ID == 0)
            {
                ModelState.AddModelError("Cuenta.Cuenta_ID", "Dato requerido");
            }

            if (Registro.MovContable.MCO_Monto == 0)
            {
                ModelState.AddModelError("MovContable.MCO_Monto", "Dato requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CON_MovContablesProxy.InsMovContable(Registro.MovContable.ConceptoContable_ID, Registro.MovContable.MCO_Mensaje,
                        Registro.MovContable.MCO_Referencia, Registro.Cuenta.Cuenta_ID, Registro.MovContable.MCO_Monto, 0, 0, 0);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness ConcContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ViewBag.ConceptoContable_ID = new SelectList(ConcContablesProxy.GetActive(Registro.MovContable.ConceptoContable_ID), "ConceptoContable_ID", "CCO_Descripcion", Registro.MovContable.ConceptoContable_ID);
            return View(Registro);
        }
    }
}