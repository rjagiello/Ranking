using Ranking.DAL;
using Ranking.Infrastructure;
using Ranking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ranking.Controllers
{
    public class ManageController : Controller
    {
        private RankContext db;
        private AccountManager accountManager;
        private SessionManager sessionManager { get; set; }
        private MatchManager matchManager;

        public ManageController()
        {
            db = new RankContext();
            sessionManager = new SessionManager();
            accountManager = new AccountManager(db, sessionManager);
            matchManager = new MatchManager(db);
        }

        public ActionResult Index()
        {
            var users = db.Users.Where(u => u.IsAdmin == false).ToList();

            Dictionary<int, string> tokenList = new Dictionary<int, string>();

            foreach(var p in users)
            {
                if (p.ResetPasswordToken != null)
                    tokenList.Add(p.UserId, p.Email + "?Subject=Przypomnienie_Hasla&body=" + Url.Action("ForgotPassword", "Account", new { token = p.ResetPasswordToken }, "http"));
                else
                    tokenList.Add(p.UserId, "");
            }
            ViewBag.TokenList = tokenList;

            return View(users);
        }

        public ActionResult AddCaptainToMember()
        {
            var users = db.Users.Where(u => u.IsAdmin == false).ToList();

            foreach(var u in users)
            {
                u.Members.Add(new Member() { IsCaptain = true, MName = u.Captain });
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult DeleteUser(int id)
        {
            var user = db.Users.Find(id);
            accountManager.DeleteUser(user);
            return RedirectToAction("Index", "Manage");
        }

        public ActionResult DeleteFan(int id)
        {
            var user = db.Fans.Find(id);
            accountManager.DeleteFan(user);
            return RedirectToAction("Index", "Manage");
        }

        public ActionResult AcceptUser(int id)
        {
            var user = db.Users.Find(id);

            switch(user.stat)
            {
                case Status.Modification:
                    {
                        accountManager.ChangeUserDataFinish(id);
                    }
                    break;
                case Status.Registration:
                    {
                        matchManager.AddPlayer(user);
                    }
                    break;
            }

            return RedirectToAction("Index", "Manage");
        }

        public ActionResult ChangeNextRoundDate()
        {
            var roundDate = db.RoundDate.SingleOrDefault();
            return PartialView("_ChangeNextRoundDate", roundDate != null ? roundDate : new RoundDate() { RoundDatetime = DateTime.Now });
        }

        [HttpPost]
        public ActionResult ChangeNextRoundDate(RoundDate model)
        {
            var date = db.RoundDate.SingleOrDefault();
            model.RoundDatetime = new DateTime(model.RoundDatetime.Year, model.RoundDatetime.Month, model.RoundDatetime.Day, model.Hour, model.Min, 0);
            if (date != null)
            {
                date.RoundDatetime = model.RoundDatetime;
                date.Hour = model.Hour;
                date.Min = model.Min;
                date.RoundEndDatetime = model.RoundEndDatetime;
            }
            else
                db.RoundDate.Add(new RoundDate() { RoundDatetime = model.RoundDatetime, RoundEndDatetime = model.RoundEndDatetime });
            db.SaveChanges();

            return RedirectToAction("Index", "Manage");
        }

        public ActionResult FanList()
        {
            return PartialView("_FanList", db.Fans.ToList());
        }

        public JsonResult SetToken(int id)
        {
            var user = db.Users.Find(id);
            accountManager.SetResetPasswordToken(id);
            var token = "<a href='mailto:" + user.Email + "?Subject=Przypomnienie_Hasla&amp;body=" + Url.Action("ForgotPassword", "Account", new { token = user.ResetPasswordToken }, "http") + "'>wyslij</a>";
            return Json(token, JsonRequestBehavior.AllowGet);
        }
    }
}