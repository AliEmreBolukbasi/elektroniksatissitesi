using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElektronikSatisi.Models;

namespace ElektronikSatisi.Controllers
{
    public class LaptopController : Controller
    {

        EmarketEntities23 db = new EmarketEntities23();

        // GET: Laptop
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult UrunGetir()
        {
            var urunler = db.Urunler.Where(X => X.UKategori == "1");
            return View(urunler);
        }


        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }
    }
}