using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class CatLlaves2Controller : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business TVI_Llave2Proxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave2 OBJTVI_Llave2 = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave2();

        [Logger]
        public ActionResult Index()
        {
            //try { this.TVI_Llave2Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            //catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2>();
            //Lista = TVI_Llave2Proxy.GetActive(0);
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            ViewBag.Llave2_Descripcion = CompanyInfo.DescLlave2;
            ViewBag.Llave2_Etiqueta = CompanyInfo.LabelLlave2;
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");

            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshDataSearchList(FormCollection Form)
        {
            int Estatus = Convert.ToInt32(Form["FindEstatus"]);

            try { this.TVI_Llave2Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business TVI_Llave2Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2>();
            Lista = TVI_Llave2Proxy.GetByFilters(Estatus);

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            ViewBag.Llave2_Descripcion = CompanyInfo.DescLlave2;
            ViewBag.Llave2_Etiqueta = CompanyInfo.LabelLlave2;
            return View("RefreshData", Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave2());
        }

        [Logger]
        public ActionResult Editar(int Llave2_ID)
        {
            try { this.TVI_Llave2Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJTVI_Llave2 = TVI_Llave2Proxy.GetByID(Llave2_ID);

            if (OBJTVI_Llave2 == null)
            {
                return HttpNotFound();
            }

            return View(OBJTVI_Llave2);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave2 Registro)
        {
            try { this.TVI_Llave2Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.LL2_Descripcion))
            {
                ModelState.AddModelError("LL2_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJTVI_Llave2 = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave2();
                    OBJTVI_Llave2 = TVI_Llave2Proxy.Crear(Registro);
                    if (OBJTVI_Llave2.Llave2_ID == 0)
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
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave2 Registro)
        {
            try { this.TVI_Llave2Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.LL2_Descripcion))
            {
                ModelState.AddModelError("LL2_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = TVI_Llave2Proxy.Editar(Registro);
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
        private List<ADESCOM.Models.FixedOption> GetEstatuses()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "Activos" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "Inactivos" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "Todos" });
            return SolEstatus_ID;
        }
    }
}