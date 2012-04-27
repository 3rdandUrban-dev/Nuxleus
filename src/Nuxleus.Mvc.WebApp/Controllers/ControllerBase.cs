using ServiceStack.Mvc;
using ServiceStack.Mvc.MiniProfiler;
using Nuxleus.Mvc.WebApp.Models;

namespace Nuxleus.Mvc.WebApp.Controllers
{
	[ProfilingActionFilter]
	public class ControllerBase : ServiceStackController<CustomUserSession>
	{
		 
	}
}