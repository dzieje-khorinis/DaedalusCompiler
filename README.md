# Daedalus Compiler  [![CI](https://github.com/dzieje-khorinis/DaedalusCompiler/actions/workflows/ci.yml/badge.svg)](https://github.com/dzieje-khorinis/DaedalusCompiler/actions/workflows/ci.yml) [![Release](https://github.com/dzieje-khorinis/DaedalusCompiler/actions/workflows/release.yml/badge.svg)](https://github.com/dzieje-khorinis/DaedalusCompiler/actions/workflows/release.yml)
This is repo with Daedalus ( Gothic I, II scripting language ) compiler.
Project is using internally ANTLR4 for parsing source code.
The compiler itself is written in .NET 8 C# with modern features like nullable reference types and single-file publishing.

You can listen about our compiler on below video. Video has bad audio quality, we encourage you to turn on subtitles, on the current moment we have subtitles for Polish, German and English language.

[![](screen_it_days.jpg)](http://www.youtube.com/watch?v=naPydcbJezw)
## Installation

### 🚀 Quick Install (Recommended)

Install the latest version with a single command:

```bash
# Linux & macOS
curl -fsSL https://raw.githubusercontent.com/dzieje-khorinis/DaedalusCompiler/main/install.sh | bash

# Or with wget
wget -qO- https://raw.githubusercontent.com/dzieje-khorinis/DaedalusCompiler/main/install.sh | bash
```

**Note: Quick Install supports Linux and macOS. On Windows, please use Manual Download below.**

The installer will:
- ✅ Automatically detect your OS and architecture (Linux/macOS, x64/ARM64)
- ✅ Download the appropriate self-contained binary
- ✅ Install to `/usr/local/bin` (or `~/.local/bin` if no sudo access)
- ✅ Configure your PATH automatically
- ✅ No .NET runtime required!

After installation, the `gdc` command will be available in your terminal.

### 📦 Manual Download

Alternatively, [download the latest release](https://github.com/dzieje-khorinis/DaedalusCompiler/releases/latest) manually:

1. Download the appropriate binary for your platform:
   - `gdc-linux-x64` / `gdc-linux-arm64` (Linux)
   - `gdc-macos-x64` / `gdc-macos-arm64` (macOS)  
   - `gdc-windows-x64.exe` / `gdc-windows-arm64.exe` (Windows)

2. Make it executable and move to a directory in your PATH:
```bash
# Linux & macOS
chmod +x gdc-*
sudo mv gdc-* /usr/local/bin/gdc

# Windows (PowerShell as Administrator)
Move-Item gdc-windows-*.exe C:\Windows\System32\gdc.exe
```

## Example usage:

* generate `Gothic.dat` file from `Gothic.src` file in output directory:
```
$ gdc /path/to/Gothic.src
```

* generate `result.dat` file from `Gothic.src` file in custom directory, using custom runtime:
```
$ gdc /path/to/Gothic.src --runtime /path/to/runtime.d --output-dat /path/to/result.dat
```

* generate `ou.csl`, `ou.bin` and `Gothic.dat` in output directory, ignore warnings W1 and W2:
```
$ gdc /path/to/Gothic.src --gen-ou --suppress W1:W2
```

* generate `Gothic.dat` in `Scripts/_compiled`, `ou.csl` and `ou.bin` in `Scripts/Content/Cutscene`:
```
$ gdc /path/to/Gothic.src --output-dat "Scripts/_compiled/Gothic.dat" --gen-ou --output-ou "Scripts/Content/Cutscene"
```

* generate `Gothic.dat`, enable unused symbol detection, provide zen paths to make that detection more accurate:
```
$ gdc /path/to/Gothic.src --zen-paths="/path/to/zens/*.zen" 
```

## Configure development environment
Project uses ANTLR4 and .NET 8 C#. 

For .NET 8 C# it is recommended to use Visual Studio 2022 or later, or Visual Studio Code with the C# extension.

For ANTLR4 you can use InteliJ or Visual Studio Code.

### Configure VSCode for ANTLR
1. Install Java (I think the best choice will be [Java8 SDK](http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html)).
2. Install [ANTLR4 VSCode extension](https://marketplace.visualstudio.com/items?itemName=mike-lischke.vscode-antlr4).
3. Optionally you can install [Daedalus VSCode extension](https://marketplace.visualstudio.com/items?itemName=szymonzak.daedalus).

Now you can open folder `/DaedalusCompiler/src/Parser` in VSCode and start contributing Daedalus gramar. 

ANTLR 4 VSCode extension helps you developing gramar by syntax coloring, code completion and debugging tools. For more details check out [ANTLR 4 VSCode extension](https://marketplace.visualstudio.com/items?itemName=mike-lischke.vscode-antlr4) site.
