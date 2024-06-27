using Microsoft.AspNetCore.Mvc;
using TeamLib.Models;
using TeamLib.Services;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class updateTeamController : ControllerBase
    {
        //private SqlSugarScope sqlORM;

        //public updateTeamController()
        //{
        //    ORACLEconn ORACLEConnectTry = new ORACLEconn();
        //    ORACLEConnectTry.getConn();
        //    sqlORM = ORACLEConnectTry.sqlORM;
        //}
        private readonly ITeamService _teamService;
        public updateTeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost]
        public async Task<List<TeamInGameTimeVal>> searchTeamInGameTime([FromBody] TeamInGameTimePara json)
        {
            return await _teamService.searchTeamInGameTime(json);
        }

        [HttpPost]
        public async Task<getGameByUidVal> getGameByUid([FromBody] getGameByUidPara json)
        {
            return await _teamService.getGameByUid(json);
        }

        [HttpPost]
        public async Task<List<getTeamMatchesByNameVal>> getTeamMatchesByName([FromBody] getTeamMatchesByNamePara json)
        {
            return await _teamService.getTeamMatchesByName(json);
        }

        [HttpPost]
        public async Task<List<TeamInGameTypeVal>> searchTeamInGameType([FromBody] TeamInGameTypePara json)
        {
            return await _teamService.searchTeamInGameType(json);
        }

        [HttpPost]
        public async Task<List<getTeamInfoByNameVal>> getTeamInfoByName([FromBody] getTeamInfoByNamePara json)
        {
            return await _teamService.getTeamInfoByName(json);
        }

        [HttpGet]
        public async Task<List<topScorerVal>> getTopScorers()
        {
           return await _teamService.getTopScorers();
        }

        [HttpPost]
        public async Task<List<topScorersInGameTypeVal>> topScorersInGameType([FromBody] topScorersInGameTypePara json)
        {
            return await _teamService.topScorersInGameType(json);
        }

        [HttpPost]
        public async Task<List<searchedTeamVal>> searchForTeam([FromBody] searchTeamOrPlayerPara json)
        {
            return await _teamService.searchForTeam(json);
        }
        [HttpPost]
        public async Task<List<searchedPlayerVal>> searchForPlayer([FromBody] searchTeamOrPlayerPara json)
        {
            return await _teamService.searchForPlayer(json);
        }

        [HttpPost]
        public async Task<getPlayerDetailVal> getPlayerDetail([FromBody] getPlayerDetailPara json)
        {
            return await _teamService.getPlayerDetail(json);
        }

        [HttpGet]
        public async Task<List<showRecentGamesVal>> showRecentGames()
        {
           return await _teamService.showRecentGames();
        }
    }
}


