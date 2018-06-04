using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Anuncios.Controllers
{
    public class AutAnunciosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness AnunciosProxy;
        protected ADESCOMBUSINESS.Areas.Anuncios.Models.ANU_Anuncios OBJANU_Anuncios = new ADESCOMBUSINESS.Areas.Anuncios.Models.ANU_Anuncios();

        [Logger]
        public ActionResult Index()
        {
            try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Anuncios> Lista = new List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Anuncios>();
            Lista = AnunciosProxy.GetUnauthorized();
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Anuncios> Lista = new List<ADESCOMBUSINESS.Areas.Anuncios.Models.VwANU_Anuncios>();
            Lista = AnunciosProxy.GetUnauthorized();
            return View(Lista);
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

            return View(AnuncioCompuesto);
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

        public ActionResult Rechazar(int Anuncio_ID)
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

        [HttpPost, ActionName("Rechazar")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult RechazarConfirmed(ADESCOMBUSINESS.Areas.Anuncios.Models.ANU_Anuncios Registro)
        {
            if (String.IsNullOrEmpty(Registro.ANU_MotivoRechazo))
            {
                ModelState.AddModelError("ANU_MotivoRechazo", "Campo requerido");
            }

            if (ModelState.IsValid)
            {
                try { this.AnunciosProxy = new ADESCOMBUSINESS.Areas.Anuncios.Methods.AnunciosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
                catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

                //Rechazar anuncio
                try
                {
                    ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
                    bool Status = AnunciosProxy.RechazarAnuncio(Registro, CompanyInfo);
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
    }
}