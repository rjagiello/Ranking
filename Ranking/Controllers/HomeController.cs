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
            return View();
        }
    }
}