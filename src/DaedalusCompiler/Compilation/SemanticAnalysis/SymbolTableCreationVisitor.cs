using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class SymbolTableCreationVisitor : AbstractSyntaxTreeBaseVisitor
    {
        public readonly Dictionary <string, Symbol> SymbolTable;
        private readonly List<Symbol> _stringConstSymbols;
        
        private bool _isInExternalFile;
        private int _nextSymbolIndex;
        private int _nextStringSymbolNumber;
        
        public readonly List<ConstDefinitionNode> ConstDefinitionNodes;
        public readonly List<IArrayDeclarationNode> ArrayDeclarationNodes;
        
        public readonly List<ITypedSymbol> TypedSymbols;
        public readonly List<FunctionSymbol> FunctionSymbols;
        public readonly List<SubclassSymbol> SubclassSymbols;
        public readonly List<BlockSymbol> SymbolsWithInstructions;
        public readonly List<ClassSymbol> ClassSymbols;
        
        public readonly List<DeclarationNode> DeclarationNodes;
        
        public SymbolTableCreationVisitor()
        {
            SymbolTable = new Dictionary<string, Symbol>();
            _stringConstSymbols = new List<Symbol>();
            
            _isInExternalFile = false;
            _nextSymbolIndex = 0;
            _nextStringSymbolNumber = 10000;

            ConstDefinitionNodes = new List<ConstDefinitionNode>();
            ArrayDeclarationNodes = new List<IArrayDeclarationNode>();
            TypedSymbols = new List<ITypedSymbol>();
            FunctionSymbols = new List<FunctionSymbol>();
            SubclassSymbols = new List<SubclassSymbol>();
            SymbolsWithInstructions = new List<BlockSymbol>();
            ClassSymbols = new List<ClassSymbol>();
            
            DeclarationNodes = new List<DeclarationNode>();
        }

        public override void VisitTree(AbstractSyntaxTree tree)
        {
            base.VisitTree(tree);
            foreach (Symbol symbol in _stringConstSymbols)
            {
                AddSymbol(symbol);
            }
        }

        protected override void VisitFile(FileNode node)
        {
            _isInExternalFile = node.IsExternal;
            base.VisitFile(node);
        }


        
        protected override void VisitClassDefinition(ClassDefinitionNode classDefinitionNode)
        {
            string className = classDefinitionNode.NameNode.Value;
            ClassSymbol classSymbol = new ClassSymbol(className, classDefinitionNode);
            AddSymbol(classSymbol);
            Visit(classDefinitionNode.AttributeNodes);
        }

        protected override void VisitFunctionDefinition(FunctionDefinitionNode functionDefinitionNode)
        {
            string functionName = functionDefinitionNode.NameNode.Value;
            string returnTypeName = functionDefinitionNode.TypeNameCapitalized;
            bool isExternal = functionDefinitionNode.IsExternal;
            
            FunctionSymbol functionSymbol = new FunctionSymbol(returnTypeName, functionName, isExternal, functionDefinitionNode);
            AddSymbol(functionSymbol);
            Visit(functionDefinitionNode.ParameterNodes);
            Visit(functionDefinitionNode.BodyNodes);
        }

        protected override void VisitInstanceDefinition(InstanceDefinitionNode instanceDefinitionNode)
        {
            string instanceName = instanceDefinitionNode.NameNode.Value;
            InstanceSymbol instanceSymbol = new InstanceSymbol(instanceName, instanceDefinitionNode);
            AddSymbol(instanceSymbol);
            Visit(instanceDefinitionNode.BodyNodes);
        }

        protected override void VisitPrototypeDefinition(PrototypeDefinitionNode prototypeDefinitionNode)
        {
            string prototypeName = prototypeDefinitionNode.NameNode.Value;
            PrototypeSymbol prototypeSymbol = new PrototypeSymbol(prototypeName, prototypeDefinitionNode);
            AddSymbol(prototypeSymbol);
            Visit(prototypeDefinitionNode.BodyNodes);
        }

        protected override void VisitStringLiteral(StringLiteralNode stringLiteralNode)
        {
            string name = $"{(char)255}{_nextStringSymbolNumber++}";
            string value = stringLiteralNode.Value;
            StringConstSymbol stringConstSymbol = new StringConstSymbol(value, name, stringLiteralNode);
            _stringConstSymbols.Add(stringConstSymbol);
        }

        protected override void VisitConstDefinition(ConstDefinitionNode constDefinitionNode)
        {
            BuildConst(constDefinitionNode);
        }

        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode constArrayDefinitionNode)
        {
            BuildConst(constArrayDefinitionNode, true);
        }

        protected override void VisitParameterDeclaration(ParameterDeclarationNode parameterDeclarationNode)
        {
            BuildParameter(parameterDeclarationNode);
        }

        protected override void VisitParameterArrayDeclaration(ParameterArrayDeclarationNode parameterArrayDeclarationNode)
        {
            BuildParameter(parameterArrayDeclarationNode, true);
        }
        
        protected override void VisitVarDeclaration(VarDeclarationNode varDeclarationNode)
        {
            BuildVar(varDeclarationNode);
        }

        protected override void VisitBinaryExpression(BinaryExpressionNode node)
        {
            Visit(node.RightSideNode);
            Visit(node.LeftSideNode);
        }

        protected override void VisitVarArrayDeclaration(VarArrayDeclarationNode varArrayDeclarationNode)
        {
            BuildVar(varArrayDeclarationNode, true);
        }

        private void BuildParameter(ParameterDeclarationNode parameterDeclarationNode, bool buildArray=false)
        {
            if (parameterDeclarationNode.ParentNode is FunctionDefinitionNode functionDefinitionNode)
            {
                FunctionSymbol functionSymbol = (FunctionSymbol) functionDefinitionNode.Symbol;
                string parameterName = parameterDeclarationNode.NameNode.Value;
                string parameterTypeName = parameterDeclarationNode.TypeNameCapitalized;

                ParameterSymbol parameterSymbol;
                if (_isInExternalFile)
                {
                    parameterSymbol = buildArray
                        ? new ExternalParameterArraySymbol(functionSymbol, parameterTypeName, parameterName, parameterDeclarationNode)
                        : new ExternalParameterSymbol(functionSymbol, parameterTypeName, parameterName, parameterDeclarationNode);
                }
                else
                {
                    parameterSymbol = buildArray
                        ? new ParameterArraySymbol(functionSymbol, parameterTypeName, parameterName, parameterDeclarationNode)
                        : new ParameterSymbol(functionSymbol, parameterTypeName, parameterName, parameterDeclarationNode);
                }
                AddSymbol(parameterSymbol);
            }
            else
            {
                throw new Exception();
            }
        }



        private void BuildConst(ConstDefinitionNode constDefinitionNode, bool buildArray=false)
        {
            string constName = constDefinitionNode.NameNode.Value;
            string constTypeName = constDefinitionNode.TypeNameCapitalized;

            BlockSymbol parentBlockSymbol = GetParentBlockSymbol(constDefinitionNode);
            ConstSymbol constSymbol = buildArray ? new ConstArraySymbol(parentBlockSymbol, constTypeName, constName, constDefinitionNode) : new ConstSymbol(parentBlockSymbol, constTypeName, constName, constDefinitionNode);
            AddSymbol(constSymbol);
        }

     
        private void BuildVar(VarDeclarationNode varDeclarationNode, bool buildArray=false)
        {
            string varName = varDeclarationNode.NameNode.Value;
            string varTypeName = varDeclarationNode.TypeNameCapitalized;

            BlockSymbol parentBlockSymbol = GetParentBlockSymbol(varDeclarationNode);
            VarSymbol varSymbol;
            if (parentBlockSymbol is ClassSymbol)
            {
                varSymbol = buildArray ? new AttributeArraySymbol(parentBlockSymbol, varTypeName, varName, varDeclarationNode) : new AttributeSymbol(parentBlockSymbol, varTypeName, varName, varDeclarationNode);
            }
            else
            {
                varSymbol = buildArray ? new VarArraySymbol(parentBlockSymbol, varTypeName, varName, varDeclarationNode) : new VarSymbol(parentBlockSymbol, varTypeName, varName, varDeclarationNode);

            }
            
            AddSymbol(varSymbol);
        }
        
        
        private void AddSymbol(Symbol symbol)
        {
            symbol.Index = _nextSymbolIndex++;
            
            /*
            if (IsCurrentlyParsingExternals)
            {
                if (symbol.BuiltinType == DatSymbolType.Func && symbol.Flags.HasFlag(DatSymbolFlag.Const))
                {
                    symbol.Flags |= DatSymbolFlag.External;
                }

                if (symbol.Name == "instance_help")
                {
                    symbol.Name = $"{(char) 255}INSTANCE_HELP";
                }
            } 
             */

            if (_isInExternalFile)
            {
                symbol.IsExternal = true;
                
                if (symbol.Name == "instance_help")
                {
                    symbol.Name = $"{(char) 255}{symbol.Name}";
                    symbol.Path = $"{(char) 255}{symbol.Path.ToUpper()}";
                }
            }

            if (SymbolTable.ContainsKey(symbol.Path))
            {
                Symbol previousSymbol = SymbolTable[symbol.Path];
                
                ASTNode node = symbol.Node;
                NodeLocation location = node.Location;
                if (node is DeclarationNode declarationNode)
                {
                    location = declarationNode.NameNode.Location;
                }

                ASTNode previousNode = previousSymbol.Node;
                NodeLocation previousLocation = previousNode.Location;
                if (previousNode is DeclarationNode previousDeclarationNode)
                {
                    previousLocation = previousDeclarationNode.NameNode.Location;
                }

                symbol.Node.Annotations.Add(new RedefinedIdentifierError(symbol.Name, previousLocation, location));

                return;
            }

            string[] keywords = {"break", "continue", "while"};
            if (keywords.Contains(symbol.Name.ToLower()))
            {
                if (symbol.Node is DeclarationNode declarationNode)
                {
                    symbol.Node.Annotations.Add(new KeywordUsedAsNameError(symbol.Name, declarationNode.NameNode.Location));
                }
                else
                {
                    throw new Exception();
                }
                
            }
            
            switch (symbol)
            {
                case FunctionSymbol functionSymbol:
                    //FunctionSymbols.Add(functionSymbol);
                    TypedSymbols.Add(functionSymbol);
                    if (!functionSymbol.IsExternal)
                    {
                        SymbolsWithInstructions.Add(functionSymbol);
                    }
                    break;
                
                case ITypedSymbol typedSymbol:
                    TypedSymbols.Add(typedSymbol);
                    break;
                
                
                
                
                case SubclassSymbol subclassSymbol:
                    SubclassSymbols.Add(subclassSymbol);
                    if (!subclassSymbol.IsExternal)
                    {
                        SymbolsWithInstructions.Add(subclassSymbol);
                    }
                    break;
                case ClassSymbol classSymbol:
                    ClassSymbols.Add(classSymbol);
                    break;
            }
            
            
            switch (symbol.Node)
            {
                case DeclarationNode declarationNode:
                    declarationNode.Symbol = symbol;
                    DeclarationNodes.Add(declarationNode);
                    break;
                case ReferenceNode referenceNode:
                    referenceNode.Symbol = symbol;
                    break;
                case StringLiteralNode stringLiteralNode:
                    stringLiteralNode.Symbol = (StringConstSymbol) symbol;
                    break;
            }

            switch (symbol.Node)
            {
                case IArrayDeclarationNode arrayDeclarationNode:
                    ArrayDeclarationNodes.Add(arrayDeclarationNode);
                    break;
                
                case ConstDefinitionNode constDefinitionNode:
                    ConstDefinitionNodes.Add(constDefinitionNode);
                    break;
            }
            
            SymbolTable[symbol.Path] = symbol;
        }
        
        
        private DeclarationNode GetParentBlockNode(ASTNode node)
        {
            while (node != null)
            {
                switch (node)
                {
                    case PrototypeDefinitionNode prototypeDefinitionNode:
                        return prototypeDefinitionNode;
                    
                    case InstanceDefinitionNode instanceDefinitionNode:
                        return instanceDefinitionNode;
                    
                    case FunctionDefinitionNode functionDefinitionNode:
                        return functionDefinitionNode;
                }
                node = node.ParentNode;
                
            }
            throw new Exception();
        }

        private BlockSymbol GetParentBlockSymbol(ASTNode node)
        {
            node = node.ParentNode;
            switch (node)
            {
                case FileNode _:
                    return null;
                
                case ClassDefinitionNode classDefinitionNode:
                    return (BlockSymbol) classDefinitionNode.Symbol;

                default:
                    DeclarationNode parentBlockNode = GetParentBlockNode(node);
                    switch (parentBlockNode)
                    {
                        case PrototypeDefinitionNode prototypeDefinitionNode:
                            return (BlockSymbol) prototypeDefinitionNode.Symbol;
                    
                        case InstanceDefinitionNode instanceDefinitionNode:
                            return (BlockSymbol) instanceDefinitionNode.Symbol;
                    
                        case FunctionDefinitionNode functionDefinitionNode:
                            return (BlockSymbol) functionDefinitionNode.Symbol;
                        
                        default:
                            throw new Exception();
                    }
            }
        }
    }
}