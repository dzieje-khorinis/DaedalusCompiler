# Gothic VM & Daedalus Overview

This is overview of Gothic I & II VM which runs Daedalus scipts

## DAT format specyfication

If you want see code how DAT is loaded please analyze [this](https://github.com/ataulien/ZenLib/blob/76320437c8096b2d315c8b2f63eb9da4d86c4c79/zenload/DATFile.cpp#L71). So best would be reading that document with simultaneous looking on code to undestand all concepts ;)

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

| Name                            | Start Address ( in bytes )  | Size ( in bytes ) | Remarks                                                                                                                                                             |
| ------------------------------- | --------------------------- | ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Version                         | 0                           | 1                 | Version of DAT file. Not very useful information                                                                                                                    |
| Symbols Count ( SC - shortcut ) | 1                           | 4                 | Length of symbols which interpreter needs to load                                                                                                                   |
| Symbols                         | 5                           | SC * Symbol Size  | Size of symbol is not fixed. For each symbol size is different, it depends for example from name of symbol                                                          |
| Code Stack Size                 | Symbols End Address + 1     | 1                 | Size of code stack in bytes                                                                                                                                         |
| Code Stack                      | Code Stack Size Address + 1 | Code Stack Size   | Size of single instruction depends from type of that instruction. For example instruction **RETURN** ( takes 0 args ) will be shorter then **JUMP** ( takes 1 arg ) |

#### Example:

TODO

### Symbol

#### Example:

TODO

### Code Stack

## VM

Gothic VM is simlar to other VMs. It has instruction pointer, stack, instructions  

## Tips

- All 
