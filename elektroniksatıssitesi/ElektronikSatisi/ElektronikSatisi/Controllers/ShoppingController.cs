using ElektronikSatisi.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElektronikSatisi.Controllers
{
    public class ShoppingController : Controller
    {
        EmarketEntities23 db = new EmarketEntities23();

        [Authorize]
        public ActionResult Index( int? page)
        {
            int c = Convert.ToInt32(@Session["id"]);
            var liste = db.Sepet.Where(x => x.SSepetUser == c).ToList().ToPagedList(page ?? 1, 100);
            return View(liste);
        }
            
        [Authorize]
        public ActionResult SepeteEkle(int id,string adet, int? page)
        {
            Sepet spt = new Sepet();
            var b = Convert.ToInt32(Session["id"]);
            var gelen1 = db.Sepet.Where(x => x.SSepetUrun == id).FirstOrDefault();
            var gelen3 = db.UserLogin.Where(x => x.id == b).FirstOrDefault();
            if (gelen1 == null)
            {
                var gelen = db.Urunler.Where(x => x.id == id).FirstOrDefault();
                if (Convert.ToInt32(adet) <= gelen.UAdet) {
                    spt.SBaslik = gelen.UBaslik;
                    spt.SAciklama = gelen.UAciklama;
                    spt.SFiyat = gelen.UFiyat;
                    spt.SResiyomYol = gelen.UResimYol;
                    spt.SKategori = gelen.UKategori;
                    spt.SAdet = Convert.ToInt32(adet);
                    spt.SSepetUser = b;
                    spt.SSepetUrun = gelen.id;
                    spt.SUadet = gelen.UAdet;
                    Session["tutar"] = Convert.ToInt32(Session["tutar"]) + (Convert.ToInt32(gelen.UFiyat) * Convert.ToInt32(adet));
                    Session["sepetAdet"] = Convert.ToInt32(Session["sepetAdet"]) + Convert.ToInt32(adet);
                    gelen3.USepetAdet = Convert.ToInt32(Session["sepetAdet"]);
                    gelen3.UTutar = Convert.ToInt32(Session["tutar"]);
                    db.Sepet.Add(spt);
                    db.SaveChanges();
                }
            }
            else {
                var gelen2 = db.Urunler.Where(x => x.id == id).FirstOrDefault();
                var gelen = db.Sepet.Where(x => x.SSepetUrun == id).FirstOrDefault();
                var v = Convert.ToInt32(adet);

                if ((gelen.SAdet + v) < gelen2.UAdet) {
                    gelen.SAdet = gelen.SAdet + v;
                    Session["tutar"] = Convert.ToInt32(Session["tutar"]) + (Convert.ToInt32(gelen2.UFiyat) * Convert.ToInt32(adet));
                    Session["sepetAdet"] = Convert.ToInt32(Session["sepetAdet"]) + Convert.ToInt32(adet);
                    gelen3.USepetAdet = Convert.ToInt32(Session["sepetAdet"]);
                    gelen3.UTutar = Convert.ToInt32(Session["tutar"]);
                    db.Entry(gelen).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else {
                    Session["tutar"] = Convert.ToInt32(Session["tutar"]) - (Convert.ToInt32(gelen2.UFiyat) * Convert.ToInt32(gelen.SAdet))+ (Convert.ToInt32(gelen2.UFiyat) * Convert.ToInt32(gelen2.UAdet));
                    Session["sepetAdet"] = Convert.ToInt32(Session["sepetAdet"]) - Convert.ToInt32(gelen.SAdet)+ Convert.ToInt32(gelen2.UAdet);
                    gelen3.USepetAdet = Convert.ToInt32(Session["sepetAdet"]);
                    gelen3.UTutar = Convert.ToInt32(Session["tutar"]);
                    gelen.SAdet = gelen2.UAdet;
                    db.Entry(gelen).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
            }
                


            var liste = db.Sepet.Where(x => x.SSepetUser == b).ToList().ToPagedList(page ?? 1, 100);
            
            return View(liste);
        }

        [Authorize]
        public ActionResult ListeDegistir(int? page,string arttır,string azalt,string baslik) {

            int c = Convert.ToInt32(Session["id"]);
            string art = arttır;
            string az = azalt;

            var sepet1 = db.Urunler.Where(x => x.UBaslik == baslik).FirstOrDefault();
            var sepet = db.Sepet.Where(x=> x.SBaslik == baslik).FirstOrDefault();
            var user = db.UserLogin.Where(x => x.id == c).FirstOrDefault();

            if (art != null )
            {
                if (sepet.SAdet < sepet1.UAdet )
                {
                    sepet.SAdet = sepet.SAdet + 1;
                    Session["sepetAdet"] = Convert.ToInt32(Session["sepetAdet"]) + 1;
                    Session["tutar"] = Convert.ToInt32(Session["tutar"]) + (Convert.ToInt32(sepet.SFiyat));
                    user.USepetAdet = Convert.ToInt32(Session["sepetAdet"]);
                    user.UTutar = Convert.ToInt32(Session["tutar"]);
                }
                else {
                    sepet.SAdet = sepet1.UAdet;
                }
            }
            else {
                if (sepet.SAdet > 1)
                {
                    sepet.SAdet = sepet.SAdet - 1;
                    Session["sepetAdet"] = Convert.ToInt32(Session["sepetAdet"]) - 1;
                    Session["tutar"] = Convert.ToInt32(Session["tutar"]) - (Convert.ToInt32(sepet.SFiyat));
                    user.USepetAdet = Convert.ToInt32(Session["sepetAdet"]);
                    user.UTutar = Convert.ToInt32(Session["tutar"]);
                }
                else {
                    sepet.SAdet = 1;
                }
            }

            db.Entry(user).State = EntityState.Modified;
            db.Entry(sepet).State = EntityState.Modified;
            db.SaveChanges();

            int id = Convert.ToInt32(Session["id"]);
            var y =TempData["adetdegis"];

            var liste = db.Sepet.Where(x => x.SSepetUser == id).ToList().ToPagedList(page ?? 1, 100);

            //kupon kodu
            var varmi = db.UserKupon.Where(x => x.Uuserid == user.id).ToList();
            if (varmi != null)
            {
                for (int i = 0; i < varmi.Count; i++)
                {
                    var tut = varmi[i].Ukuponid;
                    var kuponum = db.Kuponlar.Where(x => x.id == tut).FirstOrDefault();
                    if (user.UTutar < kuponum.Ktutarlimit)
                    {
                        user.UTutar += kuponum.Kindirim;
                        Session["tutar"] = user.UTutar;
                        db.UserKupon.Remove(varmi[i]);
                        db.SaveChanges();
                    }
                }

            }

            return View(liste);
        }

        [Authorize]
        public ActionResult ListeKaldır(int? page, int id)
        {
            int c = Convert.ToInt32(Session["id"]);
            var user = db.UserLogin.Where(x => x.id == c).FirstOrDefault();
            var sepet = db.Sepet.Where(x => x.id == id).FirstOrDefault();
            Session["sepetAdet"] = Convert.ToInt32(Session["sepetAdet"]) - sepet.SAdet;
            Session["tutar"] = Convert.ToInt32(Session["tutar"])-(sepet.SAdet*Convert.ToInt32(sepet.SFiyat));
            user.USepetAdet = Convert.ToInt32(Session["sepetAdet"]);
            user.UTutar = Convert.ToInt32(Session["tutar"]);
            db.Entry(user).State = EntityState.Modified;
            db.Sepet.Remove(sepet);
            db.SaveChanges();

            //kupon kodu
            var varmi = db.UserKupon.Where(x => x.Uuserid == user.id ).ToList();
            if (varmi != null) {
                for (int i = 0; i < varmi.Count; i++)
                {
                    var tut = varmi[i].Ukuponid;
                    var kuponum = db.Kuponlar.Where(x => x.id == tut).FirstOrDefault();
                    if (user.UTutar < kuponum.Ktutarlimit )
                    {
                        user.UTutar += kuponum.Kindirim;
                        Session["tutar"] = user.UTutar;
                        db.UserKupon.Remove(varmi[i]);
                        db.SaveChanges();
                    }
                }
                
            }
            
            return RedirectToAction("Index", "Shopping");
        }

        public ActionResult SepetiBosalt()
        {
            int id = Convert.ToInt32(Session["id"]);
            var user = db.UserLogin.Where(x => x.id == id).FirstOrDefault();

            var gelen = db.Sepet.Where(x => x.SSepetUser == id);
            db.Sepet.RemoveRange(gelen);
            
            user.UTutar = 0;
            user.USepetAdet = 0;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            Session["tutar"] = 0;
            Session["sepetAdet"] = 0;

            //kupon kodu
            var varmi = db.UserKupon.Where(x => x.Uuserid == user.id).ToList();
            if (varmi != null)
            {
                for (int i = 0; i < varmi.Count; i++)
                {
                    var tut = varmi[i].Ukuponid;
                    var kuponum = db.Kuponlar.Where(x => x.id == tut).FirstOrDefault();
                    db.UserKupon.Remove(varmi[i]);
                    db.SaveChanges();
                }

            }


            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult KuponKullan(string kupon)
        {
            UserKupon usrkpn = new UserKupon();
            int id = Convert.ToInt32(Session["id"]);
            var kuponum = db.Kuponlar.Where(x => x.Kisim == kupon).FirstOrDefault();
            if (kuponum != null)
            {
                var user = db.UserLogin.Where(x => x.id == id).FirstOrDefault();
                var varmi = db.UserKupon.Where(x => x.Uuserid == user.id && x.Ukuponid == kuponum.id).FirstOrDefault();
                if (varmi == null)
                {
                    if (user.UTutar >= kuponum.Ktutarlimit)
                    {
                        usrkpn.Ukuponid = kuponum.id;
                        usrkpn.Uuserid = user.id;
                        user.UTutar -= kuponum.Kindirim;
                        Session["tutar"] = user.UTutar;
                        db.UserKupon.Add(usrkpn);
                        db.SaveChanges();
                    }
                    else
                    {
                        TempData["sepetmesaj"] = "En Az " + kuponum.Ktutarlimit + "TL Kadar ürün olmalıdır.";
                    }
                }
                else
                {
                    TempData["sepetmesaj"] = "Bu kupon kodu daha önce kullanıldı.";
                }
            }
            else {
                TempData["sepetmesaj"] = "Bu kupon kodu yanlış.";
            }
            return RedirectToAction("Index", "Shopping");
        }

        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }

    }
}