using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XFIA_REST.Models;

namespace XFIA_REST.Controllers
{
    public class ValuesController : ApiController
    {
        MiscelaneousFunctions tools = new MiscelaneousFunctions();

        [Route("api/Test/encryptPassword/{password}")]
        public List<string> Get(string password)
        {
            return tools.passwordEncryptor(password);
        }

        [Route("api/Test/getToken")]
        public string Get()
        {
            return tools.getToken();
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
