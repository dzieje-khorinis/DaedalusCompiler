using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;

namespace DaedalusCompiler.Compilation
{
    public class SyntaxErrorListener : BaseErrorListener
    {
        public int ErrorsCount = 0;
        
        public override void SyntaxError(
            TextWriter output,
            IRecognizer recognizer,
            IToken offendingSymbol,
            int line,
            int charPositionInLine,
            string msg,
            RecognitionException e)
        {
            ErrorsCount++;
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
            Console.WriteLine($"ReportAmbiguity");
        }

        public override void ReportAttemptingFullContext(
            Parser recognizer,
            DFA dfa,
            int startIndex,
            int stopIndex,
            BitSet conflictingAlts,
            SimulatorState conflictState)
        {
            Console.WriteLine($"ReportAttemptingFullContext");
        }

        public override void ReportContextSensitivity(
            Parser recognizer,
            DFA dfa,
            int startIndex,
            int stopIndex,
            int prediction,
            SimulatorState acceptState)
        {
            Console.WriteLine($"ReportContextSensitivity");
        }
    }
}