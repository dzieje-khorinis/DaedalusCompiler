
using DaedalusCompiler.Dat;


/*
 *
 Dodać do drzewka wyliczanie jakiego typu jest dane wyrażenie.
 
 NoneType
 */


namespace DaedalusCompiler.Compilation
{
    public class Data
    {
        public DataCategory Category;
        public DatSymbolType Type;
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
        public Compability GetReturnCompability(DatSymbolType functionType, Data returned)
        {
            switch (functionType)
            {
                case DatSymbolType.Int:
                    if (returned.Type == DatSymbolType.Int)
                    {
                        return Compability.Full;
                    }

                    if (returned.Type == DatSymbolType.Instance && returned.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }

                    break;
                
                case DatSymbolType.String:
                    if (returned.Type == DatSymbolType.String)
                    {
                        return Compability.Full;
                    }
                    
                    break;
            }
            
            return Compability.None;
        }

        public Compability GetAssignmentCompability(DatSymbolType lValueType, Data rValue)
        {
            switch (lValueType)
            {
                case DatSymbolType.Int:
                    if (rValue.Type == DatSymbolType.Int)
                    {
                        return Compability.Full;
                    }
                    
                    if (rValue.Type == DatSymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }
                    
                    break;
                
                case DatSymbolType.Float:
                    
                    if (rValue.Type == DatSymbolType.Int && rValue.Category == DataCategory.Literal)
                    {
                        // var float x;
                        // x = 2; // 2 can be counted as IntegerLiteral or FloatLiteral
                        return Compability.Full;
                    }

                    if (rValue.Type == DatSymbolType.Float)
                    {
                        if (rValue.Category == DataCategory.Literal || rValue.Category == DataCategory.FunctionInvocation)
                        {
                            return Compability.Full;
                        }
                    }
                    
                    break;

                case DatSymbolType.String:
                    if (rValue.Type == DatSymbolType.String)
                    {
                        return Compability.Full;
                    }
                    
                    break;

                case DatSymbolType.Instance:
                    if (rValue.Type == DatSymbolType.Instance && rValue.Category == DataCategory.FunctionInvocation)
                    {
                        return Compability.Full;
                    }

                    break;

                case DatSymbolType.Func:
                    if (rValue.Type == DatSymbolType.Func && rValue.Category == DataCategory.Variable)
                    {
                        return Compability.Full;
                    }

                    if (rValue.Type == DatSymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        return Compability.Full;
                    }

                    break;
            }
            
            return Compability.None;
        }
        
        public Compability GetArgumentCompability(DatSymbolType parameterType, Data argument)
        {
            switch (parameterType)
            {
                case DatSymbolType.Int:
                    if (argument.Type == DatSymbolType.Int)
                    {
                        return Compability.Full;
                    }

                    if (argument.Type == DatSymbolType.Instance && argument.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }
                    break;
                
                case DatSymbolType.Float:
                    
                    if (argument.Type == DatSymbolType.Int && argument.Category == DataCategory.Literal)
                    {
                        // var float x;
                        // x = 2; // 2 can be counted as IntegerLiteral or FloatLiteral
                        return Compability.Full;
                    }

                    if (argument.Type == DatSymbolType.Float)
                    {
                        if (argument.Category == DataCategory.Literal || argument.Category == DataCategory.Variable)
                        {
                            return Compability.Full;
                        }
                    }
                    
                    break;
                
                case DatSymbolType.String:
                    if (argument.Type == DatSymbolType.String)
                    {
                        return Compability.Full;
                    }
                    
                    break;
                
                case DatSymbolType.Instance:
                    if (argument.Type == DatSymbolType.Instance && argument.Category == DataCategory.Variable)
                    {
                        return Compability.Full;
                    }

                    break;
                    
                case DatSymbolType.Func:
                    if (argument.Type == DatSymbolType.Func && argument.Category == DataCategory.Variable)
                    {
                        return Compability.Full;
                    }

                    if (argument.Type == DatSymbolType.Instance && argument.Category == DataCategory.Variable)
                    {
                        return Compability.Full;
                    }

                    break;
            }
            
            return Compability.None;
        }
        
        public Compability GetCompoundAssignmentCompability(DatSymbolType lValueType, Data rValue)
        {
            switch (lValueType)
            {
                case DatSymbolType.Int:
                    if (rValue.Type == DatSymbolType.Int)
                    {
                        return Compability.Full;
                    }
                    
                    if (rValue.Type == DatSymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }
                    
                    break;
            }
            
            return Compability.None;
        }
        
        
        public Compability GetOperationCompability(DatSymbolType lValueType, Data rValue)
        {
            switch (lValueType)
            {
                case DatSymbolType.Int:
                case DatSymbolType.Instance:
                    if (rValue.Type == DatSymbolType.Int)
                    {
                        return Compability.Full;
                    }
                    
                    if (rValue.Type == DatSymbolType.Instance && rValue.Category == DataCategory.Variable)
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