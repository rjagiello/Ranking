namespace Ranking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Boards", newName: "Board");
            RenameTable(name: "dbo.Comments", newName: "Comment");
            RenameTable(name: "dbo.Matches", newName: "Match");
            RenameTable(name: "dbo.MatchArcheds", newName: "MatchArched");
            RenameTable(name: "dbo.RankArches", newName: "RankArch");
            RenameTable(name: "dbo.Members", newName: "Member");
            RenameTable(name: "dbo.Ranks1", newName: "Rank");
            RenameTable(name: "dbo.RoundDates", newName: "RoundDate");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.RoundDate", newName: "RoundDates");
            RenameTable(name: "dbo.Rank", newName: "Ranks1");
            RenameTable(name: "dbo.Member", newName: "Members");
            RenameTable(name: "dbo.RankArch", newName: "RankArches");
            RenameTable(name: "dbo.MatchArched", newName: "MatchArcheds");
            RenameTable(name: "dbo.Match", newName: "Matches");
            RenameTable(name: "dbo.Comment", newName: "Comments");
            RenameTable(name: "dbo.Board", newName: "Boards");
        }
    }
}
