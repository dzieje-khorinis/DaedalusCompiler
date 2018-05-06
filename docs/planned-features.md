# Boolean values
 - ability to declare boolean constants and variables (local, fields and parameters) and return types
 - compiled to integers, but limited to values 0 and 1
 - `true` and `false` literals
 - support for assigning and returning boolean expressions (`return 2>3`, `const bool truth = DK>KM`)
 - support for testing boolean values in `if` expressions, warning if used with integer values (right: `if(true)`, wrong: `if (1)`)

# Ternary operator
 - `(1>2) ? "a" : "b" )`

# String concatenation operator
 - `"a" + "b"` compiled to `"ab"` (with constants and literals)
 - `"a" + b` compiled to `ConcatStrings("a", b)` (when at least one operand is variable)

# More warnings
 - if function should return (is not `void`) but does not (original compiler implicitly returns default value)
 
# Methods
 - `class C_Npc { func void Die() { this.attribute[atr_hp] = 0 }; }` compiled to `func void Die(C_Npc this) { this.attribute[atr_hp] = 0 };`
 - `class C_Npc { func bool CanSeeNpc(C_Npc other) = Npc_CanSeeNpc(this, other); }` inlined
 - usage: `hero.Die()`, `if(hero.CanSeeNpc(xardas))`

# Generation of .src files
 - every .d file starts with line `#package NAME` where `NAME` is name or relative path of .dat/.src file (e.g. `#package Menu` or `#package System/Menu`)
 - below it can specify which files are its dependencies that have to be compiled before: `#import C_Npc` or `#import ../Engine/C_Npc`
 - .src wouldn't be needed at all
 - needs to detect cyclic dependencies
 - optional: dependency solver (when dependencies are not specified, tries to guess them and write down in source files)
 
# Loops
 - for
 - while
 - foreach

# Declarations of external functions
 - original compiler has external functions (kind of standard library) hardcoded
 - we need compatibility with various versions of this standard library
 - `extern func void ExitGame();`
