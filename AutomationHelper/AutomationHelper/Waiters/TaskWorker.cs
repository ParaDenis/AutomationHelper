using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationHelper.Waiters
{
    public class TaskWorker
    {

        private static  ObservableCollection<Task> _task = new ObservableCollection<Task>();
        //private static readonly ILog Log = LogManager.GetLogger(typeof (TaskWorker));

        public static Task Start(Action func, int timeout)
        {
            var cancelToken = new CancellationTokenSource();
            cancelToken.CancelAfter(timeout);
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (cancelToken.Token.Register(Thread.CurrentThread.Abort))
                    {
                        //Log.DebugCustom("Task {0} was started", Task.CurrentId);
                        func();
                        //Log.DebugCustom("Task {0} finished successfully", Task.CurrentId);
                    }
                }
                catch (ThreadAbortException)
                {
                    //Log.DebugCustom("Task {0} was aborted by timeout {1}ms", Task.CurrentId, timeout);
                }
            }, cancelToken.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
            _task.Add(task);
            return task;
        }

        //public static void SuperWait()
        //{
        //    try
        //    {
        //        Task.WaitAll(_task.ToArray());
        //        _task.Clear();
        //    }
        //    catch 
        //    {
        //        Log.DebugCustom("TaskWorket disposed");
        //    }

        //}

        //public static Task StartSafe(Action func, int timeout)
        //{
        //    var cancelToken = new CancellationTokenSource();
        //    cancelToken.CancelAfter(timeout);
        //    var task = Task.Factory.StartNew(() =>
        //    {
        //        try
        //        {
        //            using (cancelToken.Token.Register(Thread.CurrentThread.Abort))
        //            {
        //                Log.DebugCustom("Task {0} was started", Task.CurrentId);
        //                func();
        //                Log.DebugCustom("Task {0} finished successfully", Task.CurrentId);
        //            }
        //        }
        //        catch (ThreadAbortException)
        //        {
        //            Log.DebugCustom("Task {0} was aborted by timeout {1}ms", Task.CurrentId, timeout);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.DebugCustom("Task {0} throwm exception '{1}'", Task.CurrentId, ex.Message);
        //        }
        //    }, cancelToken.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
        //    _task.Add(task);
        //    return task;
        //}
    }
    public class TaskWorker<T>
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(TaskWorker<T>));
        public static Task<T> Start(Func<T> func, int timeout)
        {
            var cancelToken = new CancellationTokenSource();
            cancelToken.CancelAfter(timeout);
            return Task<T>.Factory.StartNew(() =>
            {
                try
                {
                    using (cancelToken.Token.Register(Thread.CurrentThread.Abort))
                    {
                        //Log.DebugCustom("Task {0} was started", Task.CurrentId);
                        var temp = func(); 
                        //Log.DebugCustom("Task {0} finished successfully", Task.CurrentId);
                        return temp;
                    }
                }
                catch (ThreadAbortException)
                {
                    //Log.DebugCustom("Task {0} was aborted by timeout {1}ms", Task.CurrentId, timeout);
                }
                return default(T);
            }, cancelToken.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
        }

        public static Task<T> StartSafe(Func<T> func, int timeout)
        {
            var cancelToken = new CancellationTokenSource();
            cancelToken.CancelAfter(timeout);
            return Task<T>.Factory.StartNew(() =>
            {
                try
                {
                    using (cancelToken.Token.Register(Thread.CurrentThread.Abort))
                    {
                        //Log.DebugCustom("Task {0} was started", Task.CurrentId);
                        var temp = func();
                        //Log.DebugCustom("Task {0} finished successfully", Task.CurrentId);
                        return temp;
                    }
                }
                catch (ThreadAbortException)
                {
                    //Log.DebugCustom("Task {0} was aborted by timeout {1}ms", Task.CurrentId, timeout);
                }
                catch (Exception ex)
                {
                    //Log.DebugCustom("Task {0} throwm exception '{1}'", Task.CurrentId, ex.Message);
                }
                return default(T);
            }, cancelToken.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
        }
    }
}
