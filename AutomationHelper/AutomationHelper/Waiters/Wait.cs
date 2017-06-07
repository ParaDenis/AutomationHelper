using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AutomationHelper.Waiters
{
    public class Wait
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof (Wait));

        public static void UntilTrue(Func<bool> p, string err, int timeout = 15000, int interval = 1000)
        {
            var timer = new Stopwatch();
            timer.Start();
            while (true)
            {
                bool res= false;
                res = p();
                if (res)
                {
                    //Log.Info(String.Format("WaitUntilTrue passed, elapsed {0} ms", timer.ElapsedMilliseconds));
                    return;
                }
                if (timer.ElapsedMilliseconds > timeout)
                    throw new TimeoutException(String.Format("WaitUntilTrue TIMED OUT ({0}s). {1}",
                        timeout / 1000, err));
                Sleep(interval);
            }
        }

        public static void UntilProcessNotExist(string processName = "")
        {
            UntilTrue(
                () =>!Process.GetProcessesByName(processName).Any(),"'" + processName + "' process stil exist", timeout: 30*1000, interval: 10);
        }

        public static void UntilProcessExist(string processName = "")
        {
            UntilTrue(
                () =>Process.GetProcessesByName(processName).Any(),"Error: '" + processName + "' process stil exist", timeout: 30 * 1000, interval: 10);
        }

        public static void Sleep(int milliseconds)
        {
            //Log.DebugCustom("WAIT.SLEEP: " + milliseconds);
            Thread.Sleep(milliseconds);
        }

        public static T UntilNoException<T>(Func<T> f, int timeout = 30000, int interval = 1000)
        {
            var timer = new Stopwatch();
            timer.Start();
            string message = String.Empty;
            while (true)
            {
                try
                {
                    var temp = f();
                    //if (message != String.Empty)
                        //Log.Info(String.Format("WaitUntilNoException passed, elapsed {0} ms", timer.ElapsedMilliseconds));
                    return temp;
                }
                catch (Exception ex)
                {
                    if (message != ex.Message)
                    {
                        //Log.Warn(String.Format("WaitUntilNoException throws {0}. Elapsed  {1}s", ex.Message,timer.ElapsedMilliseconds));
                        message = ex.Message;
                    }
                    if (timer.ElapsedMilliseconds > timeout)
                        throw new TimeoutException(String.Format("UntilNoException TIMED OUT ({0}s). {1}",
                            timeout / 1000, ex.Message));
                    Sleep(interval);
                }
            }
        }

        public static void UntilNoException(Action f, int timeout = 30000, int interval = 1000)
        {
            //Log.DebugCustom("Wait.UntilNoException(Action f) started!");
            var timer = new Stopwatch();
            timer.Start();
            string message = String.Empty;
            while (true)
            {
                try
                {
                    f();
                    //if (message != String.Empty)
                    //    Log.Info(String.Format("WaitUntilNoException passed, elapsed {0}s", timer.ElapsedMilliseconds / 1000));
                    return;
                }
                catch (Exception ex)
                {
                    if (message != ex.Message)
                    {
                        //Log.Warn(String.Format("WaitUntilNoException throws {0}. Elapsed  {1}s", ex.Message,timer.ElapsedMilliseconds / 1000));
                        message = ex.Message;
                    }
                    if (timer.ElapsedMilliseconds > timeout)
                        throw new TimeoutException(String.Format("UntilNoException TIMED OUT ({0}s). {1}",
                            timeout / 1000, ex.Message));
                    Sleep(interval);
                }
            }
        }

        public static void UntilNumberOfExceptions(Action f, int times = 3, int interval = 100)
        {
            for (var i = 0; i < times; i++)
            {
                try
                {
                    f();
                    return;
                }
                catch (Exception ex)
                {
                    //Log.Warn("UntilNumberOfExceptions (" + i + "): " + ex.Message);
                    if (i == times - 1) throw ex;
                    Sleep(interval);
                }
            }
        }

        public static T UntilNumberOfExceptions<T>(Func<T> f, int times = 3, int interval = 100)
        {
            for (var i = 0; i < times; i++)
            {
                try
                {
                    return f();
                }
                catch (Exception ex)
                {
                    //Log.Warn("UntilNumberOfExceptions<T> (" + i + "): " + ex.Message);
                    if (i == times - 1) throw ex;
                    Sleep(interval);
                }
            }
            throw new Exception("UntilNumberOfExceptions " + times + " times");
        }
        /*
        public static string ForRequest(string url, bool exact = true, int timeout = Timeouts.WAIT_FOR_REQUEST_TIMEOUT)
        {
            string result = "";
            FiddlerBasicMethods.StartFiddler(0);
            try
            {
                UntilNoException(() =>
                {
                    result = FiddlerBasicMethods.GetLastRequestBodyByUrl(url, exact);
                }, timeout);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                FiddlerBasicMethods.StopFiddler();
            }
            return result;
        }

        public static bool ForRequests(Action f, string[] urls, bool exact = true,
            int timeout = Timeouts.WAIT_FOR_REQUEST_TIMEOUT)
        {
            FiddlerBasicMethods.StartFiddler(0);
            Sleep(1000);
            f();
            FiddlerBasicMethods.StopFiddler();
            return urls.All(url => FiddlerBasicMethods.oAllSessions.Any(s => s.url == url));
        }

        public static string ForRequest(Action f, string url, bool exact = true,
            int timeout = Timeouts.WAIT_FOR_REQUEST_TIMEOUT)
        {
            string result = "";
            FiddlerBasicMethods.StartFiddler(0);
            f();
            try
            {
                UntilNoException(() =>
                {
                    result = FiddlerBasicMethods.GetLastRequestBodyByUrl(url, exact);
                }, timeout);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                FiddlerBasicMethods.StopFiddler();
            }
            return result;
        }

        public static string ForResponse(string url)
        {
            string result = "";
            FiddlerBasicMethods.StartFiddler(0);
            try
            {
                UntilNoException(() =>
                {
                    result = FiddlerBasicMethods.GetLastResponseBodyByUrl(url);
                }, 60*2*1000);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                FiddlerBasicMethods.StopFiddler();
            }
            return result;
        }*/
    }
}
