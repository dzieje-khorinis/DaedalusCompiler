using System;

namespace Commmon.SemanticAnalysis
{
    public abstract class ErrorLogger
    {
        public abstract void LogLine(string message);
    }

    public class StdErrorLogger : ErrorLogger
    {
        public override void LogLine(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class StringBufforErrorLogger : ErrorLogger
    {   
        private string _buffer = "";

        public override void LogLine(string message)
        {
            if (_buffer == null)
            {
                _buffer = message;
            }
            else
            {
                _buffer = $"{_buffer}{Environment.NewLine}{message}";
            }
        }

        public string GetBuffer()
        {
            return _buffer;
        }
    }
}