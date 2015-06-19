using System;

namespace Sublime.Services
{
	public static class ServiceHostInfoExtensions
	{
		#region Methods

		public static IService Marshal (this ServiceHostInfo info, AppDomain domain) {
			return (IService)domain.CreateInstanceFromAndUnwrap (info.AssemblyFilename, info.TypeName);
		}

		public static IService Marshal (this ServiceHostInfo info, out AppDomain domain) {
			domain = CreateDomain (info);
			return Marshal (info, domain);
		}

		public static AppDomain CreateDomain(this ServiceHostInfo info) {
			var setup = new AppDomainSetup {
				ApplicationBase = info.BaseDirectory,
				PrivateBinPath = info.BinDirectory,
				DisallowApplicationBaseProbing = false,
				DisallowBindingRedirects = false,
			};

			return AppDomain.CreateDomain (info.TypeName, null, setup);
		}

		#endregion
	}
}

