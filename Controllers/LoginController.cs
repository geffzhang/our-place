using System;
using System.Web;
using System.Web.Mvc;
using ADESCOMUTILS;
using System.Collections.Generic;

namespace ADESCOM.Controllers
{
    public class LoginController : Controller
    {
        [Logger]
        public ActionResult Registrar()
        {
            return View();
        }

        [Logger]
        public ActionResult Recuperar()
        {
            return View();
        }

        [Logger]
        public ActionResult Login()
        {
            /*HttpCookie cookie = Request.Cookies["User"];
            if (cookie != null) {
                ADESCOMBUSINESS.DataAccess.Models.LoginRQ login = new ADESCOMBUSINESS.DataAccess.Models.LoginRQ() { UsrUsuario = cookie["User"].ToString()};
                return View(login);
            }
            return View(new ADESCOMBUSINESS.DataAccess.Models.LoginRQ());*/
            Session.Clear();
            return View();
        }

        [Logger]
        public ActionResult Logout()
        {
            Session.Abandon();
            //Session["InfoUser"] = null;
            //Session.Remove("InfoUser");
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        [Logger]
        public ActionResult Login(ADESCOMBUSINESS.DataAccess.Models.LoginRQ User)
        {
            if (String.IsNullOrEmpty(User.TipoIngreso))
            {
                ViewBag.MessageError = "Indique si es Administrador o Residente";
                ModelState.AddModelError("","");
            }

            if (!User.TipoIngreso.Equals("Administrador") && !User.TipoIngreso.Equals("Residente"))
            {
                ViewBag.MessageError = "Indique si es Administrador o Residente";
                ModelState.AddModelError("", "");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (User.TipoIngreso.Equals("Administrador"))
                    {
                        ADESCOMBUSINESS.DataAccess.Models.LoginRS login = new ADESCOMBUSINESS.DataAccess.Models.LoginRS();
                        ADESCOMBUSINESS.LoginBusiness LoginProxy = new ADESCOMBUSINESS.LoginBusiness();
                        login = LoginProxy.Login(User);

                        if (login.Usuario_ID == 0 || login.CIA_Nombre == "" || login.Compania_ID == 0 || login.Token == "")
                        {
                            ViewBag.MessageError = "El usuario no existe o la contraseña es incorrecta";
                        }
                        else
                        {
                            /* HttpCookie myCookie = new HttpCookie("lastUserLogged");
                             myCookie.Values["User"] = User.UsrUsuario;
                             myCookie.Expires = DateTime.Now.AddDays(15);
                             Response.Cookies.Add(myCookie);*/

                            Session["InfoUser"] = login;
                            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = new ADESCOMBUSINESS.DataAccess.Models.CompanyInfo();

                            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCIA_ConfigParam> ListaParametros = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwCIA_ConfigParam>();
                            ListaParametros = ADESCOMBUSINESS.Areas.Configuracion.Methods.CIA_ConfigParamBusiness.GetAll(login);

                            List<ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion> Notificaciones = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion>();
                            Notificaciones = ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_TipoNotificacionBusiness.GetAll((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);

                            foreach (ADESCOMBUSINESS.Areas.Configuracion.Models.VwCIA_ConfigParam param in ListaParametros)
                            {
                                switch (param.TCP_Nombre)
                                {
                                    case "CantLlaves":
                                        CompanyInfo.CantLlaves = Convert.ToInt32(param.CPA_Valor);
                                        break;
                                    case "Llave1_Descripcion":
                                        CompanyInfo.DescLlave1 = param.CPA_Valor.ToString();
                                        break;
                                    case "Llave2_Descripcion":
                                        CompanyInfo.DescLlave2 = param.CPA_Valor.ToString();
                                        break;
                                    case "Llave3_Descripcion":
                                        CompanyInfo.DescLlave3 = param.CPA_Valor.ToString();
                                        break;
                                    case "Llave1_Etiqueta":
                                        CompanyInfo.LabelLlave1 = param.CPA_Valor.ToString();
                                        break;
                                    case "Llave2_Etiqueta":
                                        CompanyInfo.LabelLlave2 = param.CPA_Valor.ToString();
                                        break;
                                    case "Llave3_Etiqueta":
                                        CompanyInfo.LabelLlave3 = param.CPA_Valor.ToString();
                                        break;
                                    case "EmailNotifications":
                                        CompanyInfo.NotifEmail = GetTiposNotif(Notificaciones, Convert.ToInt32(param.CPA_Valor));
                                        break;
                                    case "PushNotifications":
                                        CompanyInfo.NotifPush = GetTiposNotif(Notificaciones, Convert.ToInt32(param.CPA_Valor));
                                        break;
                                    case "AppNotifications":
                                        CompanyInfo.NotifBuzonApp = GetTiposNotif(Notificaciones, Convert.ToInt32(param.CPA_Valor));
                                        break;
                                }
                            }

                            Session["CompanyInfo"] = CompanyInfo;

                            return RedirectToAction("Index", "Inicio");
                        }
                    }
                    else
                    {
                        ADESCOMBUSINESS.App.Login.Models.Login_RQ loginRQ = new ADESCOMBUSINESS.App.Login.Models.Login_RQ();
                        loginRQ.UserName = User.UsrUsuario;
                        loginRQ.Password = User.Password;
                        loginRQ.Device_id = "WEB";
                        loginRQ.Device_type = "WEB";

                        ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS loginResidenteRS = new ADESCOMBUSINESS.DataAccess.Models.ResidenteLoginRS();
                        ADESCOMBUSINESS.App.Login.Models.Login_RS loginRS = new ADESCOMBUSINESS.App.Login.Models.Login_RS();
                        ADESCOMBUSINESS.App.Login.Methods.LoginBusiness LoginProxy = new ADESCOMBUSINESS.App.Login.Methods.LoginBusiness();
                        loginRS = LoginProxy.Login(loginRQ);

                        if (loginRS.Error != null)
                        {
                            ViewBag.MessageError = loginRS.Error.ErrorMsg;
                        }
                        else
                        {
                            loginResidenteRS.Celular = loginRS.Sesion.Celular;
                            loginResidenteRS.CIA_CodPais = loginRS.Sesion.CIA_CodPais;
                            loginResidenteRS.CIA_Nombre = loginRS.Sesion.CIA_Nombre;
                            loginResidenteRS.Compania_ID = loginRS.Sesion.Compania_ID;
                            loginResidenteRS.Direccion_ID = loginRS.Sesion.Direccion_ID;
                            loginResidenteRS.Email = loginRS.Sesion.Email;
                            loginResidenteRS.NombreCompleto = loginRS.Sesion.NombreCompleto;
                            loginResidenteRS.Referencia = loginRS.Sesion.Referencia;
                            loginResidenteRS.ReferenciaFija = loginRS.Sesion.ReferenciaFija;
                            loginResidenteRS.Residencia = loginRS.Sesion.Residencia;
                            loginResidenteRS.Residente_ID = loginRS.Sesion.Residente_ID;
                            loginResidenteRS.Telefono = loginRS.Sesion.Telefono;
                            loginResidenteRS.Token = loginRS.Sesion.Token;
                            loginResidenteRS.UserName = loginRS.Sesion.UserName;

                            Session["InfoUser"] = loginResidenteRS;

                            return RedirectToAction("Index", "ResidentesHome");
                        } //App Login incorrecto
                    }//Es residente o administrador
                }//Try
                catch (Exception ex)
                {
                    ViewBag.MessageError = ex.Message;
                }
            }
            return View(User);
        }

        [Logger]
        public List<int> GetTiposNotif(List<ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion> Elementos, int sumaBinarios)
        {
            List<int> ListaRetorna = new List<int>();

            //Si ya tenía valores seleccionados, se desglosan List<int> (1, 2, 4, 8, 16, etc.)
            List<int> ListaValores = ADESCOMBUSINESS.GlobalBusiness.DesglosaElementoBinario(sumaBinarios);

            //Se agrega cada substatus y se prende la bandera de "incluir"
            foreach (ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion em in Elementos)
            {
                foreach (int val in ListaValores)
                {
                    if (val == em.TNO_ValorRegistro)
                    {
                        ListaRetorna.Add(em.TipoNotificacion_ID);
                        break;
                    }
                }
            }
            return ListaRetorna;
        }

        [Logger]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Logger]
        public ActionResult ForgotPassword(FormCollection Form)
        {
            String email = Form["usrEmail"];

            ViewBag.Success = "";
            ViewBag.MessageError = "";

            if (String.IsNullOrEmpty(email))
            {
                ViewBag.MessageError = "No ingresó su email o no es un email válido";
                ModelState.AddModelError("", "");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ADESCOMBUSINESS.LoginBusiness LoginProxy = new ADESCOMBUSINESS.LoginBusiness();
                    int Usuario_ID = 0;
                    Usuario_ID = LoginProxy.ForgotPassword(email);

                    if (Usuario_ID == 0)
                    {
                        ViewBag.MessageError = "El email no está registrado o no fue posible recuperar su contraseña";
                    }
                    else
                    {
                        ViewBag.Success = "En breve recibirá un correo electrónico con las instrucciones para reiniciar su contraseña";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.MessageError = ex.Message;
                }
            }
            ViewBag.Password = email;
            return View();
        }
    }
}
