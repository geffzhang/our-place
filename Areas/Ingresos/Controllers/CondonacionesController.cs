using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Ingresos.Controllers
{
    public class CondonacionesController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness TVI_DireccionesProxy;

        [Logger]
        public ActionResult Index()
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones>();

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;
            ViewBag.FindEstatusMora_ID = new SelectList(GetEstatusesMora(), "Option", "Description");
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");

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
            int searchLL1 = Convert.ToInt32(Form["Llave1_ID"]);
            int searchLL2 = Convert.ToInt32(Form["Llave2_ID"]);
            int searchLL3 = Convert.ToInt32(Form["Llave3_ID"]);
            int searchSts = Convert.ToInt32(Form["FindEstatusMora_ID"]);
            int searchEstatus = Convert.ToInt32(Form["FindEstatus"]);

            try { this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones>();
            Lista = TVI_DireccionesProxy.GetByFilters(searchLL1, searchLL2, searchLL3, searchSts, searchEstatus);

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
        public ActionResult Crear(int Direccion_ID)
        {
            //ViewBags Informativos
            this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();
            Direccion = TVI_DireccionesProxy.GetByID(Direccion_ID);
            ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas();
            Cuenta = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Direccion_ID);
            ViewBag.Direccion_ID = Direccion_ID;
            ViewBag.DIR_Descripcion = Direccion.DIR_Descripcion;
            ViewBag.SaldoActual = Cuenta.CTA_Saldo;
            //--------------

            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados> CargosPendientes = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados>();
            ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness PagosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            CargosPendientes = PagosProgramadosProxy.GetPendByDireccion_ID(Direccion_ID);
            ViewBag.CargosPendientes = CargosPendientes;

            return View("CondonarCargos", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados() { Direccion_ID = Direccion_ID, PRE_FechaPago = DateTime.Now });
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados Registro)
        {
            ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness PagosRealizadosProxy = null;
            try { PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (Registro.PRE_Monto <= 0)
            {
                ModelState.AddModelError("PRE_Monto", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Registro.PRE_Referencia))
            {
                ModelState.AddModelError("PRE_Referencia", "Campo Requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    PagosRealizadosProxy.CondonarCargos(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados> CargosPendientes = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados>();
            ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness PagosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            CargosPendientes = PagosProgramadosProxy.GetPendByDireccion_ID(Registro.Direccion_ID);
            ViewBag.CargosPendientes = CargosPendientes;

            //ViewBags Informativos
            this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();
            Direccion = TVI_DireccionesProxy.GetByID(Registro.Direccion_ID);
            ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas();
            Cuenta = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Registro.Direccion_ID);
            ViewBag.Direccion_ID = Registro.Direccion_ID;
            ViewBag.DIR_Descripcion = Direccion.DIR_Descripcion;
            ViewBag.SaldoActual = Cuenta.CTA_Saldo;
            //-------------------

            return View("CondonarCargos", Registro);
        }

        /*[Logger]
        public ActionResult CondonarCargos(int Direccion_ID)
        {
            //ViewBags Informativos
            this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();
            Direccion = TVI_DireccionesProxy.GetByID(Direccion_ID);
            ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas();
            Cuenta = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Direccion_ID);
            ViewBag.Direccion_ID = Direccion_ID;
            ViewBag.DIR_Descripcion = Direccion.DIR_Descripcion;
            ViewBag.SaldoActual = Cuenta.CTA_Saldo;
            //--------------

            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados> CargosPendientes = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados>();
            ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness PagosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            CargosPendientes = PagosProgramadosProxy.GetPendByDireccion_ID(Direccion_ID);

            return View(CargosPendientes);
        }

        [HttpPost]
        [Logger]
        public ActionResult CondonarCargos(FormCollection Form)
        {
            bool error = false;
            int Direccion_ID = 0;
            String CargosCond = String.Empty;
            ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness PagosRealizadosProxy = null;

            try { PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Form["CargosSelect"]) || Form["CargosSelect"].Equals("0"))
            {
                error = true;
                ViewBag.Error = "No se selecciónó ningún cargo a condonar";
            }
            else
            {
                CargosCond = Form["CargosSelect"].ToString();
            }

            Direccion_ID = Convert.ToInt32(Form["Direccion_ID"]);

            if (error == false)
            {
                try
                {
                    bool rs = false;
                    rs = PagosRealizadosProxy.CondonarCargos(Direccion_ID, CargosCond);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados> CargosPendientes = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosProgramados>();
            ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness PagosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            CargosPendientes = PagosProgramadosProxy.GetPendByDireccion_ID(Direccion_ID);

            //ViewBags Informativos
            this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();
            Direccion = TVI_DireccionesProxy.GetByID(Direccion_ID);
            ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas();
            Cuenta = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Direccion_ID);
            ViewBag.Direccion_ID = Direccion_ID;
            ViewBag.DIR_Descripcion = Direccion.DIR_Descripcion;
            ViewBag.SaldoActual = Cuenta.CTA_Saldo;
            //-------------------

            return View(CargosPendientes);
        }*/

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetEstatusesMora()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "(Todos)" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "En Mora" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "Al Corriente" });
            return SolEstatus_ID;
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

        [Logger]
        public JsonResult GetReferencia1(int Llave1_ID)
        {
            String Referencia = String.Empty;
            ADESCOMBUSINESS.Areas.Configuracion.Models.CIA_ConfigParam parametro = new ADESCOMBUSINESS.Areas.Configuracion.Models.CIA_ConfigParam();
            parametro = ADESCOMBUSINESS.Areas.Configuracion.Methods.CIA_ConfigParamBusiness.GetByNombreParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "ReferenciaFija");
            if (Convert.ToBoolean(parametro.CPA_Valor) == true)
            {
                ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();
                Direccion = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID);
                if (Direccion != null)
                {
                    Referencia = Direccion.DIR_Referencia;
                }
            }
            return Json(Referencia);
        }

        [Logger]
        public JsonResult GetReferencia2(int Llave1_ID, int Llave2_ID)
        {
            String Referencia = String.Empty;
            ADESCOMBUSINESS.Areas.Configuracion.Models.CIA_ConfigParam parametro = new ADESCOMBUSINESS.Areas.Configuracion.Models.CIA_ConfigParam();
            parametro = ADESCOMBUSINESS.Areas.Configuracion.Methods.CIA_ConfigParamBusiness.GetByNombreParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "ReferenciaFija");
            if (Convert.ToBoolean(parametro.CPA_Valor) == true)
            {
                ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();
                Direccion = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves2((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID, Llave2_ID);
                if (Direccion != null)
                {
                    Referencia = Direccion.DIR_Referencia;
                }
            }
            return Json(Referencia);
        }

        [Logger]
        public JsonResult GetReferencia3(int Llave1_ID, int Llave2_ID, int Llave3_ID)
        {
            String Referencia = String.Empty;
            ADESCOMBUSINESS.Areas.Configuracion.Models.CIA_ConfigParam parametro = new ADESCOMBUSINESS.Areas.Configuracion.Models.CIA_ConfigParam();
            parametro = ADESCOMBUSINESS.Areas.Configuracion.Methods.CIA_ConfigParamBusiness.GetByNombreParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "ReferenciaFija");
            if (Convert.ToBoolean(parametro.CPA_Valor) == true)
            {
                ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();
                Direccion = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves3((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID, Llave2_ID, Llave3_ID);
                if (Direccion != null)
                {
                    Referencia = Direccion.DIR_Referencia;
                }
            }
            return Json(Referencia);
        }

    }
}