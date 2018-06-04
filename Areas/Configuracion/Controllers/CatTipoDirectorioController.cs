using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class CatTipoDirectorioController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoDirectorioBusiness CAT_TipoDirectorioProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoDirectorio OBJCAT_TipoDirectorio = new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoDirectorio();

        [Logger]
        public ActionResult Index()
        {
            try { this.CAT_TipoDirectorioProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoDirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoDirectorio> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoDirectorio>();
            Lista = CAT_TipoDirectorioProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.CAT_TipoDirectorioProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoDirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoDirectorio> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoDirectorio>();
            Lista = CAT_TipoDirectorioProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoDirectorio());
        }

        [Logger]
        public ActionResult Editar(int TipoDirectorio_ID)
        {
            try { this.CAT_TipoDirectorioProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoDirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCAT_TipoDirectorio = CAT_TipoDirectorioProxy.GetByID(TipoDirectorio_ID);

            if (OBJCAT_TipoDirectorio == null)
            {
                return HttpNotFound();
            }

            return View(OBJCAT_TipoDirectorio);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoDirectorio Registro)
        {
            try { this.CAT_TipoDirectorioProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoDirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.TDI_Descripcion))
            {
                ModelState.AddModelError("TDI_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJCAT_TipoDirectorio = new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoDirectorio();
                    OBJCAT_TipoDirectorio = CAT_TipoDirectorioProxy.Crear(Registro);
                    if (OBJCAT_TipoDirectorio.TipoDirectorio_ID == 0)
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
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoDirectorio Registro)
        {
            try { this.CAT_TipoDirectorioProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoDirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.TDI_Descripcion))
            {
                ModelState.AddModelError("TDI_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = CAT_TipoDirectorioProxy.Editar(Registro);
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