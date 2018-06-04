using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Eventos.Controllers
{
    public class EventoJson
    {
        public String title { get; set; }
        public String start { get; set; }
        public String allDay { get; set; }
    }

    public class EventosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness EventosProxy;
        protected ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos OBJEVE_Eventos = new ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos();

        [Logger]
        public ActionResult Index()
        {
            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos> Lista = new List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos>();
            DateTime Inicio = DateTime.Parse(DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/01");
            DateTime Fin = DateTime.Parse(DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            Lista = EventosProxy.GetByDate(Inicio, Fin);
            ViewBag.Mes = new SelectList(GetMeses(), "Option", "Description", DateTime.Now.Month);
            ViewBag.Anio = new SelectList(GetAnios(), "Option", "Description", DateTime.Now.Year);

            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshFecha(FormCollection form)
        {
            int Mes = DateTime.Now.Month;
            int Anio = DateTime.Now.Year;

            if (!string.IsNullOrEmpty(form["Mes"]))
                Mes = Convert.ToInt32(form["Mes"]);

            if (!string.IsNullOrEmpty(form["Anio"]))
                Anio = Convert.ToInt32(form["Anio"]);

            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos> Lista = new List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos>();
            DateTime Inicio = DateTime.Parse(Anio + "/" + Mes + "/01");
            DateTime Fin = DateTime.Parse(Anio + "/" + Mes + "/" + DateTime.DaysInMonth(Anio, Mes));
            Lista = EventosProxy.GetByDate(Inicio, Fin);

            return View("RefreshData", Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            ViewBag.Instalacion_ID = new SelectList(GetInstalaciones(0), "Instalacion_ID", "INS_Descripcion");
            ViewBag.Inicio = new SelectList(GetHorarios(), "Option", "Description");
            ViewBag.Fin = new SelectList(GetHorarios(), "Option", "Description");
            return View(new ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos() { EVE_Fecha = DateTime.Now.AddDays(1) });
        }

        [Logger]
        public ActionResult Editar(int Evento_ID)
        {
            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJEVE_Eventos = EventosProxy.GetByID(Evento_ID);

            if (OBJEVE_Eventos == null)
            {
                return HttpNotFound();
            }

            ViewBag.Instalacion_ID = new SelectList(GetInstalaciones(OBJEVE_Eventos.Instalacion_ID), "Instalacion_ID", "INS_Descripcion", OBJEVE_Eventos.Instalacion_ID);
            String Inicio = OBJEVE_Eventos.EVE_Inicio.ToString();
            ViewBag.Inicio = new SelectList(GetHorarios(), "Option", "Description", Inicio.Substring(0, 5));
            String Fin = OBJEVE_Eventos.EVE_Fin.ToString();
            ViewBag.Fin = new SelectList(GetHorarios(), "Option", "Description", Fin.Substring(0, 5));
            return View(OBJEVE_Eventos);
        }

        [Logger]
        public ActionResult Eliminar(int Evento_ID)
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

        [HttpPost]
        [Logger]
        public ActionResult Crear(FormCollection Form)
        {
            ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos Registro = new ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos();

            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Form["Instalacion_ID"]) || Form["Instalacion_ID"] == "0")
            {
                ModelState.AddModelError("Instalacion_ID", "Campo Requerido");
            }
            else
            {
                try { Registro.Instalacion_ID = Convert.ToInt32(Form["Instalacion_ID"]); } catch { ModelState.AddModelError("Instalacion_ID", "Formato Incorrecto"); }
            }

            try
            {
                TimeSpan ts = TimeSpan.Parse(Form["Inicio"].ToString());
                Registro.EVE_Inicio = ts;
            }
            catch
            {
                ModelState.AddModelError("EVE_Inicio", "Formato Incorrecto");
            }

            try
            {
                TimeSpan ts = TimeSpan.Parse(Form["Fin"].ToString());
                Registro.EVE_Fin = ts;
            }
            catch
            {
                ModelState.AddModelError("EVE_Fin", "Formato Incorrecto");
            }

            try
            {
                Registro.EVE_Fecha = Convert.ToDateTime(Form["EVE_Fecha"]);
            }
            catch
            {
                ModelState.AddModelError("EVE_Fecha", "Formato Incorrecto");
            }

            Registro.EVE_Tipo = "MES";
            Registro.EVE_Titulo = Form["EVE_Titulo"];
            Registro.EVE_Detalles = Form["EVE_Detalles"];
            Registro.EVE_Organizador = Form["EVE_Organizador"];

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.EVE_Estatus = "ACT";
                    OBJEVE_Eventos = EventosProxy.Crear(Registro);
                    if (OBJEVE_Eventos.Evento_ID == 0)
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

            ViewBag.Instalacion_ID = new SelectList(GetInstalaciones(OBJEVE_Eventos.Instalacion_ID), "Instalacion_ID", "INS_Descripcion", Registro.Instalacion_ID);
            String Inicio = Registro.EVE_Inicio.ToString();
            ViewBag.Inicio = new SelectList(GetHorarios(), "Option", "Description", Inicio.Substring(0, 5));
            String Fin = Registro.EVE_Fin.ToString();
            ViewBag.Fin = new SelectList(GetHorarios(), "Option", "Description", Fin.Substring(0, 5));

            return View(Registro);
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(FormCollection Form)
        {
            ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos Registro = new ADESCOMBUSINESS.Areas.Eventos.Models.EVE_Eventos();

            try { this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Form["Instalacion_ID"]) || Form["Instalacion_ID"] == "0")
            {
                ModelState.AddModelError("Instalacion_ID", "Campo Requerido");
            }
            else
            {
                try { Registro.Instalacion_ID = Convert.ToInt32(Form["Instalacion_ID"]); } catch { ModelState.AddModelError("Instalacion_ID", "Formato Incorrecto"); }
            }

            try
            {
                TimeSpan ts = TimeSpan.Parse(Form["Inicio"].ToString());
                Registro.EVE_Inicio = ts;
            }
            catch
            {
                ModelState.AddModelError("EVE_Inicio", "Formato Incorrecto");
            }

            try
            {
                TimeSpan ts = TimeSpan.Parse(Form["Fin"].ToString());
                Registro.EVE_Fin = ts;
            }
            catch
            {
                ModelState.AddModelError("EVE_Fin", "Formato Incorrecto");
            }

            try
            {
                Registro.EVE_Fecha = Convert.ToDateTime(Form["EVE_Fecha"]);
            }
            catch
            {
                ModelState.AddModelError("EVE_Fecha", "Formato Incorrecto");
            }

            Registro.Evento_ID = Convert.ToInt32(Form["Evento_ID"]);
            Registro.EVE_Tipo = "MES";
            Registro.EVE_Titulo = Form["EVE_Titulo"];
            Registro.EVE_Detalles = Form["EVE_Detalles"];
            Registro.EVE_Organizador = Form["EVE_Organizador"];

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = EventosProxy.Editar(Registro);
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

            ViewBag.Instalacion_ID = new SelectList(GetInstalaciones(OBJEVE_Eventos.Instalacion_ID), "Instalacion_ID", "INS_Descripcion", Registro.Instalacion_ID);

            ViewBag.Instalacion_ID = new SelectList(GetInstalaciones(OBJEVE_Eventos.Instalacion_ID), "Instalacion_ID", "INS_Descripcion", Registro.Instalacion_ID);
            String Inicio = Registro.EVE_Inicio.ToString();
            ViewBag.Inicio = new SelectList(GetHorarios(), "Option", "Description", Inicio.Substring(0, 5));
            String Fin = Registro.EVE_Fin.ToString();
            ViewBag.Fin = new SelectList(GetHorarios(), "Option", "Description", Fin.Substring(0, 5));

            return View(Registro);
        }

        [HttpPost, ActionName("Eliminar")]
        [Logger]
        public ActionResult ElminarConfirmed(int Evento_ID)
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

        public JsonResult GetEventosDelMes(int mes, int anio)
        {
            this.EventosProxy = new ADESCOMBUSINESS.Areas.Eventos.Methods.EventosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos> Lista = new List<ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos>();
            DateTime Inicio = DateTime.Parse(anio + "/" + mes + "/01");
            DateTime Fin = DateTime.Parse(anio + "/" + mes + "/" + DateTime.DaysInMonth(anio, mes));
            Lista = EventosProxy.GetByDate(Inicio, Fin);

            List<EventoJson> ListaRet = new List<EventoJson>();
            foreach (ADESCOMBUSINESS.Areas.Eventos.Models.VwEVE_Eventos evento in Lista)
            {
                ListaRet.Add(new EventoJson() { title = evento.EVE_Titulo, start = evento.EVE_Fecha.ToString("yyyy-MM-dd HH:mm"), allDay = "false" });
            }

            return Json(ListaRet, JsonRequestBehavior.AllowGet);
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetMeses()
        {
            List<ADESCOM.Models.FixedOption> Meses = new List<ADESCOM.Models.FixedOption>();
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "ENERO" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "FEBRERO" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 3, Description = "MARZO" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 4, Description = "ABRIL" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 5, Description = "MAYO" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 6, Description = "JUNIO" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 7, Description = "JULIO" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 8, Description = "AGOSTO" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 9, Description = "SEPTIEMBRE" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 10, Description = "OCTUBRE" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 11, Description = "NOVIEMBRE" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 12, Description = "DICIEMBRE" });
            return Meses;
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetAnios()
        {
            List<ADESCOM.Models.FixedOption> Meses = new List<ADESCOM.Models.FixedOption>();
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2016, Description = "2016" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2017, Description = "2017" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2018, Description = "2018" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2019, Description = "2019" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2020, Description = "2020" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2021, Description = "2021" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2022, Description = "2022" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2023, Description = "2023" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2024, Description = "2024" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2025, Description = "2025" });
            Meses.Add(new ADESCOM.Models.FixedOption() { Option = 2026, Description = "2026" });
            return Meses;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_Instalaciones> GetInstalaciones(int Instalacion_ID)
        {
            ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_InstalacionesBusiness InstalacionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_InstalacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_Instalaciones> Instalaciones = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_Instalaciones>();
            Instalaciones.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_Instalaciones() { Instalacion_ID = 0, INS_Descripcion = "Elija Una Opcion" });
            Instalaciones.AddRange(InstalacionesProxy.GetActive(Instalacion_ID));
            return Instalaciones;
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetHorarios()
        {
            List<ADESCOM.Models.FixedOption> Horarios = new List<ADESCOM.Models.FixedOption>();
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "00:00", Description = "00:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "00:30", Description = "00:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "01:00", Description = "01:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "01:30", Description = "01:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "02:00", Description = "02:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "02:30", Description = "02:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "03:00", Description = "03:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "03:30", Description = "03:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "04:00", Description = "04:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "04:30", Description = "04:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "05:00", Description = "05:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "05:30", Description = "05:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "06:00", Description = "06:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "06:30", Description = "06:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "07:00", Description = "07:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "07:30", Description = "07:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "08:00", Description = "08:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "08:30", Description = "08:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "09:00", Description = "09:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "09:30", Description = "09:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "10:00", Description = "10:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "10:30", Description = "10:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "11:00", Description = "11:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "11:30", Description = "11:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "12:00", Description = "12:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "12:30", Description = "12:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "13:00", Description = "13:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "13:30", Description = "13:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "14:00", Description = "14:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "14:30", Description = "14:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "15:00", Description = "15:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "15:30", Description = "15:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "16:00", Description = "16:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "16:30", Description = "16:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "17:00", Description = "17:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "17:30", Description = "17:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "18:00", Description = "18:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "18:30", Description = "18:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "19:00", Description = "19:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "19:30", Description = "19:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "20:00", Description = "20:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "20:30", Description = "20:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "21:00", Description = "21:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "21:30", Description = "21:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "22:00", Description = "22:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "22:30", Description = "22:30" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "23:00", Description = "23:00" });
            Horarios.Add(new ADESCOM.Models.FixedOption() { Option = "23:30", Description = "23:30" });
            return Horarios;
        }

    }
}