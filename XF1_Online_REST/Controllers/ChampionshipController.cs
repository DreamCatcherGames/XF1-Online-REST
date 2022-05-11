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
        /// <param name="champ"><see cref="Championship"/> object that contains the information of the championship to be added</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        [Route("api/Championship/addChampionship/{token}/{salt}")]
        public HttpResponseMessage Post([FromBody]Championship champ,string token,string salt)
        {
            return logic.championshipCreationRequest(champ,token,salt);
        }

        // PUT: api/Championship/5
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>
        /// Controller that allows to DELETE a championship
        /// </summary>
        /// <param name="champId"><see cref="Championship"/> object that contains the unique key of the championship to delete</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        [Route("api/Championship/deleteChampionship/{champId}/{token}/{salt}")]
        public HttpResponseMessage Delete(string champId,string token,string salt)
        {
            return logic.championshipDeletionRequest(champId, token, salt);
        }
    }
}
