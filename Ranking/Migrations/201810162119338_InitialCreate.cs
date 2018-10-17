namespace Ranking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Boards",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        PostDate = c.DateTime(nullable: false),
                        Author = c.String(),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.PostId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        CommentDate = c.DateTime(nullable: false),
                        Author = c.String(),
                        Text = c.String(),
                        Board_PostId = c.Int(),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Boards", t => t.Board_PostId)
                .Index(t => t.Board_PostId);
            
            CreateTable(
                "dbo.Fans",
                c => new
                    {
                        FanId = c.Int(nullable: false, identity: true),
                        Password = c.String(),
                        IpAddress = c.String(),
                        Name = c.String(),
                        TempName = c.String(),
                        stat = c.Int(nullable: false),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.FanId);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        MatchId = c.Int(nullable: false, identity: true),
                        Team1 = c.String(nullable: false),
                        Team2 = c.String(nullable: false),
                        Team1Score = c.Int(nullable: false),
                        Team2Score = c.Int(nullable: false),
                        IsFinished = c.Boolean(nullable: false),
                        NotAddedBy = c.String(),
                        Date = c.DateTime(nullable: false),
                        Colour = c.String(nullable: false),
                        MembersGoalsSplitTeam1 = c.String(),
                        MembersGoalsSplitTeam2 = c.String(),
                    })
                .PrimaryKey(t => t.MatchId);
            
            CreateTable(
                "dbo.MatchArcheds",
                c => new
                    {
                        MatchID = c.Int(nullable: false, identity: true),
                        Team1 = c.String(),
                        Team2 = c.String(),
                        Team1Score = c.Int(nullable: false),
                        Team2Score = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        RankArch_RankArchId = c.Int(),
                    })
                .PrimaryKey(t => t.MatchID)
                .ForeignKey("dbo.RankArches", t => t.RankArch_RankArchId)
                .Index(t => t.RankArch_RankArchId);
            
            CreateTable(
                "dbo.RankArches",
                c => new
                    {
                        RankArchId = c.Int(nullable: false, identity: true),
                        RoundNumber = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RankArchId);
            
            CreateTable(
                "dbo.Ranks",
                c => new
                    {
                        RankId = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Uname = c.String(),
                        Points = c.Int(nullable: false),
                        Played = c.Int(nullable: false),
                        Won = c.Int(nullable: false),
                        Lost = c.Int(nullable: false),
                        Goals = c.Int(nullable: false),
                        LostGoals = c.Int(nullable: false),
                        RankArch_RankArchId = c.Int(),
                    })
                .PrimaryKey(t => t.RankId)
                .ForeignKey("dbo.RankArches", t => t.RankArch_RankArchId)
                .Index(t => t.RankArch_RankArchId);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        MemberId = c.Int(nullable: false, identity: true),
                        MName = c.String(nullable: false, maxLength: 30),
                        IsCaptain = c.Boolean(nullable: false),
                        UserId = c.Int(nullable: false),
                        Goals = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MemberId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Password = c.String(),
                        Name = c.String(),
                        TempName = c.String(),
                        Captain = c.String(),
                        TempCaptain = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                        IsAccept = c.Boolean(nullable: false),
                        IsTwoPlayers = c.Boolean(nullable: false),
                        stat = c.Int(nullable: false),
                        Email = c.String(),
                        ResetPasswordToken = c.String(),
                        ForgotPassword = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Ranks1",
                c => new
                    {
                        RankId = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Uname = c.String(),
                        Points = c.Int(nullable: false),
                        Played = c.Int(nullable: false),
                        Won = c.Int(nullable: false),
                        Lost = c.Int(nullable: false),
                        Goals = c.Int(nullable: false),
                        LostGoals = c.Int(nullable: false),
                        Captain = c.String(),
                        IsArchives = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RankId);
            
            CreateTable(
                "dbo.RoundDates",
                c => new
                    {
                        RoundDateId = c.Int(nullable: false, identity: true),
                        RoundEndDatetime = c.DateTime(nullable: false),
                        RoundDatetime = c.DateTime(nullable: false),
                        Hour = c.Int(nullable: false),
                        Min = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoundDateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Members", "UserId", "dbo.Users");
            DropForeignKey("dbo.Ranks", "RankArch_RankArchId", "dbo.RankArches");
            DropForeignKey("dbo.MatchArcheds", "RankArch_RankArchId", "dbo.RankArches");
            DropForeignKey("dbo.Comments", "Board_PostId", "dbo.Boards");
            DropIndex("dbo.Members", new[] { "UserId" });
            DropIndex("dbo.Ranks", new[] { "RankArch_RankArchId" });
            DropIndex("dbo.MatchArcheds", new[] { "RankArch_RankArchId" });
            DropIndex("dbo.Comments", new[] { "Board_PostId" });
            DropTable("dbo.RoundDates");
            DropTable("dbo.Ranks1");
            DropTable("dbo.Users");
            DropTable("dbo.Members");
            DropTable("dbo.Ranks");
            DropTable("dbo.RankArches");
            DropTable("dbo.MatchArcheds");
            DropTable("dbo.Matches");
            DropTable("dbo.Fans");
            DropTable("dbo.Comments");
            DropTable("dbo.Boards");
        }
    }
}
