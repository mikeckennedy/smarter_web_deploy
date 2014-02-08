using System.Web.Mvc;

namespace DestinationWebsiteForTesting.Controllers
{
	public class OldController : Controller
	{
		public ActionResult Index()
		{
			return Content("Old version");
		} 

	}
}
