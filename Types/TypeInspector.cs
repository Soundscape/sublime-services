using System;
using Mono.Cecil;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Sublime.Services
{
	public class TypeInspector
	{
		#region Members

		ModuleDefinition module;
		string filename;

		#endregion

		#region Constructors

		public TypeInspector (string filename)
		{
			this.filename = filename;
			this.module = ModuleDefinition.ReadModule (filename);
		}

		#endregion

		#region Methods

		public IEnumerable<ServiceHostInfo> GetServices() {
			return this.GetTypes ().Where (TypeIsService).Select(GetServiceHostInfo);
		}

		IEnumerable<TypeDefinition> GetTypes() {
			return module.Types;
		}

		bool TypeInHerits<TBase> (TypeDefinition type) {
			return null != type.BaseType && type.BaseType.FullName.Equals (typeof(TBase).FullName);
		}

		bool TypeIsService (TypeDefinition type) {
			return this.TypeInHerits<ServiceBase> (type);
		}

		ServiceHostInfo GetServiceHostInfo (TypeDefinition type) {
			var baseDirectory = Path.GetDirectoryName (this.filename);
			var configFile = Path.Combine (baseDirectory, "config.json");
			var config = new ConfigFile<ServiceInfo> (configFile);

			return new ServiceHostInfo {
				ServiceInfo = config.Get(),
				AssemblyFilename = this.filename,
				TypeName = type.FullName,
				BaseDirectory = baseDirectory,
				BinDirectory = Path.Combine (baseDirectory, "bin")
			};
		}

		#endregion
	}
}

