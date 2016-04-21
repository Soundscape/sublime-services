using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sublime.Services
{
    public class ServiceManager : MarshalByRefObject, IDisposable
    {
        #region Events

        public event EventHandler<ServiceEventArgs> OnStart
        {
            add { onStart += value; }
            remove { onStart -= value; }
        }

        public event EventHandler<ServiceEventArgs> OnStop
        {
            add { onStop += value; }
            remove { onStop -= value; }
        }

        private EventHandler<ServiceEventArgs> onStart;
        private EventHandler<ServiceEventArgs> onStop;

        #endregion

        #region Properties

        public IDictionary<string, ServiceState> Services { get; private set; }

        #endregion

        #region Methods

        public void LoadDirectory(string directory)
        {
            this.Services = Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories)
                .Select(path => new TypeInspector(path))
                .SelectMany(inspector => inspector.GetServices())
                .ToDictionary(info => info.ServiceInfo.Key, info => new ServiceState { Info = info });
        }

        public void Start(string key)
        {
            var state = this.Services.ContainsKey(key)
                ? this.Services[key]
                : null;

            if (null == state)
                return;

            if (null == state.Service && null == state.Domain)
            {
                AppDomain domain;
                state.Service = state.Info.Marshal(out domain);
                state.Domain = domain;
            }
            else if (null == state.Service)
                state.Service = state.Info.Marshal(state.Domain);

            if (!state.Active)
            {
                state.Service.OnStart += Service_OnStart;
                state.Service.OnStop += Service_OnStop;

                state.Active = true;
                state.Service.Start();
            }
        }

        public void Stop(string key)
        {
            var state = this.Services.ContainsKey(key)
                ? this.Services[key]
                : null;

            if (null == state)
                return;

            if (state.Active)
            {
                state.Active = false;
                state.Service.Stop();

                state.Service.OnStart -= Service_OnStart;
                state.Service.OnStop -= Service_OnStop;

                state.Service = null;
            }
        }

        public void Destroy(string key)
        {
            this.Stop(key);

            var state = this.Services.ContainsKey(key)
                ? this.Services[key]
                : null;

            if (null == state)
                return;

            if (null != state.Domain)
            {
                AppDomain.Unload(state.Domain);
                state.Domain = null;
            }
        }

        public void Dispose()
        {
            if (null != this.Services && 0 < this.Services.Count)
                this.Services.Keys.ToList().ForEach(this.Destroy);

            this.Services = null;
        }

        void Service_OnStop(object sender, ServiceEventArgs e)
        {
            if (null != this.onStop)
                this.onStop(this, e);
        }

        void Service_OnStart(object sender, ServiceEventArgs e)
        {
            if (null != this.onStart)
                this.onStart(this, e);
        }

        #endregion
    }
}

