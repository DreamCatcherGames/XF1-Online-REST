using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XF1_Online_REST.LogicScripts;

namespace XF1_Online_REST.Controllers
{
    public class PilotController : ApiController
    {
        // GET: api/Pilot
        PilotLogic logic = new PilotLogic();

        [Route("api/Pilots/getAllPilots/{token}/{salt}")]
        public HttpResponseMessage getAllPilots(string token, string salt)
        {
            return logic.getAllPilots(token, salt);
        }

        [Route("api/Pilots/getPage/{token}/{salt}/{amount}/{page}")]
        public HttpResponseMessage getPagePilot(string token, string salt, int amount, int page)
        {
            return logic.getPage(token, salt, amount, page);
        }

        [Route("api/Pilots/searchPilot/{token}/{salt}/{amount}/{page}")]
        public HttpResponseMessage searchPilot([FromBody] Pilot pilot,string token, string salt, int amount, int page)
        {
            return logic.searchPilots(token,salt,amount,page,pilot.Name,pilot.Racing_Team);
        }

        [Route("api/Pilots/addPilot/{token}/{salt}")]
        public HttpResponseMessage PostPilot([FromBody] Pilot pilot, string token, string salt)
        {
            return logic.addPilot(token, salt, pilot);
        }

        [Route("api/Pilots/deletePilot/{token}/{salt}")]
        public HttpResponseMessage DeletePilot([FromBody] Pilot pilot, string token, string salt)
        {
            return logic.deletePilot(token, salt,pilot);
        }
    }
}
