using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crimescape.Controllers
{
    public class CrimeMapController : Controller
    {
        public ActionResult Index(string category, int year)
        {
            return View();
        }

    }
}
