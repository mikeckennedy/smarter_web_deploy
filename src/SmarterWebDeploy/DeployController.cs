using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SmarterWebDeploy
{
    public class DeployController : Controller
    {
		public ActionResult Complete(string id)
		{
			try
			{
				DeployManager.Init();

				Response.BufferOutput = false;
				DeployManager.Deploy(id, Response.OutputStream);

				return Content("Completed successfully.");
			}
			catch (Exception x)
			{
				return Content("<br />Completed with errors:\r\n<br />\r\n<br />" + x.ToString().Replace("\n", "\n<br />"));
			}
		}
    }
}
