using ElektronikSatisi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElektronikSatisi.Controllers
{
    public class TelefonController : Controller
    {
        EmarketEntities23 db = new EmarketEntities23();

        // GET: Telefon
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UrunGetir()
        {
            var urunler = db.Urunler.Where(X => X.UKategori == "3");
            return View(urunler);
        }

        public ActionResult UrunGetirFiltre(string filtre)
        {
            
            var urunler = db.Urunler.Where(X => X.UKategori == "3" && X.UBaslik.StartsWith(filtre));
            return View(urunler);
        }

        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }
    }
}