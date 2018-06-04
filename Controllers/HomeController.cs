using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

namespace ADESCOM.Controllers
{
    public class HomeController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Contacto.Methods.ContactoBusiness ContactoProxy;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return RedirectToAction("Login", "Login");
        }

        public ActionResult Precios()
        {
            return View();
        }

        public ActionResult Caracteristicas()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Contacto()
        {
            return View(new ADESCOMBUSINESS.Areas.Contacto.Models.SIS_Contacto());
        }

        public ActionResult AvisoPrivacidad()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PartialContacto(ADESCOMBUSINESS.Areas.Contacto.Models.SIS_Contacto Registro)
        {
            this.ContactoProxy = new ADESCOMBUSINESS.Areas.Contacto.Methods.ContactoBusiness();

            if (String.IsNullOrEmpty(Registro.CON_Email))
            {
                ModelState.AddModelError("CON_Email", "Campo requerido");
            }

            if (String.IsNullOrEmpty(Registro.CON_Mensaje))
            {
                ModelState.AddModelError("CON_Mensaje", "Campo requerido");
            }

            if (ModelState.IsValid) {
                try
                {
                    ContactoProxy.Crear(Registro);
                    ViewBag.Error = "OK";
                    //Limpiamos el modelo de vuelta
                    ModelState.Clear();
                    return View(new ADESCOMBUSINESS.Areas.Contacto.Models.SIS_Contacto());
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