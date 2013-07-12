using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace SimpleVSTestLogger
{
	[ExtensionUri("logger://Microsoft/TestPlatform/SimpleLogger/v1")]
	[FriendlyName("SimpleLogger")]
	public class SimpleLogger : ITestLogger
	{
		private bool testFailed;
		public void Initialize(TestLoggerEvents events, string testRunDirectory)
		{
			events.TestRunMessage += TestMessageHandler;
			events.TestResult += TestResultHandler;
			events.TestRunComplete += TestRunCompleteHandler;
			testFailed = false;
		}

		private static void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
		{
			Console.WriteLine("Total Executed: {0}", e.TestRunStatistics.ExecutedTests);
			Console.WriteLine("Total Passed: {0}", e.TestRunStatistics[TestOutcome.Passed]);
			var failedTests = e.TestRunStatistics[TestOutcome.Failed];
			if (failedTests > 0)
				Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Total Failed: {0}", failedTests);
            Console.ResetColor();
			Console.WriteLine("Total Skipped: {0}", e.TestRunStatistics[TestOutcome.Skipped]);
		}

		private void TestResultHandler(object sender, TestResultEventArgs e)
		{
			//only show first failed test
			if (testFailed || e.Result.Outcome != TestOutcome.Failed)
				return;

			Console.ForegroundColor = ConsoleColor.Red;

			testFailed = true;
			var name = !string.IsNullOrEmpty(e.Result.DisplayName) ? e.Result.DisplayName : e.Result.TestCase.FullyQualifiedName;
			Console.WriteLine(name + " Failed");
			Console.ResetColor();
			if (!String.IsNullOrEmpty(e.Result.ErrorStackTrace))
				Console.WriteLine(e.Result.ErrorStackTrace);

		}

		private static void TestMessageHandler(object sender, TestRunMessageEventArgs e)
		{
			switch (e.Level)
			{
				case TestMessageLevel.Informational:
					Console.WriteLine("Information: " + e.Message);
					break;

				case TestMessageLevel.Warning:
					Console.WriteLine("Warning: " + e.Message);
					break;

				case TestMessageLevel.Error:
					Console.WriteLine("Error: " + e.Message);
					break;
			}
		}
	}
}
