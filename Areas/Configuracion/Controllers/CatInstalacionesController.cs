using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class CatInstalacionesController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_InstalacionesBusiness CAT_InstalacionesProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_Instalaciones OBJCAT_Instalaciones = new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_Instalaciones();

        [Logger]
        public ActionResult Index()
        {
            try { this.CAT_InstalacionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_InstalacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_Instalaciones> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_Instalaciones>();
            Lista = CAT_InstalacionesProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.CAT_InstalacionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_InstalacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_Instalaciones> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_Instalaciones>();
            Lista = CAT_InstalacionesProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_Instalaciones());
        }

        [Logger]
        public ActionResult Editar(int Instalacion_ID)
        {
            try { this.CAT_InstalacionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_InstalacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCAT_Instalaciones = CAT_InstalacionesProxy.GetByID(Instalacion_ID);

            if (OBJCAT_Instalaciones == null)
            {
                return HttpNotFound();
            }

            return View(OBJCAT_Instalaciones);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_Instalaciones Registro)
        {
            try { this.CAT_InstalacionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_InstalacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.INS_Descripcion))
            {
                ModelState.AddModelError("INS_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJCAT_Instalaciones = new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_Instalaciones();
                    OBJCAT_Instalaciones = CAT_InstalacionesProxy.Crear(Registro);
                    if (OBJCAT_Instalaciones.Instalacion_ID == 0)
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

        [HttpPost]
        [Logger]
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_Instalaciones Registro)
        {
            try { this.CAT_InstalacionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_InstalacionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.INS_Descripcion))
            {
                ModelState.AddModelError("INS_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = CAT_InstalacionesProxy.Editar(Registro);
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