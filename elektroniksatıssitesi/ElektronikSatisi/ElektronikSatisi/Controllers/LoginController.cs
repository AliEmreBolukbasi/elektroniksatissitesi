using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElektronikSatisi.Models;
using System.Web.Security;

namespace ElektronikSatisi.Controllers
{
    public class LoginController : Controller
    {
        EmarketEntities23 db = new EmarketEntities23();

        // GET: Login
        public ActionResult Index()
        {
            if (Request.Cookies["cerezim"] != null)
            {
                HttpCookie kayitlicerez = Request.Cookies["cerezim"];
                Session["EMail"] = kayitlicerez.Values["eposta"];
                Session["Isım"] = kayitlicerez.Values["ad"];
                Session["Soyad"] = kayitlicerez.Values["soyad"];
                Session["Resim"] = kayitlicerez.Values["Resim"];
                Session["id"] = kayitlicerez.Values["id"];
                Session["Bakiye"] = kayitlicerez.Values["bakiye"];
                Session["tutar"] = kayitlicerez.Values["tutar"];
                Session["sepetAdet"] = kayitlicerez.Values["sepetadet"];
            }

            if (Session["Mail"]!=null || Session["EMail"] != null) {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email , string password,string checkbox)
        {

            var deger = db.AdminLogin.Where(x => x.AKullaniciAdi == email && x.ASifre == password ).FirstOrDefault();
            if (deger != null)
            {
                FormsAuthentication.SetAuthCookie(deger.AKullaniciAdi, true);
                string x = deger.AKullaniciAdi.ToString();
                Session["Mail"] = x;
                return RedirectToAction("Admin", "Login");
            }
            else {
                var deger1 = db.UserLogin.Where(x => x.UKullaniciAdi == email && x.USifre == password).FirstOrDefault();
                if (deger1 != null)
                {
                    if (checkbox == "on")
                    {
                        HttpCookie cerez = new HttpCookie("cerezim");
                        cerez.Values.Add("eposta", deger1.UKullaniciAdi);
                        cerez.Values.Add("sifre", deger1.USifre);
                        cerez.Values.Add("ad", deger1.Uisim);
                        cerez.Values.Add("soyad", deger1.Usoyisim);
                        cerez.Values.Add("Resim", deger1.UResimYolu);
                        cerez.Values.Add("id", deger1.id.ToString());
                        cerez.Values.Add("bakiye", deger1.UBakiye.ToString());
                        cerez.Values.Add("tutar", deger1.UTutar.ToString());
                        cerez.Values.Add("sepetadet", deger1.USepetAdet.ToString());
                        cerez.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cerez);
                    }

                    FormsAuthentication.SetAuthCookie(deger1.UKullaniciAdi, false);
                    Session["EMail"] = deger1.UKullaniciAdi.ToString();
                    Session["Isım"] = deger1.Uisim.ToString();
                    Session["Soyad"] = deger1.Usoyisim.ToString();
                    Session["Resim"] = deger1.UResimYolu.ToString();
                    Session["id"] = deger1.id.ToString();
                    Session["Bakiye"] = deger1.UBakiye.ToString();
                    Session["tutar"] = deger1.UTutar.ToString();
                    Session["sepetAdet"] = deger1.USepetAdet.ToString();
                    return RedirectToAction("Index", "Home");
                }
                else {
                    TempData["mesaj"] = "Kullanıcı adı veya şifre yanlış";
                    return RedirectToAction("Index", "Login");
                }

            }
            
            return View();
        }

            //admin panel giriş
            [Authorize]
            public ActionResult Admin()
        {
            var admin = Session["Mail"];
            var deger = db.AdminLogin.Where(x => x.AKullaniciAdi == admin).FirstOrDefault();
            if (deger == null)
            {
                return RedirectToAction("Index", "Login");
            }
                return View(db.Urunler.ToList());
        }

        [Authorize]
        public ActionResult AdminLogout() {
            FormsAuthentication.SignOut();
            // hepsini silme
            Session.RemoveAll();
            if (Request.Cookies["cerezim"] != null)
            {
                Response.Cookies["cerezim"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Index","Home");
        }
        [Authorize]
        public ActionResult UserLogout()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            if (Request.Cookies["cerezim"] != null)
            {
                Response.Cookies["cerezim"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult Edit(int id)
        {
            var admin = Session["Mail"];
            var deger = db.AdminLogin.Where(x => x.AKullaniciAdi == admin).FirstOrDefault();
            if (deger == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var gelen = db.Urunler.Where(x =>x.id==id).FirstOrDefault();
            return View(gelen);

        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Urunler urun, HttpPostedFileBase resim, IEnumerable<HttpPostedFileBase> resimler,string chc1, string chc2)
        {

            Resimler rsm = new Resimler();

            int xid = urun.id;
            var gelenx = db.Urunler.AsNoTracking().Where(x => x.id == xid).FirstOrDefault();

            if (chc1 == "true")
            {
                
                FileInfo fi = new FileInfo(Server.MapPath(gelenx.UResimYol));
                fi.Delete();
                string yolunyolu = ResimKaydet(resim);
                urun.UResimYol = "/Content/images/" + yolunyolu;
                db.Entry(urun).State = EntityState.Modified;
                db.SaveChanges();
            }
            else {
                urun.UResimYol = gelenx.UResimYol;
                db.Entry(urun).State = EntityState.Modified;
                db.SaveChanges();
            }

            var geleny = db.Resimler.AsNoTracking().Where(x => x.RResimUrun == xid);
            
            if (chc2 == "true")
            {
                foreach (var item in geleny)
                {
                    FileInfo fi = new FileInfo(Server.MapPath(item.RResimYolu));
                    fi.Delete();
                    var emre = db.Resimler.Find(item.id);
                    db.Resimler.Remove(emre);
                    
                }
                db.SaveChanges();
                foreach (var item in resimler)
                {
                    string cyol = CResimKaydet(item);
                    rsm.RResimYolu = "/Content/cimages/" + cyol;
                    rsm.RResimUrun = urun.id;
                    db.Resimler.Add(rsm);
                    db.SaveChanges();
                }
            }
            
            return View();

        }
        [Authorize]
        string ResimKaydet(HttpPostedFileBase file) {

            Image orj = Image.FromStream(file.InputStream);
            Image yeniorj =YenidenBoyutlandir(orj);
            string yol = Path.GetFileNameWithoutExtension(file.FileName) + Guid.NewGuid() + Path.GetExtension(file.FileName);
            yeniorj.Save(Server.MapPath("/Content/images/"+yol));
            return yol;

        }
        [Authorize]
        string CResimKaydet(HttpPostedFileBase file)
        {

            Image orj = Image.FromStream(file.InputStream);
            Bitmap yeniorj = YenidenBoyutlandir(orj);
            string yol = Path.GetFileNameWithoutExtension(file.FileName) + Guid.NewGuid() + Path.GetExtension(file.FileName);
            yeniorj.Save(Server.MapPath("/Content/cimages/" + yol));
            return yol;

        }
        [Authorize]
        Bitmap YenidenBoyutlandir(Image img) {

            Bitmap newImage = new Bitmap(300, 300);
            using (Graphics grph = Graphics.FromImage(newImage))
            {
                //enterpolasyon modu ayarı
                grph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //yeni resmin kalite ayarı
                grph.SmoothingMode = SmoothingMode.HighQuality;
                //piksellerin nasıl kaydırılacağı ayarı
                grph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //yeniden boyutlandırma işlemi
                grph.DrawImage(img, new Rectangle(0, 0, 300, 300));

            }
            return newImage;
        }
        [Authorize]
        public ActionResult Create()
        {
            var admin = Session["Mail"];
            var deger = db.AdminLogin.Where(x => x.AKullaniciAdi == admin).FirstOrDefault();
            if (deger == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create(Urunler urun, HttpPostedFileBase resim,IEnumerable<HttpPostedFileBase> resimler)
        {

            string yolunyolu = ResimKaydet(resim);
            urun.UResimYol = "/Content/images/" + yolunyolu;
            db.Urunler.Add(urun);
            db.SaveChanges();
            Resimler rsm = new Resimler();
            foreach (var item in resimler)

            {
                string cyol = CResimKaydet(item);
                rsm.RResimYolu = "/Content/cimages/" + cyol;
                rsm.RResimUrun = urun.id;
                db.Resimler.Add(rsm);
                db.SaveChanges();
            }
            return View();
        }

        [Authorize]
        public ActionResult  Mesaj() {
            var admin = Session["Mail"];
            var deger = db.AdminLogin.Where(x => x.AKullaniciAdi == admin).FirstOrDefault();
            if (deger == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(db.MesajBox.ToList());
        
        }

        public ActionResult SifremiUnuttum()
        {
            return View();
        }

            public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }



    }
}