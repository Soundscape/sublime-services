using System;

namespace Sublime.Services
{
	public interface IService : IDisposable
	{
		#region Properties

		ServiceInfo Info { get; }

		#endregion

		#region Methods

		void Start ();

		void Stop ();

		#endregion

		#region Events

		event EventHandler OnStart;

		event EventHandler OnStop;

		#endregion
	}
}

