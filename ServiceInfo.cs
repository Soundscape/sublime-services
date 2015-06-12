using System;

namespace Sublime.Services
{
	public class ServiceInfo
	{
		#region Properties

		public string AssemblyPath { get; set; }

		public string BinDirectory { get; set; }

		public string Name { get; set; }

		public bool Running { get; set; }

		public string TypeName { get; set; }

		#endregion
	}
}
