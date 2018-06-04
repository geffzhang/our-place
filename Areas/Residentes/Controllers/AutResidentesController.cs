using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;

namespace ADESCOM.Areas.Residentes.Controllers
{
    public class AutResidentesController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesNoAutBusiness ResidentesProxy;
        protected ADESCOMBUSINESS.Areas.Residentes.Models.RES_ResidentesNoAut OBJRES_ResidentesNoAut = new ADESCOMBUSINESS.Areas.Residentes.Models.RES_ResidentesNoAut();

        [Logger]
        public ActionResult Index()
        {
            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesNoAutBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_ResidentesNoAut> Lista = new List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_ResidentesNoAut>();
            Lista = ResidentesProxy.GetUnauthorized();

            return View(Lista);
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesNoAutBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_ResidentesNoAut> Lista = new List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_ResidentesNoAut>();
            Lista = ResidentesProxy.GetUnauthorized();
            return View(Lista);
        }

        [Logger]
        public ActionResult Editar(int ResidenteNoAut_ID)
        {
            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesNoAutBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJRES_ResidentesNoAut = ResidentesProxy.GetByID(ResidenteNoAut_ID);

            if (OBJRES_ResidentesNoAut == null)
            {
                return HttpNotFound();
            }

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    return View("Editar1", OBJRES_ResidentesNoAut);
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion");
                    return View("Editar2", OBJRES_ResidentesNoAut);
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0), "Llave3_ID", "LL3_Descripcion");
                    return View("Editar3", OBJRES_ResidentesNoAut);
            }

            return null;
        }

        [HttpPost]
        [Logger]
        public ActionResult Editar(FormCollection Form)
        {
            //bool autorizar = Convert.ToBoolean(Form["autorizar"]);
            int Llave1 = 0;
            int Llave2 = 0;
            int Llave3 = 0;

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            int cantLlaves = CompanyInfo.CantLlaves;

            ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();

            //Retornar el registro principal
            ADESCOMBUSINESS.Areas.Residentes.Models.RES_ResidentesNoAut Registro = new ADESCOMBUSINESS.Areas.Residentes.Models.RES_ResidentesNoAut();
            this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesNoAutBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]);
            Registro = ResidentesProxy.GetByID(Convert.ToInt32(Form["ResidenteNoAut_ID"]));

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

            if (ModelState.IsValid)
            {
                //Autorizar Residente
                try
                {
                    ResidentesProxy.AutorizarResidente(Registro, Direccion.Direccion_ID, CompanyInfo);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }
            /*  }
              else
              {
                  //Rechazar residente
                  try
                  {
                      Registro.RNA_Estatus = "REC";
                      bool Status = ResidentesProxy.RechazarResidente(Registro, CompanyInfo);
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
              }*/

            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion", Llave1);
                    return View("Editar1", Registro);
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion", Llave1);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion", Llave2);
                    return View("Editar2", Registro);
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0), "Llave1_ID", "LL1_Descripcion", Llave1);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0), "Llave2_ID", "LL2_Descripcion", Llave2);
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0), "Llave3_ID", "LL3_Descripcion", Llave3);
                    return View("Editar3", Registro);
            }

            return null;
        }

        public ActionResult Rechazar(int ResidenteNoAut_ID)
        {
            try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesNoAutBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            OBJRES_ResidentesNoAut = ResidentesProxy.GetByID(ResidenteNoAut_ID);

            if (OBJRES_ResidentesNoAut == null)
            {
                return HttpNotFound();
            }

            return View(OBJRES_ResidentesNoAut);
        }

        [HttpPost, ActionName("Rechazar")]
        [ValidateAntiForgeryToken]
        [Logger]
        public ActionResult RechazarConfirmed(ADESCOMBUSINESS.Areas.Residentes.Models.RES_ResidentesNoAut Registro)
        {
            if (String.IsNullOrEmpty(Registro.RNA_MotivoRechazo))
            {
                ModelState.AddModelError("RNA_MotivoRechazo", "Campo requerido");
            }

            if (ModelState.IsValid)
            {
                try { this.ResidentesProxy = new ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesNoAutBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
                catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
                OBJRES_ResidentesNoAut = ResidentesProxy.GetByID(Registro.ResidenteNoAut_ID);

                if (OBJRES_ResidentesNoAut == null)
                {
                    return HttpNotFound();
                }

                //Rechazar residente
                try
                {
                    OBJRES_ResidentesNoAut.RNA_MotivoRechazo = Registro.RNA_MotivoRechazo;
                    ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
                    bool Status = ResidentesProxy.RechazarResidente(OBJRES_ResidentesNoAut, CompanyInfo);
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

            return View(OBJRES_ResidentesNoAut);
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> GetLlave1(int LLave1_ID)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> Llaves1 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1>();
            Llaves1.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1() { Llave1_ID = 0, LL1_Descripcion = "Elija Una Opcion" });
            Llaves1.AddRange(ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business.GetDireccionLlave1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], LLave1_ID));
            return Llaves1;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2> GetLlave2(int LLave2_ID)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2> Llaves2 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2>();
            Llaves2.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave2() { Llave2_ID = 0, LL2_Descripcion = "Elija Una Opcion" });
            return Llaves2;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3> GetLlave3(int LLave3_ID)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3> Llaves3 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3>();
            Llaves3.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave3() { Llave3_ID = 0, LL3_Descripcion = "Elija Una Opcion" });
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