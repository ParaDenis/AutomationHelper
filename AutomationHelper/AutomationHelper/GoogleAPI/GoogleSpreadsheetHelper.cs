using AutomationHelper.Waiters;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutomationHelper.GoogleAPI
{
    public class GoogleSpreadsheetHelper
    {
        //protected static readonly ILog Log = LogManager.GetLogger(typeof(GoogleExcelParser));
        private string _spreadsheetId;

        public GoogleSpreadsheetHelper(string spreadsheetId, SpreadsheetsService spreadsheetsService)
        {
            WorksheetFeed worksheetFeed = null;
            _spreadsheetId = spreadsheetId;
            Wait.UntilNumberOfExceptions(() =>
            {
                service = spreadsheetsService;
                var spreadsheetLink = "https://spreadsheets.google.com/feeds/spreadsheets/" + _spreadsheetId;
                var spreadsheetQuery = new SpreadsheetQuery(spreadsheetLink);
                var spreadsheetFeed = service.Query(spreadsheetQuery);
                var spreadsheetEntry = (SpreadsheetEntry) spreadsheetFeed.Entries[0];
                worksheetFeed = spreadsheetEntry.Worksheets;
                WorksheetEntry = (WorksheetEntry) worksheetFeed.Entries[0];
            }, 60*1000);
        }

        private SpreadsheetsService service;
        public  WorksheetEntry WorksheetEntry;
        private const string columnNotPresent = "Column is not present";
        private List<ListEntry> allRows;

        public List<ListEntry> AllRowsList
        {
            get { return allRows ?? (allRows = GetAllRows().ToList()); }
        } 

        private void AddNewHeader(string header)
        {
            WorksheetEntry.Cols += 1;
            WorksheetEntry.Update();
            CellQuery cellQuery = new CellQuery(WorksheetEntry.CellFeedLink);
            cellQuery.MaximumRow = 1;
            //cellQuery.Range = "A543:L543";
            CellFeed cellFeed = service.Query(cellQuery);
            CellEntry cellEntry = new CellEntry(1, (uint)cellFeed.Entries.Count+1, header);
            cellFeed.Insert(cellEntry);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>Column letter by columnName, e.g. 'G'. If not found return 'Column is not present' message</returns>
        private string GetColumnLetterByHeader(string columnName)
        {
            var regex = new Regex("[A-Z]+");
            CellQuery cellQuery = new CellQuery(WorksheetEntry.CellFeedLink);
            cellQuery.MaximumRow = 1;
            CellFeed cellFeed = service.Query(cellQuery);
            foreach (CellEntry cellEntry in cellFeed.Entries)
            {
                if (cellEntry.Value.ToLower() == columnName.ToLower())
                    return regex.Match(cellEntry.Title.Text).Value;
            }
            //column not present need to add
            return columnNotPresent;
        }

        public string GetColumnLetterOrAddNew(string columnName)
        {
            var letter = GetColumnLetterByHeader(columnName);
            if (letter == columnNotPresent)
            {
                AddNewHeader(columnName);
                return GetColumnLetterByHeader(columnName);
            }
            return letter;
        }

/*
        private string lastVersion;
        public string LastVersion
        {
            get
            {
                if (lastVersion == null)
                    lastVersion = GetLastVersionColumnName();
                return lastVersion;
            }
        }
        public string GetLastVersionColumnName()
        {
            const string versionPattern = Consts.VersionPattern;
            var regex = new Regex(versionPattern);
            CellQuery cellQuery = new CellQuery(WorksheetEntry.CellFeedLink);
            cellQuery.MaximumRow = 1;
            CellFeed cellFeed = service.Query(cellQuery);
            var lastVersion = "v0";

            //Getting last column, independed version
            lastVersion = ((CellEntry) cellFeed.Entries.Last()).Value;
            
            return lastVersion;
        }
        */
        public ListEntry GetRow(string header, string value)
        {
            var listFeedLink = WorksheetEntry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
            var listQuery = new ListQuery(listFeedLink.HRef.ToString());
            listQuery.SpreadsheetQuery = @header + " = " + value;
            var feed = service.Query(listQuery);
            try
            {
                return feed.Entries.Select(e => (ListEntry)e).First();
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<ListEntry> GetAllRows()
        {
            AtomLink listFeedLink = WorksheetEntry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
            ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
            ListFeed feed = service.Query(listQuery);

            return feed.Entries.Select(e => (ListEntry)e);
        }

        public bool IsRowContainsColumn(ListEntry row, string columnName)
        {
            return
                row.Elements.Cast<ListEntry.Custom>()
                    .Any(element => element.LocalName.ToLower() == columnName.ToLower());
        }
/*
        public void StoreResultToSpread(string appVersion, TestContext testContext, Stopwatch textExecutionTime, string ramUsage)
        {
            var sw = new Stopwatch();
            //Get values to save
            var batScript =
                @"""%programfiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\MSTest.exe"" /testcontainer:""%ATProSolutionPath%\ATPro_Automation_NUnit\bin\Release\ATPro_Automation_NUnit.dll"" /test:" +
                testContext.TestName;
            var machineTag = testContext.Properties[Enums.Tags.Machine].ToString();
            var scopeTag = testContext.Properties[Enums.Tags.Scope].ToString();
            var testOutcome = testContext.CurrentTestOutcome;
            var testExecutionTime = textExecutionTime.Elapsed.ToString();

            var exceptionStr = testContext.CurrentTestOutcome == UnitTestOutcome.Passed
                ? ""
                : TestBase.TestExecutionArgs.Exception.Message.Replace(Convert.ToChar((byte) 0x1F), ' ');
            exceptionStr = exceptionStr.Replace("\r", "").Replace("\n", "");
            exceptionStr = exceptionStr.Length > 255 ? exceptionStr.Substring(0, 255) : exceptionStr;

            var machineName = Environment.MachineName.ToUpper();
            string timeColumn;
            if (machineName.Contains("VM"))
                timeColumn = Headers.VM_EXECUTION_TIME;
            else if (machineName.Contains("MAZ-IRL-WIN"))
                timeColumn = Headers.AZURE_TIME;
            else timeColumn = Headers.LM_EXECUTION_TIME;

            string runtimeColumn = "Unknown";
            if (testContext.Properties[Enums.Tags.Runtime].ToString() == Enums.Runtime.Daily.ToString())
                runtimeColumn = Enums.Runtime.Daily;
            else if (testContext.Properties[Enums.Tags.Runtime].ToString() == Enums.Runtime.Nightly.ToString())
                runtimeColumn = Enums.Runtime.Nightly;
            
            try
            {
                sw.Start();
                Log.Info("Storing result to google spreadsheet");

                ListEntry row = GetRow(Headers.TESTNAME.ToLower(), testContext.TestName);
                if (row != null)
                {
                    if (!IsRowContainsColumn(row, appVersion))
                    {
                        GetColumnLetterOrAddNew(appVersion);
                        row = GetRow(Headers.TESTNAME.ToLower(), testContext.TestName);
                    }
                    //If the Latest app version is more than appVersion then add a new column into a spreadsheet
                    var oldestTestResultValue = row.GetLocalNameValue(appVersion);
                    row.SetLocalNameValue(Headers.BAT_SCRIPT, batScript);
                    row.SetLocalNameValue(Headers.MACHINE, machineTag);
                    row.SetLocalNameValue(Headers.SCOPE, scopeTag);
                    row.SetLocalNameValue(Headers.RUNTIME, runtimeColumn);
                    if (testOutcome == UnitTestOutcome.Passed)
                    {
                        row.SetLocalNameValue(Headers.EXCEPTION, exceptionStr);
                        row.SetLocalNameValue(appVersion, testOutcome.ToString());
                        row.SetLocalNameValue(timeColumn, testExecutionTime);
                        row.SetLocalNameValue(Headers.RAM, ramUsage);
                        
                    }
                    else //if test passed and now became failed on this version- then skip
                    {
                        if (oldestTestResultValue != UnitTestOutcome.Passed.ToString())
                        {
                            row.SetLocalNameValue(Headers.EXCEPTION, exceptionStr);
                            row.SetLocalNameValue(appVersion, testOutcome.ToString());
                            row.SetLocalNameValue(Headers.RAM, ramUsage);
                        }
                    }
                    row.Update();
                }
                else
                {
                    WorksheetEntry.Rows += 1;
                    Log.Debug("Adding a new row started");
                    WorksheetEntry.Update();
                    Log.Info("Adding a new row completed");
                    AtomLink listFeedLink = WorksheetEntry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
                    ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
                    ListFeed feed = service.Query(listQuery);
                    row = new ListEntry();
                    row.Elements.Add(new ListEntry.Custom()
                    {
                        LocalName = Headers.TESTNAME.ToLower(),
                        Value = testContext.TestName
                    });
                    row.Elements.Add(new ListEntry.Custom()
                    {
                        LocalName = Headers.MACHINE.ToLower(),
                        Value = machineTag
                    });
                    row.Elements.Add(new ListEntry.Custom()
                    {
                        LocalName = Headers.SCOPE.ToLower(),
                        Value = scopeTag
                    });
                    row.Elements.Add(new ListEntry.Custom()
                    {
                        LocalName = Headers.RUNTIME.ToLower(),
                        Value = runtimeColumn
                    });
                    row.Elements.Add(new ListEntry.Custom()
                    {
                        LocalName = Headers.BAT_SCRIPT.ToLower(),
                        Value = batScript
                    });
                    if (testOutcome == UnitTestOutcome.Passed)
                    {
                        row.Elements.Add(new ListEntry.Custom()
                        {
                            LocalName = timeColumn.Replace(" ", "").ToLower(),
                            Value = testExecutionTime
                        });
                    }
                    row.Elements.Add(new ListEntry.Custom()
                    {
                        LocalName = Headers.EXCEPTION.ToLower(),
                        Value = exceptionStr
                    });
                    row.Elements.Add(new ListEntry.Custom()
                    {
                        LocalName = appVersion.ToLower(),
                        Value = testOutcome.ToString()
                    });
                    row.Elements.Add( new ListEntry.Custom()
                    {
                        LocalName = Headers.RAM.ToLower(),
                        Value = ramUsage
                    });
                    service.Insert(feed, row);
                }
            }
            catch (Exception ex)
            {
                Log.Info("Exception was thrown while storing in google Spreadsheet: " + ex.Message);
                throw;
            }
            finally
            {
                sw.Stop();
                Log.Info("Time spent for storing: " + sw.Elapsed);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
     
        public List<string> GetListOfTest(string[] args, double startPoint = 0, double endPoint = 1)
        {
            return Wait.UntilNoException(() =>
            {
                var upperArgs = args.Select(a => a.ToUpper());
                

                var failed = upperArgs.Contains(UnitTestOutcome.Failed.ToString().ToUpper());

                var dailyNigtlyOrAll = "ALL";
                if (upperArgs.Contains(Enums.Runtime.Daily.ToUpper()) &&
                    !upperArgs.Contains(Enums.Runtime.Nightly.ToUpper()))
                    dailyNigtlyOrAll = Enums.Runtime.Daily.ToUpper();
                else if (!upperArgs.Contains(Enums.Runtime.Daily.ToUpper()) &&
                         upperArgs.Contains(Enums.Runtime.Nightly.ToUpper()))
                    dailyNigtlyOrAll = Enums.Runtime.Nightly.ToUpper();

                var isAnyVMExist = upperArgs.Any(a => a.Contains("VM"));

                var resultList = new List<string>();
                //int rowN = 0; // first is header Row
                foreach (ListEntry row in AllRowsList)
                {
                    var shouldTake = true;
                    var testName = row.GetLocalNameValue(Headers.TESTNAME);
                    if (testName.Length > 0)
                    {
                        var testMachine = row.GetLocalNameValue(Headers.MACHINE).ToUpper();
                        var testRuntime = row.GetLocalNameValue(Headers.RUNTIME).ToUpper();
                        var testResult = row.GetLocalNameValue(LastVersion).ToUpper();
                        if (isAnyVMExist)
                            shouldTake = upperArgs.Contains(testMachine);
                        if ((dailyNigtlyOrAll != "ALL") && (shouldTake))
                            shouldTake = upperArgs.Contains(testRuntime);
                        if ((failed) && (shouldTake))
                            shouldTake = testResult != UnitTestOutcome.Passed.ToString().ToUpper();
                        if(shouldTake)
                            resultList.Add(testName);
                    }
                }
                if (startPoint == 0 && endPoint == 1)
                    return resultList;
                
                resultList.Sort();
                var count = resultList.Count;

                var startOn = (int)(count * startPoint);
                var take = (int)Math.Round((count * endPoint) - (count * startPoint));
                if (endPoint == 1)
                    return resultList.Skip(startOn).ToList();
                if (startPoint == 0)
                    return resultList.Take(take).ToList();
                var res = resultList.Skip(startOn).Take(take);
                return res.ToList();
            }, timeout: 30*1000);
        }


        private struct Headers
        {
            public const string TESTNAME = "TestName";
            public const string RUNTIME = "Runtime";
            public const string SCOPE = "Scope";
            public const string MACHINE = Enums.Tags.Machine;
            public const string BAT_SCRIPT = "bat-script";
            public const string LM_EXECUTION_TIME = "LMTime";
            public const string VM_EXECUTION_TIME = "VMTime";
            public const string AZURE_TIME = "AzureTime";
            public const string UNKNOWN = "Unknown";
            public const string COMMENT = "Comment";
            public const string EXCEPTION = "Exception";
            public const string RAM = "RAM";
        }
 * */
    }

    public static class GoogleSpreadsheetExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntry"></param>
        /// <param name="localName">LocalName for ListEntry element</param>
        /// <returns>value of element with LocalName. If not found return String.Empty</returns>
        public static string GetLocalNameValue(this ListEntry listEntry, string localName)
        {
            foreach (ListEntry.Custom element in listEntry.Elements)
            {
                if (element.LocalName.ToLower().Replace(" ","") == localName.ToLower().Replace(" ",""))
                    return element.Value;
            }
            return String.Empty;
        }

        public static void SetLocalNameValue(this ListEntry listEntry, string localName, string value)
        {
            foreach (ListEntry.Custom element in listEntry.Elements)
            {
                if (element.LocalName.ToLower().Replace(" ", "") == localName.ToLower().Replace(" ", ""))
                    element.Value = value;
            }
        }
    }
}

