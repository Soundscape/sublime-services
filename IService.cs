using System;

namespace Sublime.Services
{
	public interface IService : IDisposable
	{
		#region Methods

		void Start (params object[] args);

		void Stop ();

		#endregion

		#region Events

		event EventHandler OnStart;

		event EventHandler OnStop;

		#endregion
	}
}
