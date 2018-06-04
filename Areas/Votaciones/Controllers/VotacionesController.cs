using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;


namespace ADESCOM.Areas.Votaciones.Controllers
{
    public class VotacionesController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness VotacionesProxy;
        protected ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones OBJVOT_Votaciones = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones();

        [Logger]
        public ActionResult Index()
        {
            //try { this.VotacionesProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            //catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Votaciones> Lista = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Votaciones>();
            //Lista = VotacionesProxy.GetAll();
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");

            return View(Lista);
        }

        public ActionResult RefreshDataSearchList(FormCollection Form)
        {
            String searchTitulo = Form["FindTitulo"];
            int Estatus = Convert.ToInt32(Form["FindEstatus"]);

            try { this.VotacionesProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Votaciones> Lista = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Votaciones>();
            Lista = VotacionesProxy.GetByFilters(searchTitulo, Estatus);

            return View("RefreshData", Lista);
        }

        [Logger]
        public ActionResult Resultados(int Votacion_ID)
        {
            try { this.VotacionesProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones Votacion = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones();
            Votacion = VotacionesProxy.GetByID(Votacion_ID);

            if (Votacion.VOT_Estatus.Equals("CAN") || Votacion.VOT_Estatus.Equals("ACT"))
            {
                @ViewBag.Error = "No hay resultados";
                return View("NoResults");
            }
            else
            {
                //Llenar ResResultadoCompuesto
                ADESCOMBUSINESS.Areas.Votaciones.Models.ResResultadoCompuesto Resultado = new ADESCOMBUSINESS.Areas.Votaciones.Models.ResResultadoCompuesto();

                Resultado.Votacion = Votacion;
                Resultado.Preguntas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.ResPreguntaCompuesta>();

                //Traer preguntas de la votación
                List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas> ListaPreguntas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas>();
                ListaPreguntas = ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness.GetByVotacion_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Votacion_ID);

                //bool primera = true;
                int participaron = 0;
                foreach (ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas Pregunta in ListaPreguntas)
                {
                    List<ADESCOMBUSINESS.Areas.Votaciones.Models.ResRespuestaCompuesta> RespuestasCompuestas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.ResRespuestaCompuesta>();
                    RespuestasCompuestas = ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness.GetResultsByPregunta_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Votacion_ID, Pregunta.Pregunta_ID);

                    ADESCOMBUSINESS.Areas.Votaciones.Models.ResPreguntaCompuesta PreguntaCompuesta = new ADESCOMBUSINESS.Areas.Votaciones.Models.ResPreguntaCompuesta();
                    PreguntaCompuesta.Pregunta = Pregunta;
                    PreguntaCompuesta.Respuestas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.ResRespuestaCompuesta>();
                    foreach (ADESCOMBUSINESS.Areas.Votaciones.Models.ResRespuestaCompuesta RespuestaCompuesta in RespuestasCompuestas)
                    {
                        PreguntaCompuesta.Respuestas.Add(RespuestaCompuesta);
                        //if (primera == true)
                        //participaron++;
                        participaron += RespuestaCompuesta.Votos;
                    }

                    Resultado.Preguntas.Add(PreguntaCompuesta);
                    //primera = false;
                }

                if (Votacion.VOT_Padron <= 0)
                    ViewBag.Participacion = "0 (0.00%)";
                else
                {
                    decimal porcPart = decimal.Round((decimal)(participaron * 100) / (decimal)Votacion.VOT_Padron, 2);
                    ViewBag.Participacion = participaron.ToString() + " (" + porcPart.ToString() + "%)";
                }

                return View("Resultados", Resultado);
            }
        }

        [Logger]
        public ActionResult ResultadoDetalles(int Votacion_ID, int Pregunta_ID)
        {
            List<ADESCOMBUSINESS.Areas.Votaciones.Models.DetallesResultados> DetalleResultados = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.DetallesResultados>();
            DetalleResultados = ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness.GetDetallesResultado((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Votacion_ID, Pregunta_ID);
            return View(DetalleResultados);
        }

        [Logger]
        public ActionResult Crear()
        {
            ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones Votacion = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones();
            Votacion.VOT_Inicio = DateTime.Now.AddDays(1);
            Votacion.VOT_Fin = DateTime.Now.AddDays(10);
            return View(Votacion);
        }

        [Logger]
        public ActionResult Editar(int Votacion_ID)
        {
            try { this.VotacionesProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Votaciones.Models.VotacionesCompuesto VotacionCompuesta = new ADESCOMBUSINESS.Areas.Votaciones.Models.VotacionesCompuesto();
            ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones Votacion = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones();
            Votacion = VotacionesProxy.GetByID(Votacion_ID);

            List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas> ListaPreguntas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas>();
            ListaPreguntas = ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness.GetByVotacion_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Votacion_ID);

            VotacionCompuesta.Votacion = Votacion;
            VotacionCompuesta.Preguntas = ListaPreguntas;

            if (VotacionCompuesta.Votacion.VOT_Estatus.Equals("ACT"))
            {
                return View("Editar", VotacionCompuesta);
            }
            else
            {
                String Status = String.Empty;
                switch (VotacionCompuesta.Votacion.VOT_Estatus)
                {
                    /*Nunca se va a dar este caso
                     * case "ACT":
                        Status = "No Iniciada";
                        break;*/
                    case "INI":
                        Status = "Iniciada";
                        break;
                    case "FIN":
                        Status = "Finalizada";
                        break;
                    case "CAN":
                        Status = "Cancelada";
                        break;
                }
                ViewBag.Estatus = Status;
                return View("Detalle", VotacionCompuesta);
            }
        }

        [Logger]
        public ActionResult RefreshPreguntasVotacion(int Votacion_ID)
        {
            List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas> ListaPreguntas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas>();
            ListaPreguntas = ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness.GetByVotacion_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Votacion_ID);
            ViewBag.Votacion_ID = Votacion_ID;
            return View(ListaPreguntas);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones Registro)
        {
            try { this.VotacionesProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.VOT_Titulo))
            {
                ModelState.AddModelError("VOT_Titulo", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.VOT_Inicio = DateTime.Now;
                    Registro.VOT_Fin = DateTime.Now.AddMonths(1);
                    Registro.VOT_Estatus = "ACT";
                    OBJVOT_Votaciones = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones();
                    OBJVOT_Votaciones = VotacionesProxy.Crear(Registro);

                    if (OBJVOT_Votaciones.Votacion_ID == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    ADESCOMBUSINESS.Areas.Votaciones.Models.VotacionesCompuesto VotacionCompuesta = new ADESCOMBUSINESS.Areas.Votaciones.Models.VotacionesCompuesto();
                    VotacionCompuesta.Votacion = Registro;

                    //No queremos un modal, pues estamos en el content page
                    // ViewBag.Error = "OK";

                    return View("Editar", VotacionCompuesta);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }
            return View(Registro);
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(FormCollection Form)
        {
            bool error = false;
            int Votacion_ID = 0;
            bool TipoOperacion = false;
            try { this.VotacionesProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            Votacion_ID = Convert.ToInt32(Form["Votacion.Votacion_ID"]);
            TipoOperacion = Convert.ToBoolean(Form["tipo_operacion"]);

            ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones Registro = new ADESCOMBUSINESS.Areas.Votaciones.Models.VOT_Votaciones();
            Registro = VotacionesProxy.GetByID(Votacion_ID);

            List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas> ListaPreguntas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas>();
            ListaPreguntas = ADESCOMBUSINESS.Areas.Votaciones.Methods.PreguntasBusiness.GetByVotacion_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Votacion_ID);

            if (String.IsNullOrEmpty(Form["Votacion.VOT_Titulo"]))
            {
                ModelState.AddModelError("Votacion.VOT_Titulo", "Campo requerido");
            }

            if (String.IsNullOrEmpty(Form["Votacion.VOT_Fin"]))
            {
                ModelState.AddModelError("Votacion.VOT_Fin", "Campo requerido");
            }
            else
            {
                Registro.VOT_Fin = Convert.ToDateTime(Form["Votacion.VOT_Fin"]);
            }

            Registro.VOT_Titulo = Form["Votacion.VOT_Titulo"];
            Registro.VOT_SolComentarios = Form["Votacion.VOT_SolComentarios"].Contains("true") ? true : false;

            //GuardarCambios e Iniciar
            if (TipoOperacion == true)
            {
                if (Registro.VOT_Fin.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("Votacion.VOT_Fin", "La fecha de fin debe ser mayor o igual a la fecha de hoy");
                }

                //Validar Preguntas y respuestas
                if (ListaPreguntas == null || ListaPreguntas.Count <= 0)
                {
                    error = true;
                    ViewBag.Error = "Se debe incluir al menos una pregunta con respuestas";
                }
                else
                {
                    bool tienenResp = true;
                    foreach (ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_Preguntas preg in ListaPreguntas)
                    {
                        List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_OpcRespuestas> ListaRepuestas = new List<ADESCOMBUSINESS.Areas.Votaciones.Models.VwVOT_OpcRespuestas>();
                        ListaRepuestas = ADESCOMBUSINESS.Areas.Votaciones.Methods.OpcRespuestasBusiness.GetByPregunta_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], preg.Pregunta_ID);
                        if (ListaRepuestas == null || ListaRepuestas.Count <= 0)
                        {
                            tienenResp = false;
                            break;
                        }
                    }

                    if (!tienenResp)
                    {
                        error = true;
                        ViewBag.Error = "Todas las preguntas deben tener repuestas";
                    }
                }
            }

            if (ModelState.IsValid && error == false)
            {
                try
                {
                    if (TipoOperacion == true)
                    {
                        //GuardarCambios e Iniciar
                        bool Status = VotacionesProxy.GuardarVotacion(Registro, true);
                        if (!Status)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        //Sólo Guardar
                        bool Status = VotacionesProxy.GuardarVotacion(Registro, false);
                        if (!Status)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                    }

                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ADESCOMBUSINESS.Areas.Votaciones.Models.VotacionesCompuesto VotacionCompuesta = new ADESCOMBUSINESS.Areas.Votaciones.Models.VotacionesCompuesto();
            VotacionCompuesta.Votacion = Registro;
            VotacionCompuesta.Preguntas = ListaPreguntas;

            return View(VotacionCompuesta);
        }

        [Logger]
        public ActionResult Cancelar(int Votacion_ID)
        {
            ViewBag.Votacion_ID = Votacion_ID;
            return View();
        }

        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult CancelarConfirmed(int Votacion_ID)
        {
            ViewBag.Votacion_ID = Votacion_ID;
            try { this.VotacionesProxy = new ADESCOMBUSINESS.Areas.Votaciones.Methods.VotacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJVOT_Votaciones = VotacionesProxy.GetByID(Votacion_ID);

            if (OBJVOT_Votaciones == null)
            {
                return HttpNotFound();
            }

            try
            {
                bool Status = VotacionesProxy.CancelarVotacion(Votacion_ID);
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

            return View("Cancelar");
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetEstatuses()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "Finalizadas" }); //'FIN'
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "No Iniciada" }); //'ACT'
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "Iniciada" }); //'INI'
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 3, Description = "Cancelada" }); //'CAN'
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 4, Description = "Todas" });
            return SolEstatus_ID;
        }
    }
}