using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElektronikSatisi.Models;

namespace ElektronikSatisi.Controllers
{
    public class IletisimController : Controller
    {

        EmarketEntities23 db = new EmarketEntities23();
        // GET: Iletisim
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IletisimeGec(MesajBox mesajBox)
        {
            if (mesajBox.MBUrun=="" || mesajBox.MBUrun == null) {
                mesajBox.MBUrun = "Mesajdır";
            }
            db.MesajBox.Add(mesajBox);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Telefon(string id)
        {
            return RedirectToAction("UrunGetirFiltre", "Telefon", new { filtre = id });
        }
    }
}