using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Configuracion.Controllers
{
    public class ConfiguracionController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesEmailBusiness SIS_NotifTemplatesEmailProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesBuzonAppBusiness SIS_NotifTemplatesBuzonAppProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesPushBusiness SIS_NotifTemplatesPushNotifProxy;
        protected ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaCatalogoCompania xxx = new ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaCatalogoCompania();

        public ActionResult Index()
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaModulo> GrupoRetorna = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaModulo>();
            ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness ConfiguracionSistema = new ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            GrupoRetorna = ConfiguracionSistema.GetGrupos();
            return View(GrupoRetorna);
        }

        public ActionResult GetViewCampos(int GrupoEdicion)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania> Parametros = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania>();
            ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness ConfiguracionSistemaProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            Parametros = ConfiguracionSistemaProxy.GetParamByGrupoEdicion(GrupoEdicion);
            @ViewBag.GrupoEdicion = GrupoEdicion;

            if (GrupoEdicion == 2)
            {
                ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaNotifCompuesto ConfigNotif = new ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaNotifCompuesto();
               
                //Optimización
                List<ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion> Elementos = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion>();
                Elementos = ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_TipoNotificacionBusiness.GetAll((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);

                ConfigNotif.EmailNotifications = GetTemplates(Elementos.Where(x=> x.TNO_AplicaEmail == true).ToList(), Convert.ToInt32(ADESCOMBUSINESS.GlobalBusiness.ObtConfigParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "EmailNotifications")));
                ConfigNotif.BuzonAppNotifications = GetTemplates(Elementos.Where(x => x.TNO_AplicaBuzonApp == true).ToList(), Convert.ToInt32(ADESCOMBUSINESS.GlobalBusiness.ObtConfigParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "AppNotifications")));
                ConfigNotif.PushNotifications = GetTemplates(Elementos.Where(x => x.TNO_AplicaPushNotif == true).ToList(), Convert.ToInt32(ADESCOMBUSINESS.GlobalBusiness.ObtConfigParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "PushNotifications")));
                //GHO No es necesario, no se van a manajar por compania, se manajara un set en general
                //ConfigNotif.Parametros = Parametros;

                return View("GetViewNotificaciones", ConfigNotif);
            }
            else
            {
                int cta = Convert.ToInt32(ADESCOMBUSINESS.GlobalBusiness.ObtConfigParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "CuentaCuotas"));
                //ViewBag.Cuenta_ID = new SelectList(ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentasEmpresa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], cta), cta);
                ViewBag.Cuenta_ID = new SelectList(GetCuentas(0), "Cuenta_ID", "CTA_Alias", cta);

                String codigo = ConfiguracionSistemaProxy.GetCodigoActivacion();
                ViewBag.CodigoActivacion = codigo;

                return View("GetViewCampos", Parametros);
            }

        }

        public ActionResult GetViewCatalogos(int GrupoEdicion)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaCatalogoCompania> Catalogos = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaCatalogoCompania>();
            ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness ConfiguracionSistemaProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            Catalogos = ConfiguracionSistemaProxy.GetCatalogosByGrupoEdicion(GrupoEdicion);
            return View(Catalogos);
        }


        [HttpPost]
        public ActionResult SetViewCampos(FormCollection Form)
        {
            int Cuenta_ID = 0;
            String CodigoActivacion = String.Empty;
            String Error = String.Empty;
            int GrupoEdicion = Convert.ToInt32(Form["GrupoEdicion"]);
            @ViewBag.GrupoEdicion = GrupoEdicion;

            List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania> Parametros = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania>();
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania> ParametrosUpdate = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania>();
            ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness ConfiguracionSistema = new ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            Parametros = ConfiguracionSistema.GetParamByGrupoEdicion(GrupoEdicion);

            if (String.IsNullOrEmpty(Form["Cuenta_ID"]) || Form["Cuenta_ID"] == "0")
            {
                Error = "La cuenta receptora de pagos es requerida";
                ModelState.AddModelError("Cuenta_ID", Error);
            }

            if (String.IsNullOrEmpty(Form["CodigoActivacion"]))
            {
                Error = "El código de activación de residentes es requerido";
                ModelState.AddModelError("CodigoActivacion", Error);
            }

            try { Cuenta_ID = Convert.ToInt32(Form["Cuenta_ID"]); }
            catch { Error = "La cuenta receptora de pagos es requerida"; ModelState.AddModelError("Cuenta_ID", Error); }

            CodigoActivacion = Form["CodigoActivacion"];

            foreach (ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania parametro in Parametros)
            {
                if (parametro.CPA_UsuarioModif == true)
                {
                    switch (parametro.TipoDato_ID)
                    {
                        case 2: //Entero
                            parametro.CPA_Valor = Convert.ToInt32(Form[parametro.TCP_Nombre]);
                            break;
                        case 3: //Decimal
                            parametro.CPA_Valor = Convert.ToDecimal(Form[parametro.TCP_Nombre]);
                            break;
                        case 4: //Fecha
                            parametro.CPA_Valor = Convert.ToDateTime(Form[parametro.TCP_Nombre]);
                            break;
                        case 5: //SI/NO
                            parametro.CPA_Valor = Convert.ToBoolean(Form[parametro.TCP_Nombre]);
                            break;
                            /* default:
                                 parametro.CPA_Valor = Form[parametro.TCP_Nombre].ToString();
                                 break;*/
                    }

                    //Ignora los catálogos
                    if (parametro.TipoDato_ID <= 5)
                        ParametrosUpdate.Add(parametro);
                }
            }

            //Cuenta receptora de pagos
            ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania paramCuenta = new ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania();
            paramCuenta.TCP_Nombre = "CuentaCuotas";
            paramCuenta.CPA_Valor = Cuenta_ID;
            paramCuenta.CPA_UsuarioModif = true;
            ParametrosUpdate.Add(paramCuenta);

            if (ModelState.IsValid)
            {
                ParametrosUpdate = ConfiguracionSistema.SetParamByGrupoEdicion(ParametrosUpdate);
                ConfiguracionSistema.SetCodigoActivacion(CodigoActivacion);

                foreach (ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania parametro in ParametrosUpdate)
                {
                    if (parametro.Error)
                    {
                        ModelState.AddModelError(parametro.TCP_Nombre, parametro.ModelError);
                        Error = parametro.ModelError;
                    }
                }
                if (ModelState.IsValid)
                {
                    ViewBag.Error = "OK";
                }
                else
                {
                    ViewBag.Error = Error;
                }
            }
            else
            {
                ViewBag.Error = Error;
            }

            int cta = Convert.ToInt32(ADESCOMBUSINESS.GlobalBusiness.ObtConfigParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "CuentaCuotas"));
            ViewBag.Cuenta_ID = new SelectList(ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentasEmpresa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], cta), cta);

            return View("GetViewCampos", Parametros);
        }

        [HttpPost]
        public ActionResult SetViewNotificaciones(FormCollection Form)
        {
            int GrupoEdicion = Convert.ToInt32(Form["GrupoEdicion"]);
            @ViewBag.GrupoEdicion = GrupoEdicion;

            //List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania> Parametros = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania>();
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania> ParametrosUpdate = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania>();
            ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness ConfiguracionSistema = new ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            //Parametros = ConfiguracionSistema.GetParamByGrupoEdicion(GrupoEdicion);

            /*foreach (ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania parametro in Parametros)
            {
                switch (parametro.TipoDato_ID)
                {
                    case 2: //Entero
                        parametro.CPA_Valor = Convert.ToInt32(Form[parametro.TCP_Nombre]);
                        break;
                    case 3: //Decimal
                        parametro.CPA_Valor = Convert.ToDecimal(Form[parametro.TCP_Nombre]);
                        break;
                    case 4: //Fecha
                        parametro.CPA_Valor = Convert.ToDateTime(Form[parametro.TCP_Nombre]);
                        break;
                    case 5: //SI/NO
                        parametro.CPA_Valor = Convert.ToBoolean(Form[parametro.TCP_Nombre]);
                        break;
                    default:
                        parametro.CPA_Valor = Form[parametro.TCP_Nombre].ToString();
                        break;
                }

                ParametrosUpdate.Add(parametro);
            }*/

            //Notificaciones vía Email seleccionados
            ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania param = new ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania();
            param.TCP_Nombre = "EmailNotifications";
            param.CPA_Valor = Form["sumEmailsSelected"];
            param.CPA_UsuarioModif = true;
            ParametrosUpdate.Add(param);

            //Notificaciones vía Buzón App seleccionadas
            param = new ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania();
            param.TCP_Nombre = "AppNotifications";
            param.CPA_Valor = Form["sumAppSelected"];
            param.CPA_UsuarioModif = true;
            ParametrosUpdate.Add(param);

            //Notificaciones vía Pushseleccionadas
            param = new ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania();
            param.TCP_Nombre = "PushNotifications";
            param.CPA_Valor = Form["sumPushSelected"];
            param.CPA_UsuarioModif = true;
            ParametrosUpdate.Add(param);

            ParametrosUpdate = ConfiguracionSistema.SetParamByGrupoEdicion(ParametrosUpdate);

            foreach (ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaParamCompania parametro in ParametrosUpdate)
            {
                if (parametro.Error)
                {
                    ModelState.AddModelError(parametro.TCP_Nombre, parametro.ModelError);
                }
            }
            if (ModelState.IsValid)
            {
                ViewBag.Error = "OK";
            }

            ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaNotifCompuesto ConfigNotif = new ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaNotifCompuesto();

            //Optimización
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion> Elementos = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion>();
            Elementos = ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_TipoNotificacionBusiness.GetAll((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);

            ConfigNotif.EmailNotifications = GetTemplates(Elementos, Convert.ToInt32(ADESCOMBUSINESS.GlobalBusiness.ObtConfigParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "EmailNotifications")));
            ConfigNotif.BuzonAppNotifications = GetTemplates(Elementos, Convert.ToInt32(ADESCOMBUSINESS.GlobalBusiness.ObtConfigParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "AppNotifications")));
            ConfigNotif.PushNotifications = GetTemplates(Elementos, Convert.ToInt32(ADESCOMBUSINESS.GlobalBusiness.ObtConfigParam((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "PushNotifications")));

            //GHO No es necesario, no se van a manajar por compania, se manajara un set en general
            //ConfigNotif.Parametros = Parametros;

            return View("GetViewNotificaciones", ConfigNotif);
        }

        public List<ADESCOMBUSINESS.Areas.Configuracion.Models.TipoNotifSelect> GetTemplates(List<ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion> Elementos, int sumaBinarios)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.TipoNotifSelect> ListaRetorna = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.TipoNotifSelect>();

            //Si ya tenía valores seleccionados, se desglosan List<int> (1, 2, 4, 8, 16, etc.)
            List<int> ListaValores = ADESCOMBUSINESS.GlobalBusiness.DesglosaElementoBinario(sumaBinarios);

            //Se agrega cada substatus y se prende la bandera de "incluir"
            foreach (ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_TipoNotificacion em in Elementos)
            {
                ADESCOMBUSINESS.Areas.Configuracion.Models.TipoNotifSelect notificacion = new ADESCOMBUSINESS.Areas.Configuracion.Models.TipoNotifSelect();
                notificacion.ID = em.TipoNotificacion_ID;
                notificacion.Clave = em.TNO_ValorRegistro;
                notificacion.Descripcion = em.TNO_Nombre;
                foreach (int val in ListaValores)
                {
                    if (val == em.TNO_ValorRegistro)
                    {
                        notificacion.Incluir = true;
                        break;
                    }
                }
                ListaRetorna.Add(notificacion);
            }
            return ListaRetorna;
        }

        [HttpPost]
        public JsonResult SetViewCatalogos(int GrupoEdicion, string ValoresSeleccionados)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaCatalogoCompania> Catalogo = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.ConfiguracionSistemaCatalogoCompania>();
            ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness ConfiguracionSistema = new ADESCOMBUSINESS.Areas.Configuracion.Methods.ConfiguracionSistemaBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            List<int> Valores = new List<int>();

            if (string.IsNullOrEmpty(ValoresSeleccionados))
            {
                return Json("Debe Seleccionar Minimo una Opcion");
            }

            char[] CaracterDelimitador = { ',' };
            string[] Values = ValoresSeleccionados.Split(CaracterDelimitador);

            foreach (string value in Values)
            {
                Valores.Add(Convert.ToInt32(value));
            }

            try
            {
                ConfiguracionSistema.SetCatalogosByGrupoEdicion(GrupoEdicion, Valores);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


            return Json("Ok");
        }

        public ActionResult SetNotifLayoutEmail(int TipoNotificacion_ID)
        {
            try { this.SIS_NotifTemplatesEmailProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesEmailBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            //ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesEmail OBJNotifTemplatesEmail = SIS_NotifTemplatesEmailProxy.GetByID(NotifTemplatesEmail_ID);
            ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesEmail OBJNotifTemplatesEmail = ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesEmailBusiness.GetByTipoNotificacion((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], TipoNotificacion_ID);

            //ViewBag.TipoOrigenDatos_ID = new SelectList(ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetByTipoDatos((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "NOT"), "TipoOrigenDatos_ID", "TOD_Descripcion", OBJNotifTemplatesEmail.TipoOrigenDatos_ID);
            ViewBag.TipoOrigenDatos_ID = ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetViewByID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], OBJNotifTemplatesEmail.TipoOrigenDatos_ID);

            return View(OBJNotifTemplatesEmail);
        }

        [Logger]
        [HttpPost, ValidateInput(false)]
        public ActionResult SetNotifLayoutEmail(ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesEmail Registro)
        {
            try { this.SIS_NotifTemplatesEmailProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesEmailBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = SIS_NotifTemplatesEmailProxy.Editar(Registro);
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
            //ViewBag.TipoOrigenDatos_ID = new SelectList(ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetByTipoDatos((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "NOT"), "TipoOrigenDatos_ID", "TOD_Descripcion", Registro.TipoOrigenDatos_ID);
            ViewBag.TipoOrigenDatos_ID = ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetViewByID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Registro.TipoOrigenDatos_ID);

            return View(Registro);
        }

        public ActionResult SetNotifLayoutBuzonApp(int TipoNotificacion_ID)
        {
            try { this.SIS_NotifTemplatesBuzonAppProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesBuzonAppBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            //ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesBuzonApp OBJSIS_NotifTemplatesBuzon = SIS_NotifTemplatesBuzonAppProxy.GetByID(NotifTemplatesBuzonApp_ID);
            ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesBuzonApp OBJSIS_NotifTemplatesBuzon = ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesBuzonAppBusiness.GetByTipoNotificacion((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], TipoNotificacion_ID);

            //ViewBag.TipoOrigenDatos_ID = new SelectList(ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetByTipoDatos((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "NOT"), "TipoOrigenDatos_ID", "TOD_Descripcion", OBJNotifTemplatesEmail.TipoOrigenDatos_ID);
            ViewBag.TipoOrigenDatos_ID = ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetViewByID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], OBJSIS_NotifTemplatesBuzon.TipoOrigenDatos_ID);

            return View(OBJSIS_NotifTemplatesBuzon);
        }

        [Logger]
        [HttpPost, ValidateInput(false)]
        public ActionResult SetNotifLayoutBuzonApp(ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesBuzonApp Registro)
        {
            try { this.SIS_NotifTemplatesBuzonAppProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesBuzonAppBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = SIS_NotifTemplatesBuzonAppProxy.Editar(Registro);
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
            //ViewBag.TipoOrigenDatos_ID = new SelectList(ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetByTipoDatos((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], "NOT"), "TipoOrigenDatos_ID", "TOD_Descripcion", Registro.TipoOrigenDatos_ID);
            ViewBag.TipoOrigenDatos_ID = ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetViewByID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Registro.TipoOrigenDatos_ID);

            return View(Registro);
        }

        public ActionResult SetNotifLayoutPushNotif(int TipoNotificacion_ID)
        {
            try { this.SIS_NotifTemplatesPushNotifProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesPushBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            //ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesPush OBJSIS_NotifTemplatesPush = SIS_NotifTemplatesPushNotifProxy.GetByID(NotifTemplatesPush_ID);
            ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesPush OBJSIS_NotifTemplatesPush = ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesPushBusiness.GetByTipoNotificacion((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], TipoNotificacion_ID);

            //ViewBag.TipoOrigenDatos_ID = new SelectList(ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetByTipoDatos((TSIDATAACCESS.Models.SEG_InsLoginResponse)Session["InfoUser"], "NOT"), "TipoOrigenDatos_ID", "TOD_Descripcion", OBJNotifTemplatesSMS.TipoOrigenDatos_ID);
            ViewBag.TipoOrigenDatos_ID = ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetViewByID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], OBJSIS_NotifTemplatesPush.TipoOrigenDatos_ID);

            return View(OBJSIS_NotifTemplatesPush);
        }

        [Logger]
        [HttpPost, ValidateInput(false)]
        public ActionResult SetNotifLayoutPushNotif(ADESCOMBUSINESS.Areas.Configuracion.Models.SIS_NotifTemplatesPush Registro)
        {
            try { this.SIS_NotifTemplatesPushNotifProxy = new ADESCOMBUSINESS.Areas.Configuracion.Methods.SIS_NotifTemplatesPushBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            if (ModelState.IsValid)
            {
                try
                {
                    bool Status = SIS_NotifTemplatesPushNotifProxy.Editar(Registro);
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
            //ViewBag.TipoOrigenDatos_ID = new SelectList(ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetByTipoDatos((TSIDATAACCESS.Models.SEG_InsLoginResponse)Session["InfoUser"], "NOT"), "TipoOrigenDatos_ID", "TOD_Descripcion", Registro.TipoOrigenDatos_ID);
            ViewBag.TipoOrigenDatos_ID = ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.GetViewByID((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Registro.TipoOrigenDatos_ID);

            return View(Registro);
        }

        private List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> GetCuentas(int Cuenta_ID)
        {
            List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas> Cuentas = new List<ADESCOMBUSINESS.Areas.Contabilidad.Models.VwCON_Cuentas>();
            Cuentas.AddRange(ADESCOMBUSINESS.Areas.Contabilidad.Methods.CON_CuentasBusiness.GetCuentasEmpresa((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Cuenta_ID));
            return Cuentas;
        }

        public ActionResult GetParams(int TipoOrigenDatos_ID)
        {
            List<ADESCOMBUSINESS.Areas.Utilerias.Models.Parametros_TipoOrigenDatos> Parametros = ADESCOMBUSINESS.Areas.Utilerias.Methods.SIS_TipoOrigenDatosCatBusiness.Get_Info_TipoOrigenDatos((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], TipoOrigenDatos_ID);
            return View(Parametros);
        }
    }
}