/*using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ATPro_Automation_NUnit.Logger
{
    public static class HtmlLogger
    {
        //static readonly ILog Log = LogManager.GetLogger(typeof (HtmlLogger));
        private const string FILENAME = "ResultHTML.htm";
        private const string TEMPLATE_RESULT_FILENAME = @"\Logger\" + FILENAME;
        private const string TEMPLATE_ROW_FILENAME = @"\Logger\TableRowTemplate.htm";
        private const int WARNING_RAM_LIMIT = 400000000;
        public static void StoreTestResults(string appVersion, TestContext testContext, Stopwatch testExecutionTime, long ramUsage)
        {
            var sw = new Stopwatch();
            try
            {
                sw.Start();

                //Collect values to save
                var newResultFilePath = testContext.TestDir + @"\..\" + FILENAME;
                try
                {
                    var clipFilePath = Path.Combine(Paths.PROJECT_FULL_PATH + @"\Logger\jquery-3.1.1.js");
                    var destinationPath = Path.Combine(clipFilePath, testContext.TestDir + @"\..\jquery-3.1.1.js");
                    if (!File.Exists(destinationPath))
                        File.Copy(clipFilePath, destinationPath);
                }
                catch (Exception exception)
                {
                    //Log.Warn(exception.Message);
                }

                string contentFilePath = File.Exists(newResultFilePath)
                    ? newResultFilePath
                    : Path.Combine(Paths.PROJECT_FULL_PATH + TEMPLATE_RESULT_FILENAME);

                var content = File.ReadAllText(contentFilePath);

                var resultRowHtmlContent = GenerateResultRowContent(testContext, testExecutionTime, ramUsage);

                var resultContent = content.Replace("RESULT_ROWS", resultRowHtmlContent + "RESULT_ROWS");
                resultContent = resultContent.Replace("{APP_VERSION}", "ver. " + appVersion);
                File.WriteAllText(newResultFilePath, resultContent);
            }
            catch (Exception exception)
            {
                //Log.Info("Exception was thrown while storing in google Spreadsheet: " + exception.Message);
            }
            finally
            {
                sw.Stop();
                //Log.Info("Time spent for storing: " + sw.Elapsed);
            }
        }

        private static string GenerateResultRowContent(TestContext testContext, Stopwatch testExecutionTime, long ramUsage)
        {
            var testName = testContext.TestName;

            //var machineTag = testContext.Properties[Enums.Tags.Machine].ToString() +
            //                 testContext.Properties[Enums.Tags.Scope].ToString();
            string toggleFuncId;
            string testOutcome;
            string colorOnMouseOver;
            string colorOnMouseOut;

            if (testContext.CurrentTestOutcome == UnitTestOutcome.Passed)
            {
                testOutcome = "testOk";
                colorOnMouseOver = "green";
                colorOnMouseOut = "lime";
                toggleFuncId = "sisogagiPassed";
            }
            else
            {
                testOutcome = "testKo";
                colorOnMouseOver = "orange";
                colorOnMouseOut = "red";
                toggleFuncId = "sisogagi";
            }
            var testTime = testExecutionTime.Elapsed.ToString();

            var logFilePath = Path.Combine(testContext.ResultsDirectory, testName, testName + ".log");
            var logs = new List<string>();
            using (var fs = new FileStream(logFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                string line = String.Empty;
                while ((line = sr.ReadLine()) != null) // reading the old data
                {
                    logs.Add(line);
                }
            }

            var decoratedLogs = logs.Select(DecorateLine);

            var screenshotFilePath ="";
            try
            {
                screenshotFilePath= Path.Combine(testContext.TestResultsDirectory, testName + ".png");
                screenshotFilePath = screenshotFilePath.Replace(Environment.GetEnvironmentVariable("ATProSolutionPath") + @"\TestResults\", "");
                screenshotFilePath = screenshotFilePath.Replace(@"\", @"/");
                screenshotFilePath = @"./" + screenshotFilePath;
            }
            catch (Exception exception)
            {
                Log.Warn(exception.Message);
            } 

            var rowContentFilePath = Path.Combine(Paths.PROJECT_FULL_PATH + TEMPLATE_ROW_FILENAME);
            var rowContent = File.ReadAllText(rowContentFilePath);
            var resultRowContent =rowContent;
            resultRowContent = resultRowContent.Replace("{TOGGLE_FUNC_ID}", toggleFuncId);
            resultRowContent = resultRowContent.Replace("{TEST_NAME}", testName);
            var resultLog = String.Join("<br>", decoratedLogs); 
            resultRowContent = resultRowContent.Replace("{STACK_TRACE}", resultLog);
            resultRowContent = resultRowContent.Replace("{EXECUTION_TIME}", testTime);
            resultRowContent = resultRowContent.Replace("{TEST_OUTCOME}", testOutcome);
            resultRowContent = resultRowContent.Replace("{LOGS_DIV_ID}", BaseTypesExtensions.WordGenerator(8));
            resultRowContent = resultRowContent.Replace("{ON_MOUSE_OVER}", colorOnMouseOver);
            resultRowContent = resultRowContent.Replace("{ON_MOUSE_OUT}", colorOnMouseOut);
            resultRowContent = resultRowContent.Replace("{SCREENSHOT_PATH}", screenshotFilePath);
            resultRowContent = resultRowContent.Replace("{RAM_USAGE}", GetRamUsageHTML(ramUsage));
            return resultRowContent;
        }

        private static string DecorateLine(string line)
        {
            if (line.Contains("WARN"))
                return String.Format(@"<font color=""#CA6500"" >{0}</font>", line);
            if (line.Contains(@"] ERROR     at"))
                return String.Format(@"<font color=""red"" >{0}", line);//closing tag in PostSharpAspect.cs
            if (line.Contains(@"] ERROR"))
                return String.Format(@"<font size=""2"" color=""red"" ><b>{0}", line);//closing tag in PostSharpAspect.cs
            if (line.Contains("Test result: Passed"))
                return String.Format(@"<font size=""2"" color=""green"" ><b>{0}</b></font>", line);

            return line;
        }

        private static string GetRamUsageHTML(long ramUsage)
        {
            if (ramUsage>WARNING_RAM_LIMIT)
                return "<font color=\"red\">" + PerformanceMethods.MemoryUsage + "</font>";
            return "<font color=\"green\">" + PerformanceMethods.MemoryUsage + "</font>";
        }
    }
}*/
