using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class CatLlaves1Controller : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business TVI_Llave1Proxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave1 OBJTVI_Llave1 = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave1();

        [Logger]
        public ActionResult Index()
        {
           // try { this.TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
           // catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

           // ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1>();
          //  Lista = TVI_Llave1Proxy.GetActive(0);
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            ViewBag.Llave1_Descripcion = CompanyInfo.DescLlave1;
            ViewBag.Llave1_Etiqueta = CompanyInfo.LabelLlave1;
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");

            return View(Lista);
        }

        public ActionResult RefreshDataSearchList(FormCollection Form)
        {
            int Estatus = Convert.ToInt32(Form["FindEstatus"]);

            try { this.TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1>();
            Lista = TVI_Llave1Proxy.GetByFilters(Estatus);

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            ViewBag.Llave1_Descripcion = CompanyInfo.DescLlave1;
            ViewBag.Llave1_Etiqueta = CompanyInfo.LabelLlave1;
            return View("RefreshData", Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave1());
        }

        [Logger]
        public ActionResult Editar(int Llave1_ID)
        {
            try { this.TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJTVI_Llave1 = TVI_Llave1Proxy.GetByID(Llave1_ID);

            if (OBJTVI_Llave1 == null)
            {
                return HttpNotFound();
            }

            return View(OBJTVI_Llave1);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave1 Registro)
        {
            try { this.TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.LL1_Descripcion))
            {
                ModelState.AddModelError("LL1_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJTVI_Llave1 = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave1();
                    OBJTVI_Llave1 = TVI_Llave1Proxy.Crear(Registro);
                    if (OBJTVI_Llave1.Llave1_ID == 0)
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
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Llave1 Registro)
        {
            try { this.TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.LL1_Descripcion))
            {
                ModelState.AddModelError("LL1_Descripcion", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = TVI_Llave1Proxy.Editar(Registro);
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