using Ranking.DAL;
using Ranking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ranking.Infrastructure
{
    public static class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string UserName()
        {
            ISessionManager session = new SessionManager();
            IUser user;
            if (session.Get<Users>(SessionManager.LoginSessionKey) != null)
            {
                user = session.Get<Users>(SessionManager.LoginSessionKey) as Users;
                return user.Name;
            }
            else if (session.Get<Fans>(SessionManager.LoginFanSessionKey) != null)
            {
                user = session.Get<Fans>(SessionManager.LoginFanSessionKey) as Fans;
                return user.Name;
            }
            return " ";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsAuthenticated()
        {
            ISessionManager session = new SessionManager();
            if (session.Get<Users>(SessionManager.LoginSessionKey) == null)
                return false;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsFanAuthenticated()
        {
            ISessionManager session = new SessionManager();
            if (session.Get<Fans>(SessionManager.LoginFanSessionKey) == null)
                return false;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsAdmin()
        {
            ISessionManager session = new SessionManager();
            if (session.Get<Users>(SessionManager.LoginSessionKey) == null)
                return false;
            var user = session.Get<Users>(SessionManager.LoginSessionKey) as Users;
            if (!user.IsAdmin)
                return false;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AllowAccept(string name)
        {
            return (UserName() == name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsNotAccept()
        {
            ISessionManager session = new SessionManager();
            if (session.Get<Users>(SessionManager.LoginSessionKey) == null)
                return false;
            var user = session.Get<Users>(SessionManager.LoginSessionKey) as Users;
            if (!user.IsAccept)
                return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsNotTwoPlayers()
        {
            ISessionManager session = new SessionManager();
            if (session.Get<Users>(SessionManager.LoginSessionKey) == null)
                return false;
            var user = session.Get<Users>(SessionManager.LoginSessionKey) as Users;
            if (!user.IsTwoPlayers)
                return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool FinishLeague()
        {
            ICacheManager cache = new CacheManager();
            string finish = "";
            if (cache.IsSet(CacheManager.FinishLeagueCacheKey))
                finish = cache.Get(CacheManager.FinishLeagueCacheKey) as string;
            else
            {
                finish = FinishLeagueCheck() ? "true" : "false";
                cache.Set(CacheManager.FinishLeagueCacheKey, finish, 1);
            }
            return finish == "true";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool FinishLeagueCheck()
        {
            RankContext db = new RankContext();
            var users = db.Users.Where(u => u.IsAdmin == false && u.stat == Status.Registration && u.IsAccept == true).ToList();
            var date = db.RoundDate.SingleOrDefault();
            var rank = db.Rank.ToList();

            if (DateTime.Now >= date.RoundEndDatetime)
                return true;
            foreach (var r in rank)
                if (r.Played < users.Count - 1)
                    return false;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsAnyMatch()
        {
            RankContext db = new RankContext();
            var matches = db.Match.ToList();
            if (matches.Count == 0)
                return false;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsArchAdded()
        {
            RankContext db = new RankContext();
            var archs = db.RankArch.ToList();

            if (db.Match.ToList().Count == 0)
                return false;

            var matches = db.Match.Min(m => m.Date);

            foreach (var a in archs)
                if (matches == a.From)
                    return true;

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] Leaders()
        {
            RankContext db = new RankContext();
            if (IsAnyMatch())
            {
                var leaders = db.Rank.OrderBy(p => p.Position).Take(3).ToList();

                if (leaders != null && leaders.Count >= 3)
                {
                    string[] ls = new string[3];
                    ls[0] = leaders[0].Uname;
                    ls[1] = leaders[1].Uname;
                    ls[2] = leaders[2].Uname;
                    return ls;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string KingOfShooters()
        {
            RankContext db = new RankContext();
            var king = db.Member.OrderByDescending(k => k.Goals).Take(1).SingleOrDefault();

            if (king != null)
                return king.MName + "(" + king.Goals + ")";
            else
                return " ";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsAllAccepted()
        {
            RankContext db = new RankContext();
            string name = UserName();
            var matches = db.Match.Where(m => m.IsFinished == false && (m.Team1 == name || m.Team2 == name)).ToList();

            foreach (var m in matches)
            {
                if (m.NotAddedBy == name)
                    return false;
            }
            return true;
        }
    }
}