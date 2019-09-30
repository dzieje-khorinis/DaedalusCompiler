using System;
using System.Linq;
using DaedalusCompiler.Dat;


namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class InvalidBinaryOperationException : Exception
    {
    }

    public class InvalidUnaryOperationException : Exception
    {
    }
    
    public abstract class NodeValue
    {
    }

    public class UninitializedValue : NodeValue
    {
    }

    public class UndefinedValue : NodeValue
    {
    }
    
    public class IntValue : NodeValue
    {
        public readonly long Value;
        
        public IntValue(long value)
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

    public class FunctionValue : NodeValue
    {
        public Symbol Value;

        public FunctionValue(Symbol value)
        {
            Value = value;
        }
    }
   
    
    
    public static class ConstEvaluationHelper
    {
        public static NodeValue EvaluateUnaryOperation(UnaryOperator oper, NodeValue value)
        {
            Console.WriteLine("UnaryOperator oper, NodeValue value");
            if (value is UndefinedValue) //  || value is StringValue
            {
                return new UndefinedValue();
            }

            switch (value)
            {
                case IntValue intValue:
                    return EvaluateUnaryIntExpression(oper, intValue);
                case FloatValue floatValue:
                    return EvaluateUnaryFloatExpression(oper, floatValue);
                
                default:
                    throw new InvalidUnaryOperationException();
            }
        }
        
        public static IntValue EvaluateUnaryIntExpression(UnaryOperator oper, IntValue intValue)
        {
            Console.WriteLine("UnaryOperator oper, IntValue intValue");
            switch (oper)
            {
                case UnaryOperator.Minus:
                    return new IntValue(checked(-intValue.Value));
                case UnaryOperator.Plus:
                    return intValue;
                case UnaryOperator.Not:
                    return new IntValue(intValue.Value == 0 ? 1 : 0);
                case UnaryOperator.Negate:
                    return new IntValue(~intValue.Value);
                
                default:
                    throw new Exception();
            }
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
                
                default:
                    throw new InvalidUnaryOperationException();
            }
        }
        
        
        
        public static NodeValue EvaluateBinaryOperation(BinaryOperator oper, NodeValue left, NodeValue right)
        {
            Console.WriteLine("BinaryOperator oper, NodeValue leftValue, NodeValue rightValue");


            if (left is UndefinedValue || right is UndefinedValue)
            {
                return new UndefinedValue();
            }

            NodeValue resultValue;
 
            switch (left)
            {
                case IntValue leftIntValue:

                    switch (right)
                    {
                        case IntValue rightIntValue:
                            resultValue = EvaluateBinaryOperation(oper, leftIntValue, rightIntValue);
                            break;
                        case FloatValue rightFloatValue:
                            resultValue = EvaluateBinaryOperation(oper, new FloatValue(leftIntValue.Value), rightFloatValue);
                            break;
                        case StringValue rightStringValue:
                            resultValue = EvaluateBinaryOperation(oper, leftIntValue, rightStringValue);
                            break;
                        default:
                            throw new Exception();
                    }

                    break;


                case FloatValue leftFloatValue:

                    switch (right)
                    {
                        case IntValue rightIntValue:
                            resultValue = EvaluateBinaryOperation(oper, leftFloatValue, new FloatValue(rightIntValue.Value));
                            break;
                        case FloatValue rightFloatValue:
                            resultValue = EvaluateBinaryOperation(oper, leftFloatValue, rightFloatValue);
                            break;
                        case StringValue rightStringValue:
                            throw new InvalidBinaryOperationException();
                            //resultValue = EvaluateBinaryOperation(oper, leftFloatValue, rightStringValue);
                            break;
                        default:
                            throw new Exception();
                    }

                    break;


                case StringValue leftStringValue:

                    switch (right)
                    {
                        case IntValue rightIntValue:
                            resultValue = EvaluateBinaryOperation(oper, leftStringValue, rightIntValue);
                            break;
                        case FloatValue rightFloatValue:
                            throw new InvalidBinaryOperationException();
                            //resultValue = EvaluateBinaryOperation(oper, leftStringValue, rightFloatValue);
                            break;
                        case StringValue rightStringValue:
                            resultValue = EvaluateBinaryOperation(oper, leftStringValue, rightStringValue);
                            break;
                        default:
                            throw new Exception();
                    }

                    break;
                    
                default:
                    throw new Exception();
            }


            
            return resultValue;
        }

        private static NodeValue EvaluateBinaryOperation(BinaryOperator oper, StringValue left, IntValue right)
        {
            Console.WriteLine("BinaryOperator oper, StringValue left, IntValue right");
            switch (oper)
            {
                case BinaryOperator.Mult:
                    return new StringValue(string.Concat(Enumerable.Repeat(left.Value, (int)right.Value)));
                default:
                    throw new InvalidBinaryOperationException();
            }
        }

        private static NodeValue EvaluateBinaryOperation(BinaryOperator oper, IntValue left, StringValue right)
        {
            Console.WriteLine("BinaryOperator oper, IntValue left, StringValue right");
            switch (oper)
            {
                case BinaryOperator.Mult:
                    return new StringValue(string.Concat(Enumerable.Repeat(right.Value, (int)left.Value)));
                default:
                    throw new InvalidBinaryOperationException();
            }
        }
        

        public static StringValue EvaluateBinaryOperation(BinaryOperator oper, StringValue left, StringValue right)
        {
            Console.WriteLine("BinaryOperator oper, StringValue left, StringValue right");
            switch (oper)
            {
                case BinaryOperator.Add:
                    return new StringValue(left.Value + right.Value);
                default:
                    throw new InvalidBinaryOperationException();
            }
        }
        

        public static IntValue EvaluateBinaryOperation(BinaryOperator oper, IntValue left, IntValue right)
        {
            Console.WriteLine("BinaryOperator oper, IntValue left, IntValue right");
            switch (oper)
            {
                case BinaryOperator.Mult:
                    return new IntValue(checked(left.Value * right.Value));
                case BinaryOperator.Div:
                    return new IntValue(left.Value / right.Value);
                case BinaryOperator.Modulo:
                    return new IntValue(left.Value % right.Value);
                
                case BinaryOperator.Add:
                    return new IntValue(checked(left.Value + right.Value));
                case BinaryOperator.Sub:
                    return new IntValue(checked(left.Value - right.Value));
                
                case BinaryOperator.ShiftLeft:
                    return new IntValue(checked(left.Value << (int)right.Value));
                case BinaryOperator.ShiftRight:
                    return new IntValue(checked(left.Value << (int)right.Value));
                
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
                    return new IntValue(left.Value != 0 && right.Value != 0 ? 1 : 0);
                
                case BinaryOperator.LogOr:
                    return new IntValue(left.Value != 0 || right.Value != 0 ? 1 : 0);

                default:
                    throw new Exception();
            }
        }

        public static FloatValue EvaluateBinaryOperation(BinaryOperator oper, FloatValue left, FloatValue right)
        {
            Console.WriteLine("BinaryOperator oper, FloatValue left, FloatValue right");
            switch (oper)
            {
                case BinaryOperator.Mult:
                    return new FloatValue(left.Value * right.Value);
                case BinaryOperator.Div:
                    if (right.Value == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    return new FloatValue(left.Value / right.Value);
                case BinaryOperator.Modulo:
                    if (right.Value == 0)
                    {
                        throw new DivideByZeroException();
                    }
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
                
                default:
                    throw new Exception();
            }
        }
    }
}