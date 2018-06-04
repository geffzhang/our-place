using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class UsuariosController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.UsuariosBusiness UsuariosProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.SEG_Usuarios OBJSEG_Usuarios = new ADESCOMBUSINESS.Areas.Configuracion.Models.SEG_Usuarios();

        public ActionResult Index()
        {
            try { this.UsuariosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.UsuariosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwSEG_Usuarios> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwSEG_Usuarios>();
            Lista = UsuariosProxy.GetAll();
            return View(Lista);
        }

        public ActionResult RefreshData()
        {
            try { this.UsuariosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.UsuariosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwSEG_Usuarios> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwSEG_Usuarios>();
            Lista = UsuariosProxy.GetAll();
            return View(Lista);
        }

        public ActionResult SearchList()
        {
            try { this.UsuariosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.UsuariosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwSEG_Usuarios> Lista = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwSEG_Usuarios>();
            Lista = UsuariosProxy.GetAll();
            return View(Lista);
        }

        public ActionResult Detalle(int Usuario_ID)
        {
            try { this.UsuariosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.UsuariosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            OBJSEG_Usuarios = UsuariosProxy.GetByID(Usuario_ID);

            if (OBJSEG_Usuarios == null)
            {
                return HttpNotFound();
            }

            return View(OBJSEG_Usuarios);
        }

        public ActionResult Crear()
        {
            return View(new ADESCOMBUSINESS.Areas.Configuracion.Models.SEG_Usuarios());
        }

        public ActionResult Editar(int Usuario_ID)
        {
            try { this.UsuariosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.UsuariosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJSEG_Usuarios = UsuariosProxy.GetByID(Usuario_ID);

            if (OBJSEG_Usuarios == null)
            {
                return HttpNotFound();
            }

            return View(OBJSEG_Usuarios);
        }

        [HttpPost]
        public ActionResult Crear(ADESCOMBUSINESS.Areas.Configuracion.Models.SEG_Usuarios Registro)
        {
            if (String.IsNullOrEmpty(Registro.USR_Usuario))
            {
                ModelState.AddModelError("USR_Usuario", "Dato requerido");
            }

            if (String.IsNullOrEmpty(Registro.USR_Nombre))
            {
                ModelState.AddModelError("USR_Nombre", "Dato requerido");
            }

            if (String.IsNullOrEmpty(Registro.USR_Password))
            {
                ModelState.AddModelError("USR_Password", "Dato requerido");
            }

            try { this.UsuariosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.UsuariosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            if (ModelState.IsValid)
            {
                try
                {
                    OBJSEG_Usuarios = new ADESCOMBUSINESS.Areas.Configuracion.Models.SEG_Usuarios();
                    OBJSEG_Usuarios = UsuariosProxy.Crear(Registro);
                    if (OBJSEG_Usuarios.Usuario_ID == 0)
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
        public ActionResult Editar(ADESCOMBUSINESS.Areas.Configuracion.Models.SEG_Usuarios Registro)
        {
            try { this.UsuariosProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.UsuariosBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (String.IsNullOrEmpty(Registro.USR_Usuario))
            {
                ModelState.AddModelError("Usuario.USR_Usuario", "Dato requerido");
            }

            if (String.IsNullOrEmpty(Registro.USR_Nombre))
            {
                ModelState.AddModelError("Usuario.USR_Nombre", "Dato requerido");
            }

            if (String.IsNullOrEmpty(Registro.USR_Password))
            {
                ModelState.AddModelError("Usuario.USR_Password", "Dato requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = UsuariosProxy.Editar(Registro);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                OBJSEG_Usuarios = null;
                UsuariosProxy = null;
            }
            base.Dispose(disposing);
        }
    }
}
