using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ranking.Models
{
    public class Rank
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
        public string Captain { get; set; }
        [NotMapped]
        [DefaultValue(0)]
        public double Ratio { get; set; }
        [NotMapped]
        public string Colour { get; set; }
        [DefaultValue(false)]
        public bool IsArchives { get; set; }
    }
}