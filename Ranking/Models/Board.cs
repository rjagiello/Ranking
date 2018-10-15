using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ranking.Models
{
    public class Board
    {   
        [Key]
        public int PostId { get; set; }
        public DateTime PostDate { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }

    }

    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public DateTime CommentDate { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public Board Board { get; set; }
    }
}