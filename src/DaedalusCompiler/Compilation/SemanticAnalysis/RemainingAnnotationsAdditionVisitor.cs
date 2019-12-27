using System;
using System.Collections.Generic;


namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class RemainingAnnotationsAdditionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private readonly HashSet<string> _initializedSymbolsPaths;

        private readonly Dictionary<string, Symbol> _symbolTable;
        
        private static readonly Dictionary<string, int> Class2RequiredSize = new Dictionary<string, int>
        {
            {"C_NPC", 800},
            {"C_INFO", 48},
            {"C_ITEMREACT", 28},
        };
        
        private static readonly Dictionary<string, int> Class2Offset = new Dictionary<string, int>
        {
            {"C_NPC", 288},
            {"C_ITEM", 288},
        };

        public RemainingAnnotationsAdditionVisitor(Dictionary<string, Symbol> symbolTable)
        {
            _symbolTable = symbolTable;
        }

        public override void VisitTree(AbstractSyntaxTree tree)
        {
            base.VisitTree(tree);
            
            foreach (Symbol symbol in _symbolTable.Values)
            {
                switch (symbol)
                {
                    case FunctionSymbol functionSymbol:
                        functionSymbol.Flags |= SymbolFlag.Const;
                        
                        if (functionSymbol.BuiltinType != SymbolType.Void)
                        {
                            functionSymbol.Flags |= SymbolFlag.Return;
                        }

                        if (functionSymbol.IsExternal)
                        {
                            functionSymbol.Flags |= SymbolFlag.External;
                        }
                        
                        break;
                    
                    case InstanceSymbol instanceSymbol:
                        instanceSymbol.Flags |= SymbolFlag.Const;
                        break;
                    
                    case ConstSymbol constSymbol:
                        constSymbol.Flags |= SymbolFlag.Const;
                        break;
                    
                    case VarSymbol varSymbol:
                        if (varSymbol.ParentBlockSymbol is ClassSymbol)
                        {
                            varSymbol.Flags |= SymbolFlag.ClassVar;
                        }

                        break;
                }
            }
        }

        
        protected override void VisitCompoundAssignment(CompoundAssignmentNode node)
        {
            if (node.LeftSideNode.Symbol?.Node is ConstDefinitionNode)
            {
                node.Annotations.Add(new ConstValueChangedWarning(node.LeftSideNode.Name));
            }
            base.VisitCompoundAssignment(node);
        }

        protected override void VisitAssignment(AssignmentNode node)
        {
            ReferenceNode referenceNode = node.LeftSideNode;
            Symbol symbol = referenceNode.Symbol;
            
            if (symbol?.Node is ConstDefinitionNode)
            {
                node.Annotations.Add(new ConstValueChangedWarning(referenceNode.Name));
            } 
            
            base.VisitAssignment(node);
        }
        protected override void VisitClassDefinition(ClassDefinitionNode node)
        {
            string classNameUpper = node.NameNode.Value.ToUpper();
            int offset = 0;
            if (Class2Offset.ContainsKey(classNameUpper))
            {
                offset = Class2Offset[classNameUpper];
            }

            int size = 0;
            foreach (DeclarationNode attributeNode in node.AttributeNodes)
            {
                if (attributeNode.Symbol is AttributeSymbol attributeSymbol)
                {
                    attributeSymbol.Offset = offset + size;
                }
                
                if (attributeNode is VarArrayDeclarationNode varArrayDeclarationNode)
                {
                    NodeValue arraySizeValue = varArrayDeclarationNode.ArraySizeValue;
                    if (arraySizeValue is IntValue intValue)
                    {
                        size += (attributeNode.Symbol.BuiltinType == SymbolType.String ? 20 : 4) * Convert.ToInt32(intValue.Value);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    size += (attributeNode.Symbol.BuiltinType == SymbolType.String ? 20 : 4);
                }
            }
            
            
            if (node.Symbol is ClassSymbol classSymbol)
            {
                classSymbol.Offset = offset;
                classSymbol.Size = size;
                classSymbol.AttributesCount = node.AttributeNodes.Count;
            }

            

            if (Class2RequiredSize.ContainsKey(classNameUpper))
            {
                long requiredSize = Class2RequiredSize[classNameUpper];
                if (size != requiredSize)
                {
                    node.Annotations.Add(new WrongClassSizeError(node.NameNode.Location, node.NameNode.Value, size, requiredSize));
                }
            }
            
        }

        protected override void VisitConstDefinition(ConstDefinitionNode node) { }

        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode node) { }
        

        protected override void VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitPrototypeDefinition(node);
        }

        protected override void VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitInstanceDefinition(node);
        }

        protected override void VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            if (node.Symbol is FunctionSymbol functionSymbol)
            {
                functionSymbol.ParametersCount = node.ParameterNodes.Count;
            }
            
            
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitFunctionDefinition(node);
        }

        protected override void VisitIfStatement(IfStatementNode node)
        {
            if (node.ElseNodeBodyNodes != null)
            {
                CheckStatementsForSingleExpressionHack(node.ElseNodeBodyNodes);
            }
            
            base.VisitIfStatement(node);
        }

        protected override void VisitConditional(ConditionalNode node)
        {
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitConditional(node);
        }

        protected override void VisitBreakStatement(BreakStatementNode node)
        {
            if (!IsStatementInsideLoop(node))
            {
                node.Annotations.Add(new IterationStatementNotInLoopError("break"));
            }
        }

        protected override void VisitContinueStatement(ContinueStatementNode node)
        {
            if (!IsStatementInsideLoop(node))
            {
                node.Annotations.Add(new IterationStatementNotInLoopError("continue"));
            }
        }

        protected override void VisitIntegerLiteral(IntegerLiteralNode node)
        {
            if (!node.EvaluatedCorrectly || node.Value < Int32.MinValue || node.Value > Int32.MaxValue)
            {
                node.Annotations.Add(new IntegerLiteralTooLargeError());
            }
        }

        private void CheckStatementsForSingleExpressionHack(List<StatementNode> statementNodes)
        {
            foreach (var statementNode in statementNodes)
            {
                switch (statementNode)
                {
                    case FunctionCallNode _:
                        break;
                    case ExpressionNode _:
                        statementNode.Annotations.Add(new SingleExpressionWarning());
                        break;
                }
            }
        }

        private bool IsStatementInsideLoop(ASTNode node)
        {
            while (node != null)
            {
                node = node.ParentNode;
                switch (node)
                {
                    case WhileStatementNode _:
                        return true;
                    case FileNode _:
                        return false;
                }
            }
            throw new Exception();
        }
    }
}