using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class DaedalusListener : DaedalusBaseListener
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly int _sourceFileNumber;

        public DaedalusListener(AssemblyBuilder assemblyBuilder, int sourceFileNumber)
        {
            _assemblyBuilder = assemblyBuilder;
            _sourceFileNumber = sourceFileNumber;
        }

        public override void ExitParameterList(DaedalusParser.ParameterListContext context)
        {
            var parameterDeclContexts = context.parameterDecl();
            foreach (var parameterDeclContext in parameterDeclContexts.Reverse())
            {
                BaseExecBlockContext baseExecBlock = _assemblyBuilder.ExecBlocks.Last();
                string execBlockName = baseExecBlock.GetSymbol().Name;
                string parameterLocalName = parameterDeclContext.nameNode().GetText();
                string parameterName = $"{execBlockName}.{parameterLocalName}";
                DatSymbol parameterSymbol = _assemblyBuilder.ResolveSymbol(parameterName);

                var assignInstruction =
                    AssemblyBuilderHelpers.GetAssignInstructionForDatSymbolType(parameterSymbol.Type);

                if (parameterSymbol.Type is DatSymbolType.Instance)
                {
                    _assemblyBuilder.AddInstruction(new PushInstance(parameterSymbol));
                }
                else
                {
                    _assemblyBuilder.AddInstruction(new PushVar(parameterSymbol));
                }

                _assemblyBuilder.AddInstruction(assignInstruction);
            }
        }

        public override void EnterParameterDecl(DaedalusParser.ParameterDeclContext context)
        {
            BaseExecBlockContext baseExecBlock = _assemblyBuilder.ExecBlocks.Last();
            string execBlockName = baseExecBlock.GetSymbol().Name;
            string parameterLocalName = context.nameNode().GetText();
            string parameterName = $"{execBlockName}.{parameterLocalName}";

            int parentIndex = DatSymbol.NULL_INDEX;

            string parameterTypeName = context.typeReference().GetText();
            DatSymbolType? parameterType = DatSymbolTypeFromString(parameterTypeName);     
            if (parameterType is DatSymbolType.Class)
            {
                parameterType = DatSymbolType.Instance;
                var parentSymbol = _assemblyBuilder.ResolveSymbol(parameterTypeName);
                parentIndex = parentSymbol.Index;
            }

            DatSymbol symbol;
            var location = GetLocation(context);

            var arraySizeContext = context.arraySize();

            if (arraySizeContext != null)
            {
                if (!uint.TryParse(arraySizeContext.GetText(), out var arrIndex))
                {
                    var constSymbol = _assemblyBuilder.ResolveSymbol(arraySizeContext.GetText());
                    if (constSymbol.Flags != DatSymbolFlag.Const || constSymbol.Type != DatSymbolType.Int)
                    {
                        throw new Exception($"Expected integer constant: {arraySizeContext.GetText()}");
                    }

                    arrIndex = (uint) (int) constSymbol.Content[0];
                }

                symbol = SymbolBuilder.BuildArrOfVariables(parameterName, parameterType.Value, arrIndex, location);
            }
            else
            {
                if (_assemblyBuilder.IsCurrentlyParsingExternals)
                {
                    symbol = SymbolBuilder.BuildExternalParameter(parameterName, parameterType.Value, location, parentIndex);
                }
                else
                {
                    symbol = SymbolBuilder.BuildParameter(parameterName, parameterType.Value, location, parentIndex);
                }
            }

            _assemblyBuilder.AddSymbol(symbol);
        }

        public override void EnterConstDef([NotNull] DaedalusParser.ConstDefContext context)
        {
            _assemblyBuilder.IsInsideConstDef = true;
            var typeName = context.typeReference().GetText();
            var type = DatSymbolTypeFromString(typeName);
            if (type == DatSymbolType.Func)
            {
                type = DatSymbolType.Int;
            }
            
            for (int i = 0; i < context.ChildCount; i++)
            {
                var constContext = context.GetChild(i);

                if (constContext is TerminalNodeImpl)
                    continue; // skips ',' 

                if (constContext is DaedalusParser.ConstValueDefContext constValueContext)
                {
                    var name = constValueContext.nameNode().GetText();
                    if (_assemblyBuilder.IsContextInsideExecBlock())
                    {
                        BaseExecBlockContext baseExecBlock = _assemblyBuilder.ExecBlocks.Last();
                        string execBlockName = baseExecBlock.GetSymbol().Name;
                        name = $"{execBlockName}.{name}";
                    }

                    var location = GetLocation(context);
                    var assignmentExpression = constValueContext.constValueAssignment().expressionBlock().expression();
                    var value = EvaluatorHelper.EvaluateConst(assignmentExpression, _assemblyBuilder, type);

                    var symbol = SymbolBuilder.BuildConst(name, type, value, location); // TODO : Validate params
                    _assemblyBuilder.AddSymbol(symbol);

                    continue;
                }

                if (constContext is DaedalusParser.ConstArrayDefContext constArrayContext)
                {
                    _assemblyBuilder.ErrorContext.Context = constArrayContext;
                    
                    var name = constArrayContext.nameNode().GetText();
                    var location = GetLocation(context);

                    int declaredSize = 0;
                    bool compareDeclaredSizeAndElementsCount = true;
                    
                    try
                    {
                        _assemblyBuilder.ErrorContext.Context = constArrayContext.arraySize();
                        declaredSize = EvaluatorHelper.EvaluteArraySize(constArrayContext.arraySize(), _assemblyBuilder);
                    }
                    catch (System.ArgumentNullException)
                    {
                        compareDeclaredSizeAndElementsCount = false;
                    }

                    if (declaredSize == 0 && compareDeclaredSizeAndElementsCount)
                    {
                        compareDeclaredSizeAndElementsCount = false;
                        _assemblyBuilder.Errors.Add(
                            new InvalidArraySizeError(
                                _assemblyBuilder.ErrorContext,
                                name,
                                declaredSize
                            )
                        );
                    }
                    
                    
                    var elements = constArrayContext.constArrayAssignment().expressionBlock()
                        .Select(expr => EvaluatorHelper.EvaluateConst(expr.expression(), _assemblyBuilder, type))
                        .ToArray();

                    
                    if (compareDeclaredSizeAndElementsCount && declaredSize != elements.Length)
                    {   
                        _assemblyBuilder.Errors.Add(
                            new InconsistentArraySizeError(
                                _assemblyBuilder.ErrorContext,
                                name,
                                declaredSize,
                                elements.Length
                            )
                        );
                    }
                    
                    var symbol =
                        SymbolBuilder.BuildArrOfConst(name, type, elements, location); // TODO : Validate params
                    _assemblyBuilder.AddSymbol(symbol);
                }
            }
        }

        public override void ExitConstDef([NotNull] DaedalusParser.ConstDefContext context)
        {
            _assemblyBuilder.IsInsideConstDef = false;
        }

        public override void EnterVarDecl([NotNull] DaedalusParser.VarDeclContext context)
        {
            if (context.Parent.Parent is DaedalusParser.DaedalusFileContext || _assemblyBuilder.IsContextInsideExecBlock())
            {
                var typeName = context.typeReference().GetText();
                var type = DatSymbolTypeFromString(typeName);
                if (type == DatSymbolType.Class)
                {
                    type = DatSymbolType.Instance;
                }
                
                for (int i = 0; i < context.ChildCount; i++)
                {
                    var varContext = context.GetChild(i);

                    if (varContext is TerminalNodeImpl)
                        continue; // skips ',' 

                    if (varContext is DaedalusParser.VarValueDeclContext varValueContext)
                    {
                        var name = varValueContext.nameNode().GetText();
                        if (_assemblyBuilder.IsContextInsideExecBlock())
                        {
                            // TODO consider making assemblyBuilder.active public and using it here
                            BaseExecBlockContext baseExecBlock = _assemblyBuilder.ExecBlocks.Last();
                            string execBlockName = baseExecBlock.GetSymbol().Name;
                            name = $"{execBlockName}.{name}";
                        }

                        var location = GetLocation(context);

                        int parentIndex = DatSymbol.NULL_INDEX;
                        string parameterTypeName = context.typeReference().GetText();
                        DatSymbolType? parameterType = DatSymbolTypeFromString(parameterTypeName);
                        if (parameterType is DatSymbolType.Class)
                        {
                            var parentSymbol = _assemblyBuilder.ResolveSymbol(parameterTypeName);
                            parentIndex = parentSymbol.Index;
                        }

                        var symbol = SymbolBuilder.BuildVariable(name, type, location, parentIndex); // TODO : Validate params
                        _assemblyBuilder.AddSymbol(symbol);
                    }

                    if (varContext is DaedalusParser.VarArrayDeclContext varArrayContext)
                    {
                        var name = varArrayContext.nameNode().GetText();
                        var location = GetLocation(context);
                        var size = EvaluatorHelper.EvaluteArraySize(varArrayContext.arraySize(), _assemblyBuilder);

                        var symbol =
                            SymbolBuilder.BuildArrOfVariables(name, type, (uint) size,
                                location); // TODO : Validate params
                        _assemblyBuilder.AddSymbol(symbol);
                    }
                }
            }
        }

        public override void EnterClassDef([NotNull] DaedalusParser.ClassDefContext context)
        {
            var className = context.nameNode().GetText();
            var classSymbol = SymbolBuilder.BuildClass(className, 0, 0, GetLocation(context));
            _assemblyBuilder.AddSymbol(classSymbol);

            var classId = classSymbol.Index;
            int classVarOffset = classSymbol.ClassOffset;
            uint classLength = 0;

            // TODO: refactor later
            foreach (var varDeclContext in context.varDecl())
            {
                var typeName = varDeclContext.typeReference().GetText();
                var type = DatSymbolTypeFromString(typeName);

                for (int i = 0; i < varDeclContext.ChildCount; i++)
                {
                    var varContext = varDeclContext.GetChild(i);

                    if (varContext is TerminalNodeImpl)
                        continue; // skips ',' 

                    if (varContext is DaedalusParser.VarValueDeclContext varValueContext)
                    {
                        var name = varValueContext.nameNode().GetText();
                        var location = GetLocation(context);

                        var symbol = SymbolBuilder.BuildClassVar(name, type, 1, className, classId,
                            classVarOffset, location); // TODO : Validate params
                        _assemblyBuilder.AddSymbol(symbol);

                        classVarOffset += (type == DatSymbolType.String ? 20 : 4);
                        classLength++;
                    }

                    if (varContext is DaedalusParser.VarArrayDeclContext varArrayContext)
                    {
                        var name = varArrayContext.nameNode().GetText();
                        var location = GetLocation(context);
                        var size = EvaluatorHelper.EvaluteArraySize(varArrayContext.arraySize(), _assemblyBuilder);

                        var symbol = SymbolBuilder.BuildClassVar(name, type, (uint) size, className, classId,
                            classVarOffset, location); // TODO : Validate params
                        _assemblyBuilder.AddSymbol(symbol);

                        classVarOffset += (type == DatSymbolType.String ? 20 : 4) * size;
                        classLength++;
                    }
                }
            }

            classSymbol.ArrayLength = classLength;
            classSymbol.ClassSize = classVarOffset - classSymbol.ClassOffset;
        }

        public override void EnterPrototypeDef([NotNull] DaedalusParser.PrototypeDefContext context)
        {
            _assemblyBuilder.ErrorContext.Context = context.parentReference();

            var prototypeName = context.nameNode().GetText();
            var referenceName = context.parentReference().GetText();
            var refSymbol = _assemblyBuilder.GetSymbolByName(referenceName);
            var referenceSymbolId = refSymbol.Index;
            var location = GetLocation(context);

            var prototypeSymbol = SymbolBuilder.BuildPrototype(prototypeName, referenceSymbolId, location);
            _assemblyBuilder.AddSymbol(prototypeSymbol);

            _assemblyBuilder.ExecBlockStart(prototypeSymbol, ExecBlockType.Prototype);
        }

        public override void ExitPrototypeDef(DaedalusParser.PrototypeDefContext context)
        {
            _assemblyBuilder.AddInstruction(new Ret());
            _assemblyBuilder.ExecBlockEnd();
        }

        public override void EnterInstanceDef(DaedalusParser.InstanceDefContext context)
        {
            _assemblyBuilder.ErrorContext.Context = context.parentReference();

            var instanceName = context.nameNode().GetText();
            var referenceName = context.parentReference().GetText();
            var refSymbol = _assemblyBuilder.GetSymbolByName(referenceName);
            var referenceSymbolId = refSymbol.Index;
            var location = GetLocation(context);

            var instanceSymbol = SymbolBuilder.BuildInstance(instanceName, referenceSymbolId, location);
            instanceSymbol.Flags |= DatSymbolFlag.Const;
            _assemblyBuilder.AddSymbol(instanceSymbol);

            _assemblyBuilder.ExecBlockStart(instanceSymbol, ExecBlockType.Instance);

            if (refSymbol.Type == DatSymbolType.Prototype)
            {
                _assemblyBuilder.AddInstruction(new Call(refSymbol));
            }
        }
     
        public override void ExitInstanceDef(DaedalusParser.InstanceDefContext context)
        {
            _assemblyBuilder.AddInstruction(new Ret());
            _assemblyBuilder.ExecBlockEnd();
        }
        
        public override void EnterInstanceDecl(DaedalusParser.InstanceDeclContext context)
        {
            _assemblyBuilder.ErrorContext.Context = context.parentReference();

            var referenceName = context.parentReference().GetText();
            var refSymbol = _assemblyBuilder.GetSymbolByName(referenceName);
            var referenceSymbolId = refSymbol.Index;
            var location = GetLocation(context);

            List<DatSymbol> symbols = new List<DatSymbol>();
            
            for (int i = 0; i < context.nameNode().Length; ++i)
            {
                string instanceName = context.nameNode()[i].GetText();
                DatSymbol instanceSymbol = SymbolBuilder.BuildInstance(instanceName, referenceSymbolId, location);
                _assemblyBuilder.AddSymbol(instanceSymbol);
                symbols.Add(instanceSymbol);
            }
            
            _assemblyBuilder.SharedBlockStart(symbols);
            _assemblyBuilder.AddInstruction(new Ret());
            _assemblyBuilder.ExecBlockEnd();
        }
        
        

        public override void EnterFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            string funcName = context.nameNode().GetText();
            string returnTypeName = context.typeReference().GetText();
            DatSymbolType returnType = DatSymbolTypeFromString(returnTypeName);
            uint parametersCount = (uint)context.parameterList().parameterDecl().Length;

            var symbol = SymbolBuilder.BuildFunc(funcName, parametersCount, returnType);
            _assemblyBuilder.AddSymbol(symbol);
            _assemblyBuilder.ExecBlockStart(symbol, ExecBlockType.Function);
        }

        public override void ExitFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            _assemblyBuilder.ActiveExecBlock.GetSymbol().Location = GetLocation(context);
            _assemblyBuilder.AddInstruction(new Ret());
            _assemblyBuilder.ExecBlockEnd();
        }

        public override void EnterReturnStatement([NotNull] DaedalusParser.ReturnStatementContext context)
        {
            _assemblyBuilder.IsInsideReturnStatement = true;
        }
        
        public override void ExitReturnStatement([NotNull] DaedalusParser.ReturnStatementContext context)
        {
            _assemblyBuilder.AddInstruction(new Ret());
            _assemblyBuilder.IsInsideReturnStatement = false;
        }

        
        
        
        
        
        public override void EnterIfBlockStatement(DaedalusParser.IfBlockStatementContext context)
        {
            _assemblyBuilder.IfBlockStatementStart();
        }

        public override void ExitIfBlockStatement(DaedalusParser.IfBlockStatementContext context)
        {
            _assemblyBuilder.IfBlockStatementEnd();
        }


        
        public override void EnterIfBlock(DaedalusParser.IfBlockContext context)
        {
            _assemblyBuilder.IfBlockStart();
        }

        public override void ExitIfBlock(DaedalusParser.IfBlockContext context)
        {
            _assemblyBuilder.IfBlockEnd();
        }

        
        public override void EnterElseIfBlock(DaedalusParser.ElseIfBlockContext context)
        {
            _assemblyBuilder.ElseIfBlockStart();
        }

        public override void ExitElseIfBlock(DaedalusParser.ElseIfBlockContext context)
        {
            _assemblyBuilder.ElseIfBlockEnd();
        }

        
        public override void EnterElseBlock(DaedalusParser.ElseBlockContext context)
        {
            _assemblyBuilder.ElseBlockStart();
        }

        public override void ExitElseBlock(DaedalusParser.ElseBlockContext context)
        {
            _assemblyBuilder.ElseBlockEnd();
        }

        
        
        public override void EnterIfCondition(DaedalusParser.IfConditionContext context)
        {
            _assemblyBuilder.IsInsideIfCondition = true;
        }
        
        public override void ExitIfCondition(DaedalusParser.IfConditionContext context)
        {
            _assemblyBuilder.IsInsideIfCondition = false;
        }

        public override void ExitNullLiteralValue(DaedalusParser.NullLiteralValueContext context)
        {
            List<AssemblyElement> instructions = _assemblyBuilder.GetKeywordInstructions(context.GetText().ToLower());
            _assemblyBuilder.AddInstructions(instructions);
        }

        public override void ExitNoFuncLiteralValue(DaedalusParser.NoFuncLiteralValueContext context)
        {
            List<AssemblyElement> instructions = _assemblyBuilder.GetKeywordInstructions(context.GetText().ToLower());
            _assemblyBuilder.AddInstructions(instructions);
        }

        public override void ExitReference(DaedalusParser.ReferenceContext referenceContext)
        {
            if (referenceContext.Parent is DaedalusParser.AssignmentContext) {
                // left side of assignment
                return;
            }

            List<AssemblyElement> instructions = new List<AssemblyElement>();
            if (_assemblyBuilder.IsInsideArgList || _assemblyBuilder.IsInsideAssignment || _assemblyBuilder.IsInsideIfCondition || _assemblyBuilder.IsInsideReturnStatement)
            {
                instructions.Add(new LazyReferenceAtomInstructions(_assemblyBuilder.MakeSnapshot(), referenceContext));
            }
            else
            {
                instructions = _assemblyBuilder.GetDatSymbolReferenceInstructions(referenceContext);
            }
            
            if (!_assemblyBuilder.IsInsideConstDef)
            {
                _assemblyBuilder.AddInstructions(instructions);   
            }
        }
        

        public override void EnterAssignment(DaedalusParser.AssignmentContext context)
        {
            var referenceContext = context.reference();
            DatSymbolReference reference = _assemblyBuilder.GetDatSymbolReference(referenceContext);
            List<AssemblyElement> instructions = _assemblyBuilder.GetDatSymbolReferenceInstructions(reference);
            _assemblyBuilder.AssignmentStart(Array.ConvertAll(instructions.ToArray(), item => (SymbolInstruction) item));
            _assemblyBuilder.IsInsideAssignment = true;
            _assemblyBuilder.AssignmentType = reference.GetSymbolType();
        }


        public override void ExitAssignment(DaedalusParser.AssignmentContext context)
        {
            _assemblyBuilder.IsInsideAssignment = false;
            
            string assignmentOperator = context.assignmentOperator().GetText();

            _assemblyBuilder.AssignmentEnd(assignmentOperator);
        }

        
        public override void EnterFuncCall(DaedalusParser.FuncCallContext context)
        {
            _assemblyBuilder.ErrorContext.Context = context.nameNode();

            string funcName = context.nameNode().GetText();
            _assemblyBuilder.FuncCallStart(funcName);
        }

        public override void ExitFuncCall(DaedalusParser.FuncCallContext context)
        {
            _assemblyBuilder.FuncCallEnd();  
        }

        public override void EnterFuncArgExpression(DaedalusParser.FuncArgExpressionContext context)
        {
            _assemblyBuilder.FuncCallArgStart();
        }

        public override void ExitFuncArgExpression(DaedalusParser.FuncArgExpressionContext context)
        {
            _assemblyBuilder.FuncCallArgEnd();
        }

        public override void EnterIntegerLiteralValue(DaedalusParser.IntegerLiteralValueContext context)
        {
            if (!_assemblyBuilder.IsInsideConstDef)
            {
                bool isInsideFloatAssignment = _assemblyBuilder.IsInsideAssignment
                                               && _assemblyBuilder.AssignmentType == DatSymbolType.Float;
                bool isInsideFloatArgument = _assemblyBuilder.IsInsideArgList
                                             && _assemblyBuilder.FuncCallCtx.GetParameterType() == DatSymbolType.Float;
                
                if (isInsideFloatAssignment || isInsideFloatArgument)
                {
                    int parsedFloat = EvaluatorHelper.EvaluateFloatExpression(context.Parent.Parent.GetText());
                    _assemblyBuilder.AddInstruction(new PushInt(parsedFloat));
                }
                else
                {
                    _assemblyBuilder.AddInstruction(new PushInt(int.Parse(context.GetText())));   
                }
            }
        }
        
        public override void EnterFloatLiteralValue(DaedalusParser.FloatLiteralValueContext context)
        {
            if (!_assemblyBuilder.IsInsideConstDef)
            {
                int parsedFloat = EvaluatorHelper.EvaluateFloatExpression(context.Parent.Parent.GetText());
                _assemblyBuilder.AddInstruction(new PushInt(parsedFloat));
            }
        }

        public override void EnterStringLiteralValue(DaedalusParser.StringLiteralValueContext context)
        {
            if (!_assemblyBuilder.IsInsideConstDef)
            {
                DatSymbolLocation location = GetLocation(context);
                string value = context.GetText().Replace("\"", "");
                DatSymbol symbol = SymbolBuilder.BuildStringConst(value, location);
                _assemblyBuilder.AddInstruction(new PushVar(symbol));
            }
        }
        
        
        /*
         *  ENTER EXPRESSION
         */
        public override void EnterBracketExpression(DaedalusParser.BracketExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        
        public override void EnterOneArgExpression(DaedalusParser.OneArgExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        
        public override void EnterMultExpression(DaedalusParser.MultExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        
        public override void EnterAddExpression(DaedalusParser.AddExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        
        public override void EnterBitMoveExpression(DaedalusParser.BitMoveExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        
        public override void EnterCompExpression(DaedalusParser.CompExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        
        public override void EnterEqExpression(DaedalusParser.EqExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        
        public override void EnterBinAndExpression(DaedalusParser.BinAndExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        
        public override void EnterBinOrExpression(DaedalusParser.BinOrExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }

        public override void EnterLogAndExpression(DaedalusParser.LogAndExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }

        public override void EnterLogOrExpression(DaedalusParser.LogOrExpressionContext context)
        {
            _assemblyBuilder.ExpressionStart();
        }
        

        /*
         *  EXIT EXPRESSION
         */
        public override void ExitBracketExpression(DaedalusParser.BracketExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        
        public override void ExitOneArgExpression(DaedalusParser.OneArgExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        
        public override void ExitMultExpression(DaedalusParser.MultExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        
        public override void ExitAddExpression(DaedalusParser.AddExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        
        public override void ExitBitMoveExpression(DaedalusParser.BitMoveExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        
        public override void ExitCompExpression(DaedalusParser.CompExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        
        public override void ExitEqExpression(DaedalusParser.EqExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        
        public override void ExitBinAndExpression(DaedalusParser.BinAndExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        
        public override void ExitBinOrExpression(DaedalusParser.BinOrExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }

        public override void ExitLogAndExpression(DaedalusParser.LogAndExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }

        public override void ExitLogOrExpression(DaedalusParser.LogOrExpressionContext context)
        {
            _assemblyBuilder.ExpressionEnd();
        }
        

        /*
         *  ENTER OPERATOR
         */
        public override void ExitOneArgOperator(DaedalusParser.OneArgOperatorContext context)
        {
            if (_assemblyBuilder.IsInsideOneArgOperatorsEvaluationMode())
            {
                return;
            }
            _assemblyBuilder.ExitOperator(context.GetText(), twoArg:false);
        }
        
        public override void ExitMultOperator(DaedalusParser.MultOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        
        public override void ExitAddOperator(DaedalusParser.AddOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        
        public override void ExitBitMoveOperator(DaedalusParser.BitMoveOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        
        public override void ExitCompOperator(DaedalusParser.CompOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        
        public override void ExitEqOperator(DaedalusParser.EqOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        
        public override void ExitBinAndOperator(DaedalusParser.BinAndOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        
        public override void ExitBinOrOperator(DaedalusParser.BinOrOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        
        public override void ExitLogAndOperator(DaedalusParser.LogAndOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        
        public override void ExitLogOrOperator(DaedalusParser.LogOrOperatorContext context)
        {
            _assemblyBuilder.ExitOperator(context.GetText());
        }
        

        private DatSymbolType DatSymbolTypeFromString(string typeName)
        {
            string capitalizedTypeName = typeName.First().ToString().ToUpper() + typeName.Substring(1).ToLower();

            if (Enum.TryParse(capitalizedTypeName, out DatSymbolType type))
            {
                return type;
            }
            else
            {
                var symbol = _assemblyBuilder.ResolveSymbol(typeName);
                return symbol.Type;
            }
        }

        private DatSymbolLocation GetLocation(ParserRuleContext context)
        {
            return new DatSymbolLocation
            {
                FileNumber = _sourceFileNumber,
                Line = context.Start.Line,
                LinesCount = context.Stop.Line - context.Start.Line + 1,
                Position = context.Start.StartIndex,
                PositionsCount = context.Stop.StopIndex - context.Start.StartIndex + 1
            };
        }
    }
}
