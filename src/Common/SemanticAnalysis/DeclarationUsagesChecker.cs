using System;
using System.Collections.Generic;

namespace Common.SemanticAnalysis
{
    public class DeclarationUsagesChecker
    {
        public void Check(List<DeclarationNode> declarationNodes)
        {
            foreach (DeclarationNode declarationNode in declarationNodes)
            {
                if (declarationNode.Usages.Count == 0)
                {
                    /*
                     * TODO there are problems with detecting unused symbols:
                     * 1. We need to know what .ZENs there are since, there could be INIT_ZENNAME and STARTUP_ZENNAME function for each .ZEN.
                     * 2. We need to check every .ZEN for function usages
                     * 3  We need to ignore routine functions like Rtn_something_1500, because they are referenced by partial string ("something" in this case)
                     * 4. We need to have list of symbols used internally by the engine.
                     * Current solution is temporary one (ignoring every symbol but local variables and parameters)
                     */
                    if (declarationNode is ParameterDeclarationNode parameterDeclarationNode &&
                        parameterDeclarationNode.ParentNode is FunctionDefinitionNode functionDefinitionNode &&
                        functionDefinitionNode.IsExternal)
                    {
                        continue;
                    }
                    
                    if (!(declarationNode is VarDeclarationNode varDeclarationNode && varDeclarationNode.ParentNode is FunctionDefinitionNode))
                    {
                        continue;
                    }
                    declarationNode.NameNode.Annotations.Add(new UnusedSymbolWarning());
                    continue;
                }


                string declaredName = declarationNode.NameNode.Value;
                
                foreach (ASTNode node in declarationNode.Usages)
                {
                    string usedName;

                    switch (node)
                    {
                        case ReferenceNode referenceNode:
                            usedName = referenceNode.Name;
                            break;
                        case AttributeNode attributeNode:
                            usedName = attributeNode.Name;
                            break;
                        case NameNode nameNode:
                            usedName = nameNode.Value;
                            break;
                        default:
                            throw new Exception();
                    }

                    if (usedName != declaredName)
                    {
                        node.Annotations.Add(new NamesNotMatchingCaseWiseWarning(declarationNode.NameNode.Location, declaredName, usedName));
                    }
                }
            }
        }
    }
}