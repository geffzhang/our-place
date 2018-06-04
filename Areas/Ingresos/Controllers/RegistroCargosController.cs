using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Ingresos.Controllers
{
    public class RegistroCargosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness PagosProgramadosProxy;

        [Logger]
        public ActionResult Index()
        {
            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados> Lista = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados>();

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;
            ViewBag.TipoCargo_IDFilter = new SelectList(GetTiposCargo(0, "(Todos)"), "TipoCargo_ID", "TCA_Descripcion");
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "(Todas)"), "Llave1_ID", "LL1_Descripcion");
                    return View("Index1", Lista);
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "(Todas)"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "(Todas)"), "Llave2_ID", "LL2_Descripcion");
                    return View("Index2", Lista);
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "(Todas)"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "(Todas)"), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0, 0, "(Todas)"), "Llave3_ID", "LL3_Descripcion");
                    return View("Index3", Lista);
            }

            return null;
        }

        public ActionResult RefreshDataSearchList(FormCollection Form)
        {
            int searchLL1 = Convert.ToInt32(Form["Llave1_ID"]);
            int searchLL2 = Convert.ToInt32(Form["Llave2_ID"]);
            int searchLL3 = Convert.ToInt32(Form["Llave3_ID"]);
            int searchTipoCargo = Convert.ToInt32(Form["TipoCargo_IDFilter"]);
            int searchEstatus = Convert.ToInt32(Form["FindEstatus"]);
            String searchConcepto = Form["FindConcepto"];
            DateTime? searchFDesde = null;
            DateTime? searchFHasta = null;

            if (!String.IsNullOrEmpty(Form["FindFechaDesde"]))
            {
                searchFDesde = Convert.ToDateTime(Form["FindFechaDesde"]);
            }

            if (!String.IsNullOrEmpty(Form["FindFechaHasta"]))
            {
                searchFHasta = Convert.ToDateTime(Form["FindFechaHasta"]);
            }

            try { this.PagosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados> Lista = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados>();
            Lista = PagosProgramadosProxy.GetByFilters(searchLL1, searchLL2, searchLL3, searchTipoCargo, searchEstatus, searchConcepto, searchFDesde, searchFHasta);

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

            ViewBag.TipoCargo_ID = new SelectList(GetTiposCargo(0), "TipoCargo_ID", "TCA_Descripcion");

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    return View("Crear1", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosProgramados() { PPR_FechaPago = DateTime.Now });
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "Elija una opción"), "Llave2_ID", "LL2_Descripcion");
                    return View("Crear2", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosProgramados() { PPR_FechaPago = DateTime.Now });
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "Elija una opción"), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0, 0, "Elija una opción"), "Llave3_ID", "LL3_Descripcion");
                    return View("Crear3", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosProgramados() { PPR_FechaPago = DateTime.Now });
            }

            return null;
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(FormCollection Form)
        {
            int Llave1 = 0;
            int Llave2 = 0;
            int Llave3 = 0;

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            int cantLlaves = CompanyInfo.CantLlaves;

            ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();
            ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosProgramados Registro = new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosProgramados();

            try { this.PagosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (string.IsNullOrEmpty(Form["Llave1_ID"]) || Form["Llave1_ID"] == "0")
            {
                ModelState.AddModelError("Llave1_ID", "Campo Requerido");
                ViewBag.Error = "No ingresó " + CompanyInfo.LabelLlave1 + "y/o" + CompanyInfo.LabelLlave2;
            }
            else
            {
                Llave1 = Convert.ToInt32(Form["Llave1_ID"]);
            }

            if (cantLlaves > 1)
            {
                if (string.IsNullOrEmpty(Form["Llave2_ID"]) || Form["Llave2_ID"] == "0")
                {
                    ModelState.AddModelError("Llave2_ID", "Campo Requerido");
                    ViewBag.Error = "No ingresó " + CompanyInfo.LabelLlave1 + "y/o" + CompanyInfo.LabelLlave2;
                }
                else
                {
                    Llave2 = Convert.ToInt32(Form["Llave2_ID"]);
                }
            }

            if (cantLlaves > 2)
            {
                if (string.IsNullOrEmpty(Form["Llave3_ID"]) || Form["Llave3_ID"] == "0")
                {
                    ModelState.AddModelError("Llave3_ID", "Campo Requerido");
                    ViewBag.Error = "No ingresó " + CompanyInfo.LabelLlave3;
                }
                else
                {
                    Llave3 = Convert.ToInt32(Form["Llave3_ID"]);
                }
            }

            if (cantLlaves == 1)
                Direccion = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1);
            if (cantLlaves == 2)
                Direccion = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves2((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1, Llave2);
            if (cantLlaves == 3)
                Direccion = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves3((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1, Llave2, Llave3);
            if (Direccion == null)
            {
                ModelState.AddModelError("Llave1_ID", "Campo Requerido");
                ViewBag.Error = "No existe la dirección o está desactivada";
            }
            else
            {
                Registro.Direccion_ID = Direccion.Direccion_ID;
            }

            if (string.IsNullOrEmpty(Form["PPR_Monto"]) || Form["PPR_Monto"] == "0")
            {
                ModelState.AddModelError("PPR_Monto", "Campo Requerido");
            }
            else
            {
                if (Convert.ToDecimal(Form["PPR_Monto"]) < 0)
                    ModelState.AddModelError("PPR_Monto", "Debe ser mayor que cero");
                else
                    Registro.PPR_Monto = Convert.ToDecimal(Form["PPR_Monto"]);
            }

            if (string.IsNullOrEmpty(Form["PPR_Concepto"]))
            {
                ModelState.AddModelError("PPR_Concepto", "Campo Requerido");
            }
            else
            {
                Registro.PPR_Concepto = Form["PPR_Concepto"];
            }

            if (string.IsNullOrEmpty(Form["TipoCargo_ID"]) || Form["TipoCargo_ID"] == "0")
            {
                ModelState.AddModelError("TipoCargo_ID", "Campo Requerido");
            }
            else
            {
                Registro.TipoCargo_ID = Convert.ToInt32(Form["TipoCargo_ID"]);
            }

            Registro.PPR_FechaPago = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    PagosProgramadosProxy.Crear(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.TipoCargo_ID = new SelectList(GetTiposCargo(0), "TipoCargo_ID", "TCA_Descripcion");
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    return View("Crear1", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosProgramados() { PPR_FechaPago = DateTime.Now });
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "Elija una opción"), "Llave2_ID", "LL2_Descripcion");
                    return View("Crear2", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosProgramados() { PPR_FechaPago = DateTime.Now });
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "Elija una opción"), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0, 0, "Elija una opción"), "Llave3_ID", "LL3_Descripcion");
                    return View("Crear3", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosProgramados() { PPR_FechaPago = DateTime.Now });
            }

            return null;
        }

        [Logger]
        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos> GetTiposCargo(int TipoCargo_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos> Cuentas = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos>();
            Cuentas.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos() { TipoCargo_ID = 0, TCA_Descripcion = Description });
            ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness CAT_TipoCargosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            Cuentas.AddRange(CAT_TipoCargosProxy.GetUserActive(TipoCargo_ID));
            return Cuentas;
        }

        [Logger]
        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos> GetTiposCargo(int TipoCargo_ID)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos> Cuentas = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos>();
            ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness CAT_TipoCargosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            Cuentas.AddRange(CAT_TipoCargosProxy.GetUserActive(TipoCargo_ID));
            return Cuentas;
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetEstatuses()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "(Todos)" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "Por pagar" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "Pago Parcial" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 3, Description = "Pagado" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 4, Description = "Condonado" });
            return SolEstatus_ID;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> GetLlave1(int LLave1_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> Llaves1 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1>();
            Llaves1.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1() { Llave1_ID = 0, LL1_Descripcion = Description });
            Llaves1.AddRange(ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business.GetDireccionLlave1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], LLave1_ID));
            return Llaves1;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2> GetLlave2(int Llave1_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2> Llaves2 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2>();
            Llaves2.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2() { Llave2_ID = 0, LL2_Descripcion = Description });
            Llaves2.AddRange(ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave2Business.GetByLlave1_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID));
            return Llaves2;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3> GetLlave3(int LLave1_ID, int LLave2_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3> Llaves3 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3>();
            Llaves3.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3() { Llave3_ID = 0, LL3_Descripcion = Description });
            Llaves3.AddRange(ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave3Business.GetByLlave1and2((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], LLave1_ID, LLave2_ID));
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
    }
}