using AdmxPolicyManager.Interop;
using AdmxPolicyManager.Models.Policies;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace AdmxPolicyManager
{
    /// <summary>
    /// Represents a context for executing group policy actions.
    /// </summary>
    public sealed class GroupPolicyContext : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupPolicyContext"/> class with the specified apartment state.
        /// </summary>
        /// <param name="apartmentState">The apartment state for the thread.</param>
        public GroupPolicyContext(ApartmentState apartmentState = ApartmentState.STA)
        {
            _disposed = false;
            _waitNextRun = new AutoResetEvent(false);
            _waitResultArrive = new AutoResetEvent(false);
            _apartmentState = apartmentState;

            _thread = new Thread(new ParameterizedThreadStart(ThreadProcedure));
            _thread.SetApartmentState(_apartmentState);
            _thread.Start(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GroupPolicyContext"/> class.
        /// </summary>
        ~GroupPolicyContext()
        {
            Dispose(false);
        }

        private bool _disposed;
        private readonly AutoResetEvent _waitNextRun;
        private readonly AutoResetEvent _waitResultArrive;
        private readonly ApartmentState _apartmentState;
        private readonly Thread _thread;
        private ExecutionContext _execContext;

        /// <summary>
        /// Releases all resources used by the <see cref="GroupPolicyContext"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try { _waitNextRun.Dispose(); }
                    catch { }

                    try { _waitResultArrive.Dispose(); }
                    catch { }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Executes the specified action in the group policy context.
        /// </summary>
        /// <param name="callback">The action to execute.</param>
        /// <param name="arguments">The arguments to pass to the action.</param>
        /// <param name="targetSection">The target policy section.</param>
        /// <exception cref="InvalidOperationException">Thrown when a previous task was not done.</exception>
        /// <exception cref="GroupPolicyManagementException">Thrown when the thread does not run correctly.</exception>
        public void Execute(Action callback, object[] arguments = default, PolicySection? targetSection = default)
        {
            if (_execContext != null && !_execContext._completed)
                throw new InvalidOperationException("Previous task was not done.");

            _execContext = new ExecutionContext(callback, arguments, targetSection.HasValue, targetSection ?? default);
            _waitNextRun.Set();
            _waitResultArrive.WaitOne();

            if (!_execContext._completed)
                throw new GroupPolicyManagementException("Thread does not run correctly.");

            if (_execContext._thrownException != null)
                _execContext._thrownException.Throw();
        }

        /// <summary>
        /// Executes the specified function in the group policy context and returns the result.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="callback">The function to execute.</param>
        /// <param name="arguments">The arguments to pass to the function.</param>
        /// <param name="targetSection">The target policy section.</param>
        /// <returns>The result of the function.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a previous task was not done.</exception>
        /// <exception cref="GroupPolicyManagementException">Thrown when the thread does not run correctly.</exception>
        public T Execute<T>(Func<T> callback, object[] arguments = default, PolicySection? targetSection = default)
        {
            if (_execContext != null && !_execContext._completed)
                throw new InvalidOperationException("Previous task was not done.");

            _execContext = new ExecutionContext(callback, arguments, targetSection.HasValue, targetSection ?? default);
            _waitNextRun.Set();
            _waitResultArrive.WaitOne();

            if (!_execContext._completed)
                throw new GroupPolicyManagementException("Thread does not run correctly.");

            if (_execContext._thrownException != null)
                _execContext._thrownException.Throw();

            return (T)_execContext._callbackResult;
        }

        /// <summary>
        /// Executes the specified asynchronous action in the group policy context.
        /// </summary>
        /// <param name="callback">The asynchronous action to execute.</param>
        /// <param name="arguments">The arguments to pass to the action.</param>
        /// <param name="targetSection">The target policy section.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a previous task was not done.</exception>
        /// <exception cref="GroupPolicyManagementException">Thrown when the thread does not run correctly.</exception>
        public Task ExecuteAsync(Func<Task> callback, object[] arguments = default, PolicySection? targetSection = default, CancellationToken cancellationToken = default)
        {
            if (_execContext != null && !_execContext._completed)
                throw new InvalidOperationException("Previous task was not done.");

            var tcs = new TaskCompletionSource<object>();
            _execContext = new ExecutionContext(new Func<Task>(async () =>
            {
                using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await callback();
                        tcs.TrySetResult(null);
                    }
                }
            }), arguments, targetSection.HasValue, targetSection ?? default, cancellationToken);
            _waitNextRun.Set();
            _waitResultArrive.WaitOne();

            if (!_execContext._completed && !cancellationToken.IsCancellationRequested)
                throw new GroupPolicyManagementException("Thread does not run correctly.");

            if (_execContext._thrownException != null && !cancellationToken.IsCancellationRequested)
                _execContext._thrownException.Throw();

            return tcs.Task;
        }

        /// <summary>
        /// Executes the specified asynchronous function in the group policy context and returns the result.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="callback">The asynchronous function to execute.</param>
        /// <param name="arguments">The arguments to pass to the function.</param>
        /// <param name="targetSection">The target policy section.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation and containing the result of the function.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a previous task was not done.</exception>
        /// <exception cref="GroupPolicyManagementException">Thrown when the thread does not run correctly.</exception>
        public Task<T> ExecuteAsync<T>(Func<Task<T>> callback, object[] arguments = default, PolicySection? targetSection = default, CancellationToken cancellationToken = default)
        {
            if (_execContext != null && !_execContext._completed)
                throw new InvalidOperationException("Previous task was not done.");

            var tcs = new TaskCompletionSource<T>();
            _execContext = new ExecutionContext(new Func<Task>(async () =>
            {
                using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        tcs.TrySetResult(await callback());
                    }
                }
            }), arguments, targetSection.HasValue, targetSection ?? default, cancellationToken);
            _waitNextRun.Set();
            _waitResultArrive.WaitOne();

            if (!_execContext._completed && !cancellationToken.IsCancellationRequested)
                throw new GroupPolicyManagementException("Thread does not run correctly.");

            if (_execContext._thrownException != null && !cancellationToken.IsCancellationRequested)
                _execContext._thrownException.Throw();

            return tcs.Task;
        }

        private static void ThreadProcedure(object param)
        {
            var baseContext = param as GroupPolicyContext;

            if (baseContext == null)
                return;

            while (!baseContext._disposed)
            {
                baseContext._waitNextRun.WaitOne();

                if (baseContext._execContext == null)
                    continue;

                if (baseContext._execContext._callback == null)
                    continue;

                var handle = default(SafeCriticalPolicySectionHandle);

                try
                {
                    handle = baseContext._execContext._requireCriticalSection ?
                        NativeMethods.EnterCriticalPolicySection(baseContext._execContext._targetSection == PolicySection.Machine) :
                        null;

                    if (baseContext._execContext._callback is Func<Task> taskCallback)
                    {
                        taskCallback().GetAwaiter().GetResult();
                    }
                    else if (baseContext._execContext._callback is Func<Task<object>> taskResultCallback)
                    {
                        baseContext._execContext._callbackResult = taskResultCallback().GetAwaiter().GetResult();
                    }
                    else
                    {
                        baseContext._execContext._callbackResult = baseContext._execContext._callback.DynamicInvoke(baseContext._execContext._arguments);
                    }
                }
                catch (Exception ex) { baseContext._execContext._thrownException = ExceptionDispatchInfo.Capture(ex); }
                finally
                {
                    if (handle != null)
                    {
                        try { handle.Dispose(); }
                        catch { }

                        handle = default;
                    }

                    baseContext._execContext._completed = true;
                    baseContext._waitResultArrive.Set();
                }
            }
        }

        private sealed class ExecutionContext
        {
            public ExecutionContext(Delegate callback, object[] arguments, bool requireCriticalSection, PolicySection targetSection, CancellationToken cancellationToken = default)
            {
                _callback = callback;
                _arguments = arguments;
                _requireCriticalSection = requireCriticalSection;
                _targetSection = targetSection;
                _thrownException = default;
                _callbackResult = default;
                _completed = false;
                _cancellationToken = cancellationToken;
            }

            public Delegate _callback;
            public object[] _arguments;
            public bool _requireCriticalSection;
            public PolicySection _targetSection;
            public ExceptionDispatchInfo _thrownException;
            public object _callbackResult;
            public bool _completed;
            public CancellationToken _cancellationToken;
        }
    }
}
