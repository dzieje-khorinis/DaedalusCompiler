using System;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
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
        
        public void Log(string message)
        {
            Console.Write(message);
        }
    }

    public class StringBufforErrorLogger : ErrorLogger
    {   
        private string _buffor = "";

        public override void LogLine(string message)
        {
            if (_buffor == null)
            {
                _buffor = message;
            }
            else
            {
                _buffor = $"{_buffor}{Environment.NewLine}{message}";
            }
        }

        public string GetBuffor()
        {
            return _buffor;
        }
    }
}