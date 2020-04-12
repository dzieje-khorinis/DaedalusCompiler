using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Common;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;
using Xunit;
using Ionic.Zip;
using Xunit.Abstractions;
using ZipFile = Ionic.Zip.ZipFile;

namespace DaedalusCompiler.Tests
{
    public class DatComparisonTests
    {
        private const string ProjectName = "DaedalusCompiler.Tests";
        private readonly ITestOutputHelper _output;
        private readonly string _downloadTo;
        private string _scriptsPath;
        private readonly  Dictionary<string, DatFile> _originalDatFiles;
        
        private List<string> _srcPaths = new List<string>()
        {
            "Scripts/Content/*.src",
            "Scripts/System/*.src"
        };
        private string _datPath = "Scripts/_compiled/*.dat";
        private string _ouPath = "Scripts/Content/Cutscene/ou.*";
        private string _outputPathOuDir = "output";
        
        private Dictionary<string, string> _srcPathToDatPath;

        private static readonly HashSet<string> NameExceptions = new HashSet<string>()
        {
            "FACE_N_TOUGH_LEE_ÄHNLICH"
        };
        
        public DatComparisonTests(ITestOutputHelper output)
        {
            // string projectPath = Path.Combine(Environment.CurrentDirectory.Split(ProjectName).First(), ProjectName);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(baseDirectory);
            string solutionPath = baseDirectoryInfo.Parent?.Parent?.Parent?.Parent?.Parent?.ToString();
            string projectPath = Path.Combine(solutionPath, "test", "DaedalusCompiler.Tests");

            _downloadTo = "{PROJECT_PATH}/TestFiles/".Replace("{PROJECT_PATH}", projectPath);
            _output = output;
            _originalDatFiles = new Dictionary<string, DatFile>();
        }

        private void PrepareScripts(string name, string url, string zipPassword)
        {
            _scriptsPath = Path.Combine(_downloadTo, name);

            string scriptsFileName = Path.ChangeExtension(name, "zip");
            string scriptsFilePath = Path.Combine(_downloadTo, scriptsFileName);

            (new FileInfo(scriptsFilePath)).Directory?.Create();
            
            File.Delete(scriptsFilePath);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, scriptsFilePath);
                _output.WriteLine($"Downloaded {scriptsFileName}.");
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (ZipFile archive = new ZipFile(scriptsFilePath))
            {
                archive.Password = zipPassword ;
                archive.ExtractAll(_scriptsPath, ExtractExistingFileAction.OverwriteSilently);
                _output.WriteLine($"Extracted {scriptsFileName} into {_scriptsPath}.");
            }
        }

        private void CompileScripts(string compileTime="", string compileUsername="")
        {
            _srcPathToDatPath = new Dictionary<string, string>();
            List<string> srcPaths = WildcardExpansion(_srcPaths);
            List<string> datPaths = WildcardExpansion(_datPath);
            Dictionary<string, string> filenameToSrcPath = GetFilenameWithoutExtensionToPathMapping(srcPaths);
            Dictionary<string, string> filenameToDatPath = GetFilenameWithoutExtensionToPathMapping(datPaths);
            foreach (string filename in filenameToSrcPath.Keys)
            {
                string srcPath = filenameToSrcPath[filename];
                string datPath = filenameToDatPath[filename];
                _srcPathToDatPath[srcPath] = datPath;
            }
            
            string outputPathOuDir = Path.Combine(_scriptsPath, _outputPathOuDir);
            Directory.CreateDirectory(outputPathOuDir);

            foreach(KeyValuePair<string, string> entry in _srcPathToDatPath)
            {
                string srcPath = entry.Key;
                string datPath = entry.Value;
                string datFileName = Path.GetFileName(datPath).ToLower();

                string srcFileName = Path.GetFileNameWithoutExtension(srcPath).ToLower();
                string outputPathDat = Path.Combine(outputPathOuDir, srcFileName + ".dat");

                bool generateOutputUnits = (datFileName == "gothic.dat");
                
                Compiler compiler = new Compiler(outputPathOuDir, verbose:false, strictSyntax:false, globallySuppressedCodes:new HashSet<string>{"W1", "W2", "W3", "W4", "W5"});
                if (generateOutputUnits)
                {
                    compiler.SetCompilationDateTimeText(compileTime);
                    compiler.SetCompilationUserName(compileUsername);
                }
                compiler.CompileFromSrc(srcPath, runtimePath:String.Empty, outputPathDat:outputPathDat, verbose:false, generateOutputUnits: generateOutputUnits);
                _originalDatFiles[datFileName] = compiler.DatFile;
            }
        }

        private List<string> WildcardExpansion(string wildcardPath)
        {
            return WildcardExpansion(new List<string>() {wildcardPath});
        }
        
        private List<string> WildcardExpansion(List<string> wildcardPaths)
        {
            List<string> paths = new List<string>();

            foreach (var wildcardPath in wildcardPaths)
            {  
                string dirPath = Path.Combine(_scriptsPath, Path.GetDirectoryName(wildcardPath));
                string filenamePattern = Path.GetFileName(wildcardPath);
                EnumerationOptions options = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive };
                paths.AddRange(Directory.GetFiles(dirPath, filenamePattern, options).ToList());
            }

            return paths;
        }
        
        private static Dictionary<string, string> GetFilenameWithoutExtensionToPathMapping(List<string> paths)
        {
            return paths.ToDictionary(x => Path.GetFileNameWithoutExtension(x).ToLower(), x => x);
        }
        

        private static Dictionary<string, string> GetFilenameToPathMapping(List<string> paths)
        {
            return paths.ToDictionary(x => Path.GetFileName(x).ToLower(), x => x);
        }
        

        private void CompareDats()
        {
            string outputDirPath = Path.Combine(_scriptsPath, _outputPathOuDir);
            List<string> alreadyLoaded = new List<string>();
            foreach (KeyValuePair<string, string> entry in _srcPathToDatPath)
            {
                string datPath = entry.Value;
                string datFileName = Path.GetFileName(datPath).ToLower();
                string outputDatPath = Path.Combine(outputDirPath, datFileName);
                
                DatFile expectedDatFile = new DatFile(datPath);
                DatFile datFile = new DatFile(outputDatPath);
            
                // First compare the same .DAT file before saving VS after save&load
                DatFile originalDatFile = _originalDatFiles[datFileName];
                Assert.Equal(originalDatFile.Version, datFile.Version);
                CompareSymbols(originalDatFile.DatSymbols.ToList(), datFile.DatSymbols.ToList());
                CompareTokens(originalDatFile.DatTokens.ToList(), datFile.DatTokens.ToList());
                
                Assert.Equal(expectedDatFile.Version, datFile.Version);
                CompareSymbols(expectedDatFile.DatSymbols.ToList(), datFile.DatSymbols.ToList());
                CompareTokens(expectedDatFile.DatTokens.ToList(), datFile.DatTokens.ToList());
                
                alreadyLoaded.Add(datFileName);
            }
        }

        private void CompareOuFiles()
        {
            string outputDirPath = Path.Combine(_scriptsPath, _outputPathOuDir);
            
            string ouCslFileName = "ou.csl";
            string ouBinFileName = "ou.bin";

            string outputOuCslPath = Path.Combine(outputDirPath, ouCslFileName);
            string outputOuBinPath = Path.Combine(outputDirPath, ouBinFileName);

            List<string> ouPaths = WildcardExpansion(_ouPath);
            Dictionary<string, string> filenameToOuPath = GetFilenameToPathMapping(ouPaths);

            CompareFilesByteByByte(filenameToOuPath[ouCslFileName], outputOuCslPath);
            CompareFilesByteByByte(filenameToOuPath[ouBinFileName], outputOuBinPath);
        }

        private void CompareSymbols(List<DatSymbol> expectedSymbols, List<DatSymbol> symbols)
        {
            HashSet<SymbolType> parentLessTypes = new HashSet<SymbolType>
            {
                SymbolType.Int,
                SymbolType.Float,
                SymbolType.String,
                SymbolType.Func
            };
            int lastParentIndex = -1;
            
            Assert.Equal(expectedSymbols.Count, symbols.Count);
            for (int i = 0; i < symbols.Count; i++)
            {
                DatSymbol expectedSymbol = expectedSymbols[i];
                DatSymbol symbol = symbols[i];
                Assert.Equal(expectedSymbol.Index, symbol.Index);
                if (!NameExceptions.Contains(symbol.Name))
                {
                    Assert.Equal(expectedSymbol.Name, symbol.Name);
                }
                Assert.Equal(expectedSymbol.OffClsRet, symbol.OffClsRet);
                Assert.Equal(expectedSymbol.Count, symbol.Count);
                Assert.Equal(expectedSymbol.BuiltinType, symbol.BuiltinType);
                Assert.Equal(expectedSymbol.Flags, symbol.Flags);
                

                if (symbol.BuiltinType == SymbolType.Func && symbol.Flags.HasFlag(SymbolFlag.External))
                {
                    
                }
                else
                {
                    Assert.Equal(expectedSymbol.Content, symbol.Content);
                }
                

                bool isParentLessType = parentLessTypes.Contains(symbol.BuiltinType);
                bool isBuggedParentIndex = lastParentIndex == expectedSymbol.ParentIndex && isParentLessType;

                if (!isBuggedParentIndex)
                {
                    Assert.Equal(expectedSymbol.ParentIndex, symbol.ParentIndex);
                }

                if (!isParentLessType)
                {
                    lastParentIndex = symbol.ParentIndex;
                }
                
            }
        }

        private void CompareTokens(List<DatToken> expectedTokens, List<DatToken> tokens)
        {
            Assert.Equal(expectedTokens.Count, tokens.Count);
            for (int i = 0; i < tokens.Count; i++)
            {
                DatToken expectedToken = expectedTokens[i];
                DatToken token = tokens[i];
                Assert.Equal(expectedToken.TokenType, token.TokenType);
                Assert.Equal(expectedToken.ByteParam, token.ByteParam);
                Assert.Equal(expectedToken.IntParam, token.IntParam);
            }
        }

        private void CompareFilesByteByByte(string expectedOuPath, string ouPath)
        {
            using (FileStream expectedOu = new FileStream(expectedOuPath, FileMode.Open))
            using (FileStream ou = new FileStream(ouPath, FileMode.Open))
            {
                Assert.Equal(expectedOu.Length, ou.Length);
                for(int i = 0; i < expectedOu.Length; i++)
                {
                    Assert.Equal(expectedOu.ReadByte(), ou.ReadByte());
                }
            }
        }


        [Fact]
        public void TestIfCompiledScriptsMatchOriginalDatAndOuFilesG2NotR()
        {
            PrepareScripts(
                "G2NotR",
                "https://drive.google.com/uc?authuser=0&id=1TZFfADoOPrmdNHbrbxMAad7Mk63HKloT&export=download",
                "dziejekhorinis"
                );
            CompileScripts("13.11.2018 15:30:55", "kisio");
                
            CompareDats();
            CompareOuFiles();
        }
        
        [Fact]
        public void TestIfCompiledScriptsMatchOriginalDatAndOuFilesIkarusAndLeGo()
        {
            PrepareScripts(
                "IkarusAndLego",
                "https://drive.google.com/uc?authuser=0&id=1OkTUUHYt7tXTg_ewmyFQaqYMFv_6gOxW&export=download",
                "dziejekhorinis"
            );
            
            CompileScripts();
            CompareDats();
        }        
    }
}
