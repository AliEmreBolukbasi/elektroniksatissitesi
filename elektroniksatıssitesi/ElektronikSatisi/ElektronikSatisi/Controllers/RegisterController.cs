using ElektronikSatisi.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElektronikSatisi.Controllers
{
    
    public class RegisterController : Controller
    {

        EmarketEntities23 db = new EmarketEntities23();
        // GET: Register
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Index(UserLogin lgn, HttpPostedFileBase resim)
        {
            if (Session["Mail"] != null || Session["EMail"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var deger = db.UserLogin.Where(x => x.UKullaniciAdi == lgn.UKullaniciAdi).FirstOrDefault();
            var deger1 = db.AdminLogin.Where(x => x.AKullaniciAdi == lgn.UKullaniciAdi).FirstOrDefault();
            //Kullanıcı adı kullanılmış mı için
            if (lgn != null && deger == null && deger1 == null)
            {
                if (resim != null) {
                    string yolunyolu = ResimKaydet(resim);
                    lgn.UResimYolu = "/Content/images/" + yolunyolu;
                }else
                {
                    lgn.UResimYolu = "/Content/images/avatar.png";
                }
                
                lgn.UBakiye = 0;
                lgn.UTutar = 0;
                lgn.USepetAdet = 0;
                db.UserLogin.Add(lgn);
                db.SaveChanges();
                Response.Redirect("/Login/Index/");
                
            }
            TempData["kayitli"] = "Daha önce kayıtlı kullanıcı..";
            return View();
        }

        string ResimKaydet(HttpPostedFileBase file)
        {

            Image orj = Image.FromStream(file.InputStream);
            Image yeniorj = YenidenBoyutlandir(orj);
            string yol = Path.GetFileNameWithoutExtension(file.FileName) + Guid.NewGuid() + Path.GetExtension(file.FileName);
            yeniorj.Save(Server.MapPath("/Content/images/" + yol));
            return yol;

        }

        Bitmap YenidenBoyutlandir(Image img)
        {

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

        public ActionResult Abone(string email)
        {
            if (Session["Mail"] != null || Session["EMail"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["mail"] = email;
            return View();
        }

        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }
    }
}