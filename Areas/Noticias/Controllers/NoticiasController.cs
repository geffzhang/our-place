using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Noticias.Controllers
{
    public class NoticiasController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Noticias.Methods.NoticiasBusiness NoticiasProxy;
        protected ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias OBJNEW_Noticias = new ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias();

        [Logger]
        public ActionResult Index()
        {
            try { this.NoticiasProxy = new ADESCOMBUSINESS.Areas.Noticias.Methods.NoticiasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Noticias.Models.VwNEW_Noticias> Lista = new List<ADESCOMBUSINESS.Areas.Noticias.Models.VwNEW_Noticias>();
            Lista = NoticiasProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.NoticiasProxy = new ADESCOMBUSINESS.Areas.Noticias.Methods.NoticiasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Noticias.Models.VwNEW_Noticias> Lista = new List<ADESCOMBUSINESS.Areas.Noticias.Models.VwNEW_Noticias>();
            Lista = NoticiasProxy.GetActive(0);
            return View(Lista);
        }

        [Logger]
        public ActionResult Detalle(int Noticia_ID)
        {
            try { this.NoticiasProxy = new ADESCOMBUSINESS.Areas.Noticias.Methods.NoticiasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            OBJNEW_Noticias = new ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias();
            OBJNEW_Noticias = NoticiasProxy.GetByID(Noticia_ID);

            List<ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Imagenes> ListaImagenes = new List<ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Imagenes>();
            ListaImagenes = ADESCOMBUSINESS.Areas.Noticias.Methods.ImagenesBusiness.GetByNoticia_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Noticia_ID);

            ADESCOMBUSINESS.Areas.Noticias.Models.NoticiaCompuesta NoticiaCompuesta = new ADESCOMBUSINESS.Areas.Noticias.Models.NoticiaCompuesta();
            NoticiaCompuesta.Noticia = OBJNEW_Noticias;
            NoticiaCompuesta.Imagenes = new List<ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Imagenes>();
            NoticiaCompuesta.Imagenes = ListaImagenes;
            return View(NoticiaCompuesta);
        }

        [Logger]
        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias());
        }

        [Logger]
        public ActionResult Editar(int Noticia_ID)
        {
            try { this.NoticiasProxy = new ADESCOMBUSINESS.Areas.Noticias.Methods.NoticiasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            OBJNEW_Noticias = new ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias();
            OBJNEW_Noticias = NoticiasProxy.GetByID(Noticia_ID);

            List<ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Imagenes> ListaImagenes = new List<ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Imagenes>();
            ListaImagenes = ADESCOMBUSINESS.Areas.Noticias.Methods.ImagenesBusiness.GetByNoticia_ID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Noticia_ID);

            if (ListaImagenes.Count > 0)
            {
                ViewBag.Imagen1 = ListaImagenes[0].IMG_URL;
                if(ListaImagenes.Count > 1)
                    ViewBag.Imagen2 = ListaImagenes[1].IMG_URL;
                if(ListaImagenes.Count > 2)
                    ViewBag.Imagen3 = ListaImagenes[2].IMG_URL;
                if (ListaImagenes.Count > 3)
                    ViewBag.Imagen4 = ListaImagenes[3].IMG_URL;
                if (ListaImagenes.Count > 4)
                    ViewBag.Imagen5 = ListaImagenes[4].IMG_URL;
            }
            return View(OBJNEW_Noticias);
        }

        [HttpPost]
        [Logger]
        public ActionResult Crear(FormCollection Form)
        {
            ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias Registro = new ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias();
            List<String> Imagenes = new List<string>();
            try { this.NoticiasProxy = new ADESCOMBUSINESS.Areas.Noticias.Methods.NoticiasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Form["NEW_Titulo"]))
            {
                ModelState.AddModelError("NEW_Titulo", "Campo requerido");
            }

            if (String.IsNullOrEmpty(Form["NEW_Detalles"]))
            {
                ModelState.AddModelError("NEW_Detalles", "Campo requerido");
            }

            if (!String.IsNullOrEmpty(Form["SavedImage1"])) Imagenes.Add(Form["SavedImage1"]);
            if (!String.IsNullOrEmpty(Form["SavedImage2"])) Imagenes.Add(Form["SavedImage2"]);
            if (!String.IsNullOrEmpty(Form["SavedImage3"])) Imagenes.Add(Form["SavedImage3"]);
            if (!String.IsNullOrEmpty(Form["SavedImage4"])) Imagenes.Add(Form["SavedImage4"]);
            if (!String.IsNullOrEmpty(Form["SavedImage5"])) Imagenes.Add(Form["SavedImage5"]);

            Registro.NEW_Titulo = Form["NEW_Titulo"];
            Registro.NEW_Detalles = Form["NEW_Detalles"];

            if (ModelState.IsValid)
            {
                try
                {
                    Registro.BS_Activo = true;
                    OBJNEW_Noticias = new ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias();
                    OBJNEW_Noticias = NoticiasProxy.Crear(Registro, Imagenes);
                    if (OBJNEW_Noticias.Noticia_ID == 0)
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

            ViewBag.Imagen1 = Form["SavedImage1"];
            ViewBag.Imagen2 = Form["SavedImage2"];
            ViewBag.Imagen3 = Form["SavedImage3"];
            ViewBag.Imagen4 = Form["SavedImage4"];
            ViewBag.Imagen5 = Form["SavedImage5"];

            return View(Registro);
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(FormCollection Form)
        {
            ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias Registro = new ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias();
            List<String> Imagenes = new List<string>();
            try { this.NoticiasProxy = new ADESCOMBUSINESS.Areas.Noticias.Methods.NoticiasBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Form["NEW_Titulo"]))
            {
                ModelState.AddModelError("NEW_Titulo", "Campo requerido");
            }

            if (String.IsNullOrEmpty(Form["NEW_Detalles"]))
            {
                ModelState.AddModelError("NEW_Detalles", "Campo requerido");
            }

            if (!String.IsNullOrEmpty(Form["SavedImage1"])) Imagenes.Add(Form["SavedImage1"]);
            if (!String.IsNullOrEmpty(Form["SavedImage2"])) Imagenes.Add(Form["SavedImage2"]);
            if (!String.IsNullOrEmpty(Form["SavedImage3"])) Imagenes.Add(Form["SavedImage3"]);
            if (!String.IsNullOrEmpty(Form["SavedImage4"])) Imagenes.Add(Form["SavedImage4"]);
            if (!String.IsNullOrEmpty(Form["SavedImage5"])) Imagenes.Add(Form["SavedImage5"]);

            String activo = Form["BS_Activo"];
            Registro.BS_Activo = activo.Contains("true") ? true : false;
            Registro.Noticia_ID = Convert.ToInt32(Form["Noticia_ID"]);
            Registro.NEW_Titulo = Form["NEW_Titulo"];
            Registro.NEW_Detalles = Form["NEW_Detalles"];

            if (ModelState.IsValid)
            {
                try
                {
                    OBJNEW_Noticias = new ADESCOMBUSINESS.Areas.Noticias.Models.NEW_Noticias();
                    NoticiasProxy.Editar(Registro, Imagenes);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.Imagen1 = Form["SavedImage1"];
            ViewBag.Imagen2 = Form["SavedImage2"];
            ViewBag.Imagen3 = Form["SavedImage3"];
            ViewBag.Imagen4 = Form["SavedImage4"];
            ViewBag.Imagen5 = Form["SavedImage5"];

            return View(Registro);
        }
    }
}