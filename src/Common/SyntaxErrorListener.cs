using System.IO;
using Antlr4.Runtime;

namespace Commmon
{
    public class SyntaxErrorListener : BaseErrorListener
    {
        public int ErrorsCount;
        
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
    }
}