using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamLib.Models;

namespace TeamLib.Services
{
    public interface ITeamService
    {
        public Task<List<TeamInGameTimeVal>> searchTeamInGameTime(TeamInGameTimePara json);
        public Task<getGameByUidVal> getGameByUid(getGameByUidPara json);
        public Task<List<getTeamMatchesByNameVal>> getTeamMatchesByName(getTeamMatchesByNamePara json);
        public Task<List<TeamInGameTypeVal>> searchTeamInGameType(TeamInGameTypePara json);
        public Task<List<getTeamInfoByNameVal>> getTeamInfoByName(getTeamInfoByNamePara json);
        public Task<List<topScorerVal>> getTopScorers();
        public Task<List<topScorersInGameTypeVal>> topScorersInGameType(topScorersInGameTypePara json);
        public Task<List<searchedTeamVal>> searchForTeam(searchTeamOrPlayerPara json);
        public Task<List<searchedPlayerVal>> searchForPlayer(searchTeamOrPlayerPara json);
        public Task<getPlayerDetailVal> getPlayerDetail(getPlayerDetailPara json);
        public Task<List<showRecentGamesVal>> showRecentGames();
    }
}
