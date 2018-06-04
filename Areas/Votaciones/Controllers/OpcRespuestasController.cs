using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Votaciones.Controllers
{
    public class OpcRespuestasController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness RespuestasProxy;
        protected ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_OpcRespuestas OBJVOT_OpcRespuestas = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_OpcRespuestas();

        [Logger]
        public ActionResult CrearRespuesta(int Pregunta_ID, int Votacion_ID)
        {
            ViewBag.Pregunta_ID = Pregunta_ID;
            ViewBag.Votacion_ID = Votacion_ID;
            return View(new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_OpcRespuestas());
        }

        [HttpPost]
        [Logger]
        public ActionResult CrearRespuesta(FormCollection Form)
        {
            try { this.RespuestasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_OpcRespuestas Registro = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_OpcRespuestas();

            if (String.IsNullOrEmpty(Form["RES_OpcRespuesta"]))
            {
                ModelState.AddModelError("RES_OpcRespuesta", "Campo requerido");
            }

            Registro.RES_OpcRespuesta = Form["RES_OpcRespuesta"];
            Registro.Pregunta_ID = Convert.ToInt32(Form["Pregunta_ID"]);
            Registro.Votacion_ID = Convert.ToInt32(Form["Votacion_ID"]);

            if (ModelState.IsValid)
            {
                try
                {
                    RespuestasProxy.Crear(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.Votacion_ID = Registro.Votacion_ID;
            ViewBag.Pregunta_ID = Registro.Pregunta_ID;

            return View(Registro);
        }

        [Logger]
        public ActionResult EditarRespuesta(int Respuesta_ID)
        {
            try { this.RespuestasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJVOT_OpcRespuestas = RespuestasProxy.GetByID(Respuesta_ID);

            if (OBJVOT_OpcRespuestas == null)
            {
                return HttpNotFound();
            }

            return View(OBJVOT_OpcRespuestas);
        }

        [HttpPost]
        [Logger]
        public ActionResult EditarRespuesta(ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_OpcRespuestas Registro)
        {
            try { this.RespuestasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.RES_OpcRespuesta))
            {
                ModelState.AddModelError("RES_OpcRespuesta", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = RespuestasProxy.Editar(Registro);
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

        [Logger]
        public ActionResult EliminarRespuesta(int Respuesta_ID)
        {
            try { this.RespuestasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJVOT_OpcRespuestas = RespuestasProxy.GetByID(Respuesta_ID);

            if (OBJVOT_OpcRespuestas == null)
            {
                return HttpNotFound();
            }
            return View(OBJVOT_OpcRespuestas);
        }

        [HttpPost, ActionName("EliminarRespuesta")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmed(int Respuesta_ID)
        {
            try { this.RespuestasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            try
            {
                OBJVOT_OpcRespuestas = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_OpcRespuestas();
                OBJVOT_OpcRespuestas = RespuestasProxy.GetByID(Respuesta_ID);
                bool Status = RespuestasProxy.Eliminar(Respuesta_ID);
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
            return View(OBJVOT_OpcRespuestas);
        }
    }
}