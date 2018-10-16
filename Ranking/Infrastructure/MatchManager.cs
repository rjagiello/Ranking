using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ranking.Models;
using Ranking.DAL;
using Ranking.ViewModels;

namespace Ranking.Infrastructure
{
    public class MatchManager
    {
        RankContext db;
        BoardManager boardManager;

        public MatchManager(RankContext db)
        {
            this.db = db;
            boardManager = new BoardManager(db);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        public void AddMatch(Match match)
        {
            match.NotAddedBy = Helpers.UserName() == match.Team1 ? match.Team2 : match.Team1;
            match.Date = DateTime.Now;
            if(match.Colour == "niebieska")
            {
                string tempName = "";
                string tempMembersGoalsSplitTeam1 = "";
                int tempScore;
                tempName = match.Team1;
                tempScore = match.Team1Score;
                tempMembersGoalsSplitTeam1 = match.MembersGoalsSplitTeam1;

                match.Team1 = match.Team2;
                match.Team1Score = match.Team2Score;
                match.Team2 = tempName;
                match.Team2Score = tempScore;
                match.MembersGoalsSplitTeam1 = match.MembersGoalsSplitTeam2;
                match.MembersGoalsSplitTeam2 = tempMembersGoalsSplitTeam1;
            }
            db.Match.Add(match);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        public void DeleteMatch(Match match)
        {
            db.Match.Remove(match);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        public void AcceptMatch(Match match)
        {
            var rank1 = db.Rank.Where(r => r.Uname == match.Team1).SingleOrDefault();
            var rank2 = db.Rank.Where(r => r.Uname == match.Team2).SingleOrDefault();

            db.Match.Find(match.MatchId).IsFinished = true;
            db.Match.Find(match.MatchId).Colour = "czerowny";

            if(match.Team1Score > match.Team2Score)
            {
                rank1.Won += 1;
                rank1.Points += 3;
                rank1.Played += 1;

                rank2.Lost += 1;
                rank2.Played += 1;
            }
            else
            {
                rank2.Won += 1;
                rank2.Points += 3;
                rank2.Played += 1;

                rank1.Lost += 1;
                rank1.Played += 1;
            }
            rank1.Goals += match.Team1Score;
            rank2.Goals += match.Team2Score;

            var user1 = db.Users.Where(u => u.Name == match.Team1).SingleOrDefault();
            var user2 = db.Users.Where(u => u.Name == match.Team2).SingleOrDefault();

            string[] value1 = match.MembersGoalsSplitTeam1.ToString().TrimEnd().Split(' ');
            string[] value2 = match.MembersGoalsSplitTeam2.ToString().TrimEnd().Split(' ');

            if (match.MembersGoalsSplitTeam1 == "admin")
            {
                int count = user1.Members.Count();
                int mod = match.Team1Score % count;
                if (mod == 0)
                    foreach (var m in user1.Members)
                        m.Goals += match.Team1Score / count;
                else
                {
                    int result = match.Team1Score - mod;
                    foreach (var m in user1.Members)
                        m.Goals += result / count;
                    user1.Members.Where(m => m.IsCaptain == true).SingleOrDefault().Goals += int.Parse(value1[1]);
                }
            }
            else
            {
                foreach(var v in value1)
                {
                    string[] val = v.Split('|');
                    user1.Members.Where(m => m.MemberId == int.Parse(val[0])).SingleOrDefault().Goals += int.Parse(val[1]);
                }
            }

            if (match.MembersGoalsSplitTeam2 == "admin")
            {
                int count = user2.Members.Count();
                int mod = match.Team2Score % count;
                if (mod == 0)
                    foreach (var m in user2.Members)
                        m.Goals += match.Team2Score / count;
                else
                {
                    int result = match.Team2Score - mod;
                    foreach (var m in user2.Members)
                        m.Goals += result / count;
                    user2.Members.Where(m => m.IsCaptain == true).SingleOrDefault().Goals += int.Parse(value1[1]);
                }
            }
            else
            {
                foreach (var v in value2)
                {
                    string[] val = v.Split('|');
                    user2.Members.Where(m => m.MemberId == int.Parse(val[0])).SingleOrDefault().Goals += int.Parse(val[1]);
                }
            }

            db.SaveChanges();

            boardManager.MatchPost(match);

            PlayersPosition();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="win"></param>
        /// <param name="played"></param>
        /// <returns></returns>
        public double GetRatio(int win, int played)
        {
            if(played !=0)
            {
                double ratio = (win * 100) / played;
                return Math.Round(ratio);
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        public void temp()
        {
            var matches = db.Match.Where(m => m.IsFinished == true).ToList();
            var ranks = db.Rank.ToList();

            for(int i =0; i< ranks.Count; i++)
            {
                int lostGoals = 0;
                for(int j = 0; j < matches.Count; j++)
                {
                    if (ranks[i].Uname == matches[j].Team1)
                        lostGoals += matches[j].Team2Score;
                    if (ranks[i].Uname == matches[j].Team2)
                        lostGoals += matches[j].Team1Score;
                }
                ranks[i].LostGoals = lostGoals;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        public void PlayersPosition()
        {
            var rank = db.Rank.ToList();

            rank.Sort((x, y) => x.Points.CompareTo(y.Points));

            int p = rank.Count;
            foreach(var r in rank)
            {
                r.Position = p;
                p--;
            }

            for(int i = 0; i<rank.Count; i++)
            {
                for (int j = 0; j < rank.Count; j++)
                {
                    if(rank[j].Points == rank[i].Points && i != j)
                    {
                        if(rank[j].Position < rank[i].Position && rank[j].Lost > rank[i].Lost)
                        {
                            int pos = rank[j].Position;
                            rank[j].Position = rank[i].Position;
                            rank[i].Position = pos;
                        }
                        if(rank[j].Position < rank[i].Position && rank[j].LostGoals > rank[i].LostGoals && rank[j].Lost == rank[i].Lost)
                        {
                            int pos = rank[j].Position;
                            rank[j].Position = rank[i].Position;
                            rank[i].Position = pos;
                        }
                        if (rank[j].Position < rank[i].Position && rank[j].LostGoals == rank[i].LostGoals && rank[j].Lost == rank[i].Lost && rank[j].Goals < rank[i].Goals)
                        {
                            int pos = rank[j].Position;
                            rank[j].Position = rank[i].Position;
                            rank[i].Position = pos;
                        }
                    }
                }
            }
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool PlayerValid(string[] names)
        {
            for (int i = 0; i < names.Count(); i++)
                for (int j = 0; j < names.Count(); j++)
                    if (names[i] == names[j] && i != j)
                        return false;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TS1"></param>
        /// <param name="TS2"></param>
        /// <returns></returns>
        public bool ScoreValid(int TS1, int TS2)
        {
            if (TS1 == TS2)
                return false;
            if (!(TS1 == 10 || TS2 == 10))
                return false;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="team"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool YourTeamValid(string team, string name)
        {
            return team == name || name == "admin";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="membersgoals"></param>
        /// <param name="teamgoals"></param>
        /// <returns></returns>
        public bool MembersGoalsValid(int membersgoals, int teamgoals)
        {
            return membersgoals == teamgoals;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool MatchValidate(string[] names)
        {
            var matches = db.Match.ToList();
            foreach(var m in matches)
            {
                if (names[0] == m.Team1 && names[1] == m.Team2 ||
                    names[1] == m.Team1 && names[0] == m.Team2)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <param name="Uname"></param>
        /// <returns></returns>
        public bool IsPlayedMatch(string[] names, string Uname)
        {
            var admin = db.Users.Where(a => a.Name == Uname).SingleOrDefault();
            foreach (var p in names)
            {
                return p == Uname || admin.IsAdmin;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void AddPlayer(Users user)
        {
            int pos = 0;
            if (db.Rank.Count() > 0)
                pos = db.Rank.Max(p => p.Position);
            var player = new Rank() { Uname = user.Name, Position = pos + 1, Captain = user.Captain };
            var User = db.Users.Where(u => u.Name == user.Name).SingleOrDefault();
            user.IsAccept = true;
            db.Rank.Add(player);
            db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private List<Match> GetYourMatchesList(string name)
        {
            return db.Match.Where(u => u.Team1 == name || u.Team2 == name).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<MatchViewModel> GetPlayedMatchList(int id = 0)
        {
            Users user = null;
            if (id != 0)
                user = db.Users.Find(id);

            List<MatchViewModel> mVM = new List<MatchViewModel>();
            var playedMatches = GetYourMatchesList(user == null ? Helpers.UserName() : user.Name);

            string playersIM = "";

            foreach(var lm in playedMatches)
            {
                mVM.Add(new MatchViewModel()
                {
                    Team1 = lm.Team1,
                    Team2 = lm.Team2,
                    Date = lm.Date,
                    Team1Score = lm.Team1Score,
                    Team2Score = lm.Team2Score,
                    IsPlayed = true
                });
                if (lm.Team1 != (user == null ? Helpers.UserName() : user.Name))
                    playersIM += lm.Team1;
                if (lm.Team2 != (user == null ? Helpers.UserName() : user.Name))
                    playersIM += lm.Team2;
            }

            var users = db.Users.Where(u => u.IsAccept == true && u.IsAdmin == false).ToList();
            for(int i=0; i< users.Count; i++)
                if (!playersIM.Contains(users[i].Name) && users[i].Name != (user == null ? Helpers.UserName() : user.Name))
                    mVM.Add(new MatchViewModel()
                    {
                        Team1 = users[i].Name,
                        Team2 = user == null ? Helpers.UserName() : user.Name,
                        Email = users[i].Email
                    });
            mVM.Reverse();

            return mVM;
        }
    }
}