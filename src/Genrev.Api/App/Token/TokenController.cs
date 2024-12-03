using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Genrev.Api.App.Token.Models;

namespace Genrev.Api.App.Token
{
    public class TokenController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage Index(TokenRequestModel model) {
            try {

                TokenResponseModel responseModel = _service.GetToken(model);

                return Request.CreateResponse(HttpStatusCode.Created, responseModel);

            } catch (NotAuthorizedException) {
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new ErrorResult("401", "Unauthorized: invalid ApiKey or Password"));
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    new ErrorResult("500", "Internal Server Error"));
            }
            
        }




        private TokenService _service;

        public TokenController() {
            _service = new TokenService();
        }

    }
}