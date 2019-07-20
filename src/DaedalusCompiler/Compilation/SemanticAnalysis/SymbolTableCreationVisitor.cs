using System;
using System.Collections.Generic;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class SymbolTableCreationVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private readonly Dictionary <string, DatSymbol> _symbolTable;
        private bool _isInExternal;
        private int _nextSymbolIndex;
        private int _nextStringSymbolNumber;

        public SymbolTableCreationVisitor()
        {
            _symbolTable = new Dictionary<string, DatSymbol>();
            _isInExternal = false;
            _nextSymbolIndex = 0;
            _nextStringSymbolNumber = 10000;
        }

        protected override void VisitFile(FileNode node)
        {
            _isInExternal = node.IsExternal;
            base.VisitFile(node);
        }

        protected override void VisitClassDefinition(ClassDefinitionNode classDefinitionNode)
        {
            string className = classDefinitionNode.NameNode.Value;
            
            if (_symbolTable.ContainsKey(className))
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
            DatSymbol prototypeSymbol = SymbolBuilder.BuildInstance(prototypeName);
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
        }

        protected override void VisitParameterDeclaration(ParameterDeclarationNode parameterDeclarationNode)
        {
            BuildParameter(parameterDeclarationNode);
        }

        protected override void VisitParameterArrayDeclaration(ParameterArrayDeclarationNode parameterArrayDeclarationNode)
        {
            BuildParameter(parameterArrayDeclarationNode);
        }
        
        protected override void VisitVarDeclaration(VarDeclarationNode varDeclarationNode)
        {
            BuildVar(varDeclarationNode);
        }

        protected override void VisitVarArrayDeclaration(VarArrayDeclarationNode varArrayDeclarationNode)
        {
            BuildVar(varArrayDeclarationNode);
        }

        private void BuildParameter(ParameterDeclarationNode parameterDeclarationNode)
        {
            string varName = parameterDeclarationNode.NameNode.Value;
            DatSymbolType varType = GetBuiltinType(parameterDeclarationNode.TypeName);
            DatSymbol parameterSymbol = _isInExternal ? SymbolBuilder.BuildExternalParameter(varName, varType) : SymbolBuilder.BuildParameter(varName, varType);
            AddSymbol(parameterSymbol, parameterDeclarationNode);
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
                case FunctionDefinitionNode functionDefinitionNode:
                    DatSymbol functionSymbol = functionDefinitionNode.Symbol;
                    constSymbol = SymbolBuilder.BuildLocalConst(constName, constType, functionSymbol);
                    break;
                case FileNode _:
                    constSymbol = SymbolBuilder.BuildGlobalConst(constName, constType);
                    break;
                default:
                    throw new Exception();
            }
            
            AddSymbol(constSymbol, constDefinitionNode);
            
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
                case FunctionDefinitionNode functionDefinitionNode:
                    DatSymbol functionSymbol = functionDefinitionNode.Symbol;
                    varSymbol = SymbolBuilder.BuildLocalVar(varName, varType, functionSymbol);
                    break;
                case FileNode _:
                    varSymbol = SymbolBuilder.BuildGlobalVar(varName, varType);
                    break;
                default:
                    throw new Exception();
            }
            
            AddSymbol(varSymbol, varDeclarationNode);
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

            _symbolTable[symbol.Name.ToUpper()] = symbol;
        }
    }
}