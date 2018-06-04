using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class CatTipoCargosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness CAT_TipoCargosProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoCargos OBJCAT_TipoCargos = new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoCargos();

        [Logger]
        public ActionResult Index()
        {
            try { this.CAT_TipoCargosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos>();
            Lista = CAT_TipoCargosProxy.GetUserActive(0);

            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.CAT_TipoCargosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos>();
            Lista = CAT_TipoCargosProxy.GetUserActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoCargos());
        }

        [Logger]
        public ActionResult Editar(int TipoCargo_ID)
        {
            try { this.CAT_TipoCargosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCAT_TipoCargos = CAT_TipoCargosProxy.GetByID(TipoCargo_ID);

            if (OBJCAT_TipoCargos == null)
            {
                return HttpNotFound();
            }

            return View(OBJCAT_TipoCargos);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoCargos Registro)
        {
            try { this.CAT_TipoCargosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.TCA_Descripcion))
            {
                ModelState.AddModelError("TCA_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJCAT_TipoCargos = new ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoCargos();
                    OBJCAT_TipoCargos = CAT_TipoCargosProxy.Crear(Registro);
                    if (OBJCAT_TipoCargos.TipoCargo_ID == 0)
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
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.CAT_TipoCargos Registro)
        {
            try { this.CAT_TipoCargosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.TCA_Descripcion))
            {
                ModelState.AddModelError("TCA_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = CAT_TipoCargosProxy.Editar(Registro);
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