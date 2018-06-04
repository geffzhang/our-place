using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Egresos.Controllers
{
    public class CatGastosProgramadosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Egresos.Methods.GastosProgramadosBusiness GastosProgramadosProxy;
        protected ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosProgramados OBJEGR_GastosProgramados = new ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosProgramados();

        [Logger]
        public ActionResult Index()
        {
            try { this.GastosProgramadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosProgramados> Lista = new List<ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosProgramados>();
            Lista = GastosProgramadosProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.GastosProgramadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosProgramados> Lista = new List<ADESCOMBUSINESS.Areas.Egresos.Models.VwEGR_GastosProgramados>();
            Lista = GastosProgramadosProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            ViewBag.TipoGasto_ID = new SelectList(GetTipoGastos(0), "TipoGasto_ID", "TGA_Descripcion");
            ViewBag.Cuenta_ID = new SelectList(GetCuentas(0), "Cuenta_ID", "CTA_Alias");
            return View(new ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosProgramados() { GPR_FechaAplicacion = DateTime.Now.AddDays(1) });
        }

        [Logger]
        public ActionResult Editar(int GastoProgramado_ID)
        {
            try { this.GastosProgramadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJEGR_GastosProgramados = GastosProgramadosProxy.GetByID(GastoProgramado_ID);

            if (OBJEGR_GastosProgramados == null)
            {
                return HttpNotFound();
            }

            ViewBag.TipoGasto_ID = new SelectList(GetTipoGastos(0), "TipoGasto_ID", "TGA_Descripcion", OBJEGR_GastosProgramados.TipoGasto_ID);
            ViewBag.Cuenta_ID = new SelectList(GetCuentas(0), "Cuenta_ID", "CTA_Alias", OBJEGR_GastosProgramados.Cuenta_ID);

            if (OBJEGR_GastosProgramados.GPR_Aplicado)
                return View("Detalle", OBJEGR_GastosProgramados);
            else
                return View(OBJEGR_GastosProgramados);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosProgramados Registro)
        {
            try { this.GastosProgramadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (Registro.TipoGasto_ID <= 0)
            {
                ModelState.AddModelError("TipoGasto_ID", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Registro.GPR_Descripcion))
            {
                ModelState.AddModelError("GPR_Descripcion", "Campo Requerido");
            }

            if (Registro.GPR_Monto <= 0)
            {
                ModelState.AddModelError("GPR_Monto", "Campo Requerido");
            }

             if (Registro.Cuenta_ID <= 0)
             {
                 ModelState.AddModelError("Cuenta_ID", "Campo Requerido");
             }

            if (Registro.GPR_Recurrente == false)
            {
                if (Registro.GPR_FechaAplicacion == null || Registro.GPR_FechaAplicacion.Date <= DateTime.Now.Date)
                    ModelState.AddModelError("GPR_FechaAplicacion", "La fecha debe ser mayor o igual a " + DateTime.Now.AddDays(1).ToString("dd-MMM-yyyy"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJEGR_GastosProgramados = new ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosProgramados();
                    OBJEGR_GastosProgramados = GastosProgramadosProxy.Crear(Registro);
                    if (OBJEGR_GastosProgramados.GastoProgramado_ID == 0)
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

            ViewBag.TipoGasto_ID = new SelectList(GetTipoGastos(0), "TipoGasto_ID", "TGA_Descripcion", OBJEGR_GastosProgramados.TipoGasto_ID);
            ViewBag.Cuenta_ID = new SelectList(GetCuentas(0), "Cuenta_ID", "CTA_Alias", OBJEGR_GastosProgramados.Cuenta_ID);
            return View(Registro);
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Egresos.Models.EGR_GastosProgramados Registro)
        {
            try { this.GastosProgramadosProxy = new ADESCOMBUSINESS.Areas.Egresos.Methods.GastosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (Registro.TipoGasto_ID <= 0)
            {
                ModelState.AddModelError("TipoGasto_ID", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Registro.GPR_Descripcion))
            {
                ModelState.AddModelError("GPR_Descripcion", "Campo Requerido");
            }

            if (Registro.GPR_Monto <= 0)
            {
                ModelState.AddModelError("GPR_Monto", "Campo Requerido");
            }

             if (Registro.Cuenta_ID <= 0)
             {
                 ModelState.AddModelError("Cuenta_ID", "Campo Requerido");
             }

            if (Registro.GPR_Recurrente == false)
            {
                if (Registro.GPR_FechaAplicacion == null || Registro.GPR_FechaAplicacion.Date <= DateTime.Now.Date)
                    ModelState.AddModelError("GPR_FechaAplicacion", "La fecha debe ser mayor o igual a " + DateTime.Now.AddDays(1).ToString("dd-MMM-yyyy"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = GastosProgramadosProxy.Editar(Registro);
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

            ViewBag.TipoGasto_ID = new SelectList(GetTipoGastos(0), "TipoGasto_ID", "TGA_Descripcion", OBJEGR_GastosProgramados.TipoGasto_ID);
            ViewBag.Cuenta_ID = new SelectList(GetCuentas(0), "Cuenta_ID", "CTA_Alias", OBJEGR_GastosProgramados.Cuenta_ID);
            return View(Registro);
        }

         private List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> GetCuentas(int Cuenta_ID)
         {
             List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> Cuentas = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas>();
             Cuentas.Add(new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas() { Cuenta_ID = 0, CTA_Alias = "Elija Una Opcion" });
             Cuentas.AddRange(ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentasEmpresa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta_ID));
             return Cuentas;
         }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos> GetTipoGastos(int TipoGasto_ID)
        {
            ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness TipoGastosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoGastosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos> TGastos = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos>();
            TGastos.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoGastos() { TipoGasto_ID = 0, TGA_Descripcion = "Elija Una Opcion" });
            TGastos.AddRange(TipoGastosProxy.GetActive(TipoGasto_ID));
            return TGastos;
        }
    }
}