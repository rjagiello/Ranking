using Ranking.DAL;
using Ranking.Models;
using Ranking.ViewModels;
using Ranking.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ranking.Controllers
{
    public class MatchController : Controller
    {
        private RankContext db;
        private MatchManager matchManager;

        public  MatchController()
        {
            db = new RankContext();
            matchManager = new MatchManager(db);
        }

        public ActionResult Index(bool Suc = false)
        {
            ViewBag.UserList = GetUserList();
            ViewBag.ColourList = GetColourList();

            ViewBag.Nortification = Suc ? "Mecz dodany" : "";
            return View();
        }

        [HttpPost]
        public ActionResult SubmitMatch(Match model)
        {
            if(ModelState.IsValid)
            {
                var match = new Match() { Team1 = model.Team1, Team2 = model.Team2, Team1Score = model.Team1Score, Team2Score = model.Team2Score, MembersGoalsSplitTeam1 = model.MembersGoalsSplitTeam1, Colour = model.Colour };

                string[] names = { model.Team1, model.Team2 };
                bool notValid = false;

                if(!matchManager.PlayerValid(names))
                {
                    notValid = true;
                    ModelState.AddModelError("playererror", "Wybrałeś dwie te same drużyny");
                }
                if (!matchManager.ScoreValid(match.Team1Score, match.Team2Score))
                {
                    notValid = true;
                    ModelState.AddModelError("playererror", "Nieprawidłowy wynik meczu");
                }
                if (!matchManager.YourTeamValid(match.Team1, Helpers.UserName()))
                {
                    notValid = true;
                    ModelState.AddModelError("playererror", "Twoja drużyna powinna być po lewej stronie");
                }
                if (!matchManager.MatchValidate(names))
                {
                    notValid = true;
                    ModelState.AddModelError("playererror", "W lidze można zagrać tylko raz z tą samą drużyną");
                }
                if (Helpers.UserName() == " ")
                    return RedirectToAction("Index", "Home");
                if (!matchManager.IsPlayedMatch(names, Helpers.UserName()))
                {
                    notValid = true;
                    ModelState.AddModelError("playererror", "Twoja drużyna nie gra w tym meczu");
                }
                if (!matchManager.MembersGoalsValid(model.MemberGoals, model.Team1Score))
                {
                    notValid = true;
                    ModelState.AddModelError("playererror", "Suma strzelonych bramek przez graczy musi być równa liczbie strzelonych bramek przez drużynę");
                }

                if(!notValid)
                {
                    TempData["Valid"] = true;
                    TempData["Score"] = model.Team1 + " " + model.Team1Score + " : " + model.Team2Score + " " + model.Team2;

                    string Tname = Helpers.UserName() == model.Team1 ? model.Team2 : model.Team1;

                    TempData["Email"] = (from n in db.Users
                                         where n.Name == Tname
                                         select n.Email).SingleOrDefault();
                    matchManager.AddMatch(match);

                    return RedirectToAction("Index", "Match", new { suc = true });
                }
            }
            model.MemberGoals = 0;
            model.Team1Score = 0;
            ViewBag.UserList = GetUserList();
            ViewBag.ColourList = GetColourList();

            return View("Index", model);
        }

        public ActionResult GetMembers(string name)
        {
            var user = db.Users.Where(u => u.Name == name).SingleOrDefault();
            if(user !=null)
            {
                var members = user.Members.ToList();
                List<string> memb = new List<string>();

                foreach(var m in members)
                {
                    memb.Add("<tr class='id'><td>" + m.MName + " <input style='width: 50px; float: right;' type = 'number' min='0' max='10' required='required' value='0' onchange='SumInputs()' class ='member' id ='" + m.MemberId + "'/> </td></tr>");
                }
                return Json(memb, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MatchList()
        {
            var matchlist = db.Match.ToList();
            matchlist.Reverse();
            return PartialView("_MatchList", matchlist);
        }

        public ActionResult AcceptMatch(int id)
        {
            string name = Helpers.UserName();
            var match = db.Match.Find(id);
            int score;

            score = match.Team1 == name ? match.Team1Score : match.Team2Score;

            ViewBag.Members = db.Users.Where(u => u.Name == name).SingleOrDefault().Members.ToList();

            var model = new AcceptMatchViewModel() { MatchId = id, Team2 = name, Team2Score = score };

            if(name == "admin")
            {
                model.MembersGoals = score;
                model.MembersGoalsSplit = "admin";
            }

            return PartialView("_AcceptMatch", model);
        }

        [HttpPost]
        public ActionResult AcceptMatch(AcceptMatchViewModel model)
        {
            if(ModelState.IsValid)
            {
                bool notValid = false;
                var match = db.Match.Find(model.MatchId);
                if(!matchManager.MembersGoalsValid(model.MembersGoals, model.Team2Score))
                {
                    notValid = true;
                    model.MembersGoals = 0;
                    ModelState.AddModelError("accepterror", "Suma strzelonych bramek przez graczy musi być równa liczbie strzelonych bramek przez drużynę");
                    return RedirectToAction("Index", "Match", model);
                }

                if(!notValid)
                {
                    if (match.MembersGoalsSplitTeam1 == null)
                        match.MembersGoalsSplitTeam1 = model.MembersGoalsSplit;
                    else
                        match.MembersGoalsSplitTeam2 = model.MembersGoalsSplit;
                    matchManager.AcceptMatch(match);
                    return RedirectToAction("Index", "Match");
                }
            }
            return RedirectToAction("Index", "Match");
        }

        public ActionResult DeleteMatch(int id)
        {
            var match = db.Match.Find(id);
            matchManager.DeleteMatch(match);
            return RedirectToAction("Index", "Match");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private SelectList GetColourList()
        {
            List<string> Colours = new List<string>();
            Colours.Add("czerwona");
            Colours.Add("niebieska");

            return new SelectList(Colours);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private SelectList GetUserList()
        {
            List<Users> users = db.Users.Where(u => u.IsAdmin == false && u.IsAccept == true && u.IsTwoPlayers == true).ToList();

            return new SelectList(users, "Name", "Name");
        }
    }
}