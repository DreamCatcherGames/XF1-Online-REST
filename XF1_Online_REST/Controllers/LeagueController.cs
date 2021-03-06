using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XF1_Online_REST.LogicScripts;

namespace XF1_Online_REST.Controllers
{
    public class LeagueController : ApiController
    {
        LeagueLogic logic = new LeagueLogic();

        [Route("api/League/getPage/{leagueId}/{token}/{salt}/{amount}/{page}")]
        public HttpResponseMessage getPage(string leagueId,string token,string salt,int amount,int page)
        {
            return logic.getPage(leagueId,token,salt,amount,page);
        }

        [Route("api/League/getPublicLeague/{token}/{salt}")]
        public HttpResponseMessage getPublicLeague(string token, string salt)
        {
            return logic.getPublicLeague(token,salt);
        }

        [Route("api/League/getPlayerPos/{leagueId}/{token}/{salt}")]
        public HttpResponseMessage getPlayerPos(string leagueId,string token, string salt)
        {
            return logic.getLeaguePos(leagueId,token,salt);
        }
        
        [Route("api/League/getLeagueById/{token}/{salt}")]
        public HttpResponseMessage getLeagueById([FromBody]League league,string token, string salt)
        {
            return logic.getLeagueById(league,token, salt);
        }


        [Route("api/League/createPrivateLeague/{token}/{salt}")]
        public HttpResponseMessage createPrivateLeague([FromBody]League league,string token,string salt)
        {
            return logic.createPrivateLeague(league,token,salt);
        }

        [Route("api/League/getPrivateLeagues/{token}/{salt}")]
        public HttpResponseMessage getPrivateLeagues(string token, string salt)
        {
            return logic.getPrivateLeagues(token, salt);
        }

        [Route("api/League/joinLeague/{token}/{salt}")]
        public HttpResponseMessage joinLeague([FromBody] League league, string token, string salt)
        {
            return logic.joinLeagueRequest(league, token, salt);
        }

        [Route("api/League/aproveJoin/{cond}/{token}/{salt}")]
        public HttpResponseMessage aproveJoin([FromBody] Notification notification, string token, string salt,Boolean cond)
        {
            return logic.approveJoin(notification, token, salt,cond);
        }
    }
}
