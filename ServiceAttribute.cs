using System;

namespace Sublime.Services
{
	[AttributeUsage (AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ServiceAttribute : Attribute
	{
		#region Properties

		public string Name { get; private set; }

		#endregion

		#region Constructors

		public ServiceAttribute (string name)
		{
			this.Name = name;
		}

		#endregion
	}
}
