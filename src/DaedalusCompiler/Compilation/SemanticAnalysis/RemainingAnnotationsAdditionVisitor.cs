using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;


namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    class SymbolUsage
    {
        public string Path;
        public ASTNode Node;
        
        public SymbolUsage(ASTNode node, string path)
        {
            Node = node;
            Path = path;
        }
    }

    
    public class RemainingAnnotationsAdditionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private readonly HashSet<string> _initializedSymbolsPaths;
        
        private static readonly Dictionary<string, long> Class2RequiredSize = new Dictionary<string, long>
        {
            {"C_NPC", 800},
            {"C_INFO", 48},
            {"C_ITEMREACT", 28},
        };

        public RemainingAnnotationsAdditionVisitor()
        {
            _initializedSymbolsPaths = new HashSet<string>();
        }

        /*
        protected override void Visit(ASTNode node)
        {
            if (node.Annotations.Count > 0)
            {
                string message = node.GetType().ToString().Split(".").Last();
                switch (node)
                {
                    case ConstDefinitionNode constDefinitionNode:
                        message = $"{message} {constDefinitionNode.NameNode.Value}";
                        break;
                    case ReferenceNode referenceNode:
                        message = $"{message} {referenceNode.Name}";
                        break;
                    case AttributeNode attributeNode:
                        message = $"{message} {attributeNode.Name}";
                        break;
                }

                Console.WriteLine($"{message}");
                foreach (var annotation in node.Annotations)
                {
                    Console.WriteLine(annotation.GetType());
                }
            }
            base.Visit(node);
        }*/

        
        protected override void VisitCompoundAssignment(CompoundAssignmentNode node)
        {
            if (node.LeftSideNode.Symbol?.Node is ConstDefinitionNode)
            {
                node.Annotations.Add(new ConstValueChangedWarning(node.LeftSideNode.Name));
            }
            base.VisitCompoundAssignment(node);
        }
        
        /*
        private List<SymbolUsage> GetSymbolUsagesFromReference(ReferenceNode referenceNode)
        {
            List<SymbolUsage> symbolUsages = new List<SymbolUsage>();    
            
            Symbol symbol = referenceNode.BaseSymbol;
            if (symbol == null)
            {
                return null;
            }
            
            
            string symbolLocalPath = referenceNode.Name;
            SymbolUsage symbolUsage = new SymbolUsage(referenceNode, symbolLocalPath.ToUpper());

            bool arrayIndexNodeFound = false;


            foreach (ReferencePartNode partNode in referenceNode.PartNodes)
            {
                symbolUsages.Add(symbolUsage);

                if (arrayIndexNodeFound)
                {
                    break;
                }

                if (partNode is AttributeNode attributeNode)
                {
                    symbol = attributeNode.Symbol;
                    if (symbol == null)
                    {
                        break;
                    }
                    symbolLocalPath = $"{symbolLocalPath}.{attributeNode.Name}";
                    symbolUsage = new SymbolUsage(attributeNode, symbolLocalPath.ToUpper());
                    symbolUsages.Add(symbolUsage);
                }
                else if (partNode is ArrayIndexNode arrayIndexNode)
                {
                    arrayIndexNodeFound = true;
                        
                    if (!(symbol.Node is IArrayDeclarationNode))
                    {
                        break;
                    }

                    NodeValue nodeValue = arrayIndexNode.Value;
                    if (nodeValue is IntValue intValue)
                    {
                        symbolLocalPath = $"{symbolLocalPath}[{(int) intValue.Value}]";
                        symbolUsage.Path = symbolLocalPath;
                    }
                    else
                    {
                        symbolUsage = null;
                        break;
                    }
                }
            }
            
            if (symbolUsage != null)
            {
                symbolUsages.Add(symbolUsage);
            }
        
            
            
            return symbolUsages;
        }
        */
        
        /*
        private string GetInitializedSymbolPathFromReference(ReferenceNode referenceNode)
        {
            Symbol symbol = referenceNode.BaseSymbol;
            if (symbol == null)
            {
                return null;
            }
            
            string symbolLocalPath = referenceNode.Name;
            ASTNode ancestorNode = referenceNode.GetFirstSignificantAncestorNode();
            switch (ancestorNode)
            {
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    break;
                case InstanceDefinitionNode instanceDefinitionNode:
                    break;
                case FunctionDefinitionNode functionDefinitionNode:
                    break;
            }
            
            

            bool arrayIndexNodeFound = false;
            
            foreach (ReferencePartNode partNode in referenceNode.PartNodes)
            {
                if (arrayIndexNodeFound)
                {
                    return null;
                }
                
                switch (partNode)
                {
                    case AttributeNode attributeNode:
                        symbol = attributeNode.Symbol;
                        if (symbol == null)
                        {
                            return null;
                        }
                        symbolLocalPath = $"{symbolLocalPath}.{attributeNode.Name}";
                        break;
                    
                    case ArrayIndexNode arrayIndexNode:
                        arrayIndexNodeFound = true;
                        
                        if (!(symbol.Node is IArrayDeclarationNode))
                        {
                            return null;
                        }

                        NodeValue nodeValue = arrayIndexNode.Value;
                        switch (nodeValue)
                        {
                            case IntValue intValue:
                                symbolLocalPath = $"{symbolLocalPath}[{(int) intValue.Value}]";
                                break;
                            default:
                                return null;
                        }
                        
                        break;
                    default:
                        throw new Exception();
                }
            }

            return symbolLocalPath.ToUpper();
        }
        
        
        */
        
        protected override void VisitAssignment(AssignmentNode node)
        {
            ReferenceNode referenceNode = node.LeftSideNode;
            Symbol symbol = referenceNode.Symbol;
            
            if (symbol?.Node is ConstDefinitionNode)
            {
                node.Annotations.Add(new ConstValueChangedWarning(referenceNode.Name));
            } 
            
            if (symbol?.Node is VarDeclarationNode)
            {
                //string symbolPath = GetInitializedSymbolPathFromReference(referenceNode);
                //_initializedSymbolsPaths.Add(symbolPath);
            }
            base.VisitAssignment(node);
        }

        protected override void VisitReference(ReferenceNode referenceNode)
        {
            /*
            List<SymbolUsage> symbolUsages = GetSymbolUsagesFromReference(referenceNode);
            if (symbolUsages == null)
            {
                return;
            }
            
            foreach (SymbolUsage symbolUsage in symbolUsages)
            {
                string symbolPath = symbolUsage.Path;
                if (!_initializedSymbolsPaths.Contains(symbolPath))
                {
                    symbolUsage.Node.Annotations.Add(new UsageOfNonInitializedVariableWarning());
                }
            }
            */
            base.VisitReference(referenceNode);
        }


        protected override void VisitClassDefinition(ClassDefinitionNode node)
        {
            string classNameUpper = node.NameNode.Value.ToUpper();
            if (!Class2RequiredSize.ContainsKey(classNameUpper))
            {
                return;
            }
            
            long size = 0;
            long requiredSize = Class2RequiredSize[classNameUpper];
            foreach (DeclarationNode attributeNode in node.AttributeNodes)
            {
                if (attributeNode is VarArrayDeclarationNode varArrayDeclarationNode)
                {
                    NodeValue arraySizeValue = varArrayDeclarationNode.ArraySizeValue;
                    if (arraySizeValue is IntValue intValue)
                    {
                        size += (attributeNode.Symbol.BuiltinType == SymbolType.String ? 20 : 4) * intValue.Value;
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

            if (size != requiredSize)
            {
                node.Annotations.Add(new WrongClassSizeError(node.NameNode.Location, node.NameNode.Value, size, requiredSize));
            }
        }

        protected override void VisitConstDefinition(ConstDefinitionNode node)
        {
        }

        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
        }
        

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
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitFunctionDefinition(node);
        }

        protected override void VisitIfStatement(IfStatementNode node)
        {
            CheckStatementsForSingleExpressionHack(node.ElseNodeBodyNodes);
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