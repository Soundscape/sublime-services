using System;

namespace Sublime.Services
{
	public class ServiceState
	{
		#region Properties

		public ServiceHostInfo Info { get; set; }
		public AppDomain Domain { get; set; }
		public IService Service { get; set; }
		public bool Active { get; set; }

		#endregion
	}
}

