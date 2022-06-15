using ElektronikSatisi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElektronikSatisi.Controllers
{
    public class MasaustuController : Controller
    {
        // GET: Masaustu
        EmarketEntities23 db = new EmarketEntities23();

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult UrunGetir()
        {
            var urunler = db.Urunler.Where(X => X.UKategori == "2");
            return View(urunler);
        }

        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }
    }
}