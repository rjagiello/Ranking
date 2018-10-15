using Ranking.DAL;
using Ranking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ranking.Infrastructure
{
    public class BoardManager
    {
        RankContext db;

        public BoardManager(RankContext db)
        {
            this.db = db;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="auto"></param>
        public void AddPost(string text, bool auto = false)
        {
            var Post = new Board() { Author = auto ? "POL-2018" : Helpers.UserName(), PostDate = DateTime.Now, Text = text };
            db.Board.Add(Post);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PostId"></param>
        /// <param name="text"></param>
        public void AddComment(int PostId, string text)
        {
            var Post = db.Board.Find(PostId);
            var comment = new Comment() { Author = Helpers.UserName(), CommentDate = DateTime.Now, Text = text };
            Post.Comment.Add(comment);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        public void DelegatePost(Board post)
        {
            db.Board.Remove(post);

            var temp = db.Comment.Where(c => c.Board.PostId == post.PostId).ToList();
            db.Comment.RemoveRange(temp);

            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comment"></param>
        public void DeleteComment(Comment comment)
        {
            db.Comment.Remove(comment);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        public void MatchPost(Match match)
        {
            string text = "W dniu " + match.Date.ToShortDateString() + " odbył się mecz drużyn: "
                    + match.Team1 + " i " + match.Team2 + " z wynikiem: " + match.Team1 + " "
                    + match.Team1Score + " : " + match.Team2Score + " " + match.Team2;

            AddPost(text, true);
        }

    }
}