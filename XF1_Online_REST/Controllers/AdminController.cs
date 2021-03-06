using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XF1_Online_REST;
using XF1_Online_REST.LogicScripts;

namespace XFIA_REST.Controllers
{
    public class AdminController : ApiController
    {
        AdminLogic logic=new AdminLogic();
        /// <summary>
        /// Endpoint for login requests from the administration login page
        /// </summary>
        /// <param name="admin"><see cref="Administrator"/> object that contains the credentials that need to be validated</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>

        [Route("api/Admin/loginRequest")]
        public HttpResponseMessage Post([FromBody] Administrator admin)
        {
            return logic.loginRequest(admin);
        }
    }
}
