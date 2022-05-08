using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XFIA_REST.Models;

namespace XFIA_REST.Controllers
{
    public class AdminController : ApiController
    {
        MiscelaneousFunctions tools = new MiscelaneousFunctions();
        /// <summary>
        /// Endpoint for login requests from the administration login page
        /// </summary>
        /// <param name="admin"><see cref="Administrator"/> object that contains the credentials that need to be validated</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>

        [Route("api/Admin/loginRequest")]
        public HttpResponseMessage Get([FromBody]Administrator admin)
        {
            if(tools.passwordVerifier(admin))
            {
                string token = tools.getToken();
                tools.assignToken(admin, token);
                return Request.CreateResponse(HttpStatusCode.OK,token);
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, "Usuario o Contraseña incorrectos");
        }

        [Route("api/Admin/test")]
        public string Get(int id)
        {
            return "Hola Zeledon";
        }

        // POST: api/Admin
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Admin/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Admin/5
        public void Delete(int id)
        {
        }
    }
}
