using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Ingresos.Controllers
{
    public class AutPagosRealizadosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness PagosRealizadosProxy;
        protected ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados OBJING_PagosRealizados = new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados();

        [Logger]
        public ActionResult Index()
        {
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados> Lista = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados>();
            Lista = PagosRealizadosProxy.GetUnauthorized();
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados> Lista = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados>();
            Lista = PagosRealizadosProxy.GetUnauthorized();
            return View(Lista);
        }

        [Logger]
        public ActionResult VerComprobante(String Comprobante)
        {
            ViewBag.Comprobante = Comprobante;
            return View();
        }

        [Logger]
        public ActionResult Autorizar(int PagoRealizado_ID)
        {
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJING_PagosRealizados = PagosRealizadosProxy.GetByID(PagoRealizado_ID);

            if (OBJING_PagosRealizados == null)
            {
                return HttpNotFound();
            }

            return View(OBJING_PagosRealizados);
        }

        [Logger]
        public ActionResult Rechazar(int PagoRealizado_ID)
        {
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJING_PagosRealizados = PagosRealizadosProxy.GetByID(PagoRealizado_ID);

            if (OBJING_PagosRealizados == null)
            {
                return HttpNotFound();
            }

            return View(OBJING_PagosRealizados);
        }

        [HttpPost, ActionName("Autorizar")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult AutorizarConfirmed(int PagoRealizado_ID)
        {
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJING_PagosRealizados = PagosRealizadosProxy.GetByID(PagoRealizado_ID);

            if (OBJING_PagosRealizados == null)
            {
                return HttpNotFound();
            }

            try
            {
                bool Status = PagosRealizadosProxy.AutorizarPago(OBJING_PagosRealizados, CompanyInfo);
                if (!Status)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.Error = "OK";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(OBJING_PagosRealizados);
        }

        [HttpPost, ActionName("Rechazar")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult RechazarConfirmed(ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados Registro)
        {
            if (String.IsNullOrEmpty(Registro.PRE_MotivoRechazo))
            {
                ModelState.AddModelError("PRE_MotivoRechazo", "Campo requerido");
            }

            if (ModelState.IsValid)
            {
                ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
                try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

                try
                {
                    bool Status = PagosRealizadosProxy.RechazarPago(Registro, CompanyInfo);
                    if (!Status)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

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