using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ranking.DAL;
using Ranking.Infrastructure;
using Ranking.Models;
using Ranking.ViewModels;
using System.Web.Helpers;

namespace Ranking.Controllers
{
    public class UserManageController : Controller
    {
        private RankContext db;
        private AccountManager accountManager;
        private MatchManager matchManager;
        private SessionManager sessionManager { get; set; }

        public UserManageController()
        {
            db = new RankContext();
            sessionManager = new SessionManager();
            accountManager = new AccountManager(db, sessionManager);
            matchManager = new MatchManager(db);
        }

        public ActionResult YourMatches()
        {
            List<MatchViewModel> mVM = matchManager.GetPlayedMatchList();
            return View(mVM);
        }

        public ActionResult ChangeData(bool alert1 = false, bool alert2 = false, bool alert3 = false, string name = "", bool nolist = false)
        {
            ViewBag.Alert1 = alert1 ? "Nie możesz dodać więcej zawodników" : "";
            ViewBag.Alert2 = alert2 ? "Zawodnik o podanej nazwie należy już do drużyny" : "";
            ViewBag.Alert3 = alert3 ? "Wszystkie Twoje mecze muszą być zaakceptowane, żeby dodać zawodnika" : "";

            string cName = Helpers.UserName();
            var captian = (from c in db.Users
                           where c.Name == cName
                           select c.Captain).SingleOrDefault();
            UserChangeViewModel userCVM = new UserChangeViewModel() { Name = cName, Captain = captian };
            ViewBag.Members = accountManager.GetMembers();

            return View(userCVM);
        }

        [HttpPost]
        public ActionResult ChangeData(UserChangeViewModel model)
        {
            if(ModelState.IsValid)
            {
                bool notValid = false;

                if(accountManager.IsDuplicateName(model.Name, true))
                {
                    notValid = true;
                    ModelState.AddModelError("NameError", "Drużyna o podanej nazwie istnieje");
                }
                if(accountManager.isDuplicateMember(model.Captain))
                {
                    notValid = true;
                    ModelState.AddModelError("NameError", "Zawodnik o podanej nazwie należy już do drużyny");
                }
                if(!notValid)
                {
                    var sha256 = model.Password != null ? Crypto.SHA256(model.Password) : null;
                    accountManager.ChangeUserData(new UserChangeViewModel() { Name = model.Name, Password = sha256, Captain = model.Captain });
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Members = accountManager.GetMembers();
            return View(model);
        }

        public ActionResult Members(bool profile = false)
        {
            ViewBag.Profile = profile;
            return PartialView("_Members");
        }

        [HttpPost]
        public ActionResult MembersPost(Member model, bool profile = false)
        {
            string action = profile ? "Index" : "ChangeData";
            string Controller = profile ? "Profile" : "UserManage";

            if(ModelState.IsValid)
            {
                bool notValid = false;
                string name = "";
                if (Helpers.UserName() == "")
                    return RedirectToAction("Index", "Home");
                else
                    name = Helpers.UserName();
                if(!accountManager.LimitMembers(name))
                {
                    notValid = true;
                    return RedirectToAction(action, Controller, new { alert1 = true, name = name, nolist = true });
                }
                if (accountManager.isDuplicateMember(model.MName))
                {
                    notValid = true;
                    return RedirectToAction(action, Controller, new { alert = false, alert2 = true, name = name, nolist = true });
                }
                if (accountManager.AreNotAcceptedMatches(name))
                {
                    notValid = true;
                    return RedirectToAction(action, Controller, new { alert = false, alert2 = true, alert3 = true, name = name, nolist = true });
                }
                if(!notValid)
                {
                    accountManager.AddMember(model);

                    var userName = name;
                    var user = db.Users.Where(u => u.Name == userName).SingleOrDefault();

                    accountManager.SetLoginState(false);
                    accountManager.Authentication(null);
                    accountManager.SetLoginState(true);
                    accountManager.Authentication(accountManager.GetUser(new LoginViewModel() { Name = user.Name, Password = user.Password}));

                    return RedirectToAction(action, Controller, new { name = name, nolist = true });
                }
            }
            return RedirectToAction(action, Controller, new { name = Helpers.UserName(), nolist = true });
        }

        public ActionResult DeleteMember(int id)
        {
            Member member = db.Member.Find(id);

            accountManager.DeleteMember(member);
            var rankid = (from r in db.Rank
                          join u in db.Users
                                 on r.Uname equals u.Name
                          where u.UserId == member.UserId
                          select r.RankId).SingleOrDefault();

            return RedirectToAction("Index", "Profile", new { id = rankid });
        }
    }
}