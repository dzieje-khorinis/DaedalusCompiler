using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Common.Zen;

namespace Common.SemanticAnalysis
{
    public class DeclarationUsagesChecker
    {
        private readonly Dictionary <string, Symbol> _symbolTable;

        private HashSet<string> _engineUsedSymbols = new HashSet<string> {
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

        private HashSet<string> _onStateFuncs = new HashSet<string>();

        private HashSet<string> _conditionFuncs = new HashSet<string>();
        private HashSet<string> _scriptFuncs = new HashSet<string>();

        private HashSet<string> _focusNames = new HashSet<string>();

        /*
        It does look like some triggers have Daedalus function associated with them.
        Not sure if they really work, and also, it's only 3 of them,
        so I guess it's not important to have triggerVobNames;
        */
        // HashSet<string> triggerVobNames = new HashSet<string>();

        public DeclarationUsagesChecker(Dictionary<string, Symbol> symbolTable, List<ZenFileNode> zenFileNodes)
        {
            _symbolTable = symbolTable;
            foreach(ZenFileNode zenFileNode in zenFileNodes)
            {
                if (zenFileNode.VobTree == null)
                {
                    continue;
                }
                foreach(ZenNode zenNode in zenFileNode.VobTree.Children)
                {
                    if (zenNode is ZenAttrNode)
                    {
                        continue;
                    }
                    ZenBlockNode blockNode = (ZenBlockNode) zenNode;

                    foreach(ZenNode childNode in blockNode.Children)
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

                        switch(attrNode.Name.ToUpper())
                        {
                            case "ONSTATEFUNC":
                                _onStateFuncs.Add(attrNode.TextValue + "_S1");
                                break;
                            case "CONDITIONFUNC":
                                _conditionFuncs.Add(attrNode.TextValue);
                                break;
                            case "SCRIPTFUNC":
                                _scriptFuncs.Add(attrNode.TextValue);
                                break;
                            case "FOCUSNAME":
                                _focusNames.Add(attrNode.TextValue);
                                break;
                            
                        }
                    }
                }
            }
        }

        public void Check(List<DeclarationNode> declarationNodes)
        {
            //GET FROM ZEN
            // onStateFuncs + _S1
            // conditionFuncs
            // scriptFuncs
            // focusNames
            // plus, there are like 3 func that are the same as Trigger vobName


            // HashSet<string> onStateFuncs = new HashSet<string> {
            //     "MAKERUNE_S1",
            //     "SMITHWEAPON_S1",
            //     "BOOKSTAND_MILTEN_03_S1",
            //     "SLEEPABIT_S1",
            //     "BOOKSTAND_MILTEN_02_S1",
            //     "BOOKSTAND_MILTEN_01_S1",
            //     "EVT_OC_MAINGATE_FUNC_S1",
            //     "POTIONALCHEMY_S1",
            //     "BOOKSTAND_ENGOR_01_S1",
            //     "B_SPAWN_SKELETON_S1",
            //     "PRAYIDOL_S1",
            //     "EVT_TRUHE_OW_01_S1",
            //     "USE_BOOKSTANDALCHEMY2_S1",
            //     "USE_BOOKSTANDANIMALS2_S1",
            //     "USE_BOOKSTANDALCHEMY3_S1",
            //     "USE_BOOKSTANDANIMALS1_S1",
            //     "PRAYSHRINE_S1",
            //     "USE_BOOKSTANDHISTORY2_S1",
            //     "USE_BOOKSTANDALCHEMY1_S1",
            //     "USE_BOOKSTANDHISTORY1_S1",
            //     "USE_BOOKSTANDDEMENTOR_S1",
            //     "EVT_VINOSKELLEREI_FUNC_S1",
            //     "EVT_ORKOBERST_SWITCH_S1",
            //     "USE_BOOKSTAND_KREISE_01_S1",
            //     "USE_BOOKSTAND_KREISE_05_S1",
            //     "USE_BOOKSTAND_KREISE_06_S1",
            //     "USE_BOOKSTAND_KREISE_04_S1",
            //     "USE_BOOKSTAND_KREISE_03_S1",
            //     "USE_FINALDRAGONEQUIPMENT_S1",
            //     "USE_BOOKSTANDHISTORY3_S1",
            //     "USE_BOOKSTANDANIMALS3_S1",
            //     "EVT_OPEN_DOOR_LIBRARY_S1",
            //     "USE_BOOKSTAND_KREISE_02_S1",
            //     "USE_BOOKSTAND_01_S1",
            //     "EVT_MONASTERY_SECRETLIBRARY_S1",
            //     "B_SCGETTREASURE_S1",
            //     "PRAYIDOL_S1",
            //     "GOLDHACKEN_S1",
            //     "OPEN_ADANOSTEMPELCHEST_02_FUNC_S1",
            //     "OPEN_ADANOSTEMPELCHEST_01_FUNC_S1",
            //     "USE_BOOKSTANDMAYAHIERCHARY_02_S1",
            //     "USE_BOOKSTANDMAYAHIERCHARY_01_S1",
            //     "USE_BOOKSTANDMAYAHIERCHARY_03_S1",
            //     "USE_BOOKSTANDMAYAHIERCHARY_04_S1",
            //     "USE_BOOKSTANDMAYAHIERCHARY_05_S1",
            //     "GREGISBACK_S1",
            //     "B_OPENGREGSDOOR_S1",
            //     "USE_BOOKSTAND_ADDON_BL_S1",
            // };

            // HashSet<string> scriptFuncs = new HashSet<string> {
            //     "ENTER_OLDWORLD_FIRSTTIME_TRIGGER",
            //     "EVT_CAVALORNSGOBBOS_FUNC",
            //     "EVT_FINAL_DOOR_SAY_01",
            //     "EVT_UNDEADDRAGONDEAD_ENDSITUATION",
            //     "EVT_UNDEADDRAGON_TRAP_01_FUNC",
            //     "EVT_UNDEADDRAGON_TRAP_02_FUNC",
            //     "EVT_UNDEADDRAGON_TRAP_03_FUNC",
            //     "EVT_KEYMASTER_03",
            //     "EVT_LEFT_ROOM_02_SKEL_05",
            //     "EVT_LEFT_ROOM_02_SKEL_04",
            //     "EVT_LEFT_ROOM_02_SKEL_03",
            //     "EVT_LEFT_ROOM_02_SKEL_02",
            //     "EVT_LEFT_ROOM_02_SKEL_01",
            //     "EVT_RIGHT_UP_02_SKEL_03",
            //     "EVT_RIGHT_UP_02_SKEL_02",
            //     "EVT_RIGHT_UP_02_SKEL_01",
            //     "EVT_RIGHT_UP_01_SKEL_02",
            //     "EVT_RIGHT_UP_01_SKEL_01",
            //     "EVT_RIGHT_ROOM_01_SKEL_02",
            //     "EVT_RIGHT_ROOM_01_SKEL_01",
            //     "EVT_RIGHT_DOWN_01_SKEL_01",
            //     "EVT_RIGHT_DOWN_01_SKEL_02",
            //     "EVT_RIGHT_DOWN_01_SKEL_03",
            //     "EVT_RIGHT_DOWN_01_SKEL_04",
            //     "EVT_RIGHT_ROOM_02_SKEL_02",
            //     "EVT_RIGHT_ROOM_02_SKEL_01",
            //     "EVT_KEYMASTER_01",
            //     "EVT_KEYMASTER_02",
            //     "EVT_LEFT_UP_01_SPAWNNEW",
            //     "EVT_KEYMASTER_04",
            //     "EVT_LEFT_DOWN_01_SKEL_03",
            //     "EVT_LEFT_DOWN_01_SKEL_01",
            //     "EVT_LEFT_DOWN_01_SKEL_02",
            //     "EVT_ARCHOLDEAD_FUNC",
            //     "EVT_ORNAMENT_SWITCH_FOREST_01_FUNC",
            //     "EVT_TELEPORTSTATION_FUNC",
            //     "EVT_DIBRIDGE_OPEN_FUNC",
            //     "EVT_ORKOBERST",
            //     "ENTER_DI_FIRSTTIME_TRIGGER",
            //     "EVT_SC_ENTER_LIBRARY",
            //     "EVT_ORNAMENT_SWITCH_FARM_01_FUNC",
            //     "EVT_CRYPT_FINAL",
            //     "EVT_CRYPT_02_ENTRANCE",
            //     "EVT_CRYPT_02",
            //     "EVT_CRYPT_01_ENTRANCE",
            //     "EVT_CRYPT_01",
            //     "EVT_CRYPT_03",
            //     "EVT_CRYPT_03_ENTRANCE",
            //     "EVT_ORNAMENT_SWITCH_BIGFARM_01_FUNC",
            //     "B_EVENT_PORTAL_FIRST_EARTHQUAKE",
            //     "B_MAGECAVETRIGGER",
            //     "EVT_TROLL_GRAVE_01",
            //     "B_ADDON_PORTAL_ACTIVATED",
            //     "ADW_PORTALTEMPEL_FOCUS_FUNC",
            //     "VALLEY_SHOWCASE_TRIGGERSCRIPT_FUNC",
            //     "B_FUNCTION_TOUCH_AND_GET_KILLED",
            //     "EVT_RAVEN_AWAKE_FUNC",
            //     "ADW_ADANOSTEMPEL_STONEGRD_TRIGG_FUNC_02",
            //     "ADW_ADANOSTEMPEL_STONEGRD_TRIGG_FUNC_01",
            //     "SC_COMESINTO_CANYONLIBRARY_FUNC",
            //     "B_GIVESTUNTBONUS_FUNC",
            //     "ENTER_ADDONWORLD_FIRSTTIME_TRIGGER_FUNC",
            //     "EVT_ADDON_ADANOSDOOR_ENTRANCE_VOICESCRIPT",
            //     "B_RAVENSESCAPEINTOTEMPELAVI",
            // };

            // HashSet<string> focusNames = new HashSet<string> {
            //     "MOBNAME_CHEST",
            //     "MOBNAME_BBQ_SCAV",
            //     "MOBNAME_RUNEMAKER",
            //     "MOBNAME_ANVIL",
            //     "MOBNAME_CAULDRON",
            //     "MOBNAME_BOOKSTAND",
            //     "MOBNAME_DOOR",
            //     "MOBNAME_BENCH",
            //     "MOBNAME_BARBQ_SCAV",
            //     "MOBNAME_BED",
            //     "MOBNAME_CHAIR",
            //     "MOBNAME_WATERPIPE",
            //     "MOBNAME_THRONE",
            //     "MOBNAME_BUCKET",
            //     "MOBNAME_GRINDSTONE",
            //     "MOBNAME_FORGE",
            //     "MOBNAME_SWITCH",
            //     "MOBNAME_LAB",
            //     "MOBNAME_GRAVE_09",
            //     "MOBNAME_STOVE",
            //     "MOBNAME_ADDON_IDOL",
            //     "MOBNAME_ORE",
            //     "TORCH",
            //     "CAMP",
            //     "TORCH",
            //     "MOBNAME_INNOS",
            //     "MOBNAME_SAW",
            //     "MOBNAME_CITY",
            //     "MOBNAME_BOOKSBOARD",
            //     "MOBNAME_ARMCHAIR",
            //     "MOBNAME_GRAVE_18",
            //     "MOBNAME_DRAGONDOOR",
            //     "MOBNAME_BOOKBOARD",
            //     "MOBNAME_PRISON",
            //     "MOBNAME_PAN",
            //     "MOBNAME_GRAVE_33",
            //     "MOBNAME_GRAVE_32",
            //     "MOBNAME_GRAVE_31",
            //     "MOBNAME_GRAVE_30",
            //     "MOBNAME_GRAVE_29",
            //     "MOBNAME_GRAVE_28",
            //     "MOBNAME_CITY2",
            //     "MOBNAME_LIGHTHOUSE",
            //     "MOBNAME_WHEEL",
            //     "MOBNAME_GR_PEASANT",
            //     "MOBNAME_MONASTERY",
            //     "MOBNAME_PASSOW",
            //     "MOBNAME_TAVERN",
            //     "CHEST",
            //     "MOBNAME_TAVERN_01",
            //     "MOBNAME_SEAT",
            //     "MOBNAME_GRAVE_04",
            //     "MOBNAME_GRAVE_23",
            //     "MOBNAME_GRAVE_24",
            //     "MOBNAME_GRAVE_25",
            //     "MOBNAME_GRAVE_26",
            //     "MOBNAME_GRAVE_27",
            //     "MOBNAME_GRAVE_20",
            //     "MOBNAME_GRAVE_21",
            //     "MOBNAME_GRAVE_19",
            //     "MOBNAME_GRAVE_17",
            //     "MOBNAME_GRAVE_22",
            //     "MOBNAME_ADDON_ORNAMENTSWITCH",
            //     "MOBNAME_VORRATSKAMMER",
            //     "MOBNAME_MONASTERY2",
            //     "MOBNAME_LEVER",
            //     "MOBNAME_ALMANACH",
            //     "MOBNAME_LAB",
            //     "MOBNAME_DOOR",
            //     "MOBNAME_WINEMAKER",
            //     "MOBNAME_LIBRARYLEVER",
            //     "MOBNAME_BIBLIOTHEK",
            //     "MOBNAME_SCHATZKAMMER",
            //     "MOBNAME_IGARAZ",
            //     "CHEST",
            //     "MOBNAME_BBQ_SHEEP",
            //     "MOBNAME_REPAIR",
            //     "MOBNAME_GRAVE_10",
            //     "MOBNAME_GRAVETEAM_13",
            //     "MOBNAME_GRAVETEAM_14",
            //     "MOBNAME_GRAVETEAM_12",
            //     "MOBNAME_GRAVETEAM_11",
            //     "MOBNAME_GR_PEASANT2",
            //     "MOBNAME_GRAVETEAM_04",
            //     "MOBNAME_GRAVETEAM_09",
            //     "MOBNAME_GRAVETEAM_08",
            //     "MOBNAME_GRAVETEAM_06",
            //     "MOBNAME_GRAVETEAM_03",
            //     "MOBNAME_GRAVETEAM_07",
            //     "MOBNAME_GRAVETEAM_05",
            //     "MOBNAME_GRAVETEAM_01",
            //     "MOBNAME_GRAVETEAM_02",
            //     "MOBNAME_GRAVETEAM_10",
            //     "MOBNAME_GRAVE_11",
            //     "MOBNAME_GRAVE_07",
            //     "MOBNAME_GRAVE_03",
            //     "MOBNAME_GRAVE_12",
            //     "MOBNAME_GRAVE_14",
            //     "MOBNAME_GRAVE_13",
            //     "MOBNAME_GRAVETEAM_16",
            //     "MOBNAME_GRAVETEAM_15",
            //     "MOBNAME_INCITY03",
            //     "MOBNAME_INCITY04",
            //     "MOBNAME_INCITY05",
            //     "MOBNAME_INCITY02",
            //     "MOBNAME_SECRETSWITCH",
            //     "MOBNAME_BOOK",
            //     "MOBNAME_MIX_01",
            //     "MOBNAME_MIX_02",
            //     "MOBNAME_SMITH_01",
            //     "MOBNAME_BAR_01",
            //     "MOBNAME_BAR_02",
            //     "MOBNAME_HOTEL_02",
            //     "MOBNAME_HOTEL_01",
            //     "MOBNAME_BOW_01",
            //     "MOBNAME_INCITY01",
            //     "MOBNAME_SALANDRIL",
            //     "MOBNAME_ADDON_SOCKEL",
            //     "MOBNAME_ADDON_GOLD",
            //     "MOBNAME_ADDON_STONEBOOK",
            //     "MOBNAME_ADDON_TELEPORT_02",
            //     "MOBNAME_ADDON_TELEPORT_03",
            //     "MOBNAME_ADDON_TELEPORT_04",
            //     "MOBNAME_ADDON_TELEPORT_05",
            //     "MOBNAME_ADDON_TELEPORT_01",
            //     "MOBNAME_ADDON_FORTUNO",
            // };

            /*
            It does look like some triggers have Daedalus function associated with them
            BUT ONLY 3 of them, so I guess it's not important to have triggerVobNames;
            */
            // HashSet<string> triggerVobNames = new HashSet<string>
            // {
            //     "EVT_OC_MAINGATE_TRIGGER",
            //     "EVT_UNDEADDRAGON_TRAP_01_TRIGGERIN",
            //     "EVT_UNDEADDRAGON_TRAP_02_TRIGGERIN",
            //     "EVT_UNDEADDRAGON_TRAP_03_TRIGGERIN",
            //     "EVT_KEYMASTER_TRIGGERMASTER_SPAWN_04",
            //     "EVT_KEYMASTER_TRIGGERMASTER_SPAWN_03",
            //     "EVT_LEFT_UP_ROOM_02_TRIGGERMASTER_SPAWN_01",
            //     "EVT_RIGHT_UP_02_TRIGGERMASTER_SPAWN_01",
            //     "EVT_RIGHT_UP_01_TRIGGERMASTER_SPAWN_01",
            //     "EVT_RIGHT_DOWN_01_TRIGGERMASTER_SPAWN_01",
            //     "EVT_RIGHT_DOWN_01_TRIGGERMASTER_SPAWN_02",
            //     "EVT_RIGHT_UP_01_TRIGGERMASTER_SPAWN_02",
            //     "EVT_KEYMASTER_TRIGGERMASTER_SPAWN_01",
            //     "EVT_KEYMASTER_TRIGGERMASTER_SPAWN_02",
            //     "EVT_LEFT_DOWN_01_TRIGGERMASTER_SPAWN_01",
            //     "ISLE_BIGBRIDGE_FAR_TRIGGER_02",
            //     "ISLE_BIGBRIDGE_FAR_TRIGGER_01",
            //     "EVT_DOOR_PALSECRETCHAMBERTRIGGER",
            //     "EVT_CRYPT_ROOM_MIDDLE_ALLOPEN",
            //     "EVT_CRYPT_ROOM_02_TRIGGERENTRANCE",
            //     "EVT_CRYPT_02_TRIGGER",
            //     "EVT_CRYPT_ROOM_01_TRIGGERENTRANCE",
            //     "EVT_CRYPT_01_TRIGGER",
            //     "EVT_CRYPT_03_TRIGGER",
            //     "EVT_CRYPT_ROOM_03_TRIGGERENTRANCE",
            //     "NW_THIEVES_ISLAND_SPEAR_TRIGGER_01",
            //     "EVT_NW_TROLLAREA_TEMPELDOOR_MASTER_01",
            //     "EVT_ADDON_TROLLPORTAL_MASTER_01",
            //     "EVT_ADDON_TROLLPORTAL_CAMERAMASTER_01",
            //     "EVT_ADDON_TROLLPORTAL_PFX_MASTER_01",
            //     "EVT_ADANOS_ROOM03_MASTER_05",
            //     "EVT_ADANOS_ROOM03_MASTER_04",
            //     "EVT_ADANOS_ROOM04_MASTER_01",
            //     "EVT_ADANOS_ROOM03_MASTER_03",
            //     "EVT_ADANOS_ROOM03_MASTER_01",
            //     "EVT_ADANOS_ROOM03_TOUCHTRIGGER_01_01",
            //     "EVT_ADANOS_ROOM02_MASTER_00",
            //     "EVT_ADANOS_ROOM02_TOUCHTRIGGER_00_01",
            //     "EVT_ADANOS_ROOM03_TRAPMASTER_04",
            //     "EVT_ADANOS_ROOM03_TRAPMASTER_03",
            //     "EVT_ADANOS_ROOM02_MASTER_02",
            //     "EVT_ADANOS_ROOM02_MASTER_01",
            //     "EVT_ADANOS_ROOM01_MASTER_01",
            //     "EVT_ADANOS_ROOM01_MASTER_02",
            //     "EVT_ADANOS_ROOM02_MASTER_03",
            //     "EVT_ADANOS_ROOM02_MASTER_04",
            //     "EVT_ADANOS_ROOM02_MASTER_05",
            //     "EVT_ADANOS_ROOM02_TOUCHTRIGGER_05_01",
            //     "EVT_ADANOS_ROOM03_TRAPMASTER_01",
            //     "EVT_ADANOS_ROOM03_TRAPMASTER_02",
            //     "EVT_ADANOS_ROOM03_MASTER_02",
            //     "EVT_ADANOS_ROOM05_MASTER_01",
            //     "EVT_ADANOS_ROOM05_TOUCHTRIGGER_05_01",
            //     "EVT_ADANOS_ROOM05_TOUCHTRIGGER_05_02",
            //     "EVT_ADANOS_ROOM06_MASTER_01",
            //     "EVT_ADDON_LSTTMP_DOORMASTER_01",
            // };

            // HashSet<string> Result = new HashSet<string>();

            foreach (DeclarationNode declarationNode in declarationNodes)
            {
                if (declarationNode.Usages.Count == 0)
                {                   
                    // ignore symbols used by engine
                    if (_engineUsedSymbols.Contains(declarationNode.NameNode.Value.ToUpper()))
                    {
                        continue;
                    }

                    
                    // Ignore unused parameters of functions used by engine
                    if (declarationNode is ParameterDeclarationNode parameterDeclarationNode)
                    {
                        FunctionDefinitionNode functionDefinitionNode = (FunctionDefinitionNode) parameterDeclarationNode.ParentNode;

                        if (functionDefinitionNode.IsExternal) {
                             continue;
                        }

                        string parentNameUpper = functionDefinitionNode.NameNode.Value.ToUpper();
                        if (_engineUsedSymbols.Contains(parentNameUpper)) {
                            continue;
                        }

                        if (parentNameUpper.StartsWith("SPELL_CAST_")) {
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
                        if (match.Success && _symbolTable.ContainsKey(match.Groups[0].Value)) {
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

                        if (nameUpper.EndsWith("_S1") && _onStateFuncs.Contains(nameUpper)) {
                            continue;
                        }

                        if (nameUpper.StartsWith("SPELL_CAST_"))
                        {
                            continue;
                        }
                    }

                    if (declarationNode is InstanceDefinitionNode instanceDefinitionNode)
                    {
                        // actually we can just ignore all instances, since almost always
                        // they can be used even without explicit reference in code
                        continue;

                        // ClassSymbol baseClassSymbol = ((InstanceSymbol)instanceDefinitionNode.Symbol).BaseClassSymbol;
                        // string baseClassNameUpper = baseClassSymbol.Name.ToUpper();

                        // // C_INFO instances (dialogue options) are always assigned to NPC and can be used
                        // if (baseClassNameUpper == "C_INFO")
                        // {
                        //     continue;
                        // }

                        // // C_ITEM and C_SPELL instances can be always be spawned using codes
                        // // if wanted to search for unused items, should also search in Zen
                        // if (baseClassNameUpper == "C_ITEM" || baseClassNameUpper == "C_SPELL")
                        // {
                        //     continue;
                        // }

                        // // C_SVM classes are referenced by number (I think voice attr on NPC)
                        // if (baseClassNameUpper == "C_SVM")
                        // {
                        //     continue;
                        // }

                        // if (baseClassNameUpper == "C_GILVALUES")
                        // {
                        //     continue;
                        // }
                    }

                    // some consts are used inside Zen in "focusName" attribute
                    if (declarationNode is ConstDefinitionNode constDefinitionNode)
                    {
                        if (_focusNames.Contains(constDefinitionNode.NameNode.Value.ToUpper()))
                        {
                            continue;
                        }
                    }

                    // Result.Add(declarationNode.NameNode.Value.ToUpper());
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

            // Console.WriteLine($"Count: {Result.Count}");
            // foreach(string name in Result)
            // {
            //     Console.WriteLine(name);
            // }
        }
    }
}