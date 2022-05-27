using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XF1_Online_REST.LogicScript;

namespace XF1_Online_REST.Controllers
{
    public class RaceController : ApiController
    { 
        private RaceLogic logic = new RaceLogic();

        [Route("api/Race/getRacesChamp/{champId}/{token}/{salt}")]
        public HttpResponseMessage GetRacesByChamp(string champId,string token,string salt)
        {
            return logic.getRacesByChampionshipRequest(champId, token, salt);
        }

        [Route("api/Race/getRaceId/{champId}/{raceName}/{country}/{token}/{salt}")]
        public HttpResponseMessage GetRaceById(string champId,string raceName,string country,string token,string salt)
        {
            return logic.getRaceByIdRequest(champId,raceName,country,token,salt);
        }

       
        [Route("api/Race/addRace/{token}/{salt}")]
        public HttpResponseMessage Post([FromBody]Race race,string token,string salt)
        {
            return logic.raceCreationRequest(race, token,salt);
        }

        // PUT: api/Race/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Race/5
        public void Delete(int id)
        {
        }
    }
}
