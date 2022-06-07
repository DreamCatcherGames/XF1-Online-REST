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
        // POST: api/League
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/League/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/League/5
        public void Delete(int id)
        {
        }
    }
}
