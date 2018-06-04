using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Anuncios.Controllers
{
    public class AnunciosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness AnunciosProxy;
        protected ADESCOMBUSINESS.Areas.Anuncios.Models.ANU_Anuncios OBJANU_Anuncios = new ADESCOMBUSINESS.Areas.Anuncios.Models.ANU_Anuncios();

        [Logger]
        public ActionResult Index()
        {
            //try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            //catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Anuncios> Lista = new List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Anuncios>();
            //Lista = AnunciosProxy.GetActive();

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
                    ViewBag.Llave3_IDFilter = new SelectList(GetLlave3(0), "Llave3_ID", "LL3_Descripcion");
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

            try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Anuncios> Lista = new List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Anuncios>();
            Lista = AnunciosProxy.GetByFilters(searchLL1, searchLL2, searchLL3, Estatus);

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
        public ActionResult Editar(int Anuncio_ID)
        {
            try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Anuncios.Models.ANU_Anuncios Anuncio = new ADESCOMBUSINESS.Areas.Anuncios.Models.ANU_Anuncios();
            Anuncio = AnunciosProxy.GetByID(Anuncio_ID);

            List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Imagenes> ListaImagenes = new List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Imagenes>();
            ListaImagenes = ADESCOMBUSINESS.Areas.Anuncios.Methods.ImagenesBusiness.GetByAnuncio_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Anuncio.Anuncio_ID);

            ADESCOMBUSINESS.Areas.Anuncios.Models.AnuncioCompuesto AnuncioCompuesto = new ADESCOMBUSINESS.Areas.Anuncios.Models.AnuncioCompuesto();
            AnuncioCompuesto.Anuncio = Anuncio;
            AnuncioCompuesto.Imagenes = ListaImagenes;

            //ViewBags
            ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes Residente = new ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes();
            Residente = ResidentesProxy.GetByID(Anuncio.Residente_ID);

            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness DireccionProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();
            ViewBag.Direccion = DireccionProxy.GetByID(Residente.Direccion_ID).DIR_Descripcion;

            ViewBag.TipoTrans = new SelectList(GetTipos(), "Option", "Description", Anuncio.ANU_Tipo);
            ViewBag.Categoria = new SelectList(GetCategorias(), "Option", "Description", Anuncio.ANU_Categoria);

            if (Anuncio.ANU_Estatus.Equals("AUT"))
            {
                return View(AnuncioCompuesto);
            }
            else
            {
                return View("Detalle", AnuncioCompuesto);
            }
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Anuncios.Models.AnuncioCompuesto Registro)
        {
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            //Autorizar Anuncio
            try
            {
                AnunciosProxy.AutorizarAnuncio(Registro.Anuncio, CompanyInfo);
                ViewBag.Error = "OK";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            //ViewBags
            ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes Residente = new ADESCOMBUSINESS.Areas.Residentes.Models.RES_Residentes();
            Residente = ResidentesProxy.GetByID(Registro.Anuncio.Residente_ID);

            ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness DireccionProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_DireccionesBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TVI_Direcciones();
            ViewBag.Direccion = DireccionProxy.GetByID(Residente.Direccion_ID).DIR_Descripcion;

            ViewBag.TipoTrans = new SelectList(GetTipos(), "Option", "Description", Registro.Anuncio.ANU_Tipo);
            ViewBag.Categoria = new SelectList(GetCategorias(), "Option", "Description", Registro.Anuncio.ANU_Categoria);

            return View(Registro);
        }

        public ActionResult Finalizar(int Anuncio_ID)
        {
            try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJANU_Anuncios = AnunciosProxy.GetByID(Anuncio_ID);

            if (OBJANU_Anuncios == null)
            {
                return HttpNotFound();
            }

            return View(OBJANU_Anuncios);
        }

        [HttpPost, ActionName("Finalizar")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult FinalizarConfirmed(ADESCOMBUSINESS.Areas.Anuncios.Models.ANU_Anuncios Registro)
        {
            if (ModelState.IsValid)
            {
                try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
                catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

                //Finalizar anuncio
                try
                {
                    bool Status = AnunciosProxy.FinalizarAnuncio(Registro);
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

        private List<ADESCOM.Models.FixedOption> GetTipos()
        {
            List<ADESCOM.Models.FixedOption> Tipos = new List<ADESCOM.Models.FixedOption>();
            Tipos.Add(new ADESCOM.Models.FixedOption() { Option = "UNK", Description = "Elija Una Opcion" });
            Tipos.Add(new ADESCOM.Models.FixedOption() { Option = "REN", Description = "Renta" });
            Tipos.Add(new ADESCOM.Models.FixedOption() { Option = "VEN", Description = "Venta" });
            return Tipos;
        }

        private List<ADESCOM.Models.FixedOption> GetCategorias()
        {
            List<ADESCOM.Models.FixedOption> Categorias = new List<ADESCOM.Models.FixedOption>();
            Categorias.Add(new ADESCOM.Models.FixedOption() { Option = "UNK", Description = "Elija Una Opcion" });
            Categorias.Add(new ADESCOM.Models.FixedOption() { Option = "CAS", Description = "Casas" });
            Categorias.Add(new ADESCOM.Models.FixedOption() { Option = "VEH", Description = "Vehículos" });
            Categorias.Add(new ADESCOM.Models.FixedOption() { Option = "MUE", Description = "Muebles" });
            Categorias.Add(new ADESCOM.Models.FixedOption() { Option = "FIE", Description = "Fiestas" });
            Categorias.Add(new ADESCOM.Models.FixedOption() { Option = "HOG", Description = "Servicios del Hogar" });
            Categorias.Add(new ADESCOM.Models.FixedOption() { Option = "SER", Description = "Servicios Profesionales" });
            Categorias.Add(new ADESCOM.Models.FixedOption() { Option = "VAR", Description = "Varios" });
            return Categorias;
        }

        [Logger]
        private List<ADESCOM.Models.FixedOption> GetEstatuses()
        {
            List<ADESCOM.Models.FixedOption> SolEstatus_ID = new List<ADESCOM.Models.FixedOption>();
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 0, Description = "Activos" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 1, Description = "Finalizado" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 2, Description = "Rechazado" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 3, Description = "Por autorizar" });
            SolEstatus_ID.Add(new ADESCOM.Models.FixedOption() { Option = 4, Description = "Todos" });
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