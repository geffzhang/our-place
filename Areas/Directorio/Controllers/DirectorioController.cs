using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Directorio.Controllers
{
    public class DirectorioController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Directorio.Methods.DirectorioBusiness DIR_DIrectorioProxy;
        protected ADESCOMBUSINESS.Areas.Directorio.Models.DIR_Directorio OBJDIR_Directorio = new ADESCOMBUSINESS.Areas.Directorio.Models.DIR_Directorio();

        [Logger]
        public ActionResult Index()
        {
            try { this.DIR_DIrectorioProxy = new ADESCOMBUSINESS.Areas.Directorio.Methods.DirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Directorio.Models.VwDIR_Directorio> Lista = new List<ADESCOMBUSINESS.Areas.Directorio.Models.VwDIR_Directorio>();
            Lista = DIR_DIrectorioProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.DIR_DIrectorioProxy = new ADESCOMBUSINESS.Areas.Directorio.Methods.DirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Directorio.Models.VwDIR_Directorio> Lista = new List<ADESCOMBUSINESS.Areas.Directorio.Models.VwDIR_Directorio>();
            Lista = DIR_DIrectorioProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            ViewBag.TipoDirectorio_ID = new SelectList(GetTipoDirectorio(0), "TipoDirectorio_ID", "TDI_Descripcion");
            return View(new ADESCOMBUSINESS.Areas.Directorio.Models.DIR_Directorio());
        }

        [Logger]
        public ActionResult Editar(int Directorio_ID)
        {
            try { this.DIR_DIrectorioProxy = new ADESCOMBUSINESS.Areas.Directorio.Methods.DirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJDIR_Directorio = DIR_DIrectorioProxy.GetByID(Directorio_ID);

            if (OBJDIR_Directorio == null)
            {
                return HttpNotFound();
            }

            ViewBag.TipoDirectorio_ID = new SelectList(GetTipoDirectorio(0), "TipoDirectorio_ID", "TDI_Descripcion", OBJDIR_Directorio.TipoDirectorio_ID);
            return View(OBJDIR_Directorio);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Directorio.Models.DIR_Directorio Registro)
        {
            try { this.DIR_DIrectorioProxy = new ADESCOMBUSINESS.Areas.Directorio.Methods.DirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (Registro.TipoDirectorio_ID == 0)
            {
                ModelState.AddModelError("TipoDirectorio_ID", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Registro.DIR_Descripcion))
            {
                ModelState.AddModelError("DIR_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJDIR_Directorio = new ADESCOMBUSINESS.Areas.Directorio.Models.DIR_Directorio();
                    OBJDIR_Directorio = DIR_DIrectorioProxy.Crear(Registro);
                    if (OBJDIR_Directorio.Directorio_ID == 0)
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

            ViewBag.TipoDirectorio_ID = new SelectList(GetTipoDirectorio(0), "TipoDirectorio_ID", "TDI_Descripcion", Registro.TipoDirectorio_ID);
            return View(Registro);
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Directorio.Models.DIR_Directorio Registro)
        {
            try { this.DIR_DIrectorioProxy = new ADESCOMBUSINESS.Areas.Directorio.Methods.DirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (Registro.TipoDirectorio_ID == 0)
            {
                ModelState.AddModelError("TipoDirectorio_ID", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Registro.DIR_Descripcion))
            {
                ModelState.AddModelError("DIR_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = DIR_DIrectorioProxy.Editar(Registro);
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

            ViewBag.TipoDirectorio_ID = new SelectList(GetTipoDirectorio(0), "TipoDirectorio_ID", "TDI_Descripcion", Registro.TipoDirectorio_ID);
            return View(Registro);
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoDirectorio> GetTipoDirectorio(int TipoDirectorio_ID)
        {
            ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoDirectorioBusiness TipoDirectorioProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoDirectorioBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoDirectorio> TDirectorio = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoDirectorio>();
            TDirectorio.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoDirectorio() { TipoDirectorio_ID = 0, TDI_Descripcion = "Elija Una Opcion" });
            TDirectorio.AddRange(TipoDirectorioProxy.GetActive(TipoDirectorio_ID));
            return TDirectorio;
        }
    }
}