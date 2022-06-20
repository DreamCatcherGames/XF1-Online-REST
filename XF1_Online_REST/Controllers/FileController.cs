using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using XF1_Online_REST.LogicScripts;

namespace XF1_Online_REST.Controllers
{
    public class FileController : ApiController
    {
        FileLogic logic = new FileLogic();

        [Route("api/File/uploadFile/{updatePoints}/{updatePrice}/{token}/{salt}")]
        public HttpResponseMessage Post(bool updatePoints,bool updatePrice,string token,string salt)
        {
            HttpFileCollection files= HttpContext.Current.Request.Files;
            return logic.uploadAndParseFile(files,updatePoints, updatePrice, token, salt);
        }

    }
}
