using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Contabilidad.Controllers
{
    public class ConceptosContablesController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness CON_ConceptosContablesProxy;
        protected ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_ConceptosContables OBJCON_ConceptosContables = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_ConceptosContables();


        public ActionResult Index()
        {
            try { this.CON_ConceptosContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_ConceptosContables> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_ConceptosContables>();
            Lista = CON_ConceptosContablesProxy.GetActive(0);
            return View(Lista);
        }

        public ActionResult RefreshData()
        {
            try { this.CON_ConceptosContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_ConceptosContables> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_ConceptosContables>();
            Lista = CON_ConceptosContablesProxy.GetActive(0);
            return View(Lista);
        }

        public ActionResult SearchList()
        {
            try { this.CON_ConceptosContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_ConceptosContables> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_ConceptosContables>();
            Lista = CON_ConceptosContablesProxy.GetActive(0);
            return View(Lista);
        }

        public ActionResult Detalle(int ConceptoContable_ID)
        {
            try { this.CON_ConceptosContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCON_ConceptosContables = CON_ConceptosContablesProxy.GetByID(ConceptoContable_ID);

            if (OBJCON_ConceptosContables == null)
            {
                return HttpNotFound();
            }

            ViewBag.CCO_TipoMovimiento = new SelectList(GetTiposMovimiento(), "Option", "Description", OBJCON_ConceptosContables.CCO_TipoMovimiento);
            return View(OBJCON_ConceptosContables);
        }

        public ActionResult Crear()
        {
            ViewBag.CCO_TipoMovimiento = new SelectList(GetTiposMovimiento(), "Option", "Description");
            return View();
        }

        public ActionResult Editar(int ConceptoContable_ID)
        {
            try { this.CON_ConceptosContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCON_ConceptosContables = CON_ConceptosContablesProxy.GetByID(ConceptoContable_ID);

            if (OBJCON_ConceptosContables == null)
            {
                return HttpNotFound();
            }

            ViewBag.CCO_TipoMovimiento = new SelectList(GetTiposMovimiento(), "Option", "Description", OBJCON_ConceptosContables.CCO_TipoMovimiento);
            return View(OBJCON_ConceptosContables);
        }

        [HttpPost]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_ConceptosContables Registro)
        {
            try { this.CON_ConceptosContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            if (ModelState.IsValid)
            {
                try
                {
                    OBJCON_ConceptosContables = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_ConceptosContables();
                    OBJCON_ConceptosContables = CON_ConceptosContablesProxy.Crear(Registro);
                    if (OBJCON_ConceptosContables.ConceptoContable_ID == 0)
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
            ViewBag.CCO_TipoMovimiento = new SelectList(GetTiposMovimiento(), "Option", "Description", Registro.CCO_TipoMovimiento);
            return View(Registro);
        }

        [HttpPost]
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_ConceptosContables Registro)
        {
            try { this.CON_ConceptosContablesProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_ConceptosContablesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = CON_ConceptosContablesProxy.Editar(Registro);
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
            ViewBag.CCO_TipoMovimiento = new SelectList(GetTiposMovimiento(), "Option", "Description", Registro.CCO_TipoMovimiento);
            return View(Registro);
        }

        private List<ADESCOM.Models.FixedOption> GetTiposMovimiento()
        {
            List<ADESCOM.Models.FixedOption> TiposMovimiento = new List<ADESCOM.Models.FixedOption>();
            TiposMovimiento.Add(new ADESCOM.Models.FixedOption() { Option = "D", Description = "DEBITO -" });
            TiposMovimiento.Add(new ADESCOM.Models.FixedOption() { Option = "C", Description = "CREDITO +" });
            return TiposMovimiento;
        }
    }
}