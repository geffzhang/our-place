using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Ingresos.Controllers
{
    public class RegistroPagosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness PagosRealizadosProxy;
        protected ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados OBJING_PagosRealizados = new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados();

        [Logger]
        public ActionResult Index()
        {
            //try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            //catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados> Lista = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados>();
            //Lista = PagosRealizadosProxy.GetAplicados();

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");
            ViewBag.FindTipoFecha = new SelectList(GetTiposFecha(), "Option", "Description");

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_IDFilter = new SelectList(GetLlave1(0, "(Todas)"), "Llave1_ID", "LL1_Descripcion");
                    return View("Index1", Lista);
                case 2:
                    ViewBag.Llave1_IDFilter = new SelectList(GetLlave1(0, "(Todas)"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_IDFilter = new SelectList(GetLlave2(0, "(Todas)"), "Llave2_ID", "LL2_Descripcion");
                    return View("Index2", Lista);
                case 3:
                    ViewBag.Llave1_IDFilter = new SelectList(GetLlave1(0, "(Todas)"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_IDFilter = new SelectList(GetLlave2(0, "(Todas)"), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_IDFilter = new SelectList(GetLlave3(0, 0, "(Todas)"), "Llave3_ID", "LL3_Descripcion");
                    return View("Index3", Lista);
            }

            return null;
        }

        public ActionResult RefreshDataSearchList(FormCollection Form)
        {
            int searchLL1 = Convert.ToInt32(Form["Llave1_IDFilter"]);
            int searchLL2 = Convert.ToInt32(Form["Llave2_IDFilter"]);
            int searchLL3 = Convert.ToInt32(Form["Llave3_IDFilter"]);
            int Estatus = Convert.ToInt32(Form["FindEstatus"]);
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

            int searchTFecha = Convert.ToInt32(Form["FindTipoFecha"]);

            String sarchRef = Form["FindReferencia"];

            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados> Lista = new List<ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados>();
            Lista = PagosRealizadosProxy.GetByFilters(searchLL1, searchLL2, searchLL3, Estatus, sarchRef, searchFDesde, searchFHasta, searchTFecha);

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
        public ActionResult Detalle(int PagoRealizado_ID)
        {
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados PagoRealizado = new ADESCOMBUSINESS.Areas.Ingresos.Models.VwING_PagosRealizados();
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
        public ActionResult Crear()
        {
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            ViewBag.Cuenta_ID = new SelectList(GetCuentas(0), "Cuenta_ID", "CTA_Alias");
            ViewBag.ComprobanteName = "";
            ViewBag.ComprobanteFile = "";

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    return View("Crear1", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados() { PRE_FechaPago = DateTime.Now });
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "Elija una opción"), "Llave2_ID", "LL2_Descripcion");
                    return View("Crear2", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados() { PRE_FechaPago = DateTime.Now });
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "Elija una opción"), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0, 0, "Elija una opción"), "Llave3_ID", "LL3_Descripcion");
                    return View("Crear3", new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados() { PRE_FechaPago = DateTime.Now });
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
            ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados Registro = new ADESCOMBUSINESS.Areas.Ingresos.Models.ING_PagosRealizados();

            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

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

            if (string.IsNullOrEmpty(Form["PRE_Monto"]) || Form["PRE_Monto"] == "0")
            {
                ModelState.AddModelError("PRE_Monto", "Campo Requerido");
            }
            else
            {
                if(Convert.ToDecimal(Form["PRE_Monto"]) < 0)
                    ModelState.AddModelError("PRE_Monto", "Debe ser mayor que cero");
                else
                    Registro.PRE_Monto = Convert.ToDecimal(Form["PRE_Monto"]);
            }

            if (string.IsNullOrEmpty(Form["PRE_Referencia"]))
            {
                ModelState.AddModelError("PRE_Referencia", "Campo Requerido");
            }
            else
            {
                Registro.PRE_Referencia = Form["PRE_Referencia"];
            }

            if (string.IsNullOrEmpty(Form["Cuenta_ID"]) || Form["Cuenta_ID"] == "0")
            {
                ModelState.AddModelError("Cuenta_ID", "Campo Requerido");
            }
            else
            {
                Registro.Cuenta_ID = Convert.ToInt32(Form["Cuenta_ID"]);
            }

            if (string.IsNullOrEmpty(Form["PRE_FechaPago"]))
            {
                ModelState.AddModelError("PRE_FechaPago", "Campo Requerido");
            }
            else
            {
                Registro.PRE_FechaPago = Convert.ToDateTime(Form["PRE_FechaPago"]);
            }

            //Comprobante
            if (!String.IsNullOrEmpty(Form["SavedFile"]))
            {
                Registro.PRE_Comprobante = Form["SavedFile"];
            }

            if (ModelState.IsValid)
            {
                try
                {
                    PagosRealizadosProxy.Crear(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.Cuenta_ID = new SelectList(GetCuentas(0), "Cuenta_ID", "CTA_Alias", Registro.Cuenta_ID);
            ViewBag.ComprobanteName = Form["fileName"];
            ViewBag.ComprobanteFile = Form["SavedFile"];

            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Llave1);
                    return View("Crear1", Registro);
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Llave1);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(Llave2, "Elija una opción"), "Llave2_ID", "LL2_Descripcion", Llave2);
                    return View("Crear2", Registro);
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Llave1);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(Llave1, "Elija una opción"), "Llave2_ID", "LL2_Descripcion", Llave2);
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(Llave1, Llave2, "Elija una opción"), "Llave3_ID", "LL3_Descripcion");
                    return View("Crear3", Registro);
            }

            return null;
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetEstatuses()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "Aplicados" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "Cancelados" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "Rechazados" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 3, Description = "Todos" });
            return SolEstatus_ID;
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetTiposFecha()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "Fecha de Pago" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "Fecha de Ingreso" });
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

        private List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> GetCuentas(int Cuenta_ID)
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> CuentasDisp = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas>();
            CuentasDisp = ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentasEmpresa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta_ID);

            if (CuentasDisp.Count == 1)
            {
                CuentasDisp.Add(new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas() { Cuenta_ID = CuentasDisp[0].Cuenta_ID, CTA_Alias = CuentasDisp[0].CTA_Alias });
            }
            else
            {
                CuentasDisp.Add(new ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas() { Cuenta_ID = 0, CTA_Alias = "Elija Una Opcion" });
                CuentasDisp.AddRange(CuentasDisp);
            }
            return CuentasDisp;
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
            if(Convert.ToBoolean(parametro.CPA_Valor) == true)
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

        [Logger]
        public ActionResult CancelarPago(int PagoRealizado_ID)
        {
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJING_PagosRealizados = PagosRealizadosProxy.GetByID(PagoRealizado_ID);

            if (OBJING_PagosRealizados == null)
            {
                return HttpNotFound();
            }

            return View(OBJING_PagosRealizados);
        }

        [HttpPost, ActionName("CancelarPago")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult CancelarPagoConfirmed(int PagoRealizado_ID)
        {
            try { this.PagosRealizadosProxy = new ADESCOMBUSINESS.Areas.Ingresos.Methods.PagosRealizadosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJING_PagosRealizados = PagosRealizadosProxy.GetByID(PagoRealizado_ID);

            if (OBJING_PagosRealizados == null)
            {
                return HttpNotFound();
            }

            try
            {
                bool Status = PagosRealizadosProxy.CancelarPago(PagoRealizado_ID);
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

            return View(OBJING_PagosRealizados);
        }
    }
}