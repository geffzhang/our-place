using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Residentes.Controllers
{
    public class ResidentesController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness ResidentesProxy;
        protected ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes OBJRES_Residentes = new ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes();

        [Logger]
        public ActionResult Index()
        {
            //try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            //catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> Lista = new List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes>();
            //Lista = ResidentesProxy.GetActive(0);

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;
            ViewBag.FindEstatus = new SelectList(GetEstatuses(), "Option", "Description");

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
            String searchName = Form["FindName"];
            String searchEmail = Form["FindEmail"];
            int Estatus = Convert.ToInt32(Form["FindEstatus"]);

            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> Lista = new List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes>();
            Lista = ResidentesProxy.GetByFilters(searchLL1, searchLL2, searchLL3, searchName, searchEmail, Estatus);

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

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    return View("Crear1", new ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes() { BS_Activo = true});
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "Elija una opción"), "Llave2_ID", "LL2_Descripcion");
                    return View("Crear2", new ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes() { BS_Activo = true });
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "Elija una opción"), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0, 0, "Elija una opción"), "Llave3_ID", "LL3_Descripcion");
                    return View("Crear3", new ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes() { BS_Activo = true });
            }

            return null;
        }

        [Logger]
        public ActionResult Editar(int Residente_ID)
        {
            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJRES_Residentes = ResidentesProxy.GetByID(Residente_ID);

            if (OBJRES_Residentes == null)
            {
                return HttpNotFound();
            }

            ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();
            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness DireccionesProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            Direccion = DireccionesProxy.GetByID(OBJRES_Residentes.Direccion_ID);

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Direccion.Llave1_ID);
                    return View("Editar1", OBJRES_Residentes);
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Direccion.Llave1_ID);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(Direccion.Llave1_ID, "Elija una opción"), "Llave2_ID", "LL2_Descripcion", Direccion.Llave2_ID);
                    return View("Editar2", OBJRES_Residentes);
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Direccion.Llave1_ID);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(Direccion.Llave1_ID, "Elija una opción"), "Llave2_ID", "LL2_Descripcion", Direccion.Llave2_ID);
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(Direccion.Llave1_ID, Direccion.Llave2_ID, "Elija una opción"), "Llave3_ID", "LL3_Descripcion", Direccion.Llave3_ID);
                    return View("Editar3", OBJRES_Residentes);
            }

            return null;
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(FormCollection Form)
        {
            int Llave1 = 0;
            int Llave2 = 0;
            int Llave3 = 0;

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            int cantLlaves = CompanyInfo.CantLlaves;

            ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();
            ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes Registro = new ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes();

            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (string.IsNullOrEmpty(Form["Llave1_ID"]) || Form["Llave1_ID"] == "0")
            {
                ModelState.AddModelError("Llave1_ID", "Campo Requerido");
                ViewBag.Error = "No ingresó " + CompanyInfo.LabelLlave1;
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
                    ViewBag.Error = "No ingresó " + CompanyInfo.LabelLlave2;
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

            String activo = Form["BS_Activo"];
            Registro.BS_Activo = activo.Contains("true") ? true : false;
            Registro.Residente_ID = Convert.ToInt32(Form["Residente_ID"]);
            Registro.CiaResidente_ID = Convert.ToInt32(Form["CiaResidente_ID"]);
            Registro.RES_Telefono = Form["RES_Telefono"].ToString(); //Opcional, si viene en blanco, se guarda en blanco
            Registro.RES_Movil = Form["RES_Movil"].ToString(); //Opcional, si viene en blanco, se guarda en blanco

            if (string.IsNullOrEmpty(Form["RES_RelDuenio"]))
            {
                ModelState.AddModelError("RES_RelDuenio", "Campo Requerido");
            }
            else
            {
                Registro.RES_RelDuenio = Form["RES_RelDuenio"];
            }

            if (string.IsNullOrEmpty(Form["RES_Email"]))
            {
                ModelState.AddModelError("RES_Email", "Campo Requerido");
            }
            else
            {
                Registro.RES_Email = Form["RES_Email"];
            }

            if (string.IsNullOrEmpty(Form["RES_Nombre"]))
            {
                ModelState.AddModelError("RES_Nombre", "Campo Requerido");
            }
            else
            {
                Registro.RES_Nombre = Form["RES_Nombre"];
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ResidentesProxy.Editar(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Llave1);
                    return View("Editar1", Registro);
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Llave1);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(Llave2, "Elija una opción"), "Llave2_ID", "LL2_Descripcion", Llave2);
                    return View("Editar2", Registro);
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Llave1);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(Llave1, "Elija una opción"), "Llave2_ID", "LL2_Descripcion", Llave2);
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(Llave1, Llave2, "Elija una opción"), "Llave3_ID", "LL3_Descripcion", Direccion.Llave3_ID);
                    return View("Editar3", Registro);
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

            ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();
            ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes Registro = new ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes();
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            int cantLlaves = CompanyInfo.CantLlaves;

            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (string.IsNullOrEmpty(Form["Llave1_ID"]) || Form["Llave1_ID"] == "0")
            {
                ModelState.AddModelError("Llave1_ID", "Campo Requerido");
                ViewBag.Error = "No ingresó " + CompanyInfo.LabelLlave1;
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
                    ViewBag.Error = "No ingresó " + CompanyInfo.LabelLlave2;
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
                //Agregar un error pero no a un campo en especifico
                ModelState.AddModelError("Llave1_ID", "Campo Requerido");
                ViewBag.Error = "No existe la dirección o está desactivada";
            }
            else
            {
                Registro.Direccion_ID = Direccion.Direccion_ID;
            }

            String activo = Form["BS_Activo"];
            Registro.BS_Activo = activo.Contains("true") ? true : false;
            Registro.RES_Telefono = Form["RES_Telefono"].ToString(); //Opcional, si viene en blanco, se guarda en blanco
            Registro.RES_Movil = Form["RES_Movil"].ToString(); //Opcional, si viene en blanco, se guarda en blanco

            if (string.IsNullOrEmpty(Form["RES_RelDuenio"]))
            {
                ModelState.AddModelError("RES_RelDuenio", "Campo Requerido");
            }
            else
            {
                Registro.RES_RelDuenio = Form["RES_RelDuenio"];
            }

            if (string.IsNullOrEmpty(Form["RES_Email"]))
            {
                ModelState.AddModelError("RES_Email", "Campo Requerido");
            }
            else
            {
                Registro.RES_Email = Form["RES_Email"];
            }

            if (string.IsNullOrEmpty(Form["RES_Nombre"]))
            {
                ModelState.AddModelError("RES_Nombre", "Campo Requerido");
            }
            else
            {
                Registro.RES_Nombre = Form["RES_Nombre"];
            }

            if (string.IsNullOrEmpty(Form["RES_Nombre"]))
            {
                ModelState.AddModelError("RES_Nombre", "Campo Requerido");
            }
            else
            {
                Registro.RES_Nombre = Form["RES_Nombre"];
            }

            if (string.IsNullOrEmpty(Form["RES_Password"]))
            {
                ModelState.AddModelError("RES_Password", "Campo Requerido");
            }
            else
            {
                Registro.RES_Password = Form["RES_Password"];
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ResidentesProxy.Crear(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

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
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(Llave1, Llave2, "Elija una opción"), "Llave3_ID", "LL3_Descripcion", Direccion.Llave3_ID);
                    return View("Crear3", Registro);
            }

            return null;
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

        [Logger]
        public ActionResult GetResidentes(int Llave1_ID, int Llave2_ID, int Llave3_ID)
        {
            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones dir = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            int cantLlaves = CompanyInfo.CantLlaves;

            if (cantLlaves == 1)
                dir = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID);

            if (cantLlaves == 2)
                dir = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves2((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID, Llave2_ID);

            if (cantLlaves == 3)
                dir = ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness.GetByLlaves3((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID, Llave2_ID, Llave3_ID);

            List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> ListaResidentes = new List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes>();
            ListaResidentes = ResidentesProxy.GetByDireccion_ID(dir.Direccion_ID);

            ViewBag.Direccion = dir.DIR_Descripcion;
            return View(ListaResidentes);
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