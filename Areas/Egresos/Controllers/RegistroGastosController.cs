using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Egresos.Controllers
{
    public class RegistroGastosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Egresos.Methods.GastosRealizadosBusiness GastosRealizadosProxy;
        protected ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosRealizados OBJEGR_GastosRealizados = new ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosRealizados();

        [Logger]
        public ActionResult Index()
        {
            //try { this.GastosRealizadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            //catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosRealizados> Lista = new List<ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosRealizados>();
            //Lista = GastosRealizadosProxy.GetAplicados();

            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");
            ViewBag.FindTipoFecha = new SelectList(GetTiposFecha(), "Option", "Description");
            ViewBag.Cuenta_IDFilter = new SelectList(GetCuentas(0, "(Todas)"), "Cuenta_ID", "CTA_Alias");

            return View(Lista);
        }

        public ActionResult RefreshDataSearchList(FormCollection Form)
        {
            int searchCuenta_ID = Convert.ToInt32(Form["Cuenta_IDFilter"]);
            int Estatus = Convert.ToInt32(Form["FindEstatus"]);
            DateTime? searchFDesde = null;
            DateTime? searchFHasta = null;
            int searchTFecha = Convert.ToInt32(Form["FindTipoFecha"]);
            String sarchRef = Form["FindReferencia"];

            if (!String.IsNullOrEmpty(Form["FindFechaDesde"]))
            {
                searchFDesde = Convert.ToDateTime(Form["FindFechaDesde"]);
            }

            if (!String.IsNullOrEmpty(Form["FindFechaHasta"]))
            {
                searchFHasta = Convert.ToDateTime(Form["FindFechaHasta"]);
            }

            try { this.GastosRealizadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosRealizados> Lista = new List<ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosRealizados>();
            Lista = GastosRealizadosProxy.GetByFilters(searchCuenta_ID, Estatus, sarchRef, searchFDesde, searchFHasta, searchTFecha);

            return View("RefreshData", Lista);
        }

        [Logger]
        public ActionResult Detalle(int GastoRealizado_ID)
        {
            try { this.GastosRealizadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosRealizados PagoRealizado = new ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosRealizados();
            PagoRealizado = GastosRealizadosProxy.GetViewByID(GastoRealizado_ID);
            return View(PagoRealizado);
        }

        [Logger]
        public ActionResult VerComprobante(String Comprobante)
        { 
            ViewBag.Comprobante = Comprobante;
            return View();
        }

        [Logger]
        public ActionResult Crear()
        {
            ViewBag.TipoGasto_ID = new SelectList(GetTipoGastos(0), "TipoGasto_ID", "TGA_Descripcion");
            ViewBag.Cuenta_ID = new SelectList(GetCuentas(0, "Elija una opción"), "Cuenta_ID", "CTA_Alias");
            ViewBag.ComprobanteName = "";
            ViewBag.ComprobanteFile = "";
            return View(new ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosRealizados() { GRE_FechaPago = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Crear(FormCollection Form)
        {
            try { this.GastosRealizadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosRealizados Registro = new ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosRealizados();

            if (String.IsNullOrEmpty(Form["TipoGasto_ID"]) || Form["TipoGasto_ID"] == "0")
            {
                ModelState.AddModelError("TipoGasto_ID", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Form["Cuenta_ID"]) || Form["Cuenta_ID"] == "0")
            {
                ModelState.AddModelError("Cuenta_ID", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Form["GRE_Monto"]) || Form["Cuenta_ID"] == "0")
            {
                ModelState.AddModelError("GRE_Monto", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Form["GRE_Concepto"]))
            {
                ModelState.AddModelError("GRE_Concepto", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Form["GRE_Referencia"]))
            {
                ModelState.AddModelError("GRE_Referencia", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Form["GRE_FechaPago"]))
            {
                ModelState.AddModelError("GRE_FechaPago", "Campo Requerido");
            }

            try { Registro.TipoGasto_ID = Convert.ToInt32(Form["TipoGasto_ID"]); }
            catch { ModelState.AddModelError("TipoGasto_ID", "Formato Incorrecto"); }

            try { Registro.Cuenta_ID = Convert.ToInt32(Form["Cuenta_ID"]); }
            catch { ModelState.AddModelError("Cuenta_ID", "Formato Incorrecto"); }

            try { Registro.GRE_Monto = Convert.ToDecimal(Form["GRE_Monto"]); }
            catch { ModelState.AddModelError("GRE_Monto", "Formato Incorrecto"); }

            try { Registro.GRE_FechaPago = Convert.ToDateTime(Form["GRE_FechaPago"]); }
            catch { ModelState.AddModelError("GRE_FechaPago", "Formato Incorrecto"); }

            if (Registro.GRE_Monto <= 0)
            {
                ModelState.AddModelError("GRE_Monto", "Campo Requerido");
            }

            Registro.GRE_Concepto = Form["GRE_Concepto"];
            Registro.GRE_Referencia = Form["GRE_Referencia"];

            //Comprobante
            if (!String.IsNullOrEmpty(Form["SavedFile"]))
            {
                Registro.GRE_Comprobante = Form["SavedFile"];
            }

            if (ModelState.IsValid)
            {
                try
                {
                    GastosRealizadosProxy.Crear(Registro, true);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.TipoGasto_ID = new SelectList(GetTipoGastos(0), "TipoGasto_ID", "TGA_Descripcion", Registro.TipoGasto_ID);
            ViewBag.Cuenta_ID = new SelectList(GetCuentas(0, "Elija una opción"), "Cuenta_ID", "CTA_Alias", Registro.Cuenta_ID);
            ViewBag.ComprobanteName = Form["fileName"];
            ViewBag.ComprobanteFile = Form["SavedFile"];
            return View(Registro);
        }

        private List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> GetCuentas(int Cuenta_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> Cuentas = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas>();
            Cuentas.Add(new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas() { Cuenta_ID = 0, CTA_Alias = Description });
            Cuentas.AddRange(ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentasEmpresa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta_ID));
            return Cuentas;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos> GetTipoGastos(int TipoGasto_ID)
        {
            ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness TipoGastosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos> TGastos= new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos>();
            TGastos.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos() { TipoGasto_ID = 0, TGA_Descripcion = "Elija Una Opcion" });
            TGastos.AddRange(TipoGastosProxy.GetActive(TipoGasto_ID));
            return TGastos;
        }

        [Logger]
        public ActionResult CancelarGasto(int GastoRealizado_ID)
        {
            try { this.GastosRealizadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJEGR_GastosRealizados = GastosRealizadosProxy.GetByID(GastoRealizado_ID);

            if (OBJEGR_GastosRealizados == null)
            {
                return HttpNotFound();
            }

            return View(OBJEGR_GastosRealizados);
        }

        [HttpPost, ActionName("CancelarGasto")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult CancelarGastoConfirmed(int GastoRealizado_ID)
        {
            try { this.GastosRealizadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJEGR_GastosRealizados = GastosRealizadosProxy.GetByID(GastoRealizado_ID);

            if (OBJEGR_GastosRealizados == null)
            {
                return HttpNotFound();
            }

            try
            {
                bool Status = GastosRealizadosProxy.CancelarPago(GastoRealizado_ID);
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

            return View(OBJEGR_GastosRealizados);
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetTiposFecha()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "Fecha de Pago" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "Fecha de Ingreso" });
            return SolEstatus_ID;
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetEstatuses()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "Aplicados" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "Cancelados" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "Todos" });
            return SolEstatus_ID;
        }
    }
}