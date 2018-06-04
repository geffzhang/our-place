using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Ingresos.Controllers
{
    public class CatCargosProgramadosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Ingresos.Methods.CargosProgramadosBusiness CargosProgramadosProxy;
        protected ADESCOMBUSINESS.Areas.Ingresos.Models.ING_CargosProgramados OBJING_CargosProgramados = new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_CargosProgramados();

        [Logger]
        public ActionResult Index()
        {
            try { this.CargosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.CargosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_CargosProgramados> Lista = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_CargosProgramados>();
            Lista = CargosProgramadosProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.CargosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.CargosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_CargosProgramados> Lista = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_CargosProgramados>();
            Lista = CargosProgramadosProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult Crear()
        {
            ViewBag.TipoCargo_ID = new SelectList(GetTipoCargos(0), "TipoCargo_ID", "TCA_Descripcion");
            return View(new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_CargosProgramados() { CPR_FechaAplicacion = DateTime.Now.AddDays(1) });
        }

        [Logger]
        public ActionResult Editar(int CargoProgramado_ID)
        {
            try { this.CargosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.CargosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJING_CargosProgramados = CargosProgramadosProxy.GetByID(CargoProgramado_ID);

            if (OBJING_CargosProgramados == null)
            {
                return HttpNotFound();
            }

            ViewBag.TipoCargo_ID = new SelectList(GetTipoCargos(0), "TipoCargo_ID", "TCA_Descripcion", OBJING_CargosProgramados.TipoCargo_ID);

            if(OBJING_CargosProgramados.CPR_Aplicado)
                return View("Detalle", OBJING_CargosProgramados);
            else
                return View(OBJING_CargosProgramados);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Ingresos.Models.ING_CargosProgramados Registro)
        {
            try { this.CargosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.CargosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (Registro.TipoCargo_ID <= 0)
            {
                ModelState.AddModelError("TipoCargo_ID", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Registro.CPR_Descripcion))
            {
                ModelState.AddModelError("CPR_Descripcion", "Campo Requerido");
            }

            if (Registro.CPR_Monto <= 0)
            {
                ModelState.AddModelError("CPR_Monto", "Campo Requerido");
            }

            if (Registro.CPR_Recurrente == false)
            {
                if (Registro.CPR_FechaAplicacion == null || Registro.CPR_FechaAplicacion.Date <= DateTime.Now.Date)
                    ModelState.AddModelError("CPR_FechaAplicacion", "La fecha debe ser mayor o igual a " + DateTime.Now.AddDays(1).ToString("dd-MMM-yyyy"));
            }

            //GHO 04/Feb/2017 Loma del Valle II
            if (Registro.CPR_CausaRecargos)
            {
                if (Registro.CPR_GraciaRecargos <= 0)
                {
                    ModelState.AddModelError("CPR_GraciaRecargos", "Campo Requerido");
                }

                if (Registro.CPR_MontoRecargos <= 0)
                {
                    ModelState.AddModelError("CPR_MontoRecargos", "Campo Requerido");
                }
            }
            else
            {
                Registro.CPR_GraciaRecargos = 0;
                Registro.CPR_MontoRecargos = 0;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJING_CargosProgramados = new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_CargosProgramados();
                    OBJING_CargosProgramados = CargosProgramadosProxy.Crear(Registro);
                    if (OBJING_CargosProgramados.CargoProgramado_ID == 0)
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

            ViewBag.TipoCargo_ID = new SelectList(GetTipoCargos(0), "TipoCargo_ID", "TCA_Descripcion");
            return View(Registro);
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Ingresos.Models.ING_CargosProgramados Registro)
        {
            try { this.CargosProgramadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.CargosProgramadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (Registro.TipoCargo_ID <= 0)
            {
                ModelState.AddModelError("TipoCargo_ID", "Campo Requerido");
            }

            if (String.IsNullOrEmpty(Registro.CPR_Descripcion))
            {
                ModelState.AddModelError("CPR_Descripcion", "Campo Requerido");
            }

            if (Registro.CPR_Monto <= 0)
            {
                ModelState.AddModelError("CPR_Monto", "Campo Requerido");
            }

            if (Registro.CPR_Recurrente == false)
            {
                if(Registro.CPR_FechaAplicacion == null || Registro.CPR_FechaAplicacion.Date <= DateTime.Now.Date)
                    ModelState.AddModelError("CPR_FechaAplicacion", "La fecha debe ser mayor o igual a " + DateTime.Now.AddDays(1).ToString("dd-MMM-yyyy"));
            }

            //GHO 04/Feb/2017 Loma del Valle II
            if (Registro.CPR_CausaRecargos)
            {
                if (Registro.CPR_GraciaRecargos <= 0)
                {
                    ModelState.AddModelError("CPR_GraciaRecargos", "Campo Requerido");
                }

                if (Registro.CPR_MontoRecargos <= 0)
                {
                    ModelState.AddModelError("CPR_MontoRecargos", "Campo Requerido");
                }
            }
            else
            {
                Registro.CPR_GraciaRecargos = 0;
                Registro.CPR_MontoRecargos = 0;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = CargosProgramadosProxy.Editar(Registro);
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

            ViewBag.TipoCargo_ID = new SelectList(GetTipoCargos(0), "TipoCargo_ID", "TCA_Descripcion", OBJING_CargosProgramados.TipoCargo_ID);
            return View(Registro);
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos> GetTipoCargos(int TipoCargo_ID)
        {
            ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness TipoCargosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.CAT_TipoCargosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos> TCargos = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos>();
            TCargos.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwCAT_TipoCargos() { TipoCargo_ID = 0, TCA_Descripcion = "Elija Una Opcion" });
            TCargos.AddRange(TipoCargosProxy.GetUserActive(TipoCargo_ID));
            return TCargos;
        }
    }
}