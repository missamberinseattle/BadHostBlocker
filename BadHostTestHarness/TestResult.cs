using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace BadHostTestHarness
{
    public class TestResult
    {
        public TestResult()
        {
            var stack = new StackTrace();
            var callingFrame = stack.GetFrame(1);
            var name = callingFrame.GetMethod().Name;

            Name = name;
            Pass = true;
            Message = "";
        }

        public TestResult(string name, bool pass, string message)
        {
            Name = name;
            Pass = pass;
            Message = message;
        }

        public string Name { get; set; }
        public bool Pass { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public void Fail(string message, Exception ex = null)
        {
            Pass = false;
            Message = message;
            Exception = ex;
        }

        public override string ToString()
        {
            return (Pass ? "PASS" : "FAIL") + " " + Name + ": " + Message +
                (Exception != null ? "\r\n" + Exception.ToString() : "");
        }
    }
}
