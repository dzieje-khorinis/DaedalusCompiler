using System.Collections.Generic;
using DaedalusCompiler.Compilation;
using Xunit;


namespace DaedalusCompiler.Tests
{
    public class ParsingExtendedSyntaxToAbstractAssemblyTests : ParsingSourceToAbstractAssemblyTestsBase
    {
       
        [Fact]
        public void TestWhileLoop()
        {
            Code = @"
                func void firstFunc() {
                    var int x;
                    x = 0;
                    while(x < 5) {
                        x += 1;
                    }
                };
           ";
            
            Instructions = GetExecBlockInstructions("firstFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // x = 0;
                new PushInt(0),
                new PushVar(Ref("firstFunc.x")),
                new Assign(),
                
                // while(x < 5) {
                new AssemblyLabel("#0001_while"),
                new PushInt(5),
                new PushVar(Ref("firstFunc.x")),
                new Less(),
                new JumpIfToLabel("#0001_endwhile"),
                
                //     x += 1;
                new PushInt(1),
                new PushVar(Ref("firstFunc.x")),
                new AssignAdd(),
                
                // }
                new JumpToLabel("#0001_while"),
                new AssemblyLabel("#0001_endwhile"),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("firstFunc"),
                Ref("firstFunc.x"),
            };
            AssertSymbolsMatch();
            
        }
        
        
        [Fact]
        public void TestWhileLoopBreakContinue()
        {
            Code = @"                
                func void secondFunc() {
                    var int x;
                    x = 0;
                    while(x < 5) {
                        x += 1;
                        if (x == 3) {
                            break;
                        } else if (x == 4) {
                            continue;
                        }
                    }
                };
           ";

            Instructions = GetExecBlockInstructions("secondFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // x = 0;
                new PushInt(0),
                new PushVar(Ref("secondFunc.x")),
                new Assign(),
                
                // while(x < 5) {
                new AssemblyLabel("#0001_while"),
                new PushInt(5),
                new PushVar(Ref("secondFunc.x")),
                new Less(),
                new JumpIfToLabel("#0001_endwhile"),
                
                //     x += 1;
                new PushInt(1),
                new PushVar(Ref("secondFunc.x")),
                new AssignAdd(),
                
                //     if (x == 3) {
                new PushInt(3),
                new PushVar(Ref("secondFunc.x")),
                new Equal(),
                new JumpIfToLabel("#0001_else_if_1"),
                
                //         break;
                new JumpToLabel("#0001_endwhile"),
                
                //     } else if (x == 4) {
                new JumpToLabel("#0001_endif"),
                new AssemblyLabel("#0001_else_if_1"),
                new PushInt(4),
                new PushVar(Ref("secondFunc.x")),
                new Equal(),
                new JumpIfToLabel("#0001_endif"),
                
                //         continue;
                new JumpToLabel("#0001_while"),
                
                //     }
                new AssemblyLabel("#0001_endif"),
                
                // }
                new JumpToLabel("#0001_while"),
                new AssemblyLabel("#0001_endwhile"),


                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("secondFunc"),
                Ref("secondFunc.x"),
            };
            AssertSymbolsMatch();
            
        }
        
        
        [Fact]
        public void TestKeywordThisInsideInstance()
        {
            Code = @"
                extern func int NPC_IsPlayer(var instance par0);
                extern func void WLD_PlayEffect(var string par0, var instance par1, var instance par2, var int par3, var int par4, var int par5, var int par6);
                extern func void NPC_ChangeAttribute(var instance par0, var int par1, var int par2);
                extern func void CreateInvItems(var instance par0, var int par1, var int par2);

                const int ATR_STRENGTH =  4;
                const int ATR_DEXTERITY =  5;
                const int ATR_INDEX_MAX	=  8;
                
                class C_NPC 
                {	
                    var int attribute[ATR_INDEX_MAX];
                    var int flags;
                    var float price;
	                var float prices[2];
                    var int data[188];
                };

                func void firstFunc(var C_NPC slf){};
                
                prototype NPC_Default (C_NPC)
                {
                    var int x;
                    x = 1;
                    attribute[ATR_STRENGTH] = 10;
                    attribute[ATR_DEXTERITY] = 20;

                    price = 0;
                    price = 0.5;
                    prices[0] = 3;
                    prices[0] = 3.5;
                    firstFunc(self);
                };
                
                instance self(C_NPC);
                instance sword(C_NPC);
                
                func void useJoint()
                {
                    if (NPC_IsPlayer (self))
                    {
                        WLD_PlayEffect(""SLOW_TIME"", self, self, 0, 0, 0, 0);
                    };
                };
                
                func void gainStrength(var C_NPC slf, var int spell, var int mana)
                {
                    if (slf.attribute[ATR_STRENGTH] < 10)
                    {
                        NPC_ChangeAttribute(slf, ATR_STRENGTH, 10);
                    };
                    NPC_ChangeAttribute(slf, ATR_STRENGTH, ATR_STRENGTH + 1);
                };
                
                instance Geralt (NPC_Default)
                {
                    var float price;

                    CreateInvItems(this, sword, 2);
                    gainStrength(this, attribute[ATR_STRENGTH], attribute[ATR_DEXTERITY]);
                    firstFunc(this);

                    price = 0;
                    price = 0.5;
                    this.price = 2;
                    this.price = 2.5;
                    prices[0] = 3;
                    prices[1] = 3.5;
                    this.prices[0] = 4;
                    this.prices[1] = 4.5;
                    Geralt.price = 4;
                    Geralt.prices[1] = 4.5;
                };

                func void testFunc() {
                    Geralt.flags = 0;
                };
            ";
            char prefix = (char) 255;
            
            Instructions = GetExecBlockInstructions("NPC_Default");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // x = 1;
                new PushInt(1),
                new PushVar(Ref("NPC_Default.x")),
                new Assign(),

                // attribute[ATR_STRENGTH] = 10;
                new PushInt(10),
                new PushArrayVar(Ref("C_NPC.attribute"), 4),
                new Assign(),
                
                // attribute[ATR_DEXTERITY] = 20;
                new PushInt(20),
                new PushArrayVar(Ref("C_NPC.attribute"), 5),
                new Assign(),
                
                // price = 0;
                new PushInt(0),
                new PushVar(Ref("C_NPC.price")),
                new AssignFloat(),
                
                // price = 0.5;
                new PushInt(1056964608),
                new PushVar(Ref("C_NPC.price")),
                new AssignFloat(),

                // prices[0] = 3;
                new PushInt(1077936128),
                new PushVar(Ref("C_NPC.prices")),
                new AssignFloat(),
                
                // prices[0] = 3.5;
                new PushInt(1080033280),
                new PushVar(Ref("C_NPC.prices")),
                new AssignFloat(),

                // firstFunc(self);
                new PushInstance(Ref("self")),
                new Call(Ref("firstFunc")),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("useJoint");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if (NPC_IsPlayer (self))
                new PushInstance(Ref("self")),
                new CallExternal(Ref("NPC_IsPlayer")),
                new JumpIfToLabel("#0001_endif"),
                
                // WLD_PlayEffect(""SLOW_TIME"", self, self, 0, 0, 0, 0);
                new PushVar(Ref($"{prefix}10000")),
                new PushInstance(Ref("self")),
                new PushInstance(Ref("self")),
                new PushInt(0),
                new PushInt(0),
                new PushInt(0),
                new PushInt(0),
                new CallExternal(Ref("WLD_PlayEffect")),
                
                // endif
                new AssemblyLabel("#0001_endif"),
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("gainStrength");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("gainStrength.mana")),
                new Assign(),
                new PushVar(Ref("gainStrength.spell")),
                new Assign(),
                new PushInstance(Ref("gainStrength.slf")),
                new AssignInstance(),
                
                // if (slf.attribute[ATR_STRENGTH] < 10)
                new PushInt(10),
                new SetInstance(Ref("gainStrength.slf")),
                new PushArrayVar(Ref("C_NPC.attribute"), 4),
                new Less(),
                new JumpIfToLabel("#0002_endif"),
                
                // NPC_ChangeAttribute(slf, ATR_STRENGTH, 10);
                new PushInstance(Ref("gainStrength.slf")),
                new PushVar(Ref("ATR_STRENGTH")),
                new PushInt(10),
                new CallExternal(Ref("NPC_ChangeAttribute")),
                
                // endif
                new AssemblyLabel("#0002_endif"),
                
                // NPC_ChangeAttribute(slf, ATR_STRENGTH, ATR_STRENGTH + 1);
                new PushInstance(Ref("gainStrength.slf")),
                new PushVar(Ref("ATR_STRENGTH")),
                new PushInt(1),
                new PushVar(Ref("ATR_STRENGTH")),
                new Add(),
                new CallExternal(Ref("NPC_ChangeAttribute")),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("Geralt");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new Call(Ref("NPC_Default")),
                
                                                            
                // CreateInvItems(this, sword, 2);
                new PushInstance(Ref("Geralt")),
                new PushInt(RefIndex("sword")),
                new PushInt(2),
                new CallExternal(Ref("CreateInvItems")),
                
                // gainStrength(this, slf.attribute[ATR_STRENGTH], this.attribute[ATR_DEXTERITY]);
                new PushInstance(Ref("Geralt")),
                new PushArrayVar(Ref("C_NPC.attribute"), 4),
                new PushArrayVar(Ref("C_NPC.attribute"), 5),
                new Call(Ref("gainStrength")),
                
                // firstFunc(this);
                new PushInstance(Ref("Geralt")),
                new Call(Ref("firstFunc")),
                
                // price = 0;
                new PushInt(0),
                new PushVar(Ref("Geralt.price")),
                new AssignFloat(),
                
                // price = 0.5;
                new PushInt(1056964608),
                new PushVar(Ref("Geralt.price")),
                new AssignFloat(),

                // this.price = 2;
                new PushInt(1073741824),
                new SetInstance(Ref("Geralt")),
                new PushVar(Ref("C_NPC.price")),
                new AssignFloat(),

                // this.price = 2.5;
                new PushInt(1075838976),
                new SetInstance(Ref("Geralt")),
                new PushVar(Ref("C_NPC.price")),
                new AssignFloat(),
                
                // prices[0] = 3;
                new PushInt(1077936128),
                new PushVar(Ref("C_NPC.prices")),  // TODO shouldn't here be SetInstance?
                new AssignFloat(),
                
                // prices[1] = 3.5;
                new PushInt(1080033280),
                new PushArrayVar(Ref("C_NPC.prices"), 1), // TODO shouldn't here be SetInstance?
                new AssignFloat(),
                
                // this.prices[0] = 4;
                new PushInt(1082130432),
                new SetInstance(Ref("Geralt")),
                new PushVar(Ref("C_NPC.prices")),
                new AssignFloat(),
                
                // this.prices[1] = 4.5;
                new PushInt(1083179008),
                new SetInstance(Ref("Geralt")),
                new PushArrayVar(Ref("C_NPC.prices"), 1),
                new AssignFloat(),
                
                // Geralt.price = 4;
                new PushInt(1082130432),
                new SetInstance(Ref("Geralt")),
                new PushVar(Ref("C_NPC.price")),
                new AssignFloat(),
                
                // Geralt.prices[1] = 4.5;
                new PushInt(1083179008),
                new SetInstance(Ref("Geralt")),
                new PushArrayVar(Ref("C_NPC.prices"), 1),
                new AssignFloat(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // Geralt.flags = 0;
                new PushInt(0),
                new SetInstance(Ref("Geralt")),
                new PushVar(Ref("C_NPC.flags")),
                new Assign(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("NPC_IsPlayer"),
                Ref("NPC_IsPlayer.par0"),
                Ref("WLD_PlayEffect"),
                Ref("WLD_PlayEffect.par0"),
                Ref("WLD_PlayEffect.par1"),
                Ref("WLD_PlayEffect.par2"),
                Ref("WLD_PlayEffect.par3"),
                Ref("WLD_PlayEffect.par4"),
                Ref("WLD_PlayEffect.par5"),
                Ref("WLD_PlayEffect.par6"),
                Ref("NPC_ChangeAttribute"),
                Ref("NPC_ChangeAttribute.par0"),
                Ref("NPC_ChangeAttribute.par1"),
                Ref("NPC_ChangeAttribute.par2"),
                Ref("CreateInvItems"),
                Ref("CreateInvItems.par0"),
                Ref("CreateInvItems.par1"),
                Ref("CreateInvItems.par2"),
                
                Ref("ATR_STRENGTH"),
                Ref("ATR_DEXTERITY"),
                Ref("ATR_INDEX_MAX"),
                Ref("C_NPC"),
                Ref("C_NPC.attribute"),
                Ref("C_NPC.flags"),
                Ref("C_NPC.price"),
                Ref("C_NPC.prices"),
                Ref("C_NPC.data"),
                Ref("firstFunc"),
                Ref("firstFunc.slf"),
                Ref("NPC_Default"),
                Ref("NPC_Default.x"),
                Ref("self"),
                Ref("sword"),
                Ref("useJoint"),
                Ref("gainStrength"),
                Ref("gainStrength.slf"),
                Ref("gainStrength.spell"),
                Ref("gainStrength.mana"),
                Ref("Geralt"),
                Ref("Geralt.price"),
                // Ref("Geralt.attribute"),
                Ref("testFunc"),
                Ref($"{prefix}10000"),
            };
            AssertSymbolsMatch(); 
        }
        
        
        
        [Fact]
        public void TestNestedAttributes() // TODO check if it does work ingame
        {
            Code = @"
                class Pet {
                    var int size;
                    var Human owner;
                };

                class Human {
                    var int age;
                    var Human enemy;
                    var Pet pet;
                };

                func int getAge(var Human human) {
                    return human.age;
                };

                instance Person1(Human);
                instance Person2(Human) {
                    age = 10;
                    enemy = Person1;
                };
                instance Dog(Pet);
                instance Cat(Pet);

                func void testFunc() {
                    Person1 = Person2;
                    Person1.age = 1;
                    Person1.enemy = Person2;
                    Person1.enemy.age = 2;
                    Person1.enemy.age = Person2.enemy.age;
                    Person1.enemy.age = getAge(Person2.enemy);

                    Person1.pet = Dog;
                    Person1.pet.size = 3;
                    Person1.pet.size = Dog.size;
                    Person1.pet.owner.age = Cat.owner.age;
                
                    Person1.pet.owner.pet.owner.pet.owner = Person1;
                    Person1.pet.owner.pet.owner.pet.owner.enemy = Person2;
                    Person1.pet.owner.pet.owner.pet = Cat;
                    Person1.pet.owner.pet.owner.pet.size = 4;
                };
           ";
            //TODO add secondFunc
            //var Human Person2;
            //Person2 = Person1;
            // .. copy of testFunc
            
            Instructions = GetExecBlockInstructions("Person2");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushInt(10),
                new PushVar(Ref("Human.age")),
                new Assign(),
                new PushInstance(Ref("Person1")),
                new PushInstance(Ref("Human.enemy")),
                new AssignInstance(),
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // Person1 = Person2;;
                new PushInstance(Ref("Person2")),
                new PushInstance(Ref("Person1")),
                new AssignInstance(),

                // Person1.age = 1;
                new PushInt(1),
                new SetInstance(Ref("Person1")),
                new PushVar(Ref("Human.age")),
                new Assign(),
                
                // Person1.enemy = Person2;
                new PushInstance(Ref("Person2")),
                new SetInstance(Ref("Person1")),
                new PushInstance(Ref("Human.enemy")),
                new AssignInstance(),
                
                // Person1.enemy.age = 2;
                new PushInt(2),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.enemy")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.age")),
                
                new Assign(),
                
                
                // Person1.enemy.age = Person2.enemy.age;
                // Person2.enemy.age
                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person2")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.enemy")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.age")),
                
                // Person1.enemy.age
                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.enemy")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.age")),
                
                new Assign(),
                
                
                // Person1.enemy.age = getAge(Person2.enemy);
                // getAge(Person2.enemy)
                new SetInstance(Ref("Person2")),
                new PushInstance(Ref("Human.enemy")),
                new Call(Ref("getAge")),
                
                // Person1.enemy.age
                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.enemy")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.age")),
                
                new Assign(),
                
                // Person1.pet = Dog;
                new PushInstance(Ref("Dog")),
                new SetInstance(Ref("Person1")),
                new PushInstance(Ref("Human.pet")),
                new AssignInstance(),
                
                
                // Person1.pet.size = 3;
                new PushInt(3),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.size")),
                
                new Assign(),
                
                // Person1.pet.size = Dog.size;
                new SetInstance(Ref("Dog")),
                new PushVar(Ref("Pet.size")),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.size")),
                
                new Assign(),
                
                
                // Person1.pet.owner.age = Cat.owner.age;
                // Cat.owner.age
                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Cat")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.age")),
                
                // Person1.pet.owner.age
                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.age")),
                
                new Assign(),
                
                
                // Person1.pet.owner.pet.owner.pet.owner = Person1;
                new PushInstance(Ref("Person1")),

                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
            
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushInstance(Ref("Pet.owner")),
                new AssignInstance(),
                
                
                // Person1.pet.owner.pet.owner.pet.owner.enemy = Person2;
                new PushInstance(Ref("Person2")),

                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
            
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushInstance(Ref("Human.enemy")),
                new AssignInstance(),
                
                // Person1.pet.owner.pet.owner.pet = Cat;
                new PushInstance(Ref("Cat")),

                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
            
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),

                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushInstance(Ref("Human.pet")),
                new AssignInstance(),
                
                
                
                // Person1.pet.owner.pet.owner.pet.size = 4;
                new PushInt(4),

                new PushVar(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Person1")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),
            
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.owner")),
                new Assign(),

                new PushVar(Ref(".HELPER_INSTANCE")),
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Human.pet")),
                new Assign(),
                
                new SetInstance(Ref(".HELPER_INSTANCE")),
                new PushVar(Ref("Pet.size")),
                new Assign(),

                
                
                new Ret(),
            };
            AssertInstructionsMatch();
            

            ExpectedSymbols = new List<Symbol>
            {
                Ref("Pet"),
                Ref("Pet.size"),
                Ref("Pet.owner"),
                Ref("Human"),
                Ref("Human.age"),
                Ref("Human.enemy"),
                Ref("Human.pet"),
                Ref("getAge"),
                Ref("getAge.human"),
                Ref("Person1"),
                Ref("Person2"),
                Ref("Dog"),
                Ref("Cat"),
                Ref("testFunc"),
                Ref(".HELPER_INSTANCE"),
            };
            AssertSymbolsMatch();
            
        }
        
        
        
        [Fact]
        public void TestVarDeclAssignment()
        {
            Code = @"
                class NPC {}

                prototype Proto(NPC) {
                    var int a = 6;
                    var int b[2] = {9};
                    var int c[2] = {8, 9};
                }

                instance Inst(NPC) {
                    var int a = 6;
                    var int b[2] = {9};
                    var int c[2] = {8, 9};
                }

                func void testFunc() {
                    var int a = 6;
                    var int b[2] = {9};
                    var int c[2] = {8, 9};
                }
           ";
            
            Instructions = GetExecBlockInstructions("Proto");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // var int a = 6;
                new PushInt(6),
                new PushVar(Ref("Proto.a")),
                new Assign(),
                
                // var int b[2] = {9};
                new PushInt(9),
                new PushVar(Ref("Proto.b")),
                new Assign(),
                
                // var int c[2] = {8, 9};
                new PushInt(8),
                new PushVar(Ref("Proto.c")),
                new Assign(),
                new PushInt(9),
                new PushArrayVar(Ref("Proto.c"), 1),
                new Assign(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("Inst");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // var int a = 6;
                new PushInt(6),
                new PushVar(Ref("Inst.a")),
                new Assign(),
                
                // var int b[2] = {9};
                new PushInt(9),
                new PushVar(Ref("Inst.b")),
                new Assign(),
                
                // var int c[2] = {8, 9};
                new PushInt(8),
                new PushVar(Ref("Inst.c")),
                new Assign(),
                new PushInt(9),
                new PushArrayVar(Ref("Inst.c"), 1),
                new Assign(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // var int a = 6;
                new PushInt(6),
                new PushVar(Ref("testFunc.a")),
                new Assign(),
                
                // var int b[2] = {9};
                new PushInt(9),
                new PushVar(Ref("testFunc.b")),
                new Assign(),
                
                // var int c[2] = {8, 9};
                new PushInt(8),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                new PushInt(9),
                new PushArrayVar(Ref("testFunc.c"), 1),
                new Assign(),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("NPC"),
                Ref("Proto"),
                Ref("Proto.a"),
                Ref("Proto.b"),
                Ref("Proto.c"),
                Ref("Inst"),
                Ref("Inst.a"),
                Ref("Inst.b"),
                Ref("Inst.c"),
                Ref("testFunc"),
                Ref("testFunc.a"),
                Ref("testFunc.b"),
                Ref("testFunc.c"),
            };
            AssertSymbolsMatch();
            
        }
        
    }
}