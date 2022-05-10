using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace XF1_Online_REST.Controllers
{
    public class RaceController : ApiController
    {
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

        // POST: api/Race
        public void Post([FromBody]string value)
        {
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
