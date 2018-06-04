using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Contabilidad.Controllers
{
    public class CuentasController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness CON_CuentasProxy;
        protected ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas OBJCON_Cuentas = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas();

        public ActionResult Index()
        {
            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas>();
            Lista = CuentasProxy.GetByOrigin_Type("EMP");
            return View(Lista);
        }

        public ActionResult RefreshData()
        {
            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas>();
            Lista = CuentasProxy.GetByOrigin_Type("EMP");
            return View(Lista);
        }

        public ActionResult SearchList(bool CtasCliente, bool CtasEmpresa, bool CtasCreditos)
        {
            ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> Lista = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas>();
            Lista = CuentasProxy.GetByFilter(CtasCliente, CtasEmpresa, CtasCreditos);
            return View(Lista);
        }

        public ActionResult Detalle(int Cuenta_ID)
        {
            try { this.CON_CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCON_Cuentas = CON_CuentasProxy.GetByID(Cuenta_ID);

            if (OBJCON_Cuentas == null)
            {
                return HttpNotFound();
            }

            return View(OBJCON_Cuentas);
        }

        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas() { CTA_OrigenTipo = "EMP" });
        }

        public ActionResult Editar(int Cuenta_ID)
        {
            try { this.CON_CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCON_Cuentas = CON_CuentasProxy.GetByID(Cuenta_ID);

            if (OBJCON_Cuentas == null)
            {
                return HttpNotFound();
            }

            return View(OBJCON_Cuentas);
        }

        public ActionResult Eliminar(int Cuenta_ID)
        {
            try { this.CON_CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJCON_Cuentas = CON_CuentasProxy.GetByID(Cuenta_ID);

            if (OBJCON_Cuentas == null)
            {
                return HttpNotFound();
            }
            return View(OBJCON_Cuentas);
        }

        [HttpPost]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas Registro)
        {
            try { this.CON_CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.CTA_Alias))
            {
                ModelState.AddModelError("CTA_Alias", "Campo requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.CTA_OrigenTipo = "EMP";
                    Registro.BS_Activo = true;
                    OBJCON_Cuentas = new ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas();
                    OBJCON_Cuentas = CON_CuentasProxy.Crear(Registro);
                    if (OBJCON_Cuentas.Cuenta_ID == 0)
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
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Contabilidad.Models.CON_Cuentas Registro)
        {
            try { this.CON_CuentasProxy = new ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.CTA_Alias))
            {
                ModelState.AddModelError("CTA_Alias", "Campo requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = CON_CuentasProxy.Editar(Registro);
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
    }
}