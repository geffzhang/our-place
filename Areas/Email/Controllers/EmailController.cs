using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ADESCOMBUSINESS.Exceptions;
using ADESCOMUTILS;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.IO;

namespace ADESCOM.Areas.Email.Controllers
{
    public class EmailController : Controller
    {
        protected ADESCOMBUSINESS.Areas.Email.Methods.EmailBusiness EmailProxy;
        protected ADESCOMBUSINESS.Areas.Email.Models.EMA_Emails OBJEMA_Emails = new ADESCOMBUSINESS.Areas.Email.Models.EMA_Emails();

        [Logger]
        public ActionResult Index()
        {
            try { this.EmailProxy = new ADESCOMBUSINESS.Areas.Email.Methods.EmailBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Email.Models.VwEMA_Emails> Lista = new List<ADESCOMBUSINESS.Areas.Email.Models.VwEMA_Emails>();
            Lista = EmailProxy.GetAll();

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    return View("Index1", Lista);
                case 2:
                    return View("Index2", Lista);
                case 3:
                    return View("Index3", Lista);
            }

            return null;
        }

        [Logger]
        public ActionResult Detalle(int Email_ID)
        {
            try { this.EmailProxy = new ADESCOMBUSINESS.Areas.Email.Methods.EmailBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Email.Models.VwEMA_Emails Email = new ADESCOMBUSINESS.Areas.Email.Models.VwEMA_Emails();
            Email = EmailProxy.GetViewByEmail_ID(Email_ID);

            ViewBag.AdjuntoName = Email.EMA_AdjuntoName;
            ViewBag.AdjuntoFile = Email.EMA_AdjuntoURL;

            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    return View("Detalle1", Email);
                case 2:
                    return View("Detalle2", Email);
                case 3:
                    return View("Detalle3", Email);
            }

            return null;
        }

        [Logger]
        public ActionResult Descargar(int Email_ID)
        {
            try { this.EmailProxy = new ADESCOMBUSINESS.Areas.Email.Methods.EmailBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            ADESCOMBUSINESS.Areas.Email.Models.VwEMA_Emails Email = new ADESCOMBUSINESS.Areas.Email.Models.VwEMA_Emails();
            Email = EmailProxy.GetViewByEmail_ID(Email_ID);

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Get a reference to a container, which may or may not exist.
                var container = blobClient.GetContainerReference("archivosadjuntos");

                // Get a reference to a blob, which may or may not exist.
                var blob = container.GetBlockBlobReference(Email.EMA_AdjuntoName);

                var fileStream = new System.IO.MemoryStream();
                blob.DownloadToStream(fileStream);

                //var imgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Email.EMA_AdjuntoName);
                //blob.DownloadToFile(imgPath, FileMode.Create);

                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Email.EMA_AdjuntoName);
                fileStream.WriteTo(Response.OutputStream);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return null;
        }

        [Logger]
        public ActionResult RefreshData()
        {
            try { this.EmailProxy = new ADESCOMBUSINESS.Areas.Email.Methods.EmailBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); }
            catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }
            List<ADESCOMBUSINESS.Areas.Email.Models.VwEMA_Emails> Lista = new List<ADESCOMBUSINESS.Areas.Email.Models.VwEMA_Emails>();
            Lista = EmailProxy.GetAll();

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

            ADESCOMBUSINESS.DataAccess.Models.LoginRS InfoUser = (ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"];

            int cantLlaves = CompanyInfo.CantLlaves;
            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "(Todos)"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Residente_ID = new SelectList(GetResidentes1(0, "(Todos)"), "Residente_ID", "RES_Nombre");
                    return View("Crear1", new ADESCOMBUSINESS.Areas.Email.Models.EMA_Emails() { EMA_Remitente = "Mesa Directiva " + InfoUser.CIA_Nombre });
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "(Todos)"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "(Todos)"), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Residente_ID = new SelectList(GetResidentes2(0, 0, "(Todos)"), "Residente_ID", "RES_Nombre");
                    return View("Crear2", new ADESCOMBUSINESS.Areas.Email.Models.EMA_Emails() { EMA_Remitente = "Mesa Directiva " + InfoUser.CIA_Nombre });
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "(Todos)"), "Llave1_ID", "LL1_Descripcion");
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(0, "(Todos)"), "Llave2_ID", "LL2_Descripcion");
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(0, 0, "(Todos)"), "Llave3_ID", "LL3_Descripcion");
                    ViewBag.Residente_ID = new SelectList(GetResidentes3(0, 0, 0, "(Todos)"), "Residente_ID", "RES_Nombre");
                    return View("Crear3", new ADESCOMBUSINESS.Areas.Email.Models.EMA_Emails() { EMA_Remitente = "Mesa Directiva " + InfoUser.CIA_Nombre });
            }

            return null;
        }

        [HttpPost, ValidateInput(false)]
        [Logger]
        public ActionResult Crear(FormCollection Form)
        {
            ADESCOMBUSINESS.DataAccess.Models.CompanyInfo CompanyInfo = (ADESCOMBUSINESS.DataAccess.Models.CompanyInfo)Session["CompanyInfo"];
            int cantLlaves = CompanyInfo.CantLlaves;

            ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones Direccion = new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Direcciones();
            ADESCOMBUSINESS.Areas.Email.Models.EMA_Emails Registro = new ADESCOMBUSINESS.Areas.Email.Models.EMA_Emails();

            try { this.EmailProxy = new ADESCOMBUSINESS.Areas.Email.Methods.EmailBusiness((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"]); } catch (Exception ex) { return RedirectToAction("SesionExpired", "Inicio", new { Area = "", Mensaje = ex.Message }); }

            if (!string.IsNullOrEmpty(Form["Residente_ID"]) && Form["Residente_ID"] != "0")
            {
                Registro.Residente_ID = Convert.ToInt32(Form["Residente_ID"]);
            }

            if (!string.IsNullOrEmpty(Form["Llave1_ID"]) && Form["Llave1_ID"] != "0")
            {
                Registro.Llave1_ID = Convert.ToInt32(Form["Llave1_ID"]);
            }

            if (cantLlaves > 1)
            {
                if (!string.IsNullOrEmpty(Form["Llave2_ID"]) && Form["Llave2_ID"] != "0")
                {
                    Registro.Llave2_ID = Convert.ToInt32(Form["Llave2_ID"]);
                }
            }

            if (cantLlaves > 2)
            {
                if (!string.IsNullOrEmpty(Form["Llave3_ID"]) && Form["Llave3_ID"] != "0")
                {
                    Registro.Llave3_ID = Convert.ToInt32(Form["Llave3_ID"]);
                }
            }

            if (string.IsNullOrEmpty(Form["EMA_Remitente"]))
            {
                ModelState.AddModelError("EMA_Remitente", "Campo Requerido");
            }
            else
            {
                Registro.EMA_Remitente = Form["EMA_Remitente"];
            }

            if (string.IsNullOrEmpty(Form["EMA_Asunto"]))
            {
                ModelState.AddModelError("EMA_Asunto", "Campo Requerido");
            }
            else
            {
                Registro.EMA_Asunto = Form["EMA_Asunto"];
            }

            if (string.IsNullOrEmpty(Form["EMA_Cuerpo"]))
            {
                ModelState.AddModelError("EMA_Cuerpo", "Campo Requerido");
            }
            else
            {
                Registro.EMA_Cuerpo = Form["EMA_Cuerpo"];
            }

            //Archivo adjunto
            if (!String.IsNullOrEmpty(Form["SavedFile"]))
            {
                Registro.EMA_AdjuntoURL = Form["SavedFile"];
                Registro.EMA_AdjuntoName = Form["fileName"];
                Registro.EMA_AdjuntoType = Form["fileType"];
                Registro.EMA_AdjuntoSize = Convert.ToInt32(Form["fileSize"]);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    EmailProxy.Crear(Registro);
                    ViewBag.Error = "OK";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.AdjuntoName = Form["fileName"];
            ViewBag.AdjuntoFile = Form["SavedFile"];
            ViewBag.AdjuntoSize = Form["fileSize"];
            ViewBag.AdjuntoType = Form["fileType"];
            ViewBag.Residente_ID = Registro.Residente_ID;

            ViewBag.DescLlave1 = CompanyInfo.LabelLlave1;
            ViewBag.DescLlave2 = CompanyInfo.LabelLlave2;
            ViewBag.DescLlave3 = CompanyInfo.LabelLlave3;

            switch (cantLlaves)
            {
                case 1:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Registro.Llave1_ID);
                    ViewBag.Residente_ID = new SelectList(GetResidentes1(Registro.Llave1_ID, "(Todos)"), "Residente_ID", "RES_Nombre", Registro.Residente_ID);
                    return View("Crear1", Registro);
                case 2:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Registro.Llave1_ID);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(Registro.Llave1_ID, "Elija una opción"), "Llave2_ID", "LL2_Descripcion", Registro.Llave2_ID);
                    ViewBag.Residente_ID = new SelectList(GetResidentes2(Registro.Llave1_ID, Registro.Llave2_ID, "(Todos)"), "Residente_ID", "RES_Nombre", Registro.Residente_ID);
                    return View("Crear2", Registro);
                case 3:
                    ViewBag.Llave1_ID = new SelectList(GetLlave1(0, "Elija una opción"), "Llave1_ID", "LL1_Descripcion", Registro.Llave1_ID);
                    ViewBag.Llave2_ID = new SelectList(GetLlave2(Registro.Llave1_ID, "Elija una opción"), "Llave2_ID", "LL2_Descripcion", Registro.Llave2_ID);
                    ViewBag.Llave3_ID = new SelectList(GetLlave3(Registro.Llave1_ID, Registro.Llave2_ID, "Elija una opción"), "Llave3_ID", "LL3_Descripcion", Registro.Llave3_ID);
                    ViewBag.Residente_ID = new SelectList(GetResidentes3(Registro.Llave1_ID, Registro.Llave2_ID, Registro.Llave3_ID, "(Todos)"), "Residente_ID", "RES_Nombre", Registro.Residente_ID);
                    return View("Crear3", Registro);
            }

            return null;
        }

        private List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> GetLlave1(int LLave1_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1> Llaves1 = new List<ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1>();
            Llaves1.Add(new ADESCOMBUSINESS.Areas.Configuracion.Models.VwTVI_Llave1() { Llave1_ID = 0, LL1_Descripcion = Description });
            Llaves1.AddRange(ADESCOMBUSINESS.Areas.Configuracion.Methods.TVI_Llave1Business.GetDireccionLlave1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], LLave1_ID));
            return Llaves1;
        }

        private List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> GetResidentes1(int LLave1_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> Residentes1 = new List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes>();
            Residentes1.Add(new ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes() { Residente_ID = 0, RES_Nombre = Description });
            Residentes1.AddRange(ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness.GetResidentesLlave1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], LLave1_ID));
            return Residentes1;
        }

        private List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> GetResidentes2(int LLave1_ID, int LLave2_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> Residentes2 = new List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes>();
            Residentes2.Add(new ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes() { Residente_ID = 0, RES_Nombre = Description });
            Residentes2.AddRange(ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness.GetResidentesLlave1and2((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], LLave1_ID, LLave2_ID));
            return Residentes2;
        }

        private List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> GetResidentes3(int LLave1_ID, int LLave2_ID, int LLave3_ID, String Description)
        {
            List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes> Residentes2 = new List<ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes>();
            Residentes2.Add(new ADESCOMBUSINESS.Areas.Residentes.Models.VwRES_Residentes() { Residente_ID = 0, RES_Nombre = Description });
            Residentes2.AddRange(ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness.GetResidentesLlave1and2and3((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], LLave1_ID, LLave2_ID, LLave3_ID));
            return Residentes2;
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
        public JsonResult GetResidentes1Items(int Llave1_ID)
        {
            return Json(new SelectList(ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness.GetResidentesLlave1((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID), "Residente_ID", "RES_Nombre"));
        }

        [Logger]
        public JsonResult GetResidentes2Items(int Llave1_ID, int Llave2_ID)
        {
            return Json(new SelectList(ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness.GetResidentesLlave1and2((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID, Llave2_ID), "Residente_ID", "RES_Nombre"));
        }

        [Logger]
        public JsonResult GetResidentes3Items(int Llave1_ID, int Llave2_ID, int Llave3_ID)
        {
            return Json(new SelectList(ADESCOMBUSINESS.Areas.Residentes.Methods.ResidentesBusiness.GetResidentesLlave1and2and3((ADESCOMBUSINESS.DataAccess.Models.LoginRS)Session["InfoUser"], Llave1_ID, Llave2_ID, Llave3_ID), "Residente_ID", "RES_Nombre"));
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