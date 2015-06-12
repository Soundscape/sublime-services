using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sublime.Services
{
	public class ServiceFactory
	{
		#region Members

		private readonly IDictionary<ServiceInfo, Tuple<AppDomain, IService>> services;

		private IEnumerable<ServiceInfo> availableServices;

		#endregion

		#region Indexers

		public Tuple<AppDomain, IService> this [string name] {
			get {
				var serviceInfo = this.services.Keys.SingleOrDefault (x => x.Name == name);
				if (null == serviceInfo) return null;

				return this.services [serviceInfo];
			}
			private set {
				var key = this.availableServices.SingleOrDefault (x => x.Name == name);
				if (!this.services.ContainsKey (key)) this.services.Add (key, value);
			}
		}

		#endregion

		#region Constructors

		public ServiceFactory ()
		{
			this.services = new Dictionary<ServiceInfo, Tuple<AppDomain, IService>> ();
		}

		#endregion

		#region Methods

		private AppDomain CreateDomain (ServiceInfo info)
		{
			var setup = new AppDomainSetup {
				ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
				PrivateBinPath = info.BinDirectory,
				DisallowApplicationBaseProbing = false,
				DisallowBindingRedirects = false
			};

			return AppDomain.CreateDomain (info.Name, null, setup);
		}

		private IService CreateService (ServiceInfo info)
		{
			var domain = this.CreateDomain (info);
			var service = (IService)domain.CreateInstanceFromAndUnwrap (info.AssemblyPath, info.TypeName);
			this [info.Name] = Tuple.Create<AppDomain, IService> (domain, service);

			return service;
		}

		private IEnumerable<ServiceInfo> FindServicesInAssembly (string path)
		{
			return ModuleDefinition.ReadModule (path).Types
				.Where (type =>
					type.IsPublic &&
					null != type.Interfaces.SingleOrDefault (x => x.FullName == typeof(IService).FullName) &&
					null != type.CustomAttributes.SingleOrDefault (x => x.AttributeType.FullName == typeof(ServiceAttribute).FullName))
				.Select (type => {
					var attr = type.CustomAttributes.Single (x => x.AttributeType.FullName == typeof(ServiceAttribute).FullName);
					var name = attr.ConstructorArguments [0].Value.ToString ();
					return new ServiceInfo {
						AssemblyPath = path,
						Name = name,
						TypeName = type.FullName,
						BinDirectory = Path.GetDirectoryName (path),
						Running = null != this.services.Keys.SingleOrDefault (x => x.Name == name)
					};
				});
		}

		public IEnumerable<ServiceInfo> LoadFromDirectory (string path)
		{
			if (!Directory.Exists (path)) return null;

			this.availableServices = Directory
				.GetFiles (path, "*.dll")
				.SelectMany (this.FindServicesInAssembly);
			
			return this.availableServices;
		}

		public IService StartService (string name, params object[] args)
		{
			var info = this.availableServices.SingleOrDefault (x => x.Name == name);
			var service = this.CreateService (info);

			service.Start (args);
			info.Running = true;

			return service;
		}

		public void StopService (string name)
		{
			var tuple = this [name];
			if (tuple == null) return;

			tuple.Item2.Stop ();
			AppDomain.Unload (tuple.Item1);
			var info = this.services.Keys.SingleOrDefault (x => x.Name == name);
			this.services.Remove (info);
			info = this.availableServices.SingleOrDefault (x => x.Name == name);
			info.Running = false;
		}

		#endregion
	}
}
