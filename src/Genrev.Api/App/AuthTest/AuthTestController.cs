using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Net;



namespace Genrev.Api.App.AuthTest
{


    public class AuthTestController : ApiController {


        [ApiAuthorization]
        [HttpPost]
        public HttpResponseMessage TestAuthorizationRequired() {

            return Request.CreateResponse(HttpStatusCode.OK);

        }

        
        [HttpPost]
        public HttpResponseMessage TestNonAuthMethod() {

            return Request.CreateResponse(HttpStatusCode.Accepted);

        }

        [Authorize]
        [HttpPost]
        public HttpResponseMessage TestDefaultAuthMethod() {

            return Request.CreateResponse(HttpStatusCode.BadGateway);

        }

    }
}