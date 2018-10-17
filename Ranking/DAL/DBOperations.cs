using Ranking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ranking.DAL
{
    public class DBOperations
    {
        RankContext db;

        public DBOperations(RankContext db)
        {
            this.db = db;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void DBAdd(object model)
        {
            if (model is Users)
                db.Users.Add(model as Users);
            else if (model is Member)
                db.Member.Add(model as Member);
            else if (model is Fans)
                db.Fans.Add(model as Fans);
            else if (model is Rank)
                db.Rank.Add(model as Rank);
            else if (model is Ranks)
                db.Ranks.Add(model as Ranks);
            else if (model is MatchArched)
                db.MatchArched.Add(model as MatchArched);
            else if (model is RankArch)
                db.RankArch.Add(model as RankArch);
            else if (model is RoundDate)
                db.RoundDate.Add(model as RoundDate);
            else if (model is Match)
                db.Match.Add(model as Match);
            else if (model is Board)
                db.Board.Add(model as Board);
            else if (model is Comment)
                db.Comment.Add(model as Comment);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void DBRemove(object model)
        {
            if (model is Users)
                db.Users.Remove(model as Users);
            else if (model is Member)
                db.Member.Remove(model as Member);
            else if (model is Fans)
                db.Fans.Remove(model as Fans);
            else if (model is Rank)
                db.Rank.Remove(model as Rank);
            else if (model is Ranks)
                db.Ranks.Remove(model as Ranks);
            else if (model is MatchArched)
                db.MatchArched.Remove(model as MatchArched);
            else if (model is RankArch)
                db.RankArch.Remove(model as RankArch);
            else if (model is RoundDate)
                db.RoundDate.Remove(model as RoundDate);
            else if (model is Match)
                db.Match.Remove(model as Match);
            else if (model is Board)
                db.Board.Remove(model as Board);
            else if (model is Comment)
                db.Comment.Remove(model as Comment);
        }  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Users GetUserByName(string name)
        {
            return DBSelect(name, "Users") as Users;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Users GetUserById(int id)
        {
            return DBSelect(id, "Users") as Users;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Rank GetRankByName(string name)
        {
            return DBSelect(name, "Rank") as Rank;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Rank GetRankById(int id)
        {
            return DBSelect(id, "Rank") as Rank;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Match GetMatchById(int id)
        {
            return DBSelect(id, "Match") as Match;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object DBSelect(string name, string type)
        {
            switch (type)
            {
                case "Rank":
                    return db.Rank.Where(o => o.Uname == name).SingleOrDefault() as Rank;
                case "Users":
                    return db.Users.Where(o => o.Name == name).SingleOrDefault() as Users;
            }

            return null;
        }
        /// <summary>
        /// Record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">Type of model</param>
        /// <returns>Record of model</returns>
        public object DBSelect(int id, string type)
        {
            switch (type)
            {
                case "Rank":
                    return db.Rank.Find(id) as Rank;
                case "Users":
                    return db.Users.Find(id) as Users;
                case "Match":
                    return db.Match.Find(id) as Match;
            }

            return null;
        }
        /// <summary>
        /// List
        /// </summary>
        /// <param name="type"></param>
        /// <returns>List of model</returns>
        public object DBSelect (string type)
        {
            switch (type)
            {
                case "Rank":
                    return db.Rank.ToList() as List<Rank>;
                case "Users":
                    return db.Users.ToList() as List<Users>;
                case "Match":
                    return db.Match.ToList() as List<Match>;
            }

            return null;
        }
    }
}