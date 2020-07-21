using System.Collections.Generic;
using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using Common.SemanticAnalysis;

namespace Common
{
    public class SyntaxError
    {
        public int LineNo;
        public int ColumnNo;
        public string Message;

        public void Print(string fileName, string line, ErrorLogger errorLogger) {
            errorLogger.LogLine($"{fileName}:{LineNo}:{ColumnNo}: {Message}");
            errorLogger.LogLine(line.Replace("\t", "    "));
            if (ColumnNo >= 0)
            {
                errorLogger.LogLine(GetErrorPointerLine(line));
            }
            
        }

        private string GetErrorPointerLine(string line)
        {
            string[] buffer = new string[ColumnNo + 1];

            if (ColumnNo == 0) {
                return "^";
            }

            for (int i = 0; i < ColumnNo + 1; i++)
            {
                if (line[i] == '\t')
                {
                    buffer[i] = "    ";
                }
                else
                {
                    buffer[i] = " ";
                }
            }
            buffer[ColumnNo] = "^";
            
            return String.Join("", buffer);
        }
    }


    public class SyntaxErrorListener : BaseErrorListener
    {
        public List<SyntaxError> SyntaxErrors;

        public SyntaxErrorListener() : base() {
            SyntaxErrors = new List<SyntaxError>();
        }
        
        public override void SyntaxError(
            TextWriter output,
            IRecognizer recognizer,
            IToken offendingSymbol,
            int lineNo,
            int columnNo,
            string message,
            RecognitionException e)
        {
            SyntaxErrors.Add(new SyntaxError {
                LineNo=lineNo,
                ColumnNo=columnNo,
                Message=message,
            });
        }
        
        public override void ReportAmbiguity(
            Parser recognizer,
            DFA dfa,
            int startIndex,
            int stopIndex,
            bool exact,
            BitSet ambigAlts,
            ATNConfigSet configs)
        {
            Console.WriteLine("Dupa blada");
        }

        public override void ReportAttemptingFullContext(
            Parser recognizer,
            DFA dfa,
            int startIndex,
            int stopIndex,
            BitSet conflictingAlts,
            SimulatorState conflictState)
        {
            Console.WriteLine("Dupa blada");
        }

        public override void ReportContextSensitivity(
            Parser recognizer,
            DFA dfa,
            int startIndex,
            int stopIndex,
            int prediction,
            SimulatorState acceptState)
        {
            Console.WriteLine("Dupa blada");
        }
    }
}