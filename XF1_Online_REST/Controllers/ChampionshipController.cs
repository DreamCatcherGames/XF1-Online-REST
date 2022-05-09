using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XF1_Online_REST.LogicScript;
using XF1_Online_REST.LogicScripts;

namespace XF1_Online_REST.Controllers
{
    public class ChampionshipController : ApiController
    {
        ChampionshipLogic logic = new ChampionshipLogic();

        [Route("api/Championship")]
        public string Get()
        {
            return "Oui";
        }

        // GET: api/Championship/5
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Controller that allows to POST new championships
        /// </summary>
        /// <param name="champ"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/PostChampionship/{token}")]
        public HttpResponseMessage Post([FromBody]Championship champ,string token)
        {
            return logic.championshipCreationRequest(champ,token);
        }

        // PUT: api/Championship/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Championship/5
        public void Delete(int id)
        {
        }
    }
}
