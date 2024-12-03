using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Genrev.Api.App.Classifications
{
    [ApiAuthorization]
    public class ClassificationsController : GenrevApiController
    {


        [HttpPost]
        public HttpResponseMessage IndustryTypes(Models.IndustryTypePostRequestModel model) {

            try {

                if (model == null) {
                    return BadModelApiResult();
                }

                var responseModel = _service.SubmitIndustryTypes(model);
                return Request.CreateResponse((HttpStatusCode)responseModel.Status.Code, responseModel);
                
            } catch (NotAuthorizedException) {
                return NotAuthorizedApiResult();
            } catch (Exception e) {
                return InternalErrorApiResult(e);
            }

        }



        [HttpPost]
        public HttpResponseMessage CustomerTypes(Models.CustomerTypePostRequestModel model) {

            try {

                if (model == null) {
                    return BadModelApiResult();
                }

                var responseModel = _service.SubmitCustomerTypes(model);
                return Request.CreateResponse((HttpStatusCode)responseModel.Status.Code, responseModel);

            } catch (NotAuthorizedException) {
                return NotAuthorizedApiResult();
            } catch (Exception e) {
                return InternalErrorApiResult(e);
            }

        }


        [HttpPost]
        public HttpResponseMessage AccountTypes(Models.AccountTypesPostRequestModel model) {

            try {

                if (model == null) {
                    return BadModelApiResult();
                }

                var responseModel = _service.SubmitAccountTypes(model);
                return Request.CreateResponse((HttpStatusCode)responseModel.Status.Code, responseModel);

            }catch (NotAuthorizedException) {
                return NotAuthorizedApiResult();
            } catch (Exception e) {
                return InternalErrorApiResult(e);
            }

        }



        private ClassificationsService _service;

        public ClassificationsController() {
            _service = new ClassificationsService();
        }
        
    }
}