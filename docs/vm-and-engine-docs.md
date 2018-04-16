# Gothic VM & Daedalus Overview

This is overview of Gothic I & II VM which runs Daedalus scipts

## DAT format specyfication

If you want see code how DAT is loaded please analyze [this](https://github.com/ataulien/ZenLib/blob/76320437c8096b2d315c8b2f63eb9da4d86c4c79/zenload/DATFile.cpp#L71). So best would be reading that document with simultaneous looking on code to undestand all concepts ;)

### Data

#### How interpret numbers saved in DAT?

Let's assume we have saved somewhere number in DEC system on 2 bytes, for example:

```
0000 0101 0000 0001
```

In HEX it would be:

```
05 01
```

When we interpret that as a number we should get **261**( 05 => **5**, 01 => **256** ). So last byte is most significant.

Modern languages should properly load numbers with above representation.

#### Types

Data types which we will be using in article:

| Typ Name           | Size in bytes | Extra                                                                                                                                    |
| ------------------ | ------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| DWord              | 4             |                                                                                                                                          |
| Word               | 2             |                                                                                                                                          |
| Byte               | 1             |                                                                                                                                          |
| Float              | 4             |                                                                                                                                          |
| String of N length | 1xN           | When parser load string he recognizes end of string when occur one of that signs: `\r`, `\n`, ` ` (space). Each byte represents one sign |

### Header of DAT

| Name                            | Start Address ( in bytes )       | Size ( in bytes ) | Remarks                                                                                                                                                             |
| ------------------------------- | -------------------------------- | ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Version                         | 0                                | 1                 | Version of DAT file. Not very useful information                                                                                                                    |
| Symbols Count ( SC - shortcut ) | 1                                | 4                 | Length of symbols which interpreter needs to load                                                                                                                   |
| Sorted table                    | 5                                | SC * 4            | Some useless array with numbers ( TODO good will be make verify that )                                                                                              |
| Symbols                         | SC * 4 + 1                       | SC * Symbol Size  | Size of symbol is not fixed. For each symbol size is different, it depends for example from name of symbol                                                          |
| Code Stack Size                 | Symbols End Address + 1          | 4                 | Size of code stack in bytes                                                                                                                                         |
| Code Stack                      | Code Stack Size  End Address + 1 | Code Stack Size   | Size of single instruction depends from type of that instruction. For example instruction **RETURN** ( takes 0 args ) will be shorter then **JUMP** ( takes 1 arg ) |

#### Example:

TODO

### Symbol

TODO symbol overview

Each symbol have following structure:

| Name              | Start address (in bytes)                             | Size ( in bytes )                                 | Remarks                                                                                                                                                                                                                                                    |
| ----------------- | ---------------------------------------------------- | ------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Have name         | 0                                                    | 4                                                 | This works like flag. If value is different from 0 it means that symbol have name which we need load                                                                                                                                                       |
| Name              | 4 [ **if** exists ]                                  | String * N ( in that case we look only for `\n` ) | Name of symbol, for example: `C_MISSION.RUNNING`, `C_ITEM`, `MAX_WISPSKILL`                                                                                                                                                                                |
| Symbol Properties | 4 **if** name not exists **else** 4 + String * N + 1 | 28                                                | That field describes details of symbol. Most important thing is information of how many sub items have symbol ( needed for arrays ), what is type of symbol ( `string`, `int` e.t.c) and what modificator symbol have, ex ( `const`, `class`, `external` ) |

#### Properties in details

Structure of properties is descibed very well by that code:

```c++
struct Properties
{
    int32_t  offClsRet;  // Offset (ClassVar) | Size (Class) | ReturnType (Func)
    struct {
        uint32_t count : 12; // Count:12, Type:4 (EParType_), Flags:6 (EParFlag_), Space: 1, Reserved:9
        uint32_t type  : 4; // EParType_*
        uint32_t flags : 6; // EParFlag_*
        uint32_t space: 1;
        uint32_t reserved : 9;
    }elemProps;

    struct
    {
        uint32_t value : 19;  // Value:19, Reserved:13
        uint32_t reserved : 13;
    }fileIndex;

    struct
    {
        uint32_t value : 19;  // Value:19, Reserved:13
        uint32_t reserved : 13;
    }lineStart;

    struct
    {
        uint32_t value : 19;  // Value:19, Reserved:13
        uint32_t reserved : 13;
    }lineCount;

    struct
    {
        uint32_t value : 24;  // Value:24, Reserved:8
        uint32_t reserved : 8;
    }charStart;

    struct
    {
        uint32_t value : 24;  // Value:24, Reserved:8
        uint32_t reserved : 8;
    }charCount;

} properties;
```

That code is part of ZenLib. Let me descibe that a little bit. If you don't know C++ you maybe wonder what means that `:` in each field of struct. Number after `:` tells how many bits should be stored in field. We need use that cause symbol description have not standard fields lengths. Most important field in that strucute are in `elemProps` sub structure. First field, `offClsRet`,  is information which means something different depending from symbol type, for class variable it would be offset of that symbol, for class it's size and for function returned type (**TODO** make sure if that really usless thing, ZenLib does't use that) . That information is useless. Next thing is sub items **count**, it is stored in 12 bits. As I mentioned above **count** property tells how many sub items field have, for example for `const int items[23]`**count** would be 23, for variables which are not array **count** would be 1. Next imporatnt field is `type`, it's flag which tells type of field, it could be one of:

| Name      | Flag value |
| --------- | ---------- |
| Void      | 0          |
| Float     | 1          |
| Int       | 2          |
| String    | 3          |
| Class     | 4          |
| Func      | 5          |
| Prototype | 6          |
| Instance  | 7          |

**flags** inform us about modyficator which is used on symbol, here we have possible values:

| Name     | Flag value |
| -------- | ---------- |
| Const    | 1          |
| Return   | 2          |
| ClassVar | 4          |
| External | 8          |
| Merged   | 16         |

Rest of fields in **Properties** struct is useless, names of that fields are pretty meaningfull. 

#### Example:

TODO

### Code Stack

## VM

Gothic VM is simlar to other VMs. It has instruction pointer, stack, instructions  

## Tips

- All 
