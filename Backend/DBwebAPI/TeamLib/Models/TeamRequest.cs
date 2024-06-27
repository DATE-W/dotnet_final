using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLib.Models
{
    public class TeamInGameTimePara
    {
        public string dateTime { get; set; } = "";
        public int gameType { get; set; }
    }
    public class TeamInGameTimeVal
    {
        public string startTime { get; set; } = "null";
        public int homeTeam { get; set; } = 0;
        public int guestTeam { get; set; } = 0;
        public string homeTeamName { get; set; } = "null";
        public string guestTeamName { get; set; } = "null";
        public string status { get; set; } = "null";
        public int homeScore { get; set; } = 0;
        public int guestScore { get; set; } = 0;
        public string homeLogo { get; set; } = "null";
        public string guestLogo { get; set; } = "null";
        public string gameUid { get; set; } = "null";

    }
    public class getGameByUidPara
    {
        public string? gameUid { get; set; }
    }
    public class recentGamesVal
    {
        public string gameDate { get; set; } = "";
        public string opponentName { get; set; } = "";
        public int opponentTeamId { get; set; }
        public int homeScore { get; set; }
        public int opponentScore { get; set; }
        public string opponentLogo { get; set; } = "";
        public string gameUid { get; set; } = "";
        public string gameType { get; set; } = "";
    }
    public class getGameByUidVal
    {
        public string dateTime { get; set; } = "";
        public string startTime { get; set; } = "";
        public int homeTeam { get; set; }
        public int guestTeam { get; set; }
        public string homeTeamName { get; set; } = "";
        public string guestTeamName { get; set; } = "";
        public string leagueName { get; set; } = "";
        public int leagueType { get; set; }
        public string status { get; set; } = "";
        public int homeScore { get; set; }
        public int guestScore { get; set; }
        public string homeLogo { get; set; } = "";
        public string guestLogo { get; set; } = "";
        public string homeLink { get; set; } = "";
        public string guestLink { get; set; } = "";
        public string liveStream { get; set; } = "";
        public List<recentGamesVal> homeRecentGames { get; set; }
        public List<recentGamesVal> guestRecentGames { get; set; }

    }
    public class getTeamMatchesByNamePara
    {
        public string? teamName { get; set; }
    }
    public class getTeamMatchesByNameVal
    {
        public string gameDate { get; set; } = "";
        public int homeTeam { get; set; }
        public int opponentTeam { get; set; }
        public string opponentName { get; set; } = "";
        public int homeScore { get; set; }
        public int opponentScore { get; set; }
        public string opponentLogo { get; set; } = "";
        public string gameUid { get; set; } = "";

    }
    public class TeamInGameTypePara
    {
        public int gameType { get; set; }
    }

    public class TeamInGameTypeVal
    {
        public string? teamName { get; set; }
        public string? teamLogo { get; set; }

    }
    public class getTeamInfoByNamePara
    {
        public string teamName { get; set; } = "null";
    }
    public class teamMemberVal
    {
        public int player_id { get; set; }
        public string playerName { get; set; } = "";
        public string playerPhoto { get; set; } = "";
        public string playerPosition { get; set; } = "";
        public string playerNumber { get; set; } = "";
        public int? playerAppearance { get; set; }
        public int? playerShoot { get; set; }
        public int? playerGoal { get; set; }
        public string playerNationality { get; set; } = "";
    }
    public class getTeamInfoByNameVal
    {
        public string teamName { get; set; } = "null";
        public int team_id { get; set; }
        public string enName { get; set; } = "";
        public string logo { get; set; } = "";
        public string city { get; set; } = "";
        public int foundYear { get; set; }
        public string coach { get; set; } = "";
        public string country { get; set; } = "";
        public string telephone { get; set; } = "";
        public string address { get; set; } = "";
        public string venue_name { get; set; } = "";
        public string email { get; set; } = "";
        public int? venue_capacity { get; set; }
        public string leagueType { get; set; } = "";

        public List<teamMemberVal>? teamMember { get; set; }
        public List<recentGamesVal>? recentGames { get; set; }
    }

    public class topScorerVal
    {
        public string topScorerName { get; set; } = "";
        public int? goals { get; set; }
    }

    public class topScorersInGameTypePara
    {
        public string gameName { get; set; } = "";

    }
    public class topScorersInGameTypeVal
    {
        public int? player_id { get; set; }
        public int? goals { get; set; }
        public string? playerName { get; set; }
        public string? photo { get; set; }
        public string? teamName { get; set; }
        public string? teamLogo { get; set; }
    }

    public class searchTeamOrPlayerPara
    {
        public string key { get; set; } = "";
        public int gameType { get; set; }
    }
    public class searchedTeamVal
    {
        public int? searchedTeamId { get; set; }
        public string? searchedTeamName { get; set; }
        public string? searchedTeamLogo { get; set; }
    }
    public class searchedPlayerVal
    {
        public int? searchedPlayerId { get; set; }
        public string? searchedPlayerName { get; set; }
        public string? searchedPlayerPhoto { get; set; }
    }

    public class getPlayerDetailPara
    {
        public string? playerName { get; set; }
    }
    public class relatedPlayer
    {
        public string? playerName { get; set; }
        public string? playerPhoto { get; set; }
        public string? type { get; set; }
    }

    public class eventData
    {
        public string? seasonName { get; set; }
        public int? appearance { get; set; }
        public int? goal { get; set; }
        public int? shoot { get; set; }
        public int? pass { get; set; }
        public int? assist { get; set; }
        public int? red { get; set; }
        public int? yellow { get; set; }

    }
    public class getPlayerDetailVal
    {
        public int? team_id { get; set; }
        public int? player_id { get; set; }
        public string? club { get; set; }
        public string? enName { get; set; }
        public string? photo { get; set; }
        public string? position { get; set; }
        public string? number { get; set; }
        public string? nationality { get; set; }
        public string? age { get; set; }
        public string? height { get; set; }
        public string? dominantFoot { get; set; }

        public List<relatedPlayer>? relatedPlayer { get; set; }
        public List<eventData>? eventData { get; set; }

    }

    //lq特供
    public class showRecentGamesVal
    {
        public string? status { get; set; }
        public string? homeTeamName { get; set; }
        public string? guestTeamName { get; set; }
        public string? homeTeamLogo { get; set; }
        public string? guestTeamLogo { get; set; }
        public int? guestScore { get; set; }
        public int? homeScore { get; set; }
        public string? gameUid { get; set; }
        public int? homeTeam { get; set; }
        public int? guestTeam { get; set; }
        public string? gameName { get; set; }
        public string? gameTime { get; set; }
    }

}
