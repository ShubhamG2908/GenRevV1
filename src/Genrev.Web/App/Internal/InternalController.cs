using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc;

namespace Genrev.Web.App.Internal
{
    public class InternalController : Dymeng.Web.Mvc.ControllerBase
    {

        /// <summary>
        /// Update the "View As" context and return the specified url
        /// </summary>
        public RedirectResult UpdateViewContext(int contextID, string returnUrl) {

            AppService.Current.ViewContext.PersonID = contextID;
            return new RedirectResult(returnUrl, false);
        }

    }
}