using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    /// <summary>
    /// Class for creating symbols from attributes of class type. This allows nested attribute access.
    /// Example:
    /// <code>
    /// class Pet {
    ///     var int x;
    ///     var int y;
    /// };
    /// 
    /// class Human {
    ///     var int b;
    ///     var Pet dog;
    /// };
    /// 
    /// instance Person(Human);
    /// 
    /// func void myFunc() {
    ///     var int x;
    ///     x = Person.dog.x;
    /// };
    /// </code>
    /// Will create instance Person that has attributes:
    /// var int b;
    /// var int dog.x;
    /// var int dog.y;
    /// </summary>
    public class PrefixAttributesSymbolCreator
    {
        private HashSet<Symbol> _visitedSymbolsCurrentIteration;
        private readonly HashSet<Symbol> _symbolsInCircuralReferenceLoop;
        
        private readonly Dictionary <string, Symbol> _symbolTable;

        private readonly Dictionary<Symbol, List<NestableSymbol>> _classTypeAttribute2ExtraAttributesSymbols;

        public PrefixAttributesSymbolCreator(Dictionary<string, Symbol> symbolTable)
        {
            _symbolTable = symbolTable;
            _classTypeAttribute2ExtraAttributesSymbols = new Dictionary<Symbol, List<NestableSymbol>>();
            _symbolsInCircuralReferenceLoop = new HashSet<Symbol>();
        }

        public void Scan(List<ClassSymbol> classSymbols)
        {
            foreach (var classSymbol in classSymbols)
            {
                _visitedSymbolsCurrentIteration = new HashSet<Symbol>();
                Scan(classSymbol);
            }
            
            
            foreach (var pair in _classTypeAttribute2ExtraAttributesSymbols)
            {
                NestableSymbol nestableSymbol = (NestableSymbol) pair.Key;
                ClassSymbol classSymbol = (ClassSymbol) nestableSymbol.ParentBlockSymbol;
                List<NestableSymbol> extraAttributesSymbols = pair.Value;

                //classSymbol.RemoveBodySymbol(nestableSymbol);
                //_symbolTable.Remove(nestableSymbol.Path);
                
                
                for (int i = 0; i < extraAttributesSymbols.Count; ++i)
                {
                    NestableSymbol attributeSymbol = extraAttributesSymbols[i];
                    attributeSymbol.Index = nestableSymbol.Index;
                    attributeSymbol.SubIndex = i;
                    attributeSymbol.ParentBlockSymbol = classSymbol;
                    attributeSymbol.Path = $"{attributeSymbol.ParentBlockSymbol.Name}.{attributeSymbol.Name}".ToUpper();
                    classSymbol.AddBodySymbol(attributeSymbol);
                    _symbolTable[attributeSymbol.Path] = attributeSymbol;
                }
                
            }

        }
        
        private void Scan(ClassSymbol baseClassSymbol)
        {
            if (_symbolsInCircuralReferenceLoop.Contains(baseClassSymbol))
            {
                return;
            }
            
            _visitedSymbolsCurrentIteration.Add(baseClassSymbol);

            
            foreach (NestableSymbol nestableSymbol in baseClassSymbol.BodySymbols.Values)
            {
                if (nestableSymbol.ComplexType is ClassSymbol classSymbol)
                {
                    List<NestableSymbol> extraAttributes = CreateSymbolsFromClassTypeAttribute(nestableSymbol.Name, classSymbol, (CustomTypeDeclarationNode) nestableSymbol.Node);
                        _classTypeAttribute2ExtraAttributesSymbols[nestableSymbol] = extraAttributes;
                }
            }

        }
        
        
        
        private List<NestableSymbol> CreateSymbolsFromClassTypeAttribute(string prefix, ClassSymbol classSymbol, CustomTypeDeclarationNode node)
        {
            List<NestableSymbol> attributes = new List<NestableSymbol>();
            
            if (_symbolsInCircuralReferenceLoop.Contains(classSymbol))
            {
                return attributes;
            }
            
            if (_visitedSymbolsCurrentIteration.Contains(classSymbol))
            {
                _symbolsInCircuralReferenceLoop.UnionWith(_visitedSymbolsCurrentIteration);
                node.TypeNameNode.Annotations.Add(new InfiniteAttributeReferenceLoopError());
                return attributes;
            }
            _visitedSymbolsCurrentIteration.Add(classSymbol);   
            
            
            
            
            foreach (NestableSymbol bodySymbol in classSymbol.BodySymbols.Values)
            {
                //prefix = prefix == String.Empty ? bodySymbol.Name : $"{prefix}.{bodySymbol.Name}";
                string innerPrefix = $"{prefix}.{bodySymbol.Name}";
                
                if (bodySymbol.ComplexType is ClassSymbol innerClassSymbol)
                {
                    attributes.AddRange(CreateSymbolsFromClassTypeAttribute(innerPrefix, innerClassSymbol, node));
                }
                else if (bodySymbol.BuiltinType != SymbolType.Uninitialized)
                {
                    attributes.Add(new VarSymbol(null, bodySymbol.TypeName, innerPrefix, node));
                }
            }

            return attributes;
        }
    }
}