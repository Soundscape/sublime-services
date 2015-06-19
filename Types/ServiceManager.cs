using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sublime.Services
{
	public class ServiceManager : IDisposable
	{
		#region Properties

		public IDictionary<string, ServiceState> Services { get; private set; }

		#endregion

		#region Methods

		public void LoadDirectory(string directory) {
			this.Services = Directory.GetFiles (directory, "*.dll", SearchOption.AllDirectories)
				.Select (path => new TypeInspector (path))
				.SelectMany (inspector => inspector.GetServices ())
				.ToDictionary(info => info.TypeName, info => new ServiceState { Info = info });
		}

		public void Start(string key) {
			var state = this.Services.ContainsKey (key)
				? this.Services [key]
				: null;

			if (null == state)
				return;

			if (null == state.Service && null == state.Domain) {
				AppDomain domain;
				state.Service = state.Info.Marshal(out domain);
				state.Domain = domain;
			}
			else if (null == state.Service)
				state.Service = state.Info.Marshal(state.Domain);

			if (!state.Active) {
				state.Service.Start ();
				state.Active = true;
			}
		}

		public void Stop(string key) {
			var state = this.Services.ContainsKey (key)
				? this.Services [key]
				: null;

			if (null == state)
				return;

			if (state.Active) {
				state.Service.Stop ();
				state.Service = null;
				state.Active = false;
			}
		}

		public void Destroy(string key) {
			this.Stop (key);

			var state = this.Services.ContainsKey (key)
				? this.Services [key]
				: null;

			if (null == state)
				return;

			if (null != state.Domain) {
				AppDomain.Unload (state.Domain);
				state.Domain = null;
			}
		}

		public void Dispose() {
			if (null != this.Services && 0 < this.Services.Count)
				this.Services.Keys.ToList ().ForEach (this.Destroy);

			this.Services = null;
		}

		#endregion
	}
}

