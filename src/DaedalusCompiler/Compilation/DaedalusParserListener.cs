﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class DaedalusParserListener : DaedalusBaseListener
    {
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly int sourceFileNumber;

        public DaedalusParserListener(AssemblyBuilder assemblyBuilder, int sourceFileNumber)
        {
            this.assemblyBuilder = assemblyBuilder;
            this.sourceFileNumber = sourceFileNumber;
        }

        public override void EnterParameterDecl(DaedalusParser.ParameterDeclContext context)
        {
            FunctionBlock functionBlock = assemblyBuilder.functions.Last();
            string functionName = functionBlock.symbol.Name;
            string parameterLocalName = context.nameNode().GetText();
            string parameterName = $"{functionName}.{parameterLocalName}";

            int parentId = -1;
            DatSymbol parentSymbol = null;

            string parameterTypeName = context.typeReference().GetText();
            DatSymbolType? parameterType = DatSymbolTypeFromString(parameterTypeName);
            if (parameterType is DatSymbolType.Class) // TODO what about prototypes and instances?
            {
                parentSymbol = assemblyBuilder.resolveSymbol(parameterTypeName);
                parentId = assemblyBuilder.getSymbolId(parentSymbol);
            }

            DatSymbol symbol = null;
            var location = GetLocation(context);

            if (context.simpleValue() != null) // arrayElementsCount Context
            {
                uint arrayElementsCount = uint.Parse(context.simpleValue().GetText());
                // TODO is parentId also included in variable array?
                symbol = SymbolBuilder.BuildArrOfVariables(parameterName, parameterType.Value, arrayElementsCount,
                    location);
            }
            else
            {
                symbol = SymbolBuilder.BuildVariable(parameterName, parameterType.Value, location, parentId);
            }

            assemblyBuilder.addSymbol(symbol);
        }

        public override void EnterConstDef([NotNull] DaedalusParser.ConstDefContext context)
        {
            var typeName = context.typeReference().GetText();
            var type = DatSymbolTypeFromString(typeName);

            for (int i = 0; i < context.ChildCount; i++)
            {
                var constContext = context.GetChild(i);

                if (constContext is TerminalNodeImpl)
                    continue; // skips ',' 

                if (constContext is DaedalusParser.ConstValueDefContext)
                {
                    var constValueContext = (DaedalusParser.ConstValueDefContext) constContext;
                    var name = constValueContext.nameNode().GetText();
                    var location = GetLocation(context);
                    var assignmentExpression = constValueContext.constValueAssignment().expressionBlock().expression();
                    var value = EvaluatorHelper.EvaluateConst(assignmentExpression, assemblyBuilder, type.Value);

                    var symbol = SymbolBuilder.BuildConst(name, type.Value, value, location); // TODO : Validate params
                    assemblyBuilder.addSymbol(symbol);

                    continue;
                }

                if (constContext is DaedalusParser.ConstArrayDefContext)
                {
                    var constArrayContext = (DaedalusParser.ConstArrayDefContext) constContext;
                    var name = constArrayContext.nameNode().GetText();
                    var location = GetLocation(context);
                    var size = EvaluatorHelper.EvaluteArraySize(constArrayContext.simpleValue(), assemblyBuilder);
                    var content = constArrayContext.constArrayAssignment().expressionBlock()
                        .Select(expr => EvaluatorHelper.EvaluateConst(expr.expression(), assemblyBuilder, type.Value))
                        .ToArray();

                    if (size != content.Length)
                        throw new Exception(
                            $"Invalid const array definition '{constArrayContext.GetText()}'. Invalid items count: expected = {size}, readed = {content.Length}");

                    var symbol =
                        SymbolBuilder.BuildArrOfConst(name, type.Value, content, location); // TODO : Validate params
                    assemblyBuilder.addSymbol(symbol);

                    continue;
                }
            }
        }

        public override void EnterVarDecl([NotNull] DaedalusParser.VarDeclContext context)
        {
            if (context.Parent is DaedalusParser.DaedalusFileContext)
            {
                var typeName = context.typeReference().GetText();
                var type = DatSymbolTypeFromString(typeName);

                for (int i = 0; i < context.ChildCount; i++)
                {
                    var varContext = context.GetChild(i);

                    if (varContext is TerminalNodeImpl)
                        continue; // skips ',' 

                    if (varContext is DaedalusParser.VarValueDeclContext)
                    {
                        var varValueContext = (DaedalusParser.VarValueDeclContext) varContext;
                        var name = varValueContext.nameNode().GetText();
                        var location = GetLocation(context);

                        var symbol = SymbolBuilder.BuildVariable(name, type.Value, location); // TODO : Validate params
                        assemblyBuilder.addSymbol(symbol);
                    }

                    if (varContext is DaedalusParser.VarArrayDeclContext)
                    {
                        var varArrayContext = (DaedalusParser.VarArrayDeclContext) varContext;
                        var name = varArrayContext.nameNode().GetText();
                        var location = GetLocation(context);
                        var size = EvaluatorHelper.EvaluteArraySize(varArrayContext.simpleValue(), assemblyBuilder);

                        var symbol =
                            SymbolBuilder.BuildArrOfVariables(name, type.Value, (uint) size,
                                location); // TODO : Validate params
                        assemblyBuilder.addSymbol(symbol);
                    }
                }
            }
        }

        public override void EnterClassDef([NotNull] DaedalusParser.ClassDefContext context)
        {
            var className = context.nameNode().GetText();
            var classSymbol = SymbolBuilder.BuildClass(className, 0, 0, GetLocation(context));
            assemblyBuilder.addSymbol(classSymbol);

            var classId = assemblyBuilder.getSymbolId(classSymbol);
            int classVarOffset = 0;
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

                    if (varContext is DaedalusParser.VarValueDeclContext)
                    {
                        var varValueContext = (DaedalusParser.VarValueDeclContext) varContext;
                        var name = varValueContext.nameNode().GetText();
                        var location = GetLocation(context);

                        var symbol = SymbolBuilder.BuildClassVar(name, type.Value, 1, className, classId,
                            classVarOffset, location); // TODO : Validate params
                        assemblyBuilder.addSymbol(symbol);

                        classVarOffset += (type == DatSymbolType.String ? 20 : 4);
                        classLength++;
                    }

                    if (varContext is DaedalusParser.VarArrayDeclContext)
                    {
                        var varArrayContext = (DaedalusParser.VarArrayDeclContext) varContext;
                        var name = varArrayContext.nameNode().GetText();
                        var location = GetLocation(context);
                        var size = EvaluatorHelper.EvaluteArraySize(varArrayContext.simpleValue(), assemblyBuilder);

                        var symbol = SymbolBuilder.BuildClassVar(name, type.Value, (uint) size, className, classId,
                            classVarOffset, location); // TODO : Validate params
                        assemblyBuilder.addSymbol(symbol);

                        classVarOffset += (type == DatSymbolType.String ? 20 : 4) * size;
                        classLength++;
                    }
                }
            }

            classSymbol.ArrayLength = classLength;
            classSymbol.ClassSize = classVarOffset;
        }

        public override void EnterPrototypeDef([NotNull] DaedalusParser.PrototypeDefContext context)
        {
            var prototypeName = context.nameNode().GetText();
            var referenceName = context.referenceNode().GetText();
            var refSymbol = assemblyBuilder.getSymbolByName(referenceName);
            var referenceSymbolId = assemblyBuilder.getSymbolId(refSymbol);
            var location = GetLocation(context);

            var firstTokenAddress = 0; // TODO: Populate first token addres
            var prototypeSymbol =
                SymbolBuilder.BuildPrototype(prototypeName, referenceSymbolId, firstTokenAddress,
                    location); // TODO: Validate params
            assemblyBuilder.addSymbol(prototypeSymbol);

            assemblyBuilder.execBlockStart(prototypeSymbol, ExecutebleBlockType.PrototypeConstructor);
            assemblyBuilder.setRefSymbol(refSymbol);
        }

        public override void ExitPrototypeDef(DaedalusParser.PrototypeDefContext context)
        {
            // we invoke execBlockEnd, thanks that ab will assign all instructions
            // to currently exited prototype constructor
            assemblyBuilder.execBlockEnd();
        }

        public override void EnterInstanceDef(DaedalusParser.InstanceDefContext context)
        {
            var prototypeName = context.nameNode().GetText();
            var referenceName = context.referenceNode().GetText();
            var refSymbol = assemblyBuilder.getSymbolByName(referenceName);
            var referenceSymbolId = assemblyBuilder.getSymbolId(refSymbol);
            var location = GetLocation(context);

            var firstTokenAddress = 0; // TODO: Populate first token addres
            var prototypeSymbol =
                SymbolBuilder.BuildPrototype(prototypeName, referenceSymbolId, firstTokenAddress,
                    location); // TODO: Validate params
            assemblyBuilder.addSymbol(prototypeSymbol);

            assemblyBuilder.execBlockStart(prototypeSymbol, ExecutebleBlockType.InstanceConstructor);
            assemblyBuilder.setRefSymbol(refSymbol);
        }

        public override void ExitInstanceDef(DaedalusParser.InstanceDefContext context)
        {
            // we invoke execBlockEnd, thanks that ab will assign all instructions
            // to currently exited instance constructor
            assemblyBuilder.execBlockEnd();
        }

        public override void EnterFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            var name = context.nameNode().GetText();
            var typeName = context.typeReference().GetText();
            var type = DatSymbolTypeFromString(typeName);

            var symbol = SymbolBuilder.BuildFunc(name, type.Value); // TODO : Validate params
            assemblyBuilder.addSymbol(symbol);
            assemblyBuilder.execBlockStart(symbol, ExecutebleBlockType.Function);
        }

        public override void ExitFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            // we invoke execBlockEnd, thanks that ab will assign all instructions
            // to currently exited function
            assemblyBuilder.addInstruction(new Ret());
            assemblyBuilder.execBlockEnd();
        }

        public override void ExitReturnStatement([NotNull] DaedalusParser.ReturnStatementContext context)
        {
            assemblyBuilder.addInstruction(new Ret());
        }

        public override void EnterIfBlockStatement(DaedalusParser.IfBlockStatementContext context)
        {
            assemblyBuilder.conditionalStart();
        }

        public override void ExitIfBlockStatement(DaedalusParser.IfBlockStatementContext context)
        {
            assemblyBuilder.conditionalEnd();
            //assemblyBuilder.ifStatementEnd();
        }


        public override void EnterIfBlock(DaedalusParser.IfBlockContext context)
        {
            assemblyBuilder.conditionalBlockConditionStart(IfBlockType.If);
        }

        public override void ExitIfBlock(DaedalusParser.IfBlockContext context)
        {
            assemblyBuilder.conditionalBlockBodyEnd();
        }

        public override void EnterElseIfBlock(DaedalusParser.ElseIfBlockContext context)
        {
            assemblyBuilder.conditionalBlockConditionStart(IfBlockType.ElseIf);
        }

        public override void ExitElseIfBlock(DaedalusParser.ElseIfBlockContext context)
        {
            assemblyBuilder.conditionalBlockBodyEnd();
        }

        public override void EnterElseBlock(DaedalusParser.ElseBlockContext context)
        {
            assemblyBuilder.conditionalBlockConditionStart(IfBlockType.Else);
            // else does't have condition so we call here end of condition
            assemblyBuilder.conditionalBlockConditionEnd();
        }

        public override void ExitElseBlock(DaedalusParser.ElseBlockContext context)
        {
            assemblyBuilder.conditionalBlockBodyEnd();
        }

        public override void ExitIfCondition(DaedalusParser.IfConditionContext context)
        {
            assemblyBuilder.conditionalBlockConditionEnd();
        }

        public override void EnterBitMoveOperator(DaedalusParser.BitMoveOperatorContext context)
        {
            assemblyBuilder.expressionRightSideStart();
        }

        public override void EnterBitMoveExpression(DaedalusParser.BitMoveExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitBitMoveExpression(DaedalusParser.BitMoveExpressionContext context)
        {
            var exprOperator = context.bitMoveOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void EnterCompExpression(DaedalusParser.CompExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitCompExpression(DaedalusParser.CompExpressionContext context)
        {
            var exprOperator = context.compOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void EnterCompOperator(DaedalusParser.CompOperatorContext context)
        {
            assemblyBuilder.expressionRightSideStart();
        }

        public override void EnterEqExpression(DaedalusParser.EqExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitEqExpression(DaedalusParser.EqExpressionContext context)
        {
            var exprOperator = context.eqOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void EnterEqOperator(DaedalusParser.EqOperatorContext context)
        {
            assemblyBuilder.expressionRightSideStart();
        }

        public override void ExitComplexReference(DaedalusParser.ComplexReferenceContext context)
        {
            var referenceNodes = context.complexReferenceNode();
            var symbolPart = referenceNodes[0];
            var arrIndex = symbolPart.simpleValue();
            var symbol = assemblyBuilder.resolveSymbol(symbolPart.referenceNode().GetText());

            if (referenceNodes.Length == 2)
            {
                string typeName = assemblyBuilder.symbols[symbol.Parent].Name;
                string attributeName = referenceNodes[1].GetText();

                DatSymbol attribute = assemblyBuilder.resolveSymbol($"{typeName}.{attributeName}");

                AssemblyInstruction[] instructions = {new SetInstance(symbol), new PushVar(attribute)};
                assemblyBuilder.addInstructions(instructions);
            }
            else
            {
                if (arrIndex == null)
                {
                    assemblyBuilder.addInstruction(new PushVar(symbol));
                }
                else
                {
                    assemblyBuilder.addInstruction(new PushArrVar(symbol, int.Parse(arrIndex.GetText())));
                }
            }
        }


        public override void EnterAssignment(DaedalusParser.AssignmentContext context)
        {
            var referenceNodes = context.complexReferenceLeftSide().complexReferenceNode();

            var symbolPart = referenceNodes[0];
            var arrIndex = symbolPart.simpleValue();
            var symbol = assemblyBuilder.resolveSymbol(symbolPart.referenceNode().GetText());
            var operatorVal = context.assigmentOperator().GetText();

            if (referenceNodes.Length == 2)
            {
                string typeName = assemblyBuilder.symbols[symbol.Parent].Name;
                string attributeName = referenceNodes[1].GetText();

                DatSymbol attribute = assemblyBuilder.resolveSymbol($"{typeName}.{attributeName}");

                assemblyBuilder.assigmentStart(
                    new SetInstance(symbol),
                    new PushVar(attribute)
                );
            }
            else
            {
                if (arrIndex == null)
                {
                    assemblyBuilder.assigmentStart(new PushVar(symbol));
                }
                else
                {
                    int arrayIndex = int.Parse(arrIndex.GetText());
                    assemblyBuilder.assigmentStart(new PushArrVar(symbol, arrayIndex));
                }
            }
        }


        public override void ExitAssignment(DaedalusParser.AssignmentContext context)
        {
            string assignmentOperator = context.assigmentOperator().GetText();
            assemblyBuilder.assigmentEnd(assignmentOperator);
        }

        public override void EnterFuncCallValue(DaedalusParser.FuncCallValueContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
            assemblyBuilder.funcCallStart();
        }

        public override void ExitFuncCallValue(DaedalusParser.FuncCallValueContext context)
        {
            var funcName = context.funcCall().nameNode().GetText();
            var symbol = assemblyBuilder.getSymbolByName(funcName);

            if (symbol == null)
            {
                throw new Exception($"Function '{funcName}' not defined");
            }

            //todo implement call external
            assemblyBuilder.funcCallEnd(new Call(symbol));
        }

        public override void EnterFuncArgExpression(DaedalusParser.FuncArgExpressionContext context)
        {
            assemblyBuilder.funcCallArgStart();
        }

        public override void ExitFuncArgExpression(DaedalusParser.FuncArgExpressionContext context)
        {
            assemblyBuilder.funcCallArgEnd();
        }

        public override void EnterExpressionBlock(DaedalusParser.ExpressionBlockContext context)
        {
            var text = context.GetText();
            Console.WriteLine("enter expr block");
            Console.WriteLine(context.GetText());
        }

        public override void EnterIntegerLiteralValue(DaedalusParser.IntegerLiteralValueContext context)
        {
            assemblyBuilder.addInstruction(new PushInt(int.Parse(context.GetText())));
        }

        public override void EnterAddExpression(DaedalusParser.AddExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitAddExpression(DaedalusParser.AddExpressionContext context)
        {
            var exprOperator = context.addOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void EnterAddOperator(DaedalusParser.AddOperatorContext context)
        {
            //TODO add comment why here we invoke that
            assemblyBuilder.expressionRightSideStart();
        }

        public override void EnterMultOperator(DaedalusParser.MultOperatorContext context)
        {
            //TODO add comment why here we invoke that
            assemblyBuilder.expressionRightSideStart();
        }

        public override void EnterMultExpression(DaedalusParser.MultExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitMultExpression(DaedalusParser.MultExpressionContext context)
        {
            var exprOperator = context.multOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void EnterBinAndOperator(DaedalusParser.BinAndOperatorContext context)
        {
            assemblyBuilder.expressionRightSideStart();
        }

        public override void EnterBinAndExpression(DaedalusParser.BinAndExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitBinAndExpression(DaedalusParser.BinAndExpressionContext context)
        {
            var exprOperator = context.binAndOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void EnterBinOrOperator(DaedalusParser.BinOrOperatorContext context)
        {
            assemblyBuilder.expressionRightSideStart();
        }

        public override void EnterBinOrExpression(DaedalusParser.BinOrExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitBinOrExpression(DaedalusParser.BinOrExpressionContext context)
        {
            var exprOperator = context.binOrOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void EnterLogAndOperator(DaedalusParser.LogAndOperatorContext context)
        {
            assemblyBuilder.expressionRightSideStart();
        }

        public override void EnterLogAndExpression(DaedalusParser.LogAndExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitLogAndExpression(DaedalusParser.LogAndExpressionContext context)
        {
            var exprOperator = context.logAndOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void EnterLogOrOperator(DaedalusParser.LogOrOperatorContext context)
        {
            assemblyBuilder.expressionRightSideStart();
        }

        public override void EnterLogOrExpression(DaedalusParser.LogOrExpressionContext context)
        {
            assemblyBuilder.expressionLeftSideStart();
        }

        public override void ExitLogOrExpression(DaedalusParser.LogOrExpressionContext context)
        {
            var exprOperator = context.logOrOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator);
            assemblyBuilder.expressionEnd(instruction);
        }

        public override void ExitOneArgExpression(DaedalusParser.OneArgExpressionContext context)
        {
            var exprOperator = context.oneArgOperator().GetText();
            var instruction = AssemblyBuilderHelpers.GetInstructionForOperator(exprOperator, false);
            assemblyBuilder.addInstruction(instruction);
        }

        // TODO change return type from DatSymbolType? to DatSymbolType
        private DatSymbolType? DatSymbolTypeFromString(string typeName)
        {
            if (String.IsNullOrWhiteSpace(typeName))
                return null;

            string capitalizedTypeName = typeName.First().ToString().ToUpper() + typeName.Substring(1).ToLower();

            DatSymbolType type;
            if (Enum.TryParse(capitalizedTypeName, out type))
            {
                return type;
            }
            else
            {
                var symbol = assemblyBuilder.resolveSymbol(typeName);
                return symbol.Type;
            }
        }

        private DatSymbolLocation GetLocation(ParserRuleContext context)
        {
            return new DatSymbolLocation
            {
                FileNumber = sourceFileNumber,
                Line = context.Start.Line,
                LinesCount = context.Stop.Line - context.Start.Line + 1,
                Position = context.Start.StartIndex,
                PositionsCount = context.Stop.StopIndex - context.Start.StartIndex + 1
            };
        }
    }
}