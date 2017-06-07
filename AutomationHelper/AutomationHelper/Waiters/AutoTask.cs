using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace ATPro_Automation_NUnit.Common
{
    public class AutoTask
    {
        private ObservableCollection<Task> _task = new ObservableCollection<Task>();
        public ObservableCollection<AutoTask> _autoTasks = new ObservableCollection<AutoTask>(); 
        private static readonly ILog Log = LogManager.GetLogger(typeof(TaskWorker));

        public void Start(Action func, int timeout)
        {
            var cancelToken = new CancellationTokenSource();
            cancelToken.CancelAfter(timeout);
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (cancelToken.Token.Register(Thread.CurrentThread.Abort))
                    {
                        Log.DebugCustom("Task {0} was started", Task.CurrentId);
                        func();
                        Log.DebugCustom("Task {0} finished successfully", Task.CurrentId);
                    }
                }
                catch (ThreadAbortException)
                {
                    Log.DebugCustom("Task {0} was aborted by timeout {1}ms", Task.CurrentId, timeout);
                }
            }, cancelToken.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
            _task.Add(task);
        }

        public AutoTask StartAutoTaskSafe(Action func, int timeout)
        {
            var autoTask = new AutoTask();
            autoTask.StartSafe(func,timeout);
            _autoTasks.Add(autoTask);
            return autoTask;
        }

        public void Wait()
        {
            try
            {
                Task.WaitAll(_task.ToArray());
                foreach (var paraTask in _autoTasks)
                {
                    paraTask.Wait();
                }
                _autoTasks.Clear();
                _task.Clear();
            }
            catch 
            {
                Log.DebugCustom("TaskWorket disposed");
            }
            finally
            {
                GC.Collect();
                _task.ToList().ForEach(t=>t.Dispose());
            }

        }

        public Task StartSafe(Action func, int timeout)
        {
            var cancelToken = new CancellationTokenSource();
            cancelToken.CancelAfter(timeout);
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (cancelToken.Token.Register(Thread.CurrentThread.Abort))
                    {
                        Log.DebugCustom("Task {0} was started", Task.CurrentId);
                        func();
                        Log.DebugCustom("Task {0} finished successfully", Task.CurrentId);
                    }
                }
                catch (ThreadAbortException)
                {
                    Log.DebugCustom("Task {0} was aborted by timeout {1}ms", Task.CurrentId, timeout);
                }
                catch (Exception ex)
                {
                    Log.DebugCustom("Task {0} throwm exception '{1}'", Task.CurrentId, ex.Message);
                }
            }, cancelToken.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
            _task.Add(task);
            return task;
        }
    }
}
