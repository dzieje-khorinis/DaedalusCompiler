OU.BIN file's documentation
Author: Tomasz "TR" Broma
2018.11.09 ver.1
2018.11.13 ver.2

OU.BIN file format is binary version of OU.CSL text file.
Whole file consists of 3 main parts:
1. Text header
2. Body
3. Value's codes definitions

## 1. Header
Header consists of about a dozen of strings (one byte per char), each ending with '\n' (10dec) byte.

## 2. Body
Body consists of sub header and serie of tokens, each defining value or object (which I'll be calling 'node')
By node I mean line [% NodeName INT1 INT2] in CSL file and corresponding end of such a node: [].
Nodes can be nested.
Each node can contain one or more values.

### 2.1 Sub header
Sub header starts with int32 value of 2. This value is signal that we have reached the end of text header.
__int32	:2__ - this value means that next int32 contains node's count in file (in CSL it corresponds to line 'objects NNNNN')
__int32	:node's count__
__int32	:VALDEFOFF__ - offset where value's definitions start. We read tokens until we reach this offset in file.

### 2.2 Tokens
Each token contains beginning or end of node or value definition

#### 2.2a Beginning or end of node:
In CSL file beginning of node is similar to line below:
[% zCCSBlock 0 1]
and end of node looks like that:
[]

Here we have binary representation of such a line:
__byte	:1__ (it means that it is node or end of node)
__int16	:length of string defining node__
__byte[]	:content string__ (one byte per char)

#### 2.2b Value
In CSL file line containing value is similar to line below:
blockName=string:SVM_1_MILGreetings
As we can see it has name (ie. blockName), type (ie. string) and value (ie. SVM_1_MILGreetings).
In BIN file format values are written without name, using predefined VALCODE instead.
Instead of text representing type (ie. string) types are also coded binary.
Values (ie. SVM_1_MILGreetings) are also coded binary and their format depends on type.

Each value begins with header:
__byte	:18__ (this value means definition of value)
__int32	:VALCODE__ (defined in chapter 3 'Value's codes definitions)
__byte	:type (1=string, 2=int, 3=float, 17=enum)__

after header we have value's actual data which depends on value's type.
Below I describe data format for each type:

TYPE=1 (string)
__int16	:string's length__ in bytes
__byte[]	:bytes of string__ (one byte per char)

TYPE=2 (int)
__int32	:int value__

TYPE=3 (float)
__float(4bytes): float value__

TYPE=17 (enum)
__int32	:enum value__

We read tokens in loop until we reach file offset defined in VALDEFOFF.

## 3. Value's codes definitions

Here we have definitions linking value's name (from CSL file) with their codes (VALCODE) contained in BIN file:

### 3.1 Sub header
__int32:	number of value's codes definitions__

### 3.2 Value's codes definition:
Each value's codes definition cosists of:
__int16 - value's name length in bytes__
__int16 - VALCODE__ - code of value used in tokens (in tokens it is written as int32)
		 afaik VALCODE for each next value's name is assigned according to their order in CSL file (0-based)
__int32 - not identified yet__
__byte[] - name of value__ (one byte per char)
