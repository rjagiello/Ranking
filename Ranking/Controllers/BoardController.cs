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

        public ActionResult Index(int skip = 0, bool newPost = false)
        {
            ViewBag.End = false;
            var posts = db.Board.OrderByDescending(p => p.PostDate).ToList();

            string filePath = Server.MapPath(Url.Content("~/Content/Images/"));

            Dictionary<string, string> imagesList;

            if (cache.IsSet(CacheManager.UsersListCacheKey))
            {
                imagesList = cache.Get(CacheManager.UsersListCacheKey) as Dictionary<string, string>;
                if (newPost)
                    imagesList.Add(posts.FirstOrDefault().Author, posts.FirstOrDefault().Author);
            }
            else
            {
                //var teams = db.Users.ToList();
                //var fans = db.Fans.ToList();

                //List<IUser> users = new List<IUser>();
                //foreach (var t in teams)
                //    users.Add(t);
                //foreach (var f in fans)
                //    users.Add(f);
                List<string> users = new List<string>();

                foreach (var u in posts)
                {
                    users.Add(u.Author);
                    foreach (var c in u.Comment)
                    {
                        users.Add(c.Author);
                    }
                }

                imagesList = new Dictionary<string, string>();
                //imagesList.Add("POL-2018", "default.png");
                foreach (var p in users.Distinct())
                {
                    if (System.IO.File.Exists(filePath + p + ".png"))
                        imagesList.Add(p, p + ".png");
                    else
                        imagesList.Add(p, "default.png");
                }
                cache.Set(CacheManager.UsersListCacheKey, imagesList, 1);
            }
            ViewBag.ImageList = imagesList;

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
        
        public ActionResult PostList(int id, Dictionary<string, string> list)
        {
            var boards = db.Board.Where(b => b.PostId == id).SingleOrDefault();

            var comments = boards.Comment.OrderBy(c => c.CommentDate).ToList();

            ViewBag.Author = boards.Author;
            ViewBag.Date = boards.PostDate;
            ViewBag.Text = boards.Text;
            ViewBag.PostId = id;
            ViewBag.ImageList = list;

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
            return RedirectToAction("Index", "Board", new { skip = false, newPost = true});
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