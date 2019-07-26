using System;
using System.Diagnostics;
using System.Linq;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class UnableToEvaluateException : Exception
    {
        
    }
    
    public abstract class NodeValue
    {
        
    }

    public class IntValue : NodeValue
    {
        public int Value;
        
        public IntValue(int value)
        {
            Value = value;
        }
    }
    
    public class FloatValue : NodeValue
    {
        public float Value;
        
        public FloatValue(float value)
        {
            Value = value;
        }

        public FloatValue(double value)
        {
            Value = (float) value;
        }
    }

    public class StringValue : NodeValue
    {
        public string Value;
        
        public StringValue(string value)
        {
            Value = value;
        }
    }

    public abstract class ErrorValue : NodeValue
    {
        
    }

    public class UndeclaredIdentifierValue : ErrorValue
    {
        
    }
    
    public class UndefinedValue : ErrorValue
    {
        
    }

    public class InfiniteReferenceLoopValue : ErrorValue
    {
        
    }
    
    
    public class UnableToEvaluateValue : ErrorValue
    {
        
    }
    
    
    public class EvaluationHelper
    {
        public static NodeValue EvaluateUnaryOperation(UnaryOperator oper, NodeValue value)
        {
            Console.WriteLine("UnaryOperator oper, ConstValue value");
            if (value is UndefinedValue || value is StringValue)
            {
                return new UndefinedValue();
            }

            switch (value)
            {
                case IntValue intValue:
                    return EvaluateUnaryIntExpression(oper, intValue);
                case FloatValue floatValue:
                    return EvaluateUnaryFloatExpression(oper, floatValue);
            }
            
            throw new UnableToEvaluateException();
        }
        
        public static IntValue EvaluateUnaryIntExpression(UnaryOperator oper, IntValue intValue)
        {
            Console.WriteLine("UnaryOperator oper, IntValue intValue");
            switch (oper)
            {
                case UnaryOperator.Minus:
                    return new IntValue(-intValue.Value);
                case UnaryOperator.Plus:
                    return intValue;
                case UnaryOperator.Not:
                    return new IntValue(intValue.Value == 0 ? 1 : 0);
                case UnaryOperator.Negate:
                    return new IntValue(~intValue.Value);
            }

            throw new UnableToEvaluateException();
        }
        
        public static FloatValue EvaluateUnaryFloatExpression(UnaryOperator oper, FloatValue floatValue)
        {
            Console.WriteLine("UnaryOperator oper, FloatValue floatValue");
            switch (oper)
            {
                case UnaryOperator.Minus:
                    return new FloatValue(-floatValue.Value);
                case UnaryOperator.Plus:
                    return floatValue;
                case UnaryOperator.Not:
                    return new FloatValue(floatValue.Value == 0 ? 1 : 0);
            }

            throw new UnableToEvaluateException();
        }
        
        
        
        public static NodeValue EvaluateBinaryOperation(BinaryOperator oper, NodeValue left, NodeValue right)
        {
            Console.WriteLine("BinaryOperator oper, ConstValue leftValue, ConstValue rightValue");

            if (left is UndefinedValue || right is UndefinedValue)
            {
                return new UndefinedValue();
            }

            try
            {
                switch (left)
                {
                    case IntValue leftIntValue:

                        switch (right)
                        {
                            case IntValue rightIntValue:
                                return EvaluateBinaryOperation(oper, leftIntValue, rightIntValue);
                            case FloatValue rightFloatValue:
                                return EvaluateBinaryOperation(oper, leftIntValue, rightFloatValue);
                            case StringValue rightStringValue:
                                return EvaluateBinaryOperation(oper, leftIntValue, rightStringValue);
                        }

                        break;

                    case FloatValue leftFloatValue:

                        switch (right)
                        {
                            case IntValue rightIntValue:
                                return EvaluateBinaryOperation(oper, leftFloatValue, rightIntValue);
                            case FloatValue rightFloatValue:
                                return EvaluateBinaryOperation(oper, leftFloatValue, rightFloatValue);
                            case StringValue rightStringValue:
                                return EvaluateBinaryOperation(oper, leftFloatValue, rightStringValue);
                        }

                        break;

                    case StringValue leftStringValue:

                        switch (right)
                        {
                            case IntValue rightIntValue:
                                return EvaluateBinaryOperation(oper, leftStringValue, rightIntValue);
                            case FloatValue rightFloatValue:
                                return EvaluateBinaryOperation(oper, leftStringValue, rightFloatValue);
                            case StringValue rightStringValue:
                                return EvaluateBinaryOperation(oper, leftStringValue, rightStringValue);
                        }

                        break;
                }
            }
            catch (UnableToEvaluateException)
            {
                
            }

            return new UndefinedValue();
        }
        
        public static StringValue EvaluateBinaryOperation(BinaryOperator oper, StringValue left, IntValue right)
        {
            Console.WriteLine("BinaryOperator oper, StringValue left, IntValue right");
            return EvaluateBinaryOperation(oper, right, left);
        }
        
        public static StringValue EvaluateBinaryOperation(BinaryOperator oper, IntValue left, StringValue right)
        {
            Console.WriteLine("BinaryOperator oper, IntValue left, StringValue right");
            switch (oper)
            {
                case BinaryOperator.Mult:
                    return new StringValue(string.Concat(Enumerable.Repeat(right.Value, left.Value)));string.Concat(Enumerable.Repeat(right.Value, left.Value));
            }
            throw new UnableToEvaluateException();
        }
        

        public static StringValue EvaluateBinaryOperation(BinaryOperator oper, StringValue left, StringValue right)
        {
            Console.WriteLine("BinaryOperator oper, StringValue left, StringValue right");
            switch (oper)
            {
                case BinaryOperator.Add:
                    return new StringValue(left.Value + right.Value);
            }
            throw new UnableToEvaluateException();
        }
        

        public static IntValue EvaluateBinaryOperation(BinaryOperator oper, IntValue left, IntValue right)
        {
            Console.WriteLine("BinaryOperator oper, IntValue left, IntValue right");
            switch (oper)
            {
                case BinaryOperator.Mult:
                    return new IntValue(left.Value * right.Value);
                case BinaryOperator.Div:
                    return new IntValue(left.Value / right.Value);
                case BinaryOperator.Modulo:
                    return new IntValue(left.Value % right.Value);
                
                case BinaryOperator.Add:
                    return new IntValue(left.Value + right.Value);
                case BinaryOperator.Sub:
                    return new IntValue(left.Value - right.Value);
                
                case BinaryOperator.ShiftLeft:
                    return new IntValue(left.Value << right.Value);
                case BinaryOperator.ShiftRight:
                    return new IntValue(left.Value << right.Value);
                
                case BinaryOperator.Less:
                    return new IntValue(left.Value < right.Value ? 1 : 0);
                case BinaryOperator.Greater:
                    return new IntValue(left.Value > right.Value ? 1 : 0);
                case BinaryOperator.LessOrEqual:
                    return new IntValue(left.Value <= right.Value ? 1 : 0);
                case BinaryOperator.GreaterOrEqual:
                    return new IntValue(left.Value >= right.Value ? 1 : 0);
                
                case BinaryOperator.Equal:
                    return new IntValue(left.Value == right.Value ? 1 : 0);
                case BinaryOperator.NotEqual:
                    return new IntValue(left.Value != right.Value ? 1 : 0);
                
                case BinaryOperator.BinAnd:
                    return new IntValue(left.Value & right.Value);
                
                case BinaryOperator.BinOr:
                    return new IntValue(left.Value | right.Value);
                
                case BinaryOperator.LogAnd:
                    return new IntValue((left.Value != 0 && right.Value != 0) ? 1 : 0);
                
                case BinaryOperator.LogOr:
                    return new IntValue((left.Value != 0 || right.Value != 0) ? 1 : 0);
            }
            throw new UnableToEvaluateException();
        }

        public static FloatValue EvaluateBinaryOperation(BinaryOperator oper, FloatValue left, FloatValue right)
        {
            Console.WriteLine("BinaryOperator oper, FloatValue left, FloatValue right");
            switch (oper)
            {
                case BinaryOperator.Mult:
                    return new FloatValue(left.Value * right.Value);
                case BinaryOperator.Div:
                    return new FloatValue(left.Value / right.Value);
                case BinaryOperator.Modulo:
                    return new FloatValue(left.Value % right.Value);
                
                case BinaryOperator.Add:
                    return new FloatValue(left.Value + right.Value);
                case BinaryOperator.Sub:
                    return new FloatValue(left.Value - right.Value);

                case BinaryOperator.Less:
                    return new FloatValue(left.Value < right.Value ? 1 : 0);
                case BinaryOperator.Greater:
                    return new FloatValue(left.Value > right.Value ? 1 : 0);
                case BinaryOperator.LessOrEqual:
                    return new FloatValue(left.Value <= right.Value ? 1 : 0);
                case BinaryOperator.GreaterOrEqual:
                    return new FloatValue(left.Value >= right.Value ? 1 : 0);
                
                case BinaryOperator.Equal:
                    return new FloatValue(left.Value == right.Value ? 1 : 0);
                case BinaryOperator.NotEqual:
                    return new FloatValue(left.Value != right.Value ? 1 : 0);

                case BinaryOperator.LogAnd:
                    return new FloatValue((left.Value != 0 && right.Value != 0) ? 1 : 0);
                
                case BinaryOperator.LogOr:
                    return new FloatValue((left.Value != 0 || right.Value != 0) ? 1 : 0);
            }
            throw new UnableToEvaluateException();
        }
    }
}