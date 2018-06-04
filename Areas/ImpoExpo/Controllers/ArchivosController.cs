using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.Linq;

namespace ADESCOM.Areas.ImpoExpo.Controllers
{
    public class ArchivosController : Controller
    {
        public ActionResult Import(String Contenedor_Archivos)
        {
            ViewBag.Contenedor_Archivos = Contenedor_Archivos;
            return View();
        }

        [HttpPost]
        public ActionResult ProcesarArchivo(FormCollection Formulario)
        {
            bool ArchivoImagen = false;
            String fileName = String.Empty;

            var archivo = Request.Files.Keys[0];
            HttpPostedFileBase contenidoArchivo = Request.Files[archivo];

            if (!Formulario["Contenedor_Archivos"].Equals("archivosadjuntos"))
                ArchivoImagen = true;

            if (contenidoArchivo == null)
            {
                ViewBag.Error = "Error al cargar el archivo";
                return View();
            }

            if (!ArchivoImagen && contenidoArchivo.ContentLength > 3000000)
            {
                ViewBag.Error = "El archivo supera el tamaño máximo permitido (3MB)";
                return View();
            }

            if (ArchivoImagen)
            {
                string TipoArchivo = contenidoArchivo.ContentType;

                if (!TipoArchivo.Contains("image"))
                {
                    ViewBag.Error = "Sólo se permiten archivos de imágen";
                    return View();
                }

                String name = Guid.NewGuid().ToString();
                fileName = name + ".jpg";
            }
            else
            {
                string TipoArchivo = contenidoArchivo.ContentType;

                String[] extensions = {
                    "text/plain","3gpp", "csv", "gif", "html", "jpeg", "mpeg", "msword", "pdf", "png", "rtf", "tiff", "vnd.ms-excel", "vnd.ms-powerpoint",
                    "vnd.openxmlformats-officedocument.spreadsheetml.sheet", "vnd.visio", "x-7z-compressed", "xhtml+xml", "xml",
                    "x-msvideo", "x-rar-compressed", "x-wav", "zip", "octet-stream" };

                if (!extensions.Any(TipoArchivo.Contains))
                {
                    ViewBag.Error = "Tipo de archivo invalido";
                    return View();
                }

                //Se pasa el nombre del archivo tal cual
                fileName = contenidoArchivo.FileName;
            }

            if (fileName.Length > 150)
            {
                ViewBag.Error = "El nombre del archivo es muy largo, favor de renombrarlo para continuar";
                return View();
            }

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Get a reference to a container, which may or may not exist.
                var container = blobClient.GetContainerReference(Formulario["Contenedor_Archivos"]);

                String fisico = @"https://adescomfiles.blob.core.windows.net/" + Formulario["Contenedor_Archivos"] + "/" + fileName;

                // Get a reference to a blob, which may or may not exist.
                var blob = container.GetBlockBlobReference(fileName);

                if (ArchivoImagen)
                {
                    blob.Properties.ContentType = "jpg";

                    //Compresión
                    Image img = System.Drawing.Image.FromStream(contenidoArchivo.InputStream);
                    Size tamanio = img.Size;
                    if (tamanio.Width > 1000 || tamanio.Height > 1000)
                    {
                        Image esc = ScaleImage(img, 1000);
                        //Size tamanio2 = esc.Size;
                        //esc.Save(fisico, ImageFormat.Jpeg);
                        var stream = new System.IO.MemoryStream();
                        esc.Save(stream, ImageFormat.Jpeg);
                        stream.Position = 0;
                        blob.UploadFromStream(stream);
                    }
                    else
                    {
                        //img.Save(fisico, ImageFormat.Jpeg);
                        var stream = new System.IO.MemoryStream();
                        img.Save(stream, ImageFormat.Jpeg);
                        stream.Position = 0;
                        blob.UploadFromStream(stream);
                    }
                }
                else
                {
                    blob.UploadFromStream(contenidoArchivo.InputStream);
                }

                ViewBag.Imagen_ID = fisico;
                ViewBag.NombreArchivo = contenidoArchivo.FileName;
                ViewBag.ArchivoSize = contenidoArchivo.ContentLength;
                ViewBag.ArchivoType = contenidoArchivo.ContentType;

                ViewBag.Error = "OK";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }

        public FileContentResult Export(string NombreArchivo, int TipoInformacion_ID, int Archivo_ID)
        {
            return null;
        }

        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxHeight)
        {
            var ratio = (double)maxHeight / image.Height;
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
    }
}