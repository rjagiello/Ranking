using Ranking.DAL;
using Ranking.Infrastructure;
using Ranking.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ranking.Controllers
{
    public class ProfileController : Controller
    {
        private SessionManager sessionManager { get; set; }
        private RankContext db;
        private AccountManager accountManager { get; set; }

        public ProfileController()
        {
            db = new RankContext();
            sessionManager = new SessionManager();
            accountManager = new AccountManager(db, sessionManager);
        }

        public ActionResult Index(int id = 0, string name = "", bool alert = false, bool success = false, bool alert1 = false, bool alert2 = false, bool alert3 = false, bool nolist = false)
        {
            ViewBag.Alert1 = alert1 ? "Nie możesz dodać więcej zawodników" : "";
            ViewBag.Alert2 = alert2 ? "Zawodnik o podanej nazwie należy już do drużyny" : "";
            ViewBag.Alert3 = alert3 ? "Wszystkie Twoje mecze muszą być zaakceptowane, żeby dodać zawodnika" : "";
            ViewBag.nolist = nolist;

            if (alert)
                ViewBag.Alert = success ? "Obrazek wgrany pomyślnie" : "Zły format lub zbyt wysoki rozmiar pliku";
            else
                ViewBag.Alert = "";

            ViewBag.Colour = success ? "#00cc00" : "red";

            Rank user;
            if (name == "")
                user = db.Rank.Find(id);
            else
                user = db.Rank.Where(r => r.Uname == name).SingleOrDefault();

            var members = db.Users.Where(u => u.Name == user.Uname).SingleOrDefault().Members.ToList();
            ViewBag.Members = members;

            string filePath = Server.MapPath(Url.Content("~/Content/Images/"));
            if (System.IO.File.Exists(filePath + user.Uname + ".png"))
                ViewBag.Path = user.Uname + ".png";
            else
                ViewBag.Path = "default.png";

            ViewBag.Matches = new MatchManager(db).GetPlayedMatchList(db.Users.Where(u => u.Name == user.Uname).SingleOrDefault().UserId);

            return View(user);
        }

        public ActionResult FileUpload()
        {
            ViewBag.Alert = "";
            return PartialView("_uploadImage");
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            var name = Helpers.UserName();
            var user = db.Rank.Where(u => u.Uname == name).SingleOrDefault();
            if(file != null)
            {
                if (file.ContentType != "image/png" || file.ContentLength > 2000000)
                    return RedirectToAction("Index", "Profile", new { id = user.RankId, alert = true });

                string pic = System.IO.Path.GetFileName(file.FileName);
                pic = user.Uname + ".png";
                string path = System.IO.Path.Combine(Server.MapPath("~/Content/images/"), pic);
                file.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
                return RedirectToAction("Index", "Profile", new { id = user.RankId, alert = true, success = true });
            }
            return RedirectToAction("Index", "Profile", new { id = user.RankId });
        }
    }
}