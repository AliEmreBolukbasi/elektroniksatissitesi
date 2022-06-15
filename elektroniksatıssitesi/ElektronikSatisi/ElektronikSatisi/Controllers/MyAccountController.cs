using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElektronikSatisi.Controllers
{
    public class MyAccountController : Controller
    {
        // GET: MyAccount
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }

    }
}