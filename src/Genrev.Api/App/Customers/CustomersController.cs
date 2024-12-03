using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Genrev.Api.App.Customers
{
    [ApiAuthorization]
    public class CustomersController : GenrevApiController
    {

        [HttpPost]
        public HttpResponseMessage Index(Models.CustomerPostRequestModel model) {

            try {

                if (model == null) {
                    return BadModelApiResult();
                }

                var responseModel = _service.SubmitCustomers(model);
                return Request.CreateResponse((HttpStatusCode)responseModel.Status.Code, responseModel);

            } catch (NotAuthorizedException) {
                return NotAuthorizedApiResult();
            } catch (Exception e) {
                return InternalErrorApiResult(e);
            }

        }

        private CustomersService _service;

        public CustomersController() {
            _service = new CustomersService();
        }

    }
}