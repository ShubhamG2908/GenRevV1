using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Genrev.Api.App.Personnel
{
    [ApiAuthorization]
    public class PersonnelController : GenrevApiController
    {

        [HttpPost]
        public HttpResponseMessage Index(Models.PersonnelPostRequestModel model) {

            try {

                if (model == null) {
                    return BadModelApiResult();
                }

                var responseModel = _service.SubmitPersonnel(model);
                return Request.CreateResponse((HttpStatusCode)responseModel.Status.Code, responseModel);

            } catch (NotAuthorizedException) {
                return NotAuthorizedApiResult();
            } catch (Exception e) {
                return InternalErrorApiResult(e);
            }

        }







        private PersonnelService _service;

        public PersonnelController() {
            _service = new PersonnelService();
        }

    }
}