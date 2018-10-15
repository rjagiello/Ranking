using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ranking.Models
{
    public class Ranks
    {
        [Key]
        public int RankId { get; set; }
        public int Position { get; set; }
        public string Uname { get; set; }
        [DefaultValue(0)]
        public int Points { get; set; }
        [DefaultValue(0)]
        public int Played { get; set; }
        [DefaultValue(0)]
        public int Won { get; set; }
        [DefaultValue(0)]
        public int Lost { get; set; }
        [DefaultValue(0)]
        public int Goals { get; set; }
        [DefaultValue(0)]
        public int LostGoals { get; set; }
        public RankArch RankArch { get; set; }
    }

    public class MatchArched
    {
        [Key]
        public int MatchID { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        [DefaultValue(0)]
        public int Team1Score { get; set; }
        [DefaultValue(0)]
        public int Team2Score { get; set; }
        public DateTime Date { get; set; }
        public RankArch RankArch { get; set; }
    }

    public class RankArch
    {
        [Key]
        public int RankArchId { get; set; }
        public virtual ICollection<Ranks> Ranks { get; set; }
        public virtual ICollection<MatchArched> MatchArched { get; set; }
        public int RoundNumber { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class RoundDate
    {
        [Key]
        public int RoundDateId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime RoundEndDatetime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime RoundDatetime { get; set; }
        [Range(0, 23, ErrorMessage = "Nieprawidłowa godzina")]
        public int Hour { get; set; }
        [Range(0, 59, ErrorMessage = "Nieprawidłowa minuta")]
        public int Min { get; set; }

    }
}