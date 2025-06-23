using Genrev.Web.App.Data;
using System.Web.Mvc;

namespace Genrev.Web.App.Internal
{
	public class InternalController : Dymeng.Web.Mvc.ControllerBase
	{
		private DataService _service;
		public InternalController()
		{
			_service = new DataService();
		}		
		public RedirectResult UpdateViewContext(int contextID, string returnUrl)
		{
			//Update default userId
			int personnelId = AppService.Current.Person.PersonID;
			int result = _service.UpdateDefaultUser(personnelId, contextID);
			AppService.Current.ViewContext.PersonID = contextID;

			return new RedirectResult(returnUrl, false);
		}

	}
}