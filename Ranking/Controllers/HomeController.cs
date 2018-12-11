using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ranking.DAL;
using Ranking.Models;
using Ranking.Infrastructure;

namespace Ranking.Controllers
{
    public class HomeController : Controller
    {
        private RankContext db;
        private MatchManager matchManager;
        private RankArchManager rankArchManager;
        private ICacheManager cache;

        public HomeController()
        {
            db = new RankContext();
            matchManager = new MatchManager(db);
            rankArchManager = new RankArchManager(db);
            cache = new CacheManager();
        }

        public ActionResult Index()
        {
            matchManager.temp();

            List<Rank> ranking;

            if (cache.IsSet(CacheManager.RankingCacheKey))
                ranking = cache.Get(CacheManager.RankingCacheKey) as List<Rank>;
            else
            {
                ranking = db.Rank.OrderBy(r => r.Position).ToList();
                cache.Set(CacheManager.RankingCacheKey, ranking, 1);
            }
            foreach (var r in ranking)
            {
                r.Ratio = matchManager.GetRatio(r.Won, r.Played);
                if (r.Ratio < 25)
                    r.Colour = "red";
                else if (r.Ratio >= 25 && r.Ratio < 50)
                    r.Colour = "#ff9900";
                else if (r.Ratio >= 50 && r.Ratio < 75)
                    r.Colour = "yellow";
                else if (r.Ratio >= 75 && r.Ratio < 90)
                    r.Colour = "#4dff4d";
                else if (r.Ratio >= 90)
                    r.Colour = "#009900";
            }
            var date = db.RoundDate.SingleOrDefault();

            if (date != null)
            {
                ViewBag.Year = date.RoundDatetime.Year;
                ViewBag.Month = date.RoundDatetime.Month.ToString("D2");
                ViewBag.Day = date.RoundDatetime.Day.ToString("D2");
                ViewBag.Hour = date.RoundDatetime.Hour.ToString("D2");
                ViewBag.Minute = date.RoundDatetime.Minute.ToString("D2");
            }

            return View(ranking);
        }

        public ActionResult Regulations()
        {
            return View();
        }

        public ActionResult TeamList()
        {
            var players = db.Rank.ToList();

            return View(players);
        }

        public ActionResult ArchivesRank()
        {
            var ranks = db.RankArch.ToList();

            if (ranks != null || ranks.Count == 0)
            {
                int[] IdArray = new int[ranks.Count];
                for (int i = 0; i < ranks.Count; i++)
                {
                    IdArray[i] = ranks[i].RankArchId;
                }
                ViewBag.IdArray = IdArray;
            }
            return View();
        }

        public ActionResult ArchivesMatches(int id)
        {
            var rank = db.RankArch.Find(id);
            var matches = rank.MatchArched.OrderByDescending(m => m.Date).ToList();

            ViewBag.RoundNumber = rank.RoundNumber;
            return View(matches);
        }

        public ActionResult AddToArchive()
        {
            var ranking = db.Rank.ToList();
            rankArchManager.AddToArchive(ranking);

            return RedirectToAction("ArchivesRank");
        }

        public ActionResult ClearRanking()
        {
            rankArchManager.ClearRanking();

            return RedirectToAction("Index");
        }

        public ActionResult RankTable(int id)
        {
            var rank = db.RankArch.Where(r => r.RankArchId == id).SingleOrDefault();

            var table = rank.Ranks.OrderBy(t => t.Position).ToList();

            ViewBag.From = rank.From.ToShortDateString();
            ViewBag.To = rank.To.ToShortDateString();
            ViewBag.RoundNumber = rank.RoundNumber;

            return PartialView("_RankTable", table);
        }

        public ActionResult MembersList()
        {
            var members = db.Member.OrderByDescending(k => k.Goals).ToList();
            int i = 0;
            foreach (var m in members)
            {
                m.Lp = ++i;
            }

            return View(members);
        }
    }
}