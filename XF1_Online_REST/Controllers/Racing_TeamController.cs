using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XF1_Online_REST.LogicScripts;

namespace XF1_Online_REST.Controllers
{
    public class Racing_TeamController : ApiController
    {
        Racing_TeamLogic logic = new Racing_TeamLogic();

        [Route("api/RacingTeams/getAllRacingTeams/{token}/{salt}")]
        public HttpResponseMessage getAllRacingTeams(string token,string salt)
        {
            return logic.getAllTeams(token,salt);
        }

        [Route("api/RacingTeams/getPage/{token}/{salt}/{amount}/{page}")]
        public  HttpResponseMessage getPageRacingTeams(string token,string salt,int amount,int page)
        {
            return logic.getPage(token, salt, amount, page);
        }


        [Route("api/RacingTeams/addRacingTeam/{token}/{salt}")]
        public HttpResponseMessage PostRacingTeam([FromBody]Racing_Team team,string token,string salt)
        {
            return logic.addRacingTeam(token, salt,team);
        }

        [Route("api/RacingTeams/deleteRacingTeam/{token}/{salt}")]
        public HttpResponseMessage Delete([FromBody]Racing_Team team,string token,string salt)
        {
            return logic.deleteRacingTeam(token, salt, team);
        }
    }
}
