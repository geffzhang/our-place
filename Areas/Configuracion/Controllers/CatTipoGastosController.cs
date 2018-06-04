using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class CatTipoGastosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness CAT_TipoGastosProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoGastos OBJCAT_TipoGastos = new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoGastos();

        [Logger]
        public ActionResult Index()
        {
            try { this.CAT_TipoGastosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos>();
            Lista = CAT_TipoGastosProxy.GetActive(0);

            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.CAT_TipoGastosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos>();
            Lista = CAT_TipoGastosProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoGastos());
        }

        [Logger]
        public ActionResult Editar(int TipoGasto_ID)
        {
            try { this.CAT_TipoGastosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCAT_TipoGastos = CAT_TipoGastosProxy.GetByID(TipoGasto_ID);

            if (OBJCAT_TipoGastos == null)
            {
                return HttpNotFound();
            }

            return View(OBJCAT_TipoGastos);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoGastos Registro)
        {
            try { this.CAT_TipoGastosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.TGA_Descripcion))
            {
                ModelState.AddModelError("TCA_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJCAT_TipoGastos = new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoGastos();
                    OBJCAT_TipoGastos = CAT_TipoGastosProxy.Crear(Registro);
                    if (OBJCAT_TipoGastos.TipoGasto_ID == 0)
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
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoGastos Registro)
        {
            try { this.CAT_TipoGastosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.TGA_Descripcion))
            {
                ModelState.AddModelError("TGA_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = CAT_TipoGastosProxy.Editar(Registro);
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