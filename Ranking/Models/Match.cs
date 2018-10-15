using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ranking.Models
{
    public class Match
    {
        [Key]
        public int MatchId { get; set; }
        [Required]
        public string Team1 { get; set; }
        [Required]
        public string Team2 { get; set; }
        [Required(ErrorMessage = "Wpisz wynik")]
        [Range(0,10, ErrorMessage = "Wynik musi być w zakresie 0 - 10")]
        public int Team1Score { get; set; }
        [Required(ErrorMessage = "Wpisz wynik")]
        [Range(0, 10, ErrorMessage = "Wynik musi być w zakresie 0 - 10")]
        public int Team2Score { get; set; }
        [DefaultValue(false)]
        public bool IsFinished { get; set; }
        public string NotAddedBy { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [NotMapped]
        [DefaultValue(0)]
        public int MemberGoals { get; set; }
        [Required(ErrorMessage = "Wybierz kolor drużyny")]
        public string Colour { get; set; }
        public string MembersGoalsSplitTeam1 { get; set; }
        public string MembersGoalsSplitTeam2 { get; set; }
    }
}