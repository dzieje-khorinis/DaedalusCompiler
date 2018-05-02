using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Compilation.Evaluation
{
    public abstract class EvaluatorBase<T> : IEvaluator
    {
        object IEvaluator.EvaluateConst(DaedalusParser.ExpressionContext expression, AssemblyBuilder assemblyBuilder)
        {
            return EvaluateConst(expression, assemblyBuilder);
        }

        public T EvaluateConst(DaedalusParser.ExpressionContext expression, AssemblyBuilder assemblyBuilder)
        {
            if (expression is DaedalusParser.BracketExpressionContext)
                return EvaluateConst(((DaedalusParser.BracketExpressionContext)expression).expression(), assemblyBuilder);

            if (expression is DaedalusParser.ValExpressionContext)
                return EvaluateValueExpression(((DaedalusParser.ValExpressionContext)expression).value(), assemblyBuilder);

            if (expression is DaedalusParser.OneArgExpressionContext)
                return EvaluateUnaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.MultExpressionContext)
                return EvaluateBinaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.AddExpressionContext)
                return EvaluateBinaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.BitMoveExpressionContext)
                return EvaluateBinaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.CompExpressionContext)
                return EvaluateBinaryExpression(expression, assemblyBuilder);

            if (expression is DaedalusParser.EqExpressionContext)
                return EvaluateBinaryExpression(expression, assemblyBuilder);

            throw new Exception($"Unable to evaluate constant. Expression {expression.GetText()} is not supported.");
        }

        protected T EvaluateValueExpression(DaedalusParser.ValueContext value, AssemblyBuilder assemblyBuilder)
        {
            var valueChild = value.GetChild(0);

            // value is simple literal
            if (valueChild is TerminalNodeImpl)
                return FromStringValue(valueChild.GetText());

            // value is reference to other constant
            if (valueChild is DaedalusParser.ComplexReferenceContext)
            {
                if (IsReferenceEvaluationAllowed() == false)
                    throw new Exception($"Reference evaluation is not supported by {typeof(T).Name} type.");

                var reference = (DaedalusParser.ComplexReferenceContext)valueChild;
                var referenceName = reference.complexReferenceNode()?.FirstOrDefault()?.referenceNode()?.GetText();

                //TODO : Allow to reference arrays. Currently it is not possible. 

                if (referenceName != null)
                {
                    var referenceSymbol = assemblyBuilder.getSymbolByName(referenceName);

                    if (referenceSymbol == null)
                        throw new Exception($"Reference symbol {referenceName} is not declared.");

                    var referenceValue = referenceSymbol.Content.First();

                    if (referenceValue.GetType() != typeof(T))
                        throw new Exception($"Cannot evaluate symbol {referenceName} because it has different data type: {referenceValue.GetType().Name}");

                    return (T)referenceValue;
                }
            }

            throw new Exception("Unable to evaluate value expression. Only simple literals or reference variables (except arrays) are supported.");
        }

        protected T EvaluateUnaryExpression(DaedalusParser.ExpressionContext expression, AssemblyBuilder assemblyBuilder)
        {
            var oper = expression.GetChild(0).GetText();
            var param = EvaluateConst((DaedalusParser.ExpressionContext)expression.GetChild(1), assemblyBuilder);

            switch (oper)
            {
                case "-":
                    return Minus(param);
                case "+":
                    return Plus(param);
                case "!":
                    return Negate(param);
                case "~":
                    return BinaryNegate(param);
                default:
                    throw new Exception($"Invalid unary operator: {oper}");
            }
        }

        protected T EvaluateBinaryExpression(DaedalusParser.ExpressionContext expression, AssemblyBuilder assemblyBuilder)
        {
            var leftParam = EvaluateConst((DaedalusParser.ExpressionContext)expression.GetChild(0), assemblyBuilder);
            var oper = expression.GetChild(1).GetText();
            var rightParam = EvaluateConst((DaedalusParser.ExpressionContext)expression.GetChild(2), assemblyBuilder);

            switch (oper)
            {
                case "*":
                    return Multiplicate(leftParam, rightParam);
                case "/":
                    return Divide(leftParam, rightParam);
                case "%":
                    return Modulo(leftParam, rightParam);
                case "+":
                    return Sum(leftParam, rightParam);
                case "-":
                    return Subtract(leftParam, rightParam);
                case ">>":
                    return ShiftRight(leftParam, rightParam);
                case "<<":
                    return ShiftLeft(leftParam, rightParam);
                case ">":
                    return Greater(leftParam, rightParam);
                case "<":
                    return Less(leftParam, rightParam);
                case ">=":
                    return GreaterOrEqual(leftParam, rightParam);
                case "<=":
                    return LessOrEqual(leftParam, rightParam);
                case "==":
                    return Equal(leftParam, rightParam);
                case "!=":
                    return NotEqual(leftParam, rightParam);
                case "|":
                    return BitOr(leftParam, rightParam);
                case "&":
                    return BitAnd(leftParam, rightParam);
                case "||":
                    return LogicOr(leftParam, rightParam);
                case "&&":
                    return LogicAnd(leftParam, rightParam);
                default:
                    throw new Exception($"Invalid binary operator: {oper}");
            }
        }

        protected abstract bool IsReferenceEvaluationAllowed();
        protected abstract T FromStringValue(string param);

        protected abstract T Minus(T param);
        protected abstract T Plus(T param);
        protected abstract T Negate(T param);
        protected abstract T BinaryNegate(T param);

        protected abstract T Multiplicate(T leftParam, T rightParam);
        protected abstract T Divide(T leftParam, T rightParam);
        protected abstract T Modulo(T leftParam, T rightParam);
        protected abstract T Sum(T leftParam, T rightParam);
        protected abstract T Subtract(T leftParam, T rightParam);
        protected abstract T ShiftRight(T leftParam, T rightParam);
        protected abstract T ShiftLeft(T leftParam, T rightParam);
        protected abstract T Greater(T leftParam, T rightParam);
        protected abstract T Less(T leftParam, T rightParam);
        protected abstract T GreaterOrEqual(T leftParam, T rightParam);
        protected abstract T LessOrEqual(T leftParam, T rightParam);
        protected abstract T Equal(T leftParam, T rightParam);
        protected abstract T NotEqual(T leftParam, T rightParam);
        protected abstract T BitOr(T leftParam, T rightParam);
        protected abstract T BitAnd(T leftParam, T rightParam);
        protected abstract T LogicOr(T leftParam, T rightParam);
        protected abstract T LogicAnd(T leftParam, T rightParam);
    }
}
