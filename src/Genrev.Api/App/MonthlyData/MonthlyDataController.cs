using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Genrev.Api.App.MonthlyData
{
    [ApiAuthorization]
    public class MonthlyDataController: GenrevApiController
    {


        [HttpPost]
        public HttpResponseMessage Index(Models.MonthlyDataPostRequestModel model) {

            try {

                if (model == null) {
                    return BadModelApiResult();
                }

                var responseModel = _service.SubmitMonthlyData(model);
                return Request.CreateResponse((HttpStatusCode)responseModel.Status.Code, responseModel);

            } catch (NotAuthorizedException) {
                return NotAuthorizedApiResult();
            } catch (Exception e) {
                return InternalErrorApiResult(e);
            }

        }




        private MonthlyDataService _service;

        public MonthlyDataController() {
            _service = new MonthlyDataService();
        }

    }
}