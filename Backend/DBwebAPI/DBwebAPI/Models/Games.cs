using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Types;
using SqlSugar;
using System.Data;
using System.Security.Policy;


namespace DBwebAPI.Models
{

    //public class Team
    //{
    //    [SugarColumn(IsPrimaryKey = true)]
    //    public int team_id { get; set; }
    //    public string? teamName { get; set; }
    //    public string? city { get; set; }
    //    public int? coach_id { get; set; }
    //    public string? manager { get; set; }
    //    public string? boss { get; set; }
    //    public string? teamLogo { get; set; }
    //}

    public class Team
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int team_id { get; set; }
        public string chinesename { get; set; } = "null";
        public string logo { get; set; } = "null";
        public string enname { get; set; } = "null";
        public string city { get; set; } = "null";
        public string coach { get; set; } = "null";
        public int foundedyear { get; set; }= 0;
        public string country { get; set; } = "null";
        public string telephone { get; set; } = "null";
        public string address { get; set; } = "null";
        public string venue_name { get; set; } = "null";
        public string email { get; set; } = "null";
        public int? venue_capacity { get; set; } 
    }

    //public class Coach
    //{
    //    [SugarColumn(IsPrimaryKey = true)]
    //    public int coach_id { get; set; }
    //    public string? coachName { get; set; }
    //    public int? coachYear { get; set; }
    //}

    //public class Game
    //{
    //    [SugarColumn(IsPrimaryKey = true)]
    //    public int game_id { get; set; }
    //    public int? homeTeam { get; set; }
    //    public int? guestTeam { get; set; }
    //    public string? gameName { get; set; }
    //    public int? gameType { get; set; }
    //    public int? status { get; set; }
    //    [SugarColumn(SqlParameterDbType = System.Data.DbType.Date)]
    //    public DateTime? dateTime { get; set; }
    //    public string? city { get; set; }
    //    public string? mainReferee { get; set; }
    //    public int? homeScore { get; set; }
    //    public int? guestScore { get; set; }
    //}

    public class Players
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int player_id { get; set; }

        public string chineseName { get; set; } = "null";
        public string enName { get; set; } = "null";
        public string photo { get; set; } = "null";
        public string country { get; set; } = "null";
        public string height { get; set; } = "null";
        public string type { get; set; } = "null";
        public string age { get; set; } = "null";
        public string playerNumber { get; set; } = "null";
        public string foot { get; set; } = "null";
    }

    public class TeamOwnPlayer
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int team_id { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int player_id { get; set; }
    }

    public class Game
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int game_id { get; set; }

        //public int homeTeam { get; set; }
        //public int guestTeam { get; set; }
        public string type { get; set; } = "null";
        public DateTime startTime { get; set; } 
        public string status { get; set; } = "null";
        public string liveUrl { get; set; } = "null";
        public int homeScore { get; set; } = 0;
        public int guestScore { get; set; } = 0;
    }

    public class GameTeam
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int game_id { get; set; }
        public int homeTeam { get; set; }
        public int guestTeam { get; set; }

    }



    public class PlayerJoinGame
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int game_id { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int player_id { get; set; } 

        public int minutes { get; set; } = 0;
        public int goal { get; set; } = 0;
        public int assist { get; set; } = 0;
        public int red { get; set; } = 0;
        public int yellow { get; set; } = 0;
        public int shoot { get; set; } = 0;
        public int target { get; set; } = 0;
        public int surpass { get; set; } = 0;
        public int surpassSuccess { get; set; } = 0;        
        public int fouled { get; set; } = 0;
        public int pass { get; set; } = 0;
        public int passRate { get; set; } = 0;
        public int tackle { get; set; } = 0;
        public int intercept { get; set; } = 0;
        public int foul { get; set; } = 0;
        public int lost { get; set; } = 0;
        public int surpassed { get; set; } = 0;
    }


}
