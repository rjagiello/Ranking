using Ranking.DAL;
using Ranking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ranking.Infrastructure
{
    public class RankArchManager
    {
        RankContext db;

        public RankArchManager(RankContext db)
        {
            this.db = db; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rank"></param>
        public void AddToArchive(List<Rank> rank)
        {
            var RankArch = db.RankArch.ToList();

            int RN = 1;
            if (RankArch != null)
                RN = RankArch.Count + 1;

            var matches = db.Match.ToList();

            var Ranks = new List<Ranks>();
            var matchesArch = new List<MatchArched>();

            foreach(var r in rank)
            {
                var rankA = new Ranks()
                {
                    Goals = r.Goals,
                    Lost = r.Lost,
                    LostGoals = r.LostGoals,
                    Played = r.Played,
                    Points = r.Points,
                    Position = r.Position,
                    Uname = r.Uname,
                    Won = r.Won
                };
                Ranks.Add(rankA);
            }
            foreach(var m in matches)
            {
                var match = new MatchArched()
                {
                    Team1 = m.Team1,
                    Team2 = m.Team2,
                    Team1Score = m.Team1Score,
                    Team2Score = m.Team2Score,
                    Date = m.Date
                };
                matchesArch.Add(match);
            }
            var RA = new RankArch()
            {
                RoundNumber = RN,
                Ranks = Ranks,
                MatchArched = matchesArch,
                From = matches.Min(m => m.Date),
                To = db.RoundDate.SingleOrDefault().RoundEndDatetime
            };
            db.RankArch.Add(RA);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        public void ClearRanking()
        {
            var ranking = db.Rank.ToList();
            foreach(var r in ranking)
            {
                r.Goals = 0;
                r.Lost = 0;
                r.LostGoals = 0;
                r.Played = 0;
                r.Points = 0;
                r.Position = 0;
                r.Ratio = 0;
                r.Won = 0;
            }
            var matches = db.Match.ToList();

            db.Match.RemoveRange(matches);
            db.SaveChanges();
        }
    }
}