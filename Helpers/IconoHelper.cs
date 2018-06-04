using System.Web.Mvc;
using System.Web.Routing;

namespace ADESCOM.Helpers
{
    public static class IconoHelper
    {
        private const string Word = "/Content/img/imge.png";
        private const string Imagen = "/Content/img/imge.png";
        private const string Pdf = "/Content/img/pdf.png";

        public static MvcHtmlString IconoFormato(this HtmlHelper helper, string idIcono, string icono)
        {
            var iconoDefinitivo = string.Empty;

            switch (icono)
            {
                case ".doc":
                case ".docx":
                    iconoDefinitivo = Word;
                    break;
                case ".pdf":
                    iconoDefinitivo = Pdf;
                    break;
                case ".jpg":
                    iconoDefinitivo = Imagen;
                    break;
            }

            return MvcHtmlString.Create(ObtenerTagImagen(idIcono, icono, iconoDefinitivo, null));
        }

        private static string ObtenerTagImagen(string idIcono, string tipoIcono, string url, object htmlAttributes)
        {
            // Create tag builder
            var builder = new TagBuilder("img");

            // Create valid id
            builder.GenerateId(idIcono);

            // Add attributes
            builder.MergeAttribute("src", url);
            builder.MergeAttribute("alt", tipoIcono);
            builder.MergeAttribute("height", "28px");
            builder.MergeAttribute("width", "28px");
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag
            return builder.ToString(TagRenderMode.Normal);
        }
    }
}