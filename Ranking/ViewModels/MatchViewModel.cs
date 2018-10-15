using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Ranking.ViewModels
{
    public class MatchViewModel
    {
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        [DefaultValue(0)]
        public int Team1Score { get; set; }
        [DefaultValue(0)]
        public int Team2Score { get; set; }
        public DateTime Date { get; set; }
        [DefaultValue(false)]
        public bool IsPlayed { get; set; }
        public string Email { get; set; }
    }
    public class AcceptMatchViewModel
    {
        public string Team2 { get; set; }
        public int Team2Score { get; set; }
        public int MembersGoals { get; set; }
        public string MembersGoalsSplit { get; set; }
        public int MatchId { get; set; }
    }
}