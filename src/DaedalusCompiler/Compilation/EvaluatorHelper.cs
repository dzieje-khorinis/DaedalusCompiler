using Antlr4.Runtime.Tree;
using DaedalusCompiler.Dat;
using System;
using System.Globalization;
using System.Linq;
using Antlr4.Runtime;

namespace DaedalusCompiler.Compilation
{
    public static class EvaluatorHelper
    {
        public static int EvaluteArraySize(DaedalusParser.ArraySizeContext context, AssemblyBuilder assemblyBuilder)
        {
            string arraySizeText = context.GetText();

            // simple value
            if (int.TryParse(context.GetText(), out var arraySize) == false)
                arraySize = (int) assemblyBuilder.GetSymbolByName(arraySizeText).Content.First();

            return arraySize;
        }

        public static object EvaluateConst(DaedalusParser.ExpressionContext expression, AssemblyBuilder assemblyBuilder,
            DatSymbolType type)
        {
            switch (type)
            {
                case DatSymbolType.String:
                    return EvaluateConstStringExpression(expression);
                case DatSymbolType.Float:
                    return EvaluateConstFloatExpression(expression.GetText());
                case DatSymbolType.Int:
                    return EvaluateConstIntExpression(expression, assemblyBuilder);
                default:
                    throw new Exception($"Unable to evaluate constant. Invalid symbol type '{type}'.");
            }
        }

        private static string EvaluateConstStringExpression(DaedalusParser.ExpressionContext expression)
        {
            if (expression is DaedalusParser.ValExpressionContext context)
                return context.value().GetChild(0).GetText().Replace("\"", "");

            throw new Exception(
                $"Unable to evaluate constant. Expression '{expression.GetText()}' contains unsupported operations.");
        }

        public static int EvaluateFloatExpression(string value)
        {
            var parsedFloatValue = GetFloat(value);

            return BitConverter.ToInt32(BitConverter.GetBytes(parsedFloatValue), 0);
        }

        private static float EvaluateConstFloatExpression(string value)
        {
            return GetFloat(value);
        }

        private static float GetFloat(string value)
        {
            var isCastingSuccessful = float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedFloat);

            if (isCastingSuccessful)
            {
                return parsedFloat;
            }
            else
            {   
                throw new Exception(
                    $"Unable to evaluate constant. Expression '{value}' contains unsupported operations.");
            }
        }

        private static int EvaluateConstIntExpression(DaedalusParser.ExpressionContext expression,
            AssemblyBuilder assemblyBuilder)
        {
            if (expression is DaedalusParser.BracketExpressionContext expressionContext)
                return EvaluateConstIntExpression(expressionContext.expression(), assemblyBuilder);

            if (expression is DaedalusParser.ValExpressionContext context)
                return EvaluateIntValueExpression(context.value(), assemblyBuilder);

            if (expression is DaedalusParser.OneArgExpressionContext)
                return EvaluateIntUnaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.MultExpressionContext)
                return EvaluateIntBinaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.AddExpressionContext)
                return EvaluateIntBinaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.BitMoveExpressionContext)
                return EvaluateIntBinaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.CompExpressionContext)
                return EvaluateIntBinaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.EqExpressionContext)
                return EvaluateIntBinaryExpression(expression, assemblyBuilder);
            
            if (expression is DaedalusParser.BinOrExpressionContext)
                return EvaluateIntBinaryExpression(expression, assemblyBuilder);

            throw new Exception(
                $"Unable to evaluate constant. Expression '{expression.GetText()}' contains unsupported operations.");
        }

        private static int EvaluateIntValueExpression(DaedalusParser.ValueContext value,
            AssemblyBuilder assemblyBuilder)
        {
            var valueChild = value.GetChild(0);

            // value is simple literal
            if (valueChild is TerminalNodeImpl)
                return int.Parse(valueChild.GetText());

            // value is reference to other constant
            if (valueChild is DaedalusParser.ComplexReferenceContext reference)
            {
                var referenceName = reference.complexReferenceNode()?.FirstOrDefault()?.referenceNode()?.GetText();

                //TODO : Allow to reference arrays. Currently it is not possible. 

                if (referenceName != null)
                {
                    var referenceSymbol = assemblyBuilder.GetSymbolByName(referenceName);

                    if (referenceSymbol == null)
                        throw new Exception($"Reference symbol {referenceName} is not declared.");

                    object referenceValue;
                    if (referenceSymbol.Type == DatSymbolType.Func)
                    {
                        referenceValue = referenceSymbol.Index;
                    }
                    else
                    {
                        referenceValue = referenceSymbol.Content.First();
                    }

                    if (referenceValue is int == false)
                        throw new Exception(
                            $"Cannot evaluate symbol {referenceName} because it has different data type: {referenceValue.GetType().Name}");

                    return (int) referenceValue;
                }
            }

            throw new Exception(
                "Unable to evaluate value expression. Only simple literals or reference variables (except arrays) are supported.");
        }

        private static int EvaluateIntUnaryExpression(DaedalusParser.ExpressionContext expression,
            AssemblyBuilder assemblyBuilder)
        {
            var oper = expression.GetChild(0).GetText();
            var param = EvaluateConstIntExpression((DaedalusParser.ExpressionContext) expression.GetChild(1),
                assemblyBuilder);

            switch (oper)
            {
                case "-":
                    return -param;
                case "+":
                    return +param;
                case "!":
                    return param == 0 ? 1 : 0;
                case "~":
                    return ~param;
                default:
                    throw new Exception($"Invalid unary operator: {oper}");
            }
        }

        private static int EvaluateIntBinaryExpression(DaedalusParser.ExpressionContext expression,
            AssemblyBuilder assemblyBuilder)
        {
            var leftParam = EvaluateConstIntExpression((DaedalusParser.ExpressionContext) expression.GetChild(0),
                assemblyBuilder);
            var oper = expression.GetChild(1).GetText();
            var rightParam = EvaluateConstIntExpression((DaedalusParser.ExpressionContext) expression.GetChild(2),
                assemblyBuilder);

            switch (oper)
            {
                case "*":
                    return leftParam * rightParam;
                case "/":
                    return leftParam / rightParam;
                case "%":
                    return leftParam % rightParam;
                case "+":
                    return leftParam + rightParam;
                case "-":
                    return leftParam - rightParam;
                case ">>":
                    return leftParam >> rightParam;
                case "<<":
                    return leftParam << rightParam;
                case ">":
                    return leftParam > rightParam ? 1 : 0;
                case "<":
                    return leftParam < rightParam ? 1 : 0;
                case ">=":
                    return leftParam >= rightParam ? 1 : 0;
                case "<=":
                    return leftParam <= rightParam ? 1 : 0;
                case "==":
                    return leftParam == rightParam ? 1 : 0;
                case "!=":
                    return leftParam != rightParam ? 1 : 0;
                case "|":
                    return leftParam | rightParam;
                case "&":
                    return leftParam & rightParam;
                case "||":
                    return ((leftParam != 0) || (rightParam != 0)) ? 1 : 0;
                case "&&":
                    return ((leftParam != 0) && (rightParam != 0)) ? 1 : 0;
                default:
                    throw new Exception($"Invalid binary operator: {oper}");
            }
        }
    }
}