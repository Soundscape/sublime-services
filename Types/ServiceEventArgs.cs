using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sublime.Services
{
    [Serializable]
    public class ServiceEventArgs : EventArgs
    {
        public ServiceInfo ServiceInfo { get; set; }
    }
}
