using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Genrev.Api
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiAuthorizationAttribute : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext) {

            try {

                var authToken = actionContext?.Request?.Headers?.Authorization?.Parameter;

                int accountID;
                string tokenValue;

                parseAuth(authToken, out accountID, out tokenValue);
                
                var _context = AppService.Current.DataContext;

                var token = _context.ApiTokens.Where(x => x.AccountID == accountID && x.Value == tokenValue).FirstOrDefault();

                if (token == null) {
                    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    return;
                }

                if (token.ExpirationDate <= DateTime.Now) {
                    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    return;
                }

                return;

            } catch {

                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                return;
            }
            

        }

        private void parseAuth(string valueIn, out int accountID, out string authToken) {

            accountID = 0;
            authToken = null;

            accountID = int.Parse(valueIn.Split('.')[0]);
            authToken = valueIn.Split('.')[1];

        }
        
    }
}