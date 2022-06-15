using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElektronikSatisi.Models;
using PagedList;

namespace ElektronikSatisi.Controllers
{
    public class UrunlerController : Controller
    {

        EmarketEntities23 db = new EmarketEntities23();

        // GET: Urunler
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Detay(int id) 
        {
            var gelen = db.Urunler.Where(x => x.id == id).FirstOrDefault();
            return View(gelen);
        }

        [Authorize]
        public ActionResult SatinAl(string id) {
            var gelen = db.Urunler.Where(x => x.UBaslik== id).FirstOrDefault();
            return  View(gelen);
        }

        [HttpPost]
        public ActionResult SatinAl(SatinAlinan satinAlinan) {
            db.SatinAlinan.Add(satinAlinan);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult YorumEkle(int id, string yorummetini, int rate)
        {
            Yorumlar yrm = new Yorumlar();
            
            if (Session["EMail"] != null) {
                var v = Convert.ToInt32(Session["Id"]);
                var liste = db.Yorumlar.Where(x => x.YYorumUser == v && x.YYorumUrun == id).FirstOrDefault();

                if (liste == null)
                {
                    yrm.YIsim = Convert.ToString(Session["Isım"]);
                    yrm.YSoyisim = Convert.ToString(Session["Soyad"]);
                    yrm.YYildiz = rate;
                    yrm.YYorum = yorummetini;
                    yrm.YYorumUrun = id;
                    yrm.YYorumUser = Convert.ToInt32(Session["Id"]);
                    yrm.YResimYolu = Convert.ToString(Session["Resim"]);
                    db.Yorumlar.Add(yrm);
                    db.SaveChanges();
                }
            }
            else {
                return RedirectToAction("Index", "Login");
            }
            
            
            return RedirectToAction("Detay", new { id = id });
        }

        public ActionResult Yorumlar(int id, int? page)
        {
            var liste = db.Yorumlar.Where(x => x.YYorumUrun == id).ToList().ToPagedList(page ?? 1, 100);
            return View(liste);
        }
        [Authorize]
        public ActionResult YorumSil(int id, int? page)
        {
            var gelen = db.Yorumlar.Where(x => x.YYorumUser == id).FirstOrDefault();
            var a = gelen.YYorumUrun;
            db.Yorumlar.Remove(gelen);
            db.SaveChanges();

            return RedirectToAction("Detay", new { id = a });
        }


        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }

        
    }
}