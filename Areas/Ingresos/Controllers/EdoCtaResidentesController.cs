using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Ingresos.Controllers
{
    public class EdoCtaResidentesController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness TVI_DireccionesProxy;

        [Logger]
        public ActionResult Index()
        {
            //try { this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            //catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones>();
            //Lista = TVI_DireccionesProxy.GetActive(0);

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
        public ActionResult EdoCtaCasa(int Direccion_ID)
        {
            try { this.TVI_DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();
            Direccion = TVI_DireccionesProxy.GetByID(Direccion_ID);

            ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas();
            Cuenta = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Direccion_ID);
            ViewBag.CierresCta = new SelectList(GetCierres(Cuenta.Cuenta_ID), "Cierre_ID", "CIC_Descripcion");

            ViewBag.SaldoActual = Cuenta.CTA_Saldo;

            return View(Direccion);
        }

        public ActionResult GetCierreInfo(int Direccion_ID, int Cierre_ID)
        {
            ADESCOMBUSINESS.Areas.Contabilidad.Models.CierreCompuesto CierreDet = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CierreCompuesto();
            ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_CierresCont CierreContInfo = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_CierresCont();
            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CierresContBusiness CON_CierresContProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CierresContBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables> ListaMovs = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables>();

            //CierreDet contable
            if (Cierre_ID > 0)
            {
                CierreContInfo = CON_CierresContProxy.GetByID(Cierre_ID);

                //MovContables
                ListaMovs = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness.GetByCierre_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cierre_ID);
            }
            else
            {
                //Traer datos de la cuenta
                ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas();
                Cuenta = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Direccion_ID);
                //Simulamos un cierre contable
                CierreContInfo = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_CierresCont();
                CierreContInfo.CIC_SaldoFin = Cuenta.CTA_Saldo;

                if (Cierre_ID < 0) //Periodo en Curso
                {
                    CierreContInfo.CIC_SaldoIni = Cuenta.CTA_SaldoIni;

                    //MovContables
                    ListaMovs = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness.GetNoAplicadosCuenta((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta.Cuenta_ID);
                }
                else
                {
                    CierreContInfo.CIC_SaldoIni = 0;

                    //MovContables
                    ListaMovs = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness.GetTodosCuenta((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta.Cuenta_ID);
                }
            }

            CierreDet.Cierre = CierreContInfo;
            CierreDet.MovContables = ListaMovs;

            return View(CierreDet);
        }

        private List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont> GetCierres(int Cuenta_ID)
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont> Cierres = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont>();
            Cierres.Add(new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont() { Cierre_ID = -1, CIC_Descripcion = "Periodo en curso" });
            Cierres.Add(new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont() { Cierre_ID = 0, CIC_Descripcion = "Estado de Cuenta General" });
            Cierres.AddRange(ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CierresContBusiness.GetByCuenta_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta_ID));
            return Cierres;
        }

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