using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Common.Zen;

namespace Common.SemanticAnalysis
{
    public class DeclarationUsagesChecker
    {
        private readonly Dictionary<string, Symbol> _symbolTable;

        private readonly HashSet<string> _engineUsedSymbols = new HashSet<string>
        {
            // functions
            "B_ASSESSMAGIC",
            "B_REFRESHARMOR",
            "B_REFRESHATINSERT",
            "C_CANNPCCOLLIDEWITHSPELL",
            "C_DROPUNCONSCIOUS",
            "G_CANNOTCAST",
            "G_CANNOTUSE",
            "G_CANSTEAL",
            "G_PICKLOCK",
            "INITPERCEPTIONS",
            "INIT_GLOBAL",
            "PLAYER_HOTKEY_LAME_HEAL",
            "PLAYER_HOTKEY_LAME_POTION",
            "PLAYER_HOTKEY_SCREEN_MAP",
            "PLAYER_MOB_ANOTHER_IS_USING",
            "PLAYER_MOB_MISSING_ITEM",
            "PLAYER_MOB_MISSING_KEY",
            "PLAYER_MOB_MISSING_KEY_OR_LOCKPICK",
            "PLAYER_MOB_MISSING_LOCKPICK",
            "PLAYER_MOB_NEVER_OPEN",
            "PLAYER_MOB_TOO_FAR_AWAY",
            "PLAYER_MOB_WRONG_SIDE",
            "PLAYER_PLUNDER_IS_EMPTY",
            "PLAYER_RANGED_NO_AMMO",
            "PLAYER_TRADE_NOT_ENOUGH_GOLD",
            "SPELL_PROCESSMANA",
            "SPELL_PROCESSMANA_RELEASE",
            "STARTUP_GLOBAL",
            "ZS_ASSESSMAGIC",
            "ZS_ASSESSSTOPMAGIC",
            "ZS_ATTACK",
            "ZS_DEAD",
            "ZS_MAGICBURN",
            "ZS_MAGICFREEZE",
            "ZS_MAGICSLEEP",
            "ZS_MM_ATTACK",
            "ZS_PYRO",
            "ZS_SHORTZAPPED",
            "ZS_TALK",
            "ZS_UNCONSCIOUS",
            "ZS_WHIRLWIND",
            "ZS_ZAPPED",

            // instances
            "FOCUS_MAGIC",
            "FOCUS_MELEE",
            "FOCUS_NORMAL",
            "FOCUS_RANGED",
            "FOCUS_THROW_ITEM",
            "FOCUS_THROW_MOB",

            // constants
            "BLOOD_DAMAGE_MAX",
            "BLOOD_SIZE_DIVISOR",
            "BS_AIMFAR",
            "BS_AIMNEAR",
            "BS_CASTING",
            "BS_CLIMB",
            "BS_CONTROLLING",
            "BS_CRAWL",
            "BS_DEAD",
            "BS_DIVE",
            "BS_DROPITEM",
            "BS_FALL",
            "BS_HIT",
            "BS_INVENTORY",
            "BS_ITEMINTERACT",
            "BS_JUMP",
            "BS_LIE",
            "BS_MOBINTERACT",
            "BS_MOBINTERACT_INTERRUPT",
            "BS_MOD_BURNING",
            "BS_MOD_CONTROLLED",
            "BS_MOD_DRUNK",
            "BS_MOD_HIDDEN",
            "BS_MOD_NUTS",
            "BS_MOD_TRANSFORMED",
            "BS_PARADE",
            "BS_PETRIFIED",
            "BS_PICKPOCKET",
            "BS_RUN",
            "BS_SIT",
            "BS_SNEAK",
            "BS_SPRINT",
            "BS_STAND",
            "BS_STUMBLE",
            "BS_SWIM",
            "BS_TAKEITEM",
            "BS_THROWITEM",
            "BS_UNCONSCIOUS",
            "BS_WALK",
            "DAMAGE_FLY_CM_MAX",
            "DAMAGE_FLY_CM_MIN",
            "DAMAGE_FLY_CM_PER_POINT",
            "DEFAULT",
            "FALSE",
            "GIL_ATTITUDES",
            "HAI_TIME_UNCONSCIOUS",
            "MOB_CLIMB",
            "MOB_LIE",
            "MOB_NOTINTERRUPTABLE",
            "MOB_SIT",
            "NAME_CURRENCY",
            "NPC_ANGRY_TIME",
            "NPC_ATTACK_FINISH_DISTANCE",
            "NPC_BURN_DAMAGE_POINTS_PER_INTERVALL",
            "NPC_BURN_TICKS_PER_DAMAGE_POINT",
            "NPC_COLLISION_CORRECTION_SCALER",
            "NPC_DAM_DIVE_TIME",
            "NPC_MINIMAL_DAMAGE",
            "NPC_MINIMAL_PERCENT",
            "NPC_VOICE_VARIATION_MAX",
            "PLAYER_PERC_ASSESSMAGIC",
            "RANGED_CHANCE_MAXDIST",
            "RANGED_CHANCE_MINDIST",
            "SPELLFXANILETTERS",
            "SPELLFXINSTANCENAMES",
            "SVM_MODULES",
            "TEXT_FONT_10",
            "TEXT_FONT_20",
            "TEXT_FONT_DEFAULT",
            "TEXT_FONT_INVENTORY",
            "TRADE_CURRENCY_INSTANCE",
            "TRADE_VALUE_MULTIPLIER",
            "TRUE",
            "TXT_GUILDS",
            "TXT_SPELLS",
            "TXT_SPELLS_DESC",
            "TXT_TALENTS",
            "TXT_TALENTS_DESC",
            "TXT_TALENTS_SKILLS",
            "VIEW_TIME_PER_CHAR",
        };

        private readonly HashSet<string> _onStateFuncs = new HashSet<string>();

        private readonly HashSet<string> _conditionFuncs = new HashSet<string>();
        private readonly HashSet<string> _scriptFuncs = new HashSet<string>();

        private readonly HashSet<string> _focusNames = new HashSet<string>();

        /*
        It does look like some triggers have Daedalus function associated with them.
        Not sure if they really work, and also, it's only 3 of them,
        so I guess it's not important to have triggerVobNames;
        */
        // HashSet<string> triggerVobNames = new HashSet<string>();

        public DeclarationUsagesChecker(Dictionary<string, Symbol> symbolTable, List<ZenFileNode> zenFileNodes)
        {
            _symbolTable = symbolTable;
            foreach (ZenFileNode zenFileNode in zenFileNodes)
            {
                if (zenFileNode.VobTree == null)
                {
                    continue;
                }

                foreach (ZenNode zenNode in zenFileNode.VobTree.Children)
                {
                    if (zenNode is ZenAttrNode)
                    {
                        continue;
                    }

                    ZenBlockNode blockNode = (ZenBlockNode) zenNode;

                    foreach (ZenNode childNode in blockNode.Children)
                    {
                        if (childNode is ZenBlockNode)
                        {
                            continue;
                        }

                        ZenAttrNode attrNode = (ZenAttrNode) childNode;

                        if (attrNode.TextValue == String.Empty)
                        {
                            continue;
                        }

                        string textValueUpper = attrNode.TextValue.ToUpper();
                        
                        switch (attrNode.Name.ToUpper())
                        {
                            case "ONSTATEFUNC":
                                _onStateFuncs.Add(textValueUpper + "_S1");
                                break;
                            case "CONDITIONFUNC":
                                _conditionFuncs.Add(textValueUpper);
                                break;
                            case "SCRIPTFUNC":
                                _scriptFuncs.Add(textValueUpper);
                                break;
                            case "FOCUSNAME":
                                _focusNames.Add(textValueUpper);
                                break;
                        }
                    }
                }
            }
        }

        public void Check(List<DeclarationNode> declarationNodes)
        {
            foreach (DeclarationNode declarationNode in declarationNodes)
            {
                // Ignore attributes of a class
                if (declarationNode is VarDeclarationNode && declarationNode.ParentNode is ClassDefinitionNode)
                {
                    continue;
                }

                if (declarationNode.Usages.Count == 0)
                {
                    // Ignore symbols used by engine
                    if (_engineUsedSymbols.Contains(declarationNode.NameNode.Value.ToUpper()))
                    {
                        continue;
                    }

                    // Ignore unused parameters of functions used by engine
                    if (declarationNode is ParameterDeclarationNode parameterDeclarationNode)
                    {
                        FunctionDefinitionNode functionDefinitionNode =
                            (FunctionDefinitionNode) parameterDeclarationNode.ParentNode;

                        if (functionDefinitionNode.IsExternal)
                        {
                            continue;
                        }

                        string parentNameUpper = functionDefinitionNode.NameNode.Value.ToUpper();
                        if (_engineUsedSymbols.Contains(parentNameUpper))
                        {
                            continue;
                        }

                        if (parentNameUpper.StartsWith("SPELL_CAST_"))
                        {
                            continue;
                        }
                    }

                    if (declarationNode is FunctionDefinitionNode funcDefinitionNode)
                    {
                        // External functions shouldn't be required to be used
                        if (funcDefinitionNode.IsExternal)
                        {
                            continue;
                        }

                        // Functions named INIT_<ZEN_NAME> and STARTUP_<ZEN_NAME> are used by the engine
                        string nameUpper = funcDefinitionNode.NameNode.Value.ToUpper();
                        if (nameUpper.StartsWith("INIT_") || nameUpper.StartsWith("STARTUP_"))
                        {
                            continue;
                        }

                        // Functions named <NAME>_LOOP and <NAME>_END are used by the engine
                        // if there is function <NAME> that is registered state.
                        Regex stateRegex = new Regex("^([a-zA-Z_][a-zA-Z0-9_]*)_(LOOP|END)$");
                        Match match = stateRegex.Match(nameUpper);
                        if (match.Success && _symbolTable.ContainsKey(match.Groups[0].Value))
                        {
                            continue;
                        }

                        // Detecting usages of routine functions is very hard and in some cases next to impossible
                        // or even impossible, since they are referenced by their middle part (between RTN_ and _<NUM>) by string
                        Regex regex = new Regex("^RTN_[_A-Z0-9]+_[0-9]+$");
                        if (regex.IsMatch(nameUpper))
                        {
                            continue;
                        }

                        if (_scriptFuncs.Contains(nameUpper))
                        {
                            continue;
                        }

                        if (_conditionFuncs.Contains(nameUpper))
                        {
                            continue;
                        }

                        if (nameUpper.EndsWith("_S1") && _onStateFuncs.Contains(nameUpper))
                        {
                            continue;
                        }

                        if (nameUpper.StartsWith("SPELL_CAST_"))
                        {
                            continue;
                        }
                    }

                    if (declarationNode is InstanceDefinitionNode)
                    {
                        // We can just ignore all instances, since almost always
                        // they can be used even without explicit reference in code
                        continue;
                    }

                    // Some constants are used inside Zen in "focusName" attribute
                    if (declarationNode is ConstDefinitionNode constDefinitionNode)
                    {
                        if (_focusNames.Contains(constDefinitionNode.NameNode.Value.ToUpper()))
                        {
                            continue;
                        }
                    }

                    declarationNode.NameNode.Annotations.Add(new UnusedSymbolWarning());
                    continue;
                }
            }
            
            
            foreach (DeclarationNode declarationNode in declarationNodes)
            {
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
                        node.Annotations.Add(new NamesNotMatchingCaseWiseWarning(declarationNode.NameNode.Location,
                            declaredName, usedName));
                    }
                }
            }
            
        }
    }
}