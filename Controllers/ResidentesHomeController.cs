using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Controllers
{
    public class ResidentesHomeController : Controller
    {
        protected ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Methods.PagosRealizadosBusiness PagosRealizadosProxy;
        protected ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.ING_PagosRealizados OBJING_PagosRealizados = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.ING_PagosRealizados();

        public ActionResult Index()
        {
            ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS InfoUser = (ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"];
            ViewBag.Direccion_ID = InfoUser.Direccion_ID;
            ViewBag.DIR_Descripcion = InfoUser.Residencia;
            ViewBag.Referencia = InfoUser.Referencia;

            ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_Cuentas();
            Cuenta = ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"], InfoUser.Direccion_ID);
            ViewBag.CierresCta = new SelectList(GetCierres(Cuenta.Cuenta_ID), "Cierre_ID", "CIC_Descripcion");

            ViewBag.SaldoActual = Cuenta.CTA_Saldo;
            return View();
        }

        public ActionResult EdoCtaResidente()
        {
            ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS InfoUser = (ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"];
            ViewBag.Direccion_ID = InfoUser.Direccion_ID;
            ViewBag.DIR_Descripcion = InfoUser.Residencia;
            ViewBag.Referencia = InfoUser.Referencia;

            ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_Cuentas();
            Cuenta = ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"], InfoUser.Direccion_ID);
            ViewBag.CierresCta = new SelectList(GetCierres(Cuenta.Cuenta_ID), "Cierre_ID", "CIC_Descripcion");

            ViewBag.SaldoActual = Cuenta.CTA_Saldo;
            return View();
        }

        public ActionResult GetCierreInfo(int Direccion_ID, int Cierre_ID)
        {
            ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.CierreCompuesto CierreDet = new ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.CierreCompuesto();
            ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.CON_CierresCont CierreContInfo = new ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.CON_CierresCont();
            ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Methods.CON_CierresContBusiness CON_CierresContProxy = new ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Methods.CON_CierresContBusiness((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_MovContables> ListaMovs = new List<ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_MovContables>();

            //CierreDet contable
            if (Cierre_ID != 0)
            {
                CierreContInfo = CON_CierresContProxy.GetByID(Cierre_ID);

                //MovContables
                ListaMovs = ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Methods.CON_MovContablesBusiness.GetByCierre_ID((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"], Cierre_ID);
            }
            else
            {
                //Traer datos de la cuenta
                ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_Cuentas Cuenta = new ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_Cuentas();
                Cuenta = ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Methods.CON_CuentasBusiness.GetCuentaCasa((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"], Direccion_ID);

                //Simulamos un cierre contable
                CierreContInfo = new ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.CON_CierresCont();
                CierreContInfo.CIC_SaldoIni = Cuenta.CTA_SaldoIni;
                CierreContInfo.CIC_SaldoFin = Cuenta.CTA_Saldo;

                //MovContables
                ListaMovs = ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Methods.CON_MovContablesBusiness.GetNoAplicadosCuenta((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"], Cuenta.Cuenta_ID);
            }

            CierreDet.Cierre = CierreContInfo;
            CierreDet.MovContables = ListaMovs;

            return View(CierreDet);
        }

        private List<ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_CierresCont> GetCierres(int Cuenta_ID)
        {
            List<ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_CierresCont> Cierres = new List<ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_CierresCont>();
            Cierres.Add(new ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Models.VwCON_CierresCont() { Cierre_ID = 0, CIC_Descripcion = "Periodo en curso" });
            Cierres.AddRange(ADESCOMBUSINESS.AccesoResidentes.EstadoCuenta.Methods.CON_CierresContBusiness.GetByCuenta_ID((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"], Cuenta_ID));
            return Cierres;
        }

        public ActionResult RegistroPagos()
        {
            ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS InfoUser = (ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"];
            List<ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.VwING_PagosRealizados> listaPagos = new List<ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.VwING_PagosRealizados>();
            ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Methods.PagosRealizadosBusiness PagosRealizadosProxy = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"]);
            listaPagos = PagosRealizadosProxy.GetByDireccion_ID(InfoUser.Direccion_ID);
            return View(listaPagos);
        }

        public ActionResult RefreshPagos()
        {
            ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS InfoUser = (ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"];
            List<ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.VwING_PagosRealizados> listaPagos = new List<ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.VwING_PagosRealizados>();
            ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Methods.PagosRealizadosBusiness PagosRealizadosProxy = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"]);
            listaPagos = PagosRealizadosProxy.GetByDireccion_ID(InfoUser.Direccion_ID);
            return View(listaPagos);
        }

       [Logger]
        public ActionResult DetallePago(int PagoRealizado_ID)
        {
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.VwING_PagosRealizados PagoRealizado = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.VwING_PagosRealizados();
            PagoRealizado = PagosRealizadosProxy.GetViewByID(PagoRealizado_ID);
            return View(PagoRealizado);
        }

        [Logger]
        public ActionResult VerComprobante(String Comprobante)
        {
            ViewBag.Comprobante = Comprobante;
            return View();
        }

        [Logger]
        public ActionResult RegistrarPago()
        {
            ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS InfoUser = (ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"];
            ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.ING_PagosRealizados pago = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.ING_PagosRealizados();
            pago.PRE_Referencia = InfoUser.Referencia;
            return View(pago);
        }

        [Logger]
        [HttpPost]
        public ActionResult RegistrarPago(FormCollection Form)
        {
            ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS InfoUser = (ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"];

            ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.ING_PagosRealizados Registro = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.ING_PagosRealizados();
           
            //Comprobante
            Registro.PRE_Comprobante = Form["SavedFile"];

            if (String.IsNullOrEmpty(Registro.PRE_Comprobante))
            {
                ModelState.AddModelError("PRE_Comprobante", "La imágen del comprobante es requerida");
            }

            try { Registro.PRE_Monto = Convert.ToDecimal(Form["PRE_Monto"]); }
            catch { ModelState.AddModelError("PRE_Monto", "Formato Incorrecto"); }

            if (Registro.PRE_Monto <= 0)
            {
                ModelState.AddModelError("PRE_Monto", "El monto debe ser mayor a 0");
            }

            Registro.PRE_Referencia = Form["PRE_Referencia"];

            int Cuenta_ID = Convert.ToInt32(ADESCOMBUSINESS.App.Common.GlobalBusiness.ObtConfigParam(InfoUser.Compania_ID, "CuentaCuotas"));
            if (Cuenta_ID == 0)
            {
                ModelState.AddModelError("PRE_Monto", "");
                ViewBag.Error = "Error al obtener la cuenta destino";
            }

            try
            {
                this.PagosRealizadosProxy = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"]);
            }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.VwING_PagosRealizados> PagosPendientes = new List<ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Models.VwING_PagosRealizados>();
            PagosPendientes = PagosRealizadosProxy.GetUnauthorizedByResident(InfoUser.Compania_ID, InfoUser.Direccion_ID);
            if (PagosPendientes != null && PagosPendientes.Count > 0)
            {
                ModelState.AddModelError("PRE_Monto", "");
                ViewBag.Error = "Usted tiene pagos pendientes para su autorización. Si necesita ayuda, comuníquese con la mesa directiva.";
            }

            Registro.Cuenta_ID = Cuenta_ID;
            Registro.Compania_ID = InfoUser.Compania_ID;
            Registro.Direccion_ID = InfoUser.Direccion_ID;
            Registro.Residente_ID = InfoUser.Residente_ID;
            Registro.PRE_Monto = Registro.PRE_Monto;
            Registro.PRE_Referencia = InfoUser.Referencia;
            Registro.UsuarioCreo = InfoUser.UserName;
            Registro.PRE_Estatus = "ACT";
            Registro.PRE_FechaPago = ADESCOMBUSINESS.GlobalBusiness.ObtFechaHoraServer();

            if (ModelState.IsValid)
            {
                try
                {
                    try
                    {
                        this.PagosRealizadosProxy = new ADESCOMBUSINESS.AccesoResidentes.RegistroPagos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS)Session["InfoUser"]);
                    }
                    catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

                    PagosRealizadosProxy.Crear(Registro);

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