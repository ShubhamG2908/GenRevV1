using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Genrev.Api.App
{
    public class GenrevApiController : ApiController
    {

        public HttpResponseMessage BadModelApiResult() {

            return Request.CreateResponse(
                HttpStatusCode.BadRequest, 
                new ErrorResult("400", "Body cannot be translated"));
        }

        public HttpResponseMessage InternalErrorApiResult(Exception e) {

            System.Diagnostics.Debug.WriteLine(e.ToString());

            return Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new ErrorResult("500", "Internal Server Error"));

        }

        public HttpResponseMessage NotAuthorizedApiResult() {
            
            return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new ErrorResult("401", "Unauthorized: invalid ApiKey or Password"));

        }

    }
}