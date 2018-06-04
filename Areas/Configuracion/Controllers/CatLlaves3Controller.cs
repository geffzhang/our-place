using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class CatLlaves3Controller : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business TVI_Llave3Proxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave3 OBJTVI_Llave3 = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave3();

        [Logger]
        public ActionResult Index()
        {
            //try { this.TVI_Llave3Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            //catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3>();
            //Lista = TVI_Llave3Proxy.GetActive(0);
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            ViewBag.Llave3_Descripcion = CompanyInfo.DescLlave3;
            ViewBag.Llave3_Etiqueta = CompanyInfo.LabelLlave3;
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");

            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshDataSearchList(FormCollection Form)
        {
            int Estatus = Convert.ToInt32(Form["FindEstatus"]);

            try { this.TVI_Llave3Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business TVI_Llave3Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3>();
            Lista = TVI_Llave3Proxy.GetByFilters(Estatus);

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            ViewBag.Llave3_Descripcion = CompanyInfo.DescLlave3;
            ViewBag.Llave3_Etiqueta = CompanyInfo.LabelLlave3;
            return View("RefreshData", Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave3());
        }

        [Logger]
        public ActionResult Editar(int Llave3_ID)
        {
            try { this.TVI_Llave3Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJTVI_Llave3 = TVI_Llave3Proxy.GetByID(Llave3_ID);

            if (OBJTVI_Llave3 == null)
            {
                return HttpNotFound();
            }

            return View(OBJTVI_Llave3);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave3 Registro)
        {
            try { this.TVI_Llave3Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.LL3_Descripcion))
            {
                ModelState.AddModelError("LL3_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJTVI_Llave3 = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave3();
                    OBJTVI_Llave3 = TVI_Llave3Proxy.Crear(Registro);
                    if (OBJTVI_Llave3.Llave3_ID == 0)
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
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave3 Registro)
        {
            try { this.TVI_Llave3Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.LL3_Descripcion))
            {
                ModelState.AddModelError("LL3_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = TVI_Llave3Proxy.Editar(Registro);
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