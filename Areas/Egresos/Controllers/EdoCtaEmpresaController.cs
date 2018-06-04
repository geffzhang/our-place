using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Egresos.Controllers
{
    public class EdoCtaEmpresaController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness CON_CuentasProxy;

        [Logger]
        public ActionResult Index()
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas>();
            Lista = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentasEmpresa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], 0);
            return View(Lista);
        }

        [Logger]
        public ActionResult EdoCtaColonia(int Cuenta_ID)
        {
            try { this.CON_CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas CuentaEmp = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas();
            CuentaEmp = CON_CuentasProxy.GetByID(Cuenta_ID);

            ViewBag.CierresCta = new SelectList(GetCierres(CuentaEmp.Cuenta_ID), "Cierre_ID", "CIC_Descripcion");

            return View(CuentaEmp);
        }

        public ActionResult GetCierreInfo(int Cuenta_ID, int Cierre_ID)
        {
            ADESCOMBUSINESS.Areas.Contabilidad.Models.CierreCompuesto CierreDet = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CierreCompuesto();
            ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_CierresCont CierreContInfo = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_CierresCont();
            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CierresContBusiness CON_CierresContProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CierresContBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables> ListaMovs = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_MovContables>();

            //CierreDet contable
            if (Cierre_ID != 0)
            {
                CierreContInfo = CON_CierresContProxy.GetByID(Cierre_ID);

                //MovContables
                ListaMovs = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness.GetByCierre_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cierre_ID);
            }
            else
            {
                //Traer datos de la cuenta
                ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas Cuenta = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas();
                ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
                Cuenta = CuentasProxy.GetByID(Cuenta_ID);

                //Simulamos un cierre contable
                CierreContInfo = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_CierresCont();
                CierreContInfo.CIC_SaldoIni = Cuenta.CTA_SaldoIni;
                CierreContInfo.CIC_SaldoFin = Cuenta.CTA_Saldo;

                //MovContables
                ListaMovs = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_MovContablesBusiness.GetNoAplicadosCuenta((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta.Cuenta_ID);
            }

            CierreDet.Cierre = CierreContInfo;
            CierreDet.MovContables = ListaMovs;

            return View(CierreDet);
        }

        private List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont> GetCierres(int Cuenta_ID)
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont> Cierres = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont>();
            Cierres.Add(new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_CierresCont() { Cierre_ID = 0, CIC_Descripcion = "Periodo en curso" });
            Cierres.AddRange(ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CierresContBusiness.GetByCuenta_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta_ID));
            return Cierres;
        }
    }
}