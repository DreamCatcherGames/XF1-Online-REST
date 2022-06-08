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

        [Route("api/Player/hasNotifications/{token}/{salt}")]
        public HttpResponseMessage hasNotifications(string token, string salt)
        {
            return logic.hasNotifications(token, salt);
        }

        [Route("api/Player/getNotifications/{token}/{salt}")]
        public HttpResponseMessage getNotifications(string token, string salt)
        {
            return logic.getNotifications(token, salt);
        }

        [Route("api/Player/updateProfile/{token}/{salt}")]
        public HttpResponseMessage updateProfile([FromBody] Player player,string token,string salt)
        {
            return logic.updateProfile(token, salt, player);
        }

        [Route("api/Player/deleteNotification/{token}/{salt}")]
        public HttpResponseMessage deleteNotification([FromBody]Notification notification, string token, string salt)
        {
            return logic.deleteNotification(notification, token, salt);
        }

        [Route("api/Player/getPrivateLeague/{token}/{salt}")]
        public HttpResponseMessage getPrivateLeague(string token, string salt)
        {
            return logic.getPrivateLeague(token, salt);
        }
    }
}
