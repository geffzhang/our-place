using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Votaciones.Controllers
{
    public class PreguntasController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness PreguntasProxy;
        protected ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas OBJVOT_Preguntas = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas();

        [Logger]
        public ActionResult CrearPregunta(int Votacion_ID)
        {
            ViewBag.Votacion_ID = Votacion_ID;
            return View(new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas(){ Votacion_ID = Votacion_ID});
        }

        [Logger]
        public ActionResult EditarPregunta(int Pregunta_ID)
        {
            try { this.PreguntasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Votaciones.Models.PreguntaCompuesta PreguntaCompuesta = new ADESCOMBUSINESS.Areas.Votaciones.Models.PreguntaCompuesta();
            ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas Pregunta = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas();
            Pregunta = PreguntasProxy.GetByID(Pregunta_ID);

            List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_OpcRespuestas> ListaRespuestas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_OpcRespuestas>();
            ListaRespuestas = ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness.GetByPregunta_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Pregunta_ID);

            PreguntaCompuesta.Pregunta = Pregunta;
            PreguntaCompuesta.Respuestas = ListaRespuestas;

            return View(PreguntaCompuesta);
        }

        [HttpPost]
        [Logger]
        public ActionResult EditarPregunta(FormCollection Form)
        {
            try { this.PreguntasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas Registro = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas();
            if (String.IsNullOrEmpty(Form["Pregunta.PRE_Pregunta"]))
            {
                ModelState.AddModelError("Pregunta.PRE_Pregunta", "Campo requerido");
            }

            Registro.Pregunta_ID = Convert.ToInt32(Form["Pregunta.Pregunta_ID"]);
            Registro.Votacion_ID = Convert.ToInt32(Form["Pregunta.Votacion_ID"]);
            Registro.PRE_Pregunta = Form["Pregunta.PRE_Pregunta"];

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = PreguntasProxy.Editar(Registro);
                    if (!Status)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ViewBag.ErrorEdicion = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorEdicion = ex.Message;
                }
            }

            List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_OpcRespuestas> ListaRespuestas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_OpcRespuestas>();
            ListaRespuestas = ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness.GetByPregunta_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Registro.Pregunta_ID);

            ADESCOMBUSINESS.Areas.Votaciones.Models.PreguntaCompuesta PreguntaCompuesta = new ADESCOMBUSINESS.Areas.Votaciones.Models.PreguntaCompuesta();
            PreguntaCompuesta.Pregunta = Registro;
            PreguntaCompuesta.Respuestas = ListaRespuestas;
            return View(PreguntaCompuesta);
        }

        [HttpPost]
        [Logger]
        public ActionResult CrearPregunta(FormCollection Form)
        {
            try { this.PreguntasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas Registro = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas();
            if (String.IsNullOrEmpty(Form["PRE_Pregunta"]))
            {
                ModelState.AddModelError("PRE_Pregunta", "Campo requerido");
            }

            Registro.PRE_Pregunta = Form["PRE_Pregunta"];
            Registro.Votacion_ID = Convert.ToInt32(Form["Votacion_ID"]);

            if (ModelState.IsValid)
            {
                try
                {
                    PreguntasProxy.Crear(Registro);
                }
                catch(Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    return View(Registro);
                }

                //Exito!!!
                ViewBag.Votacion_ID = Registro.Votacion_ID;
                ADESCOMBUSINESS.Areas.Votaciones.Models.PreguntaCompuesta PreguntaCompuesta = new ADESCOMBUSINESS.Areas.Votaciones.Models.PreguntaCompuesta();
                PreguntaCompuesta.Pregunta = Registro;
                ViewBag.Error = "OK"; //Dispara el click del refresh de las preguntas de la votación
                return View("EditarPregunta", PreguntaCompuesta);
            }
            else
            {
                ViewBag.Votacion_ID = Registro.Votacion_ID;
                return View(Registro);
            }
        }

        [Logger]
        public ActionResult EliminarPregunta(int Pregunta_ID)
        {
            try { this.PreguntasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJVOT_Preguntas = PreguntasProxy.GetByID(Pregunta_ID);

            if (OBJVOT_Preguntas == null)
            {
                return HttpNotFound();
            }
            return View(OBJVOT_Preguntas);
        }

        [HttpPost, ActionName("EliminarPregunta")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmed(int Pregunta_ID)
        {
            try { this.PreguntasProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            try
            {
                OBJVOT_Preguntas = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Preguntas();
                OBJVOT_Preguntas = PreguntasProxy.GetByID(Pregunta_ID);
                bool Status = PreguntasProxy.Eliminar(Pregunta_ID);
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
            return View(OBJVOT_Preguntas);
        }

        [Logger]
        public ActionResult RefreshRespuestasPregunta(int Votacion_ID, int Pregunta_ID)
        {
            List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_OpcRespuestas> ListaRespuestas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_OpcRespuestas>();
            ListaRespuestas = ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness.GetByPregunta_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Pregunta_ID);

            ViewBag.Pregunta_ID = Pregunta_ID;
            ViewBag.Votacion_ID = Votacion_ID;
            return View(ListaRespuestas);
        }
    }
}