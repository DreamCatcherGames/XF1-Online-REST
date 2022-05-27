using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XF1_Online_REST.LogicScripts;

namespace XF1_Online_REST.Controllers
{
    public class PlayerController : ApiController
    {
        PlayerLogic logic = new PlayerLogic();

        /// <summary>
        /// Endpoint for login requests from the player login page
        /// </summary>
        /// <param name="player"><see cref="Player"/> object that contains the credentials that need to be validated</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>

        [Route("api/Player/loginRequest")]
        public HttpResponseMessage loginRequest([FromBody] Player player)
        {
            return logic.loginRequest(player);
        }

        /// <summary>
        /// Endpoint for registration requests from the player page
        /// </summary>
        /// <param name="player"><see cref="Player"/> object that contains the player's info</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>

        [Route("api/Player/registerRequest")]
        public HttpResponseMessage registerRequest([FromBody] Player player)
        {
            return logic.registerRequest(player);
        }


        [Route("api/Player/verificationRequest")]
        public HttpResponseMessage verificationRequest([FromBody] Verification_Request request)
        {
            return logic.verificationRequest(request);
        }

        [Route("api/Player/getProfile/{token}/{salt}")]
        public HttpResponseMessage getProfile(string token,string salt)
        {
            return logic.getProfile(token,salt);
        }
        [Route("api/Player/updateProfile/{token}/{salt}")]
        public HttpResponseMessage Put(string token,string salt, [FromBody]Player player)
        {
            return logic.updateProfile(token, salt, player);
        }
        // DELETE: api/Player/5
        public void Delete(int id)
        {
        }
    }
}
