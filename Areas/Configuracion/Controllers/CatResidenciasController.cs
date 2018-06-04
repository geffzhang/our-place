using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class CatResidenciasController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness TVI_DireccionesProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones OBJTVI_Direcciones = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();

         [Logger]
        public ActionResult Index()
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones>();
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");
            ViewBag.FindHabitadas = new SelectList(GetHabitadas(), "Option", "Description");

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    return View("Index1", Lista);
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion");
                    return View("Index2", Lista);
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0), "Llave3_ID", "LL3_Descripcion");
                    return View("Index3", Lista);
            }

            return null;
        }

        public ActionResult RefreshDataSearchList(FormCollection Form)
        {
            int Estatus = Convert.ToInt32(Form["FindEstatus"]);
            int searchLL1 = Convert.ToInt32(Form["Llave1_ID"]);
            int searchLL2 = Convert.ToInt32(Form["Llave2_ID"]);
            int searchLL3 = Convert.ToInt32(Form["Llave3_ID"]);
            int Habitada = Convert.ToInt32(Form["FindHabitadas"]);

            try { this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones>();
            Lista = TVI_DireccionesProxy.GetBySimpleFilter(searchLL1, searchLL2, searchLL3, Estatus, Habitada);

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    return View("RefreshData1", Lista);
                case 2:
                    return View("RefreshData2", Lista);
                case 3:
                    return View("RefreshData3", Lista);
            }

            return null;
        }

        [Logger]
        public ActionResult Crear()
        {
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    return View("Crear1", new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones() { BS_Activo = true });
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion");
                    return View("Crear2", new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones() { BS_Activo = true });
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0), "Llave3_ID", "LL3_Descripcion");
                    return View("Crear3", new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones() { BS_Activo = true });
            }

            return null;
        }

        [Logger]
        public ActionResult Editar(int Direccion_ID)
        {
            try { this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJTVI_Direcciones = TVI_DireccionesProxy.GetByID(Direccion_ID);

            if (OBJTVI_Direcciones == null)
            {
                return HttpNotFound();
            }

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business TVI_Llave2Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business TVI_Llave3Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = TVI_Llave1Proxy.GetByID(OBJTVI_Direcciones.Llave1_ID).LL1_Descripcion;
                    return View("Editar1", OBJTVI_Direcciones);
                case 2:
                    ViewBag.Llave1_ID = TVI_Llave1Proxy.GetByID(OBJTVI_Direcciones.Llave1_ID).LL1_Descripcion;
                    ViewBag.Llave2_ID = TVI_Llave2Proxy.GetByID(OBJTVI_Direcciones.Llave2_ID).LL2_Descripcion;
                    return View("Editar2", OBJTVI_Direcciones);
                case 3:
                    ViewBag.Llave1_ID = TVI_Llave1Proxy.GetByID(OBJTVI_Direcciones.Llave1_ID).LL1_Descripcion;
                    ViewBag.Llave2_ID = TVI_Llave2Proxy.GetByID(OBJTVI_Direcciones.Llave2_ID).LL2_Descripcion;
                    ViewBag.Llave3_ID = TVI_Llave3Proxy.GetByID(OBJTVI_Direcciones.Llave3_ID).LL3_Descripcion;
                    return View("Editar3", OBJTVI_Direcciones);
            }

            return null;
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Registro)
        {
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            try { this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            int cantLlaves = CompanyInfo.CantLlaves;

            if(Registro.Llave1_ID <= 0)
            {
                ModelState.AddModelError("Llave1_ID", "Campo Requerido");
            }

            if (cantLlaves > 1)
            {
                if (Registro.Llave2_ID <= 0)
                {
                    ModelState.AddModelError("Llave2_ID", "Campo Requerido");
                }
            }

            if (cantLlaves > 2)
            {
                if (Registro.Llave3_ID <= 0)
                {
                    ModelState.AddModelError("Llave3_ID", "Campo Requerido");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TVI_DireccionesProxy.Crear(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion", Registro.Llave1_ID);
                    return View("Crear1", new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones() { BS_Activo = true });
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion", Registro.Llave1_ID);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion", Registro.Llave2_ID);
                    return View("Crear2", new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones() { BS_Activo = true });
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion", Registro.Llave1_ID);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion", Registro.Llave2_ID);
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0), "Llave3_ID", "LL3_Descripcion", Registro.Llave3_ID);
                    return View("Crear3", new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones() { BS_Activo = true });
            }

            return View(Registro);
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Registro)
        {
            try { this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (ModelState.IsValid)
            {
                try
                {
                    TVI_DireccionesProxy.Editar(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business TVI_Llave1Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business TVI_Llave2Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business TVI_Llave3Proxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = TVI_Llave1Proxy.GetByID(Registro.Llave1_ID).LL1_Descripcion;
                    return View("Editar1", Registro);
                case 2:
                    ViewBag.Llave1_ID = TVI_Llave1Proxy.GetByID(Registro.Llave1_ID).LL1_Descripcion;
                    ViewBag.Llave2_ID = TVI_Llave2Proxy.GetByID(Registro.Llave2_ID).LL2_Descripcion;
                    return View("Editar2", Registro);
                case 3:
                    ViewBag.Llave1_ID = TVI_Llave1Proxy.GetByID(Registro.Llave1_ID).LL1_Descripcion;
                    ViewBag.Llave2_ID = TVI_Llave2Proxy.GetByID(Registro.Llave2_ID).LL2_Descripcion;
                    ViewBag.Llave3_ID = TVI_Llave3Proxy.GetByID(Registro.Llave3_ID).LL3_Descripcion;
                    return View("Editar3", Registro);
            }

            return View(Registro);
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> GetLlave1(int LLave1_ID)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> Llaves1 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1>();
            Llaves1.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1() { Llave1_ID = 0, LL1_Descripcion = "(Todos)" });
            Llaves1.AddRange(ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business.GetDireccionLlave1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], LLave1_ID));
            return Llaves1;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2> GetLlave2(int LLave2_ID)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2> Llaves2 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2>();
            Llaves2.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2() { Llave2_ID = 0, LL2_Descripcion = "(Todos)" });
            return Llaves2;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3> GetLlave3(int LLave3_ID)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3> Llaves3 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3>();
            Llaves3.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3() { Llave3_ID = 0, LL3_Descripcion = "(Todos)" });
            return Llaves3;
        }

        [Logger]
        public JsonResult GetLlave2Items(int Llave1_ID)
        {
            return Json(new SelectList(ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business.GetByLlave1_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID), "Llave2_ID", "LL2_Descripcion"));
        }

        [Logger]
        public JsonResult GetLlave3Items(int Llave1_ID, int Llave2_ID)
        {
            return Json(new SelectList(ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business.GetByLlave1and2((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID, Llave2_ID), "Llave3_ID", "LL3_Descripcion"));
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

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetHabitadas()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "Habitadas" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "Deshabitadas" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "Todas" });
            return SolEstatus_ID;
        }
    }
}