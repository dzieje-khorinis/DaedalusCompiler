using System;
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
                    var constValueContext = (DaedalusParser.ConstValueDefContext)constContext;
                    var name = constValueContext.nameNode().GetText();
                    var location = GetLocation(context);
                    var assignmentExpression = constValueContext.constValueAssignment().expression();
                    var value = EvaluatorHelper.EvaluateConst(assignmentExpression, assemblyBuilder, type.Value);

                    var symbol = SymbolBuilder.BuildConst(name, type.Value, value, location); // TODO : Validate params
                    assemblyBuilder.addSymbol(symbol);

                    continue;
                }

                if (constContext is DaedalusParser.ConstArrayDefContext)
                {
                    var constArrayContext = (DaedalusParser.ConstArrayDefContext)constContext;
                    var name = constArrayContext.nameNode().GetText();
                    var location = GetLocation(context);
                    var size = EvaluatorHelper.EvaluteArraySize(constArrayContext.simpleValue(), assemblyBuilder);
                    var content = constArrayContext.constArrayAssignment().expression()
                        .Select(expr => EvaluatorHelper.EvaluateConst(expr, assemblyBuilder, type.Value))
                        .ToArray();

                    if (size != content.Length)
                        throw new Exception($"Invalid const array definition '{constArrayContext.GetText()}'. Invalid items count: expected = {size}, readed = {content.Length}");

                    var symbol = SymbolBuilder.BuildArrOfConst(name, type.Value, content, location); // TODO : Validate params
                    assemblyBuilder.addSymbol(symbol);

                    continue;
                }
            }
        }

        public override void EnterVarDecl([NotNull] DaedalusParser.VarDeclContext context)
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
                    var varValueContext = (DaedalusParser.VarValueDeclContext)varContext;
                    var name = varValueContext.nameNode().GetText();
                    var location = GetLocation(context);

                    var symbol = SymbolBuilder.BuildVariable(name, type.Value, location); // TODO : Validate params
                    assemblyBuilder.addSymbol(symbol);
                }
                
                if (varContext is DaedalusParser.VarArrayDeclContext)
                {
                    var varArrayContext = (DaedalusParser.VarArrayDeclContext)varContext;
                    var name = varArrayContext.nameNode().GetText();
                    var location = GetLocation(context);
                    var size = EvaluatorHelper.EvaluteArraySize(varArrayContext.simpleValue(), assemblyBuilder);

                    var symbol = SymbolBuilder.BuildArrOfVariables(name, type.Value, (uint)size); // TODO : Validate params
                    assemblyBuilder.addSymbol(symbol);
                }
            }
        }

        public override void EnterFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            var name = context.nameNode().GetText();
            var typeName = context.typeReference().GetText();
            var type = DatSymbolTypeFromString(typeName);

            var symbol = SymbolBuilder.BuildFunc(name, type.Value); // TODO : Validate params
            assemblyBuilder.addSymbol(symbol);
            assemblyBuilder.functionStart(symbol);
        }

        public override void ExitFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            // we invoke functionEnd, thanks that ab will assign all instructions
            // to currently exited function
            assemblyBuilder.functionEnd();
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

        public override void ExitCompExpression(DaedalusParser.CompExpressionContext context)
        {
            //TODO implement correctly
            assemblyBuilder.addInstruction(new Greater());
        }

        public override void EnterValExpression(DaedalusParser.ValExpressionContext context)
        {
            string value = context.value().GetText();
            var firstChar = value[0];
            var lastChar = value[value.Length - 1];
            var isNumber = new Regex("^[0-9]+$");

            if (firstChar == '"' && lastChar == '"')
            {
                //TODO implement
            }
            else if (isNumber.IsMatch(firstChar.ToString()))
            {
                assemblyBuilder.addInstruction(new PushInt(int.Parse(value)));
            }
            else
            {
                //todo implement identificator
            }
        }

        private DatSymbolType? DatSymbolTypeFromString(string typeReference)
        {
            if (String.IsNullOrWhiteSpace(typeReference))
                return null;

            // FirstCharToUpper 
            typeReference = typeReference.First().ToString().ToUpper() + typeReference.Substring(1).ToLower();

            DatSymbolType type;
            if (Enum.TryParse(typeReference, out type))
                return type;
            else
                return null;
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
