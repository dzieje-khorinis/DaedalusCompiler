using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class SymbolTableCreationVisitor : AbstractSyntaxTreeBaseVisitor
    {
        public readonly Dictionary <string, SymbolContext> SymbolTable;
        private bool _isInExternal;
        private int _nextSymbolIndex;
        private int _nextStringSymbolNumber;
        
        public readonly List<ConstDefinitionNode> ConstDefinitionNodes;
        public readonly List<IArrayDeclarationNode> ArrayDeclarationNodes;
        public readonly List<ParentReferenceNode> ParentReferenceNodes;
        public readonly List<ReferenceNode> ReferenceNodes;
        

        public SymbolTableCreationVisitor()
        {
            SymbolTable = new Dictionary<string, SymbolContext>();
            _isInExternal = false;
            _nextSymbolIndex = 0;
            _nextStringSymbolNumber = 10000;

            ConstDefinitionNodes = new List<ConstDefinitionNode>();
            ArrayDeclarationNodes = new List<IArrayDeclarationNode>();
            ParentReferenceNodes = new List<ParentReferenceNode>();
            ReferenceNodes = new List<ReferenceNode>();
        }

        protected override void VisitFile(FileNode node)
        {
            _isInExternal = node.IsExternal;
            base.VisitFile(node);
        }

        protected override void VisitClassDefinition(ClassDefinitionNode classDefinitionNode)
        {
            string className = classDefinitionNode.NameNode.Value;
            
            if (SymbolTable.ContainsKey(className))
            {
                return;
            }

            DatSymbol classSymbol = SymbolBuilder.BuildClass(className, classDefinitionNode.AttributeNodes.Count);
            AddSymbol(classSymbol, classDefinitionNode);

            int classVarOffset = classSymbol.ClassOffset;
            foreach (var declarationNode in classDefinitionNode.AttributeNodes)
            {
                Visit(declarationNode);
                declarationNode.Symbol.ClassVarOffset = classVarOffset;
                classVarOffset += declarationNode.Symbol.BuiltinType == DatSymbolType.String ? 20 : 4;
            }

            classSymbol.ClassSize = classVarOffset - classSymbol.ClassOffset;
        }

        protected override void VisitFunctionDefinition(FunctionDefinitionNode functionDefinitionNode)
        {
            string functionName = functionDefinitionNode.NameNode.Value;
            DatSymbolType returnType = GetBuiltinType(functionDefinitionNode.TypeName);
            int parametersCount = functionDefinitionNode.ParameterNodes.Count;
            
            DatSymbol functionSymbol = SymbolBuilder.BuildFunction(functionName, parametersCount, returnType);
            AddSymbol(functionSymbol, functionDefinitionNode);
            
            Visit(functionDefinitionNode.ParameterNodes);
            Visit(functionDefinitionNode.BodyNodes);
        }

        protected override void VisitInstanceDefinition(InstanceDefinitionNode instanceDefinitionNode)
        {
            string instanceName = instanceDefinitionNode.NameNode.Value;
            DatSymbol instanceSymbol = SymbolBuilder.BuildInstance(instanceName);
            AddSymbol(instanceSymbol, instanceDefinitionNode);
        }

        protected override void VisitPrototypeDefinition(PrototypeDefinitionNode prototypeDefinitionNode)
        {
            string prototypeName = prototypeDefinitionNode.NameNode.Value;
            DatSymbol prototypeSymbol = SymbolBuilder.BuildPrototype(prototypeName);
            AddSymbol(prototypeSymbol, prototypeDefinitionNode);
        }

        protected override void VisitStringLiteral(StringLiteralNode stringLiteralNode)
        {
            string name = $"{(char)255}{_nextStringSymbolNumber++}";
            string value = stringLiteralNode.Value;
            DatSymbol stringConstSymbol = SymbolBuilder.BuildStringConst(name, value);
            AddSymbol(stringConstSymbol, stringLiteralNode);
        }

        protected override void VisitConstDefinition(ConstDefinitionNode constDefinitionNode)
        {
            BuildConst(constDefinitionNode);
        }

        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode constArrayDefinitionNode)
        {
            BuildConst(constArrayDefinitionNode);
            if (!IsArrayTypeSupported(constArrayDefinitionNode.TypeName))
            {
                constArrayDefinitionNode.Annotations.Add(new UnsupportedArrayTypeAnnotation());
            }
        }

        protected override void VisitParameterDeclaration(ParameterDeclarationNode parameterDeclarationNode)
        {
            BuildParameter(parameterDeclarationNode);
        }

        protected override void VisitParameterArrayDeclaration(ParameterArrayDeclarationNode parameterArrayDeclarationNode)
        {
            BuildParameter(parameterArrayDeclarationNode);
            if (!IsArrayTypeSupported(parameterArrayDeclarationNode.TypeName))
            {
                parameterArrayDeclarationNode.Annotations.Add(new UnsupportedArrayTypeAnnotation());
            }
        }
        
        protected override void VisitVarDeclaration(VarDeclarationNode varDeclarationNode)
        {
            BuildVar(varDeclarationNode);
        }

        protected override void VisitVarArrayDeclaration(VarArrayDeclarationNode varArrayDeclarationNode)
        {
            BuildVar(varArrayDeclarationNode);
            if (!IsArrayTypeSupported(varArrayDeclarationNode.TypeName))
            {
                varArrayDeclarationNode.Annotations.Add(new UnsupportedArrayTypeAnnotation());
            }
        }

        private void BuildParameter(ParameterDeclarationNode parameterDeclarationNode)
        {
            if (parameterDeclarationNode.Parent is FunctionDefinitionNode functionDefinitionNode)
            {
                DatSymbol functionSymbol = functionDefinitionNode.Symbol;
                string varName = parameterDeclarationNode.NameNode.Value;
                DatSymbolType varType = GetBuiltinType(parameterDeclarationNode.TypeName);
                DatSymbol parameterSymbol = _isInExternal ? SymbolBuilder.BuildExternalParameter(varName, varType, functionSymbol) : SymbolBuilder.BuildParameter(varName, varType, functionSymbol);
                AddSymbol(parameterSymbol, parameterDeclarationNode);
            }
            else
            {
                throw new Exception();
            }
            
            
        }

        private void BuildConst(ConstDefinitionNode constDefinitionNode)
        {
            string constName = constDefinitionNode.NameNode.Value;
            DatSymbolType constType = GetBuiltinType(constDefinitionNode.TypeName);
            
            DatSymbol constSymbol;

            switch (constDefinitionNode.Parent)
            {
                case ClassDefinitionNode classDefinitionNode:
                    DatSymbol classSymbol = classDefinitionNode.Symbol;
                    constSymbol = SymbolBuilder.BuildClassConst(constName, constType, classSymbol);
                    break;
                case FileNode _:
                    constSymbol = SymbolBuilder.BuildGlobalConst(constName, constType);
                    break;
                default:
                    var functionDefinitionNode = GetAncestorFunctionDefinitionNode(constDefinitionNode);
                    DatSymbol functionSymbol = functionDefinitionNode.Symbol;
                    constSymbol = SymbolBuilder.BuildLocalConst(constName, constType, functionSymbol);
                    break;
            }

            AddSymbol(constSymbol, constDefinitionNode);
        }

        private FunctionDefinitionNode GetAncestorFunctionDefinitionNode(ASTNode node)
        {
            while (node != null)
            {
                node = node.Parent;
                if (node is FunctionDefinitionNode functionDefinitionNode)
                {
                    return functionDefinitionNode;
                }
            }
            throw new Exception();
        }
        private void BuildVar(VarDeclarationNode varDeclarationNode)
        {
            string varName = varDeclarationNode.NameNode.Value;
            DatSymbolType varType = GetBuiltinType(varDeclarationNode.TypeName);

            DatSymbol varSymbol;

            switch (varDeclarationNode.Parent)
            {
                case ClassDefinitionNode classDefinitionNode:
                    DatSymbol classSymbol = classDefinitionNode.Symbol;
                    varSymbol = SymbolBuilder.BuildClassVar(varName, varType, classSymbol);
                    break;
                case FileNode _:
                    varSymbol = SymbolBuilder.BuildGlobalVar(varName, varType);
                    break;
                default:
                    var functionDefinitionNode = GetAncestorFunctionDefinitionNode(varDeclarationNode);
                    DatSymbol functionSymbol = functionDefinitionNode.Symbol;
                    varSymbol = SymbolBuilder.BuildLocalVar(varName, varType, functionSymbol);
                    break;
            }
            
            AddSymbol(varSymbol, varDeclarationNode);
        }

        private bool IsArrayTypeSupported(string typeName)
        {
            switch (typeName.ToUpper())
            {
                case "INT":
                case "STRING":
                case "FUNC":
                    return true;
                default:
                    return false;
            }
        }
        private DatSymbolType GetBuiltinType(string type)
        {
            if(Enum.TryParse(type, out DatSymbolType symbolType))
            {
                return symbolType;
            }

            return DatSymbolType.Undefined;
        }
        
        private void AddSymbol(DatSymbol symbol, DeclarationNode node)
        {
            node.Symbol = symbol;
            symbol.ComplexTypeName = node.TypeName;
            AddSymbol(symbol, (ASTNode)node);
        }
        
        private void AddSymbol(DatSymbol symbol, StringLiteralNode node)
        {
            node.Symbol = symbol;
            AddSymbol(symbol, (ASTNode)node);
        }

        private void AddSymbol(DatSymbol symbol, ASTNode node)
        {
            symbol.Index = _nextSymbolIndex++;
            symbol.Location = node.Location;

            switch (node)
            {
                case IArrayDeclarationNode arrayDeclarationNode:
                    ArrayDeclarationNodes.Add(arrayDeclarationNode);
                    break;
                
                case ConstDefinitionNode constDefinitionNode:
                    ConstDefinitionNodes.Add(constDefinitionNode);
                    break;
                
                case ParentReferenceNode parentReferenceNode:
                    ParentReferenceNodes.Add(parentReferenceNode);
                    break;
                
                case ReferenceNode referenceNode:
                    ReferenceNodes.Add(referenceNode);
                    break;
            }

            string nameUpper = symbol.Name.ToUpper();
            if (SymbolTable.ContainsKey(nameUpper))
            {
                node.Annotations.Add(new RedefinedIdentifierAnnotation());
                return;
            }

            SymbolTable[nameUpper] = new SymbolContext {Symbol = symbol, Node = node};
        }
    }
}