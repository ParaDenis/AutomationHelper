using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutomationHelper.Extensions;
using AutomationHelper.Waiters;
using Fiddler;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AutomationHelper.FiddlerCoreAPI
{
    public class FiddlerHelper
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof (FiddlerBasicMethods));
        public static List<string> RequestsList = new List<string>();
        public static List<Fiddler.Session> oAllSessions = new List<Fiddler.Session>();


        public static void CheckFullSearchAPICallExists(string url)
        {
            string s = RequestsList.Find(x => x.Contains(url));
            if (s == null)
            {
                foreach (var request in RequestsList)
                {
                    //Log.Info(request);
                }
                throw new Exception("Error: failed to find url: " + url + " in the FiddlerCore data log.");
            }
        }

        public static void DisableProxy()
        {
            //Log.Info("Disabling proxy...");
            string sRegPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings";
            //"on":

            Wait.UntilTrue(() =>
            {
                Registry.SetValue(sRegPath, "ProxyEnable", 0);
                return (int) Registry.GetValue(sRegPath, "ProxyEnable", 0) == 0;
            }, "Failed to Disable proxy");
            //else
            //    Console.WriteLine("The proxy has been turned on.");

            //// "off":
            //        Registry.SetValue(sRegPath, "ProxyEnable", 0);
            //        if ((int)Registry.GetValue(sRegPath, "ProxyEnable", 1) == 1)
            //            Console.WriteLine("Unable to disable the proxy.");
            //        else
            //            Console.WriteLine("The proxy has been turned off.");
        }

        public static void AttachEventListeners()
        {
            // It is important to understand that FiddlerCore calls event handlers on the
            // session-handling thread.  If you need to properly synchronize to the UI-thread
            // (say, because you're adding the sessions to a list view) you must call .Invoke
            // on a delegate on the window handle.

            // Simply echo notifications to the console.  Because Fiddler.CONFIG.QuietMode=true 
            // by default, we must handle notifying the user ourselves.
            FiddlerApplication.OnNotification += OnNotification;
            FiddlerApplication.Log.OnLogString += OnLogString;
            FiddlerApplication.BeforeRequest += BeforeRequest;
            FiddlerApplication.BeforeResponse += BeforeResponse;
            FiddlerApplication.AfterSessionComplete += AfterSessionComplete;
        }

        #region AttachEvent
        private static void OnNotification(object sender, NotificationEventArgs oNEA)
        {
            //Log.Info("** NotifyUser: " + oNEA.NotifyString);
            RequestsList.Add("** NotifyUser: " + oNEA.NotifyString);
        }

        private static void OnLogString(object sender, LogEventArgs oLEA)
        {
            //Log.Info("** LogString: " + oLEA.LogString);
            RequestsList.Add("** LogString: " + oLEA.LogString);
        }

        private static void BeforeRequest(Session oS)
        {
            //oAllSessions.Add(oS); not need to add this session as it added in BeforeRequest action
           // Log.Info("Before request for:\t" + oS.fullUrl);
            RequestsList.Add("Before request for:\t" + oS.fullUrl);
            // In order to enable response tampering, buffering mode must
            // be enabled; this allows FiddlerCore to permit modification of
            // the response in the BeforeResponse handler rather than streaming
            // the response to the client as the response comes in.
            oS.bBufferResponse = true;
        }

        private static void BeforeResponse(Session oS)
        {
            oAllSessions.Add(oS);
            //Log.InfoFormat("BeforeResponse {0}:HTTP {1} for {2}", oS.id, oS.responseCode, oS.fullUrl);
            RequestsList.Add("BeforeResponse " + oS.id + ":HTTP " + oS.responseCode + " for " + oS.fullUrl);
            //if(oS.url == "")

            // Uncomment the following two statements to decompress/unchunk the
            // HTTP response and subsequently modify any HTTP responses to replace 
            // instances of the word "Microsoft" with "Bayden"
            //oS.utilDecodeResponse(); oS.utilReplaceInResponse("Microsoft", "Bayden");
        }

        private static void AfterSessionComplete(Session oS)
        {
            //Log.Info("Finished session:\t" + oS.fullUrl);
            RequestsList.Add("Finished session: " + oS.fullUrl);
        }
        #endregion

        public static void StartFiddler(int timeout = 15000)
        {
            //Log.Info("Starting Fiddler...");
            oAllSessions = new List<Fiddler.Session>();
            //Console.WriteLine("Starting FiddlerCore...");
            AttachEventListeners();
           
            // For the purposes of this demo, we'll forbid connections to HTTPS 
            // sites that use invalid certificates
            Fiddler.CONFIG.IgnoreServerCertErrors = false;

            // Because we've chosen to decrypt HTTPS traffic, makecert.exe must
            // be present in the Application folder.
            Fiddler.FiddlerApplication.Startup(8877, true, true);

            Object forever = new Object();
            lock (forever)
                System.Threading.Monitor.Wait(forever, timeout);
        }

        public static void StopFiddler()
        {
            //Log.Info("Stopping Fiddler...");
            FiddlerApplication.OnNotification -= OnNotification;
            FiddlerApplication.Log.OnLogString -= OnLogString;
            FiddlerApplication.BeforeRequest -= BeforeRequest;
            FiddlerApplication.BeforeResponse -= BeforeResponse;
            FiddlerApplication.AfterSessionComplete -= AfterSessionComplete;
            FiddlerApplication.Shutdown();
            Wait.UntilTrue(() =>
            {
                FiddlerApplication.Shutdown();
                return !FiddlerApplication.IsStarted();
            }, "Error: FiddlerApplication haven't been Closed");

            Wait.Sleep(5*1000);
            DisableProxy();
            Wait.Sleep(5*1000);
        }
        /*
        /// <summary>
        /// return value from KVP while login using key. Need to add start and stop fiddler while login
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueFromKVPWhileLogin(string key, string url)
        {
            //find last Fiddler.Session with corresponding Url
            var response = oAllSessions.Where(s => s.fullUrl == url)
                .First("Can't find session with fullUrl=" + url + " in oAllSessions fiddler session");
            //convert response to string
            string responseString = response.GetResponseBodyAsString(); 
            //var responseString = Encoding.UTF8.GetString(response.ResponseBody);
            //Log.Info("Response string: " + responseString);
            //convert string response to object (TranslationKeyValuePairsClass)
            var resp = JsonConvert.DeserializeObject<TranslationKeyValuePairsClass>(responseString);
            
            var finded = resp.TranslationKeyValuePairs.First(t => t.Key == key, "Can't find object with key '" + key + "' in response");
            //Log.Info(String.Format("Key={0} : Value={1}", finded.Key, finded.Value.ToString()));
            return finded.Value.ToString();
        }*/

        public static string GetRequestValueByKey(string url, string key)
        {
            //find last Fiddler.Session with corresponding Url
            var request = oAllSessions.Where(s => s.fullUrl == url)
                .First("Can't find session with fullUrl=" + url + " in oAllSessions fiddler session");
            //convert response to string
            var requestString = Encoding.UTF8.GetString(request.RequestBody);
            //convert string response to object (TranslationKeyValuePairsClass)
            var response = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(requestString);
            return response[key];
        }

        public static string GetLastRequestBodyByUrl(string URL, bool exact = true)
        {
            //find last Fiddler.Session with corresponding Url
            var request = GetLastSessionByUrl(URL, exact); 
            return request.GetRequestBodyAsString();
        }

        public static SessionTimers GetLastSessionTimersByUrl(string URL, bool exact = true)
        {
            var request = GetLastSessionByUrl(URL, exact);
            return request.Timers;
        }


        public static string GetLastResponseBodyByUrl(string URL, bool exact = true)
        {
            //find last Fiddler.Session with corresponding Url
            var response = GetLastSessionByUrl(URL, exact);
            return response.GetResponseBodyAsString();
        }

        public static IEnumerable<string> GetResponseBodiesByUrl(string URL, bool exact = true)
        {
            return exact
                ? oAllSessions.Where(s => s.fullUrl.ToLower() == URL.ToLower()).Select(r => r.GetResponseBodyAsString())
                : oAllSessions.Where(s => s.fullUrl.ToLower().Contains(URL.ToLower())).Select(r => r.GetResponseBodyAsString());
        }

        public static IEnumerable<string> GetRequestBodiesByUrl(string URL, bool exact = true)
        {
            return exact
                ? oAllSessions.Where(s => s.fullUrl.ToLower() == URL.ToLower()).Select(r => r.GetRequestBodyAsString())
                : oAllSessions.Where(s => s.fullUrl.ToLower().Contains(URL.ToLower())).Select(r => r.GetRequestBodyAsString());
        }

        public static HTTPRequestHeaders GetLastRequestHeadersByUrl(string URL, bool exact = true)
        {
            //find last Fiddler.Session with corresponding Url
            var request = GetLastSessionByUrl(URL, exact);
            return request.RequestHeaders;
        }

        public static HTTPResponseHeaders GetLastResponseHeadersByUrl(string URL, bool exact = true)
        {
            //find last Fiddler.Session with corresponding Url
            var response = GetLastSessionByUrl(URL, exact);
            return response.ResponseHeaders;
        }
        public static string GetLastResponseHeaderValue(string headerName, string URL, bool exact = true)
        {
            return GetLastResponseHeadersByUrl(URL, exact)[headerName];
        }
        public static string GetLastRequestHeaderValue(string headerName, string URL, bool exact = true)
        {
            return GetLastRequestHeadersByUrl(URL, exact)[headerName];
        }

        private static Session GetLastSessionByUrl(string URL, bool exact = true)
        {
            return exact
                ? oAllSessions.Where(s => s.fullUrl.ToLower() == URL.ToLower())
                .Last("Can't find session with fullUrl= " + URL + " in oAllSessions fiddler session")
                : oAllSessions.Where(s => s.fullUrl.ToLower().Contains(URL.ToLower()))
                .Last("Can't find session with fullUrl= " + URL + " in oAllSessions fiddler session");
        }

        public static void ListenByFiddler(Action action)
        {
            try
            {
                StartFiddler(1);
                Wait.Sleep(2000);
                action();
                Wait.Sleep(10*1000);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception while listenByFiddler: " + ex.Message);
            }
            finally
            {
                StopFiddler();
                DisableProxy();
            }
        }

        public static bool IsSessionsContainsFullUrl(string fullUrl, bool exact = false)
        {

            return exact
                ? oAllSessions.Any(s => s.fullUrl == fullUrl)
                : oAllSessions.Any(s => s.fullUrl.Contains(fullUrl));
        }
        public static bool IsSessionsContainsUrl(string url, bool exact = false)
        {

            return exact
                ? oAllSessions.Any(s => s.url == url)
                : oAllSessions.Any(s => s.url.Contains(url));
        }
    }

    
    #region This class use for read login KVP 
    public class TranslationKeyValuePairsClass
    {
        public List<TranslationDictionary> TranslationKeyValuePairs { get; set; }
    }

    public class TranslationDictionary
    {
        public string Key { get; set; }
        public dynamic Value { get; set; }
    }
    #endregion
}
