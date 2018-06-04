using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Reservaciones.Controllers
{
    public class ReservacionesController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness EventosProxy;
        protected ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos OBJEVE_Eventos = new ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos();

        [Logger]
        public ActionResult Index()
        {
            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos> Lista = new List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos>();
            Lista = EventosProxy.GetUnauthorized();
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos> Lista = new List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos>();
            Lista = EventosProxy.GetUnauthorized();
            return View(Lista);
        }

        [Logger]
        public ActionResult Editar(int Evento_ID)
        {
            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos VwEvento = new ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos();
            VwEvento = EventosProxy.GetViewByID(Evento_ID);

            if (VwEvento == null)
            {
                return HttpNotFound();
            }

            ViewBag.Fecha = VwEvento.EVE_Fecha.ToString("dd/MMM/yyyy");
            ViewBag.Instalacion = VwEvento.INS_Descripcion;

            return View(VwEvento);
        }

        [Logger]
        public ActionResult GetEventosDelDia(DateTime Fecha, int Instalacion_ID, int Evento_ID)
        {
            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos> Lista = new List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos>();
            try
            {
                Lista = EventosProxy.GetByDateLocation(Fecha, Instalacion_ID, Evento_ID);
            }
            catch (Exception ex)
            {
            }

            return View(Lista);
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos Registro)
        {
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJEVE_Eventos = EventosProxy.GetByID(Registro.Evento_ID);

            if (OBJEVE_Eventos == null)
            {
                return HttpNotFound();
            }

            //Autorizar Residente
            try
            {
                EventosProxy.AutorizarReservacion(OBJEVE_Eventos, CompanyInfo);
                ViewBag.Error = "OK";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Fecha = Registro.EVE_Fecha.ToString("dd/MMM/yyyy");
            ViewBag.Instalacion = Registro.INS_Descripcion;

            return View(Registro);
        }

        public ActionResult Rechazar(int Evento_ID)
        {
            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJEVE_Eventos = EventosProxy.GetByID(Evento_ID);

            if (OBJEVE_Eventos == null)
            {
                return HttpNotFound();
            }

            return View(OBJEVE_Eventos);
        }

        [HttpPost, ActionName("Rechazar")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult RechazarConfirmed(ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos Registro)
        {
            if (String.IsNullOrEmpty(Registro.EVE_MotivoRechazo))
            {
                ModelState.AddModelError("EVE_MotivoRechazo", "Campo requerido");
            }

            if (ModelState.IsValid)
            {
                try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
                catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

                //Rechazar residente
                try
                {
                    ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
                    bool Status = EventosProxy.RechazarReservacion(Registro, CompanyInfo);
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