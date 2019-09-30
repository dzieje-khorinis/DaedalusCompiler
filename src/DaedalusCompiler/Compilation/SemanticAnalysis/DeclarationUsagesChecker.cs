using System;
using System.Collections.Generic;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class DeclarationUsagesChecker
    {
        
        
        public void Check(List<DeclarationNode> declarationNodes)
        {
            foreach (DeclarationNode declarationNode in declarationNodes)
            {
                if (declarationNode.Usages.Count == 0)
                {
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