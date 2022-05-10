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
        // GET: api/Race
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Race/5
        public string Get(int id)
        {
            return "value";
        }

       
        [Route("api/Race/addRace/{token}")]
        public HttpResponseMessage Post([FromBody]Race race,string token)
        {
            return logic.raceCreationRequest(race, token);
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
