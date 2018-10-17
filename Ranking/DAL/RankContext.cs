using Ranking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Ranking.DAL
{
    public class RankContext : DbContext
    {
        public RankContext()
                : base("RankContext")
        { }
        public DbSet<Match> Match { get; set; }
        public DbSet<Rank> Rank { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<RankArch> RankArch { get; set; }
        public DbSet<Ranks> Ranks { get; set; }
        public DbSet<RoundDate> RoundDate { get; set; }
        public DbSet<Board> Board { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Fans> Fans { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<MatchArched> MatchArched { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}