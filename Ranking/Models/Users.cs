using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ranking.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Name { get; set; }
        public string TempName { get; set; }
        public string Captain { get; set; }
        public string TempCaptain { get; set; }
        [DefaultValue(false)]
        public bool IsAdmin { get; set; }
        [DefaultValue(false)]
        public bool IsAccept { get; set; }
        [DefaultValue(false)]
        public bool IsTwoPlayers { get; set; }
        [DefaultValue(0)]
        public Status stat { get; set; }
        public string Email { get; set; }
        public string ResetPasswordToken { get; set; }
        [DefaultValue(false)]
        public bool ForgotPassword { get; set; }
        public virtual ICollection<Member> Members { get; set; }
    }

    public class Member
    {
        public int MemberId { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "{0} musi mieć co najmniej {2} i nie więcej niż {1} znaków.", MinimumLength = 1)]
        public string MName { get; set; }
        [DefaultValue(false)]
        public bool IsCaptain { get; set; }
        public int UserId { get; set; }
        [DefaultValue(0)]
        public int Goals { get; set; }
        [NotMapped]
        public int Lp { get; set; }
        public virtual Users Users { get; set; }
    }

    public class Fans
    {
        [Key]
        public int FanId { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public string Name { get; set; }
        public string TempName { get; set; }
        [DefaultValue(0)]
        public Status stat { get; set; }
        public string Email { get; set; }
    }

    public enum Status
    {
        Registration,
        Modification
    }
}