using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElektronikSatisi.Models;
using PagedList;
using PagedList.Mvc;

namespace ElektronikSatisi.Controllers
{
    public class HomeController : Controller
    {

        EmarketEntities23 db = new EmarketEntities23();

        // GET: Home
        public ActionResult Index()
        {
            

            return View();
        }


        public ActionResult Slider() {
            var sliders = db.Manset.Where(X =>X.id >0);
            return View(sliders);
        }


        public ActionResult UrunGetir(int? page) {

            var urunler = db.Urunler.Where(X => X.id > 0).ToList().ToPagedList(page ?? 1, 4);

            return View(urunler);
        }

        public ActionResult SearchUrunGetir(string search, int? page)
        {
            Urunler urun = new Urunler();



            var urunler = db.Urunler.Where(X => X.UBaslik.StartsWith(search)).ToList().ToPagedList(page ?? 1, 4);
            
            return View(urunler);
        }

        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }
    }

    
}