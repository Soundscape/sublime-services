using System;
using System.IO;
using Newtonsoft.Json;

namespace Sublime.Services
{
	public class ConfigFile<T>
		where T : class
	{
		#region Constructors

		public ConfigFile (string filename)
		{
			this.Filename = filename;
		}

		#endregion

		#region Properties

		public string Filename { get; private set; }

		#endregion

		#region Methods

		public T Get() {
			return JsonConvert.DeserializeObject<T> (File.ReadAllText (this.Filename));
		}

		public void Set(T data) {
			File.WriteAllText (this.Filename, JsonConvert.SerializeObject (data));
		}

		#endregion
	}
}

