using System;

namespace Sublime.Services
{
	public class ServiceInfo
	{
		#region Properties

		public string Key { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ServiceStartup Startup { get; set; }

		#endregion
	}
}

