using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ranking.DAL;
using Ranking.Models;
using Ranking.ViewModels;
using Ranking.Infrastructure;

namespace Ranking.Controllers
{
    public class BoardController : Controller
    {
        private RankContext db;
        private AccountManager accountManager;
        private MatchManager matchManager;
        private SessionManager sessionManager { get; set; }
        private BoardManager boardManager;
        private ICacheManager cache;

        public BoardController()
        {
            db = new RankContext();
            sessionManager = new SessionManager();
            accountManager = new AccountManager(db, sessionManager);
            matchManager = new MatchManager(db);
            boardManager = new BoardManager(db);
            cache = new CacheManager();
        }

        public ActionResult Index(int skip = 0)
        {
            ViewBag.End = false;
            var posts = db.Board.OrderByDescending(p => p.PostDate).ToList();

            if (!(posts.Count() > skip + 5))
                ViewBag.End = true;
            ViewBag.Skip = skip + 5;
            if (skip != 0)
                return View(posts.Skip(skip).Take(5));
            else
                return View(posts.Take(5));
        }

        public ActionResult NewPost(int skip)
        {
            skip -= 10;
            return RedirectToAction("Index", new { skip = skip });
        }
        
        public ActionResult PostList(int id)
        {
            var boards = db.Board.Where(b => b.PostId == id).SingleOrDefault();

            var comments = boards.Comment.OrderBy(c => c.CommentDate).ToList();

            ViewBag.Author = boards.Author;
            ViewBag.Date = boards.PostDate;
            ViewBag.Text = boards.Text;
            ViewBag.PostId = id;

            return PartialView("_PostList", comments);
        }

        public ActionResult AddPost()
        {
            return PartialView("_AddPost");
        }

        [HttpPost]
        public ActionResult AddPost(PostViewModel model)
        {
            if(Helpers.UserName() == " ")
                return RedirectToAction("Index", "Home");

            boardManager.AddPost(model.Text);
            return RedirectToAction("Index", "Board");
        }

        public ActionResult AddComment(int id)
        {
            return PartialView("_AddComment", new CommentViewModel() { BoardId = id });
        }

        [HttpPost]
        public ActionResult AddComment(CommentViewModel model)
        {
            if(Helpers.UserName() == " ")
                return RedirectToAction("Index", "Home");

            boardManager.AddComment(model.BoardId, model.Text);
            return RedirectToAction("Index", "Board");
        }

        public ActionResult DeletePost(int id)
        {
            var post = db.Board.Find(id);
            boardManager.DeletePost(post);
            return RedirectToAction("Index", "Board");
        }

        public ActionResult DeleteComment(int id)
        {
            var comment = db.Comment.Find(id);
            boardManager.DeleteComment(comment);
            return RedirectToAction("Index", "Board");
        }
    }
}