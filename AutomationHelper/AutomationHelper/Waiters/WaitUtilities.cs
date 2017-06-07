using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationHelper.Waiters
{
    //public class WaitTimer
    //{
    //    private static readonly ILog Log = LogManager.GetLogger(typeof (WaitTimer));
    //    private Action _f;
    //    private readonly int _interval = 100;
    //    private int _timeout;
    //    private int _taskTimeout;

    //    //public WaitTimer(Action f, int timeout)
    //    //{
    //    //    _f = f;
    //    //    _timeout = timeout;
    //    //    _taskTimeout = timeout > 10000 ? timeout/3 : _timeout;
    //    //}
        
    //    //public void Start()
    //    //{
    //    //    string innerExceptionMessage = "";
    //    //    bool isFinished = false;
    //    //    var mainTask = TaskWorker.Start(() =>
    //    //    {
    //    //        while (!isFinished)
    //    //        {
    //    //            var task = TaskWorker.Start(_f, _taskTimeout);
    //    //            try
    //    //            {
    //    //                task.SuperWait();
    //    //                isFinished = true;
    //    //            }
    //    //            catch (Exception exception)
    //    //            {

    //    //                if (exception.InnerException != null)
    //    //                {
    //    //                    if (exception.InnerException.GetType() == typeof (ThreadAbortException))
    //    //                    {
    //    //                        Log.DebugCustom("Task has been aborted");
    //    //                    }
    //    //                    else
    //    //                    {
    //    //                        innerExceptionMessage = exception.InnerException.Message;
    //    //                        Log.DebugCustom("Inner exception occured while executed f {0}", exception.InnerException);
    //    //                    }
    //    //                }
    //    //            }
    //    //            finally
    //    //            {
    //    //                task.Dispose();
    //    //            }
    //    //            Thread.Sleep(_interval);
    //    //        }
    //    //    }, _timeout);
    //    //    Exception _exception=null;
            
    //    //    try
    //    //    {
    //    //        mainTask.SuperWait();
    //    //    }
    //    //    catch (Exception exception)
    //    //    {
    //    //        _exception = exception;
    //    //    }
    //    //    GC.Collect();
    //    //    mainTask.Dispose();
    //    //    if (_exception != null)
    //    //        throw new Exception(
    //    //            string.Format("Action has timedout with timeout:{0}, taskTimeout:{1}, InnerException: {2}, Exception: {3}", _timeout,
    //    //                _taskTimeout, _exception.InnerException.Message, innerExceptionMessage));
    //    //}

    //}

    //public class WaitTimer<T>
    //{
    //    private static readonly ILog Log = LogManager.GetLogger(typeof(WaitTimer));
    //    private Func<T> _f;
    //    //private Task<T> _task;
    //    private readonly int _interval = 400;
    //    private int _timeout;
    //    private int _taskTimeout;


    //    public WaitTimer(Func<T> f, int timeout)
    //    {
    //        _f = f;
    //        _timeout = timeout;
    //        _taskTimeout = timeout > 10000 ? timeout / 3 : _timeout;
    //    }

    //    public T Start()
    //    {
    //        bool isFinished = false;
    //        var mainTask = TaskWorker<T>.Start(() =>
    //        {
    //            Task<T> task = null;
    //            while (!isFinished)
    //            {

    //                task = TaskWorker<T>.Start(_f, _taskTimeout);
    //                try
    //                {
    //                    task.SuperWait();
    //                    isFinished = true;
    //                }
    //                catch (Exception exception)
    //                {
    //                    if (exception.InnerException.GetType() == typeof(ThreadAbortException))
    //                    {
    //                        Log.DebugCustom("Task {0} has been aborted", task.Id);
    //                    }
    //                    else
    //                    {
    //                        Log.DebugCustom("Inner exception occured while executed f {0}", exception.InnerException);
    //                    }
    //                }
    //                finally
    //                {
    //                    task.Dispose();
    //                }
    //                Thread.Sleep(_interval);
    //            }
    //            return task.Result;
    //        }, _timeout);
    //        Exception _exception = null;

    //        try
    //        {
    //            mainTask.SuperWait();
    //        }
    //        catch (Exception exception)
    //        {
    //            _exception = exception;
    //        }
    //        GC.Collect();
    //        var res = mainTask.Result;
    //        try
    //        {
    //            mainTask.Dispose();
    //        }
    //        catch (Exception e )
    //        {
    //            Log.Warn(e.Message);
    //        }
            
    //        if (_exception != null)
    //            throw new Exception(
    //                string.Format("Action has timedout with timeout:{0}, taskTimeout:{1}, InnerException: {2}", _timeout,
    //                    _taskTimeout, _exception.InnerException.Message));
    //        return res;
    //    }
    //}

}
