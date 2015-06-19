using System;

namespace Sublime.Services
{
	public class ServiceHostInfo
	{
		#region Properties

		public ServiceInfo ServiceInfo { get; set; }
		public string AssemblyFilename { get; set; }
		public string TypeName { get; set; }
		public string BaseDirectory { get; set; }
		public string BinDirectory { get; set; }

		#endregion
	}
}

