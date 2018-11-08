using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;
using Newtonsoft.Json;
using Xunit;
using Ionic.Zip;
using Xunit.Abstractions;
using ZipFile = Ionic.Zip.ZipFile;

namespace DaedalusCompiler.Tests
{
    public static class Constants
    {
        public const string SrcPathsLabel = "SRC_PATHS";
        public const string DatPathsLabel = "DAT_PATHS";
        public const string ScriptsUrlLabel = "SCRIPTS_URL";
        public const string ScriptsPasswordLabel = "SCRIPTS_PASSWORD";
        
        public const string ScriptsFileName = "Scripts.zip";
        public const string ProjectName = "DaedalusCompiler.Tests";
    }
    
    
    public class Config
    {
        private List<string> SRC_PATHS;
        private List<string> DAT_PATHS;
        public string SCRIPTS_URL;
        public string SCRIPTS_PASSWORD;

        public List<string> GetPaths(string envVarName)
        {
            switch (envVarName)
            {
                case Constants.SrcPathsLabel:
                    return SRC_PATHS;
                case Constants.DatPathsLabel:
                    return DAT_PATHS;
                default:
                    return null;
            }
        }
    }
    
    public class DatComparisonTests
    {
        private readonly ITestOutputHelper _output;
        
        private string _projectPath;
        private Dictionary<string, string> _srcPathToDatPath;
        private Config _config;

        private static readonly HashSet<string> NameExceptions = new HashSet<string>()
        {
            "FACE_N_TOUGH_LEE_ÄHNLICH"
        };
        

        public DatComparisonTests(ITestOutputHelper output)
        {
            _output = output;

            LoadJsonConfig();
            DownloadScripts();
            ExtractScripts();
            InitializeSrcPathToDatPath();
        }

        private void LoadJsonConfig()
        {
            _projectPath = Path.Combine(Environment.CurrentDirectory.Split(Constants.ProjectName).First(), Constants.ProjectName);
            string configPath = Path.Combine(_projectPath, "config.json");
            if (File.Exists(configPath))
            {
                using (StreamReader reader = new StreamReader(configPath))
                {
                    string content = reader.ReadToEnd();
                    _config = JsonConvert.DeserializeObject<Config>(content);
                }
            }
        }

        private void DownloadScripts()
        {
            string scriptsUrl = _config?.SCRIPTS_URL ?? Environment.GetEnvironmentVariable(Constants.ScriptsUrlLabel);
            if (scriptsUrl == null)
            {
                return;
            }
            
            string scriptsFilePath = Path.Combine(_projectPath, Constants.ScriptsFileName);
            File.Delete(scriptsFilePath);
            
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(scriptsUrl, scriptsFilePath);
                _output.WriteLine($"Downloaded {Constants.ScriptsFileName}.");
            }
        }

        private void ExtractScripts()
        {
            string scriptsFilePath = Path.Combine(_projectPath, Constants.ScriptsFileName);
            if (!File.Exists(scriptsFilePath))
            {
                return;
            }
            
            string scriptsPassword = _config?.SCRIPTS_PASSWORD ?? Environment.GetEnvironmentVariable(Constants.ScriptsPasswordLabel);
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (ZipFile archive = new ZipFile(scriptsFilePath))
            {
                archive.Password = scriptsPassword ;
                archive.ExtractAll(_projectPath, ExtractExistingFileAction.OverwriteSilently);
                _output.WriteLine($"Extracted {Constants.ScriptsFileName} into {_projectPath}.");
            }
        }

        private void InitializeSrcPathToDatPath()
        {
            _srcPathToDatPath = new Dictionary<string, string>();
            
            List<string> srcPaths = GetPaths(Constants.SrcPathsLabel);
            List<string> datPaths = GetPaths(Constants.DatPathsLabel);

            Dictionary<string, string> filenameToSrcPath = GetFilenameToPathMapping(srcPaths);
            Dictionary<string, string> filenameToDatPath = GetFilenameToPathMapping(datPaths);

            foreach (string filename in filenameToSrcPath.Keys)
            {
                string srcPath = filenameToSrcPath[filename];
                string datPath = filenameToDatPath[filename];
                _srcPathToDatPath[srcPath] = datPath;
            }
        }

        private List<string> GetPaths(string envVarName)
        {
            List<string> wildcardPaths = _config?.GetPaths(envVarName) ?? GetListFromEnvironmentVariable(envVarName);
            
            if (wildcardPaths == null)
            {
                throw new Exception($"Couldn't load {envVarName}! Please set up proper environment variable or config.json file!");
            }

            wildcardPaths = wildcardPaths.Select(x => x.Replace("{PROJECT_PATH}", _projectPath)).ToList();
            return WildcardExpansion(wildcardPaths);

        }
        
        private List<string> GetListFromEnvironmentVariable(string variableName)
        {
            string variableContent = Environment.GetEnvironmentVariable(variableName);
            _output.WriteLine($"{variableName} = {Environment.GetEnvironmentVariable(variableName)}");
            return variableContent
                ?.Split(";")
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }

        private static List<string> WildcardExpansion(List<string> wildcardPaths)
        {
            List<string> paths = new List<string>();

            foreach (var wildcardPath in wildcardPaths)
            {  
                string dirPath = Path.GetDirectoryName(wildcardPath);
                string filenamePattern = Path.GetFileName(wildcardPath);
                EnumerationOptions options = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive };
                paths.AddRange(Directory.GetFiles(dirPath, filenamePattern, options).ToList());
            }

            return paths;
        }


        private static Dictionary<string, string> GetFilenameToPathMapping(List<string> paths)
        {
            return paths.ToDictionary(x => Path.GetFileNameWithoutExtension(x).ToLower(), x => x);
        }



        private void CompareDats(string expectedDatPath, string datPath)
        {
            DatFile expectedDatFile = new DatFile(expectedDatPath);
            DatFile datFile = new DatFile(datPath);
            
            Assert.Equal(expectedDatFile.Version, datFile.Version);
            CompareSymbols(expectedDatFile.Symbols.ToList(), datFile.Symbols.ToList());
            CompareTokens(expectedDatFile.Tokens.ToList(), datFile.Tokens.ToList());
        }

        private void CompareSymbols(List<DatSymbol> expectedSymbols, List<DatSymbol> symbols)
        {
            HashSet<DatSymbolType> parentLessTypes = new HashSet<DatSymbolType>()
            {
                DatSymbolType.Int,
                DatSymbolType.Float,
                DatSymbolType.String
            };
            int lastParentIndex = DatSymbol.NULL_INDEX;
            
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
                Assert.Equal(expectedSymbol.ArrayLength, symbol.ArrayLength);
                Assert.Equal(expectedSymbol.ParametersCount, symbol.ParametersCount);
                Assert.Equal(expectedSymbol.Type, symbol.Type);
                Assert.Equal(expectedSymbol.Flags, symbol.Flags);
                Assert.Equal(expectedSymbol.ReturnType, symbol.ReturnType);
                Assert.Equal(expectedSymbol.ClassVarOffset, symbol.ClassVarOffset);
                Assert.Equal(expectedSymbol.ClassSize, symbol.ClassSize);
                // Assert.Equal(expectedSymbol.Location, symbol.Location);
                Assert.Equal(expectedSymbol.Content, symbol.Content);
                
                if (!symbol.Flags.HasFlag(DatSymbolFlag.External))
                {
                    Assert.Equal(expectedSymbol.FirstTokenAddress, symbol.FirstTokenAddress);
                }
                Assert.Equal(expectedSymbol.ClassOffset, symbol.ClassOffset);


                bool isParentLessType = parentLessTypes.Contains(symbol.Type);
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

        [Fact]
        public void TestIfCompiledScriptsMatchOriginalDatFiles()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(baseDirectory);
            string solutionPath = baseDirectoryInfo.Parent?.Parent?.Parent?.Parent?.Parent?.ToString();
            
            
            string runtimeDirPath = Path.Combine(solutionPath, "src", "DaedalusCompiler", "DaedalusBuiltins");
            string outputDirPath = Path.Combine(solutionPath, "test", "DaedalusCompiler.Tests", "output");
            
            foreach(KeyValuePair<string, string> entry in _srcPathToDatPath)
            {
                string srcPath = entry.Key;
                string datPath = entry.Value;
                string outputDatPath = Path.Combine(outputDirPath, Path.GetFileName(datPath).ToLower());
                
                Compiler compiler = new Compiler(runtimeDirPath,  outputDirPath);
                compiler.CompileFromSrc(srcPath, compileToAssembly:false);
                
                CompareDats(datPath, outputDatPath);
            }
        }
    }
}