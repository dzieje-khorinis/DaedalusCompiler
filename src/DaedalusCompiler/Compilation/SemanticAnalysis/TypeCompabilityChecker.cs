
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;


/*
 WORK IN PROGRESS
 
 Dodać do drzewka wyliczanie jakiego typu jest dane wyrażenie.
 NoneType
 */


namespace DaedalusCompiler.Compilation
{
    public class Data
    {
        public DataCategory Category;
        public SymbolType Type;
    }
    
    public enum DataCategory
    {
        Variable = 1, //Symbol
        Literal = 2,
        FunctionInvocation = 3,
        Expression = 4
    }
    public enum Compability
    {
        None = 0,
        ImplicitConversion = 1,
        ExtraCodeRequired = 2,
        IkarusRequired = 3,
        Full = 4
    }
    
    public class TypeCompabilityChecker
    {
        public Compability GetReturnCompability(SymbolType functionType, Data returned)
        {
            switch (functionType)
            {
                case SymbolType.Int:
                    if (returned.Type == SymbolType.Int)
                    {
                        return Compability.Full;
                    }

                    if (returned.Type == SymbolType.Instance && returned.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }

                    break;
                
                case SymbolType.String:
                    if (returned.Type == SymbolType.String)
                    {
                        return Compability.Full;
                    }
                    
                    break;
            }
            
            return Compability.None;
        }

        public Compability GetAssignmentCompability(SymbolType lValueType, Data rValue)
        {
            switch (lValueType)
            {
                case SymbolType.Int:
                    if (rValue.Type == SymbolType.Int)
                    {
                        return Compability.Full;
                    }
                    
                    if (rValue.Type == SymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }
                    
                    break;
                
                case SymbolType.Float:
                    
                    if (rValue.Type == SymbolType.Int && rValue.Category == DataCategory.Literal)
                    {
                        // var float x;
                        // x = 2; // 2 can be counted as IntegerLiteral or FloatLiteral
                        return Compability.Full;
                    }

                    if (rValue.Type == SymbolType.Float)
                    {
                        if (rValue.Category == DataCategory.Literal || rValue.Category == DataCategory.FunctionInvocation)
                        {
                            return Compability.Full;
                        }
                    }
                    
                    break;

                case SymbolType.String:
                    if (rValue.Type == SymbolType.String)
                    {
                        return Compability.Full;
                    }
                    
                    break;

                case SymbolType.Instance:
                    if (rValue.Type == SymbolType.Instance && rValue.Category == DataCategory.FunctionInvocation)
                    {
                        return Compability.Full;
                    }

                    break;

                case SymbolType.Func:
                    if (rValue.Type == SymbolType.Func && rValue.Category == DataCategory.Variable)
                    {
                        return Compability.Full;
                    }

                    if (rValue.Type == SymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        return Compability.Full;
                    }

                    break;
            }
            
            return Compability.None;
        }
        
        public bool IsArgumentTypeCompatible(SymbolType parameterType, Data argument)
        {
            switch (parameterType)
            {
                case SymbolType.Int:
                    if (argument.Type == SymbolType.Int || argument.Type == SymbolType.Instance)
                    {
                        // TODO think about giving warning if somebody passes instance to int argument
                        return true;
                    }
                    break;
                
                case SymbolType.Float:
                    
                    if (argument.Type == SymbolType.Int && argument.Category == DataCategory.Literal)
                    {
                        return true;
                    }

                    if (argument.Type == SymbolType.Float)
                    {
                        if (argument.Category == DataCategory.Literal || argument.Category == DataCategory.Variable)
                        {
                            return true;
                        }
                    }
                    
                    break;
                
                case SymbolType.String:
                    if (argument.Type == SymbolType.String)
                    {
                        return true;
                    }
                    
                    break;
                
                case SymbolType.Instance:
                    if (argument.Type == SymbolType.Instance && argument.Category == DataCategory.Variable)
                    {
                        return true;
                    }

                    break;
                    
                case SymbolType.Func:
                    if (argument.Type == SymbolType.Func && argument.Category == DataCategory.Variable)
                    {
                        return true;
                    }

                    if (argument.Type == SymbolType.Instance && argument.Category == DataCategory.Variable)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }
        
        public Compability GetCompoundAssignmentCompability(SymbolType lValueType, Data rValue)
        {
            switch (lValueType)
            {
                case SymbolType.Int:
                    if (rValue.Type == SymbolType.Int)
                    {
                        return Compability.Full;
                    }
                    
                    if (rValue.Type == SymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }
                    
                    break;
            }
            
            return Compability.None;
        }
        
        
        public Compability GetOperationCompability(SymbolType lValueType, Data rValue)
        {
            switch (lValueType)
            {
                case SymbolType.Int:
                case SymbolType.Instance:
                    if (rValue.Type == SymbolType.Int)
                    {
                        return Compability.Full;
                    }
                    
                    if (rValue.Type == SymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }
                    
                    break;
            }
            
            return Compability.None;
        }
        
    }
}