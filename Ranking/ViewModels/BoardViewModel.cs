using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ranking.ViewModels
{
    public class CommentViewModel
    {
        public DateTime CommentDate { get; set; }
        public string Author { get; set; }
        [StringLength(500, ErrorMessage = "Maksymalnie 500 znaków")]
        public string Text { get; set; }
        public int BoardId { get; set; }
    }

    public class PostViewModel
    {
        public DateTime PostDate { get; set; }
        public string Author { get; set; }
        [StringLength(100, ErrorMessage = "Maksymalnie 100 znaków")]
        public string Text { get; set; }
    }
}