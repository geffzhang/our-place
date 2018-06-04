using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADESCOM.Areas.Calendario.Controllers
{
    public class CalendarioController : Controller
    {
        // GET: Calendario/Calendario
        public ActionResult Index()
        {
            return View();
        }
    }
}