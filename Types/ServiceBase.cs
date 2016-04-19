using System;
using System.Threading.Tasks;
using System.Threading;

namespace Sublime.Services
{
    public abstract class ServiceBase : MarshalByRefObject, IService
    {
        #region Events

        public event EventHandler<ServiceEventArgs> OnStart;
        public event EventHandler<ServiceEventArgs> OnStop;

        #endregion

        #region Properties

        public ServiceInfo Info { get; protected set; }

        #endregion

        #region Members

        protected CancellationTokenSource cancellationSource;
        protected CancellationToken cancellationToken;
        protected Task executionTask;

        #endregion

        #region Methods

        protected abstract void ExecutionTask();

        public virtual void Start()
        {
            this.cancellationSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationSource.Token;

            if (null != this.OnStart)
                this.OnStart(null, new ServiceEventArgs { ServiceInfo = Info });

            this.executionTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    this.cancellationToken.ThrowIfCancellationRequested();
                    this.ExecutionTask();
                }
            }, this.cancellationToken);
        }

        public virtual void Stop()
        {
            this.Dispose();

            if (null != this.OnStop)
                this.OnStop(null, new ServiceEventArgs { ServiceInfo = Info });
        }

        public virtual void Dispose()
        {
            if (null != this.executionTask)
            {
                if (!this.executionTask.IsCanceled)
                {
                    try
                    {
                        this.cancellationSource.Cancel();
                        this.executionTask.Wait();
                    }
                    catch (AggregateException)
                    {
                    }
                    finally
                    {
                        this.executionTask.Dispose();
                    }
                }

                this.executionTask = null;
                this.cancellationSource.Dispose();
                this.cancellationSource = null;
                this.cancellationToken = CancellationToken.None;
            }
        }

        #endregion
    }
}

