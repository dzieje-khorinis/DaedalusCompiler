using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace DaedalusCompiler.Compilation
{
    public class OutputUnitsBuilder
    {
        private readonly Dictionary<string, string> _wavFileNameToDialogue;
        private List<KeyValuePair<string, string>> _wavFileNameAndDialoguePairs;
        private string _generationDateTimeText;
        private string _userName;

        private const string InstanceBody = "instanceBody";
        private const string StringLiteral = "stringLiteral";
        private const string Comment = "comment";

        public OutputUnitsBuilder()
        {
            _wavFileNameToDialogue = new Dictionary<string, string>();
            _wavFileNameAndDialoguePairs = null;
            _generationDateTimeText = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
            _userName = Environment.UserName;
        }

        public void SetGenerationDateTimeText(string generationDateTimeText)
        {
            _generationDateTimeText = generationDateTimeText;
        }
        
        public void SetGenerationUserName(string userName)
        {
            _userName = userName;
        }

        private void AddDialoguesFromMatches(MatchCollection matches)
        {
            foreach (Match match in matches)
            {
                string wavFileName = match.Groups[StringLiteral].Value;
                string dialogue = match.Groups[Comment].Value.Trim();
                if (_wavFileNameToDialogue.ContainsKey(wavFileName))
                {
                    Console.WriteLine($"Already contains key '{wavFileName}'");
                }
                _wavFileNameToDialogue[wavFileName] = dialogue;
            }
        }
        
        public void ParseFile(string filePath)
        {
            string multilineComment = @"(?:/\*.*?\*/)";
            
            string fileContent = File.ReadAllText(filePath, Encoding.GetEncoding(1250));
            fileContent = Regex.Replace(fileContent, multilineComment, "", RegexOptions.Singleline);
            fileContent = Regex.Replace(fileContent, "^.*//.*(?:ai_output|=).*$", "", RegexOptions.Multiline);
            
            string ws = @"(?:[ \t])*";
            string newline = @"\r\n?|\n";
            string skip = $@"(?:{newline}|{ws})*";
            string identifier = @"[^\W0-9]\w*";
            string stringLiteral = $@"\""(?<{StringLiteral}>[^\""\r\n]*)\""";
            string inlineComment = $@"//(?<{Comment}>[^\r\n]*)";

            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            
            string cSvmInstancePattern = $@"\binstance{skip}{identifier}{skip}\({skip}C_SVM{skip}\){skip}{{(?<{InstanceBody}>[^}}]+)}}";
            MatchCollection instanceMatches = Regex.Matches(fileContent, cSvmInstancePattern, options);
            foreach (Match instanceMatch in instanceMatches)
            {
                string cSvmInstanceBody = instanceMatch.Groups[InstanceBody].Value;
                string assignmentPattern = $@"{identifier}{ws}={ws}{stringLiteral}{ws};{ws}{inlineComment}";
                MatchCollection assignmentMatches = Regex.Matches(cSvmInstanceBody, assignmentPattern, options);
                AddDialoguesFromMatches(assignmentMatches);
            }

            string aiOutputCallPattern = $@"\bai_output{ws}\({ws}{identifier}{ws},{ws}{identifier}{ws},{ws}{stringLiteral}{ws}\){ws};{ws}{inlineComment}";
            MatchCollection aiOutputCallMatches = Regex.Matches(fileContent, aiOutputCallPattern, options);
            AddDialoguesFromMatches(aiOutputCallMatches);
        }
        
        
        public void SaveOutputUnits(string dirPath)
        {
            _wavFileNameAndDialoguePairs = _wavFileNameToDialogue.ToList();
            _wavFileNameAndDialoguePairs.Sort((a, b) => string.Compare(a.Key, b.Key, StringComparison.Ordinal));
            SaveToCsl(dirPath);
            SaveToBin(dirPath);
        }


        private void SaveToCsl(string dirPath)
        {
            string cslPath = Path.Combine(dirPath, "ou.csl");
            int index = 1;
            using (FileStream fileStream = new FileStream(cslPath, FileMode.Create, FileAccess.Write))
            using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding(1250)))
            {
                int count = _wavFileNameAndDialoguePairs.Count;
                
                string csl = "ZenGin Archive\n";
                csl += "ver 1\n";
                csl += "zCArchiverGeneric\n";
                csl += "ASCII\n";
                csl += "saveGame 0\n";
                csl += $"date {_generationDateTimeText}\n";
                csl += $"user {_userName}\n";
                csl += "END\n";
                csl += $"objects {count*3+1}    \n";
                csl += "END\n\n";
                csl += "[% zCCSLib 0 0]\n";
                csl += $"	NumOfItems=int:{count}\n";
                streamWriter.Write(csl);
                
                foreach (KeyValuePair<string, string> pair in _wavFileNameAndDialoguePairs)
                {   
                    string item = "";
                    item += $"	[% zCCSBlock 0 {index++}]\n";
                    item += $"		blockName=string:{pair.Key}\n";
                    item += $"		numOfBlocks=int:1\n";
                    item += $"		subBlock0=float:0\n";
                    item += $"		[% zCCSAtomicBlock 0 {index++}]\n";
                    item += $"			[% oCMsgConversation:oCNpcMessage:zCEventMessage 0 {index++}]\n";
                    item += $"				subType=enum:0\n";
                    item += $"				text=string:{pair.Value}\n";
                    item += $"				name=string:{pair.Key.ToUpper()}.WAV\n";
                    item += $"			[]\n";
                    item += $"		[]\n";
                    item += $"	[]\n";
                    streamWriter.Write(item);

                    Console.WriteLine($"{(index - 1) / 3}/{count} csl elements written");
                }

                streamWriter.Write("[]\n");
            }
        }

        private void SaveToBin(string dirPath)
        {
            string binPath = Path.Combine(dirPath, "ou.bin");
            
            int index = 1;
            int address = 0;
            using (FileStream fileStream = new FileStream(binPath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.GetEncoding(1250)))
            {
                int count = _wavFileNameAndDialoguePairs.Count;
                
                string binHeader = "ZenGin Archive\n";
                binHeader += "ver 1\n";
                binHeader += "zCArchiverBinSafe\n";
                binHeader += "BIN_SAFE\n";
                binHeader += "saveGame 0\n";
                binHeader += $"date {_generationDateTimeText}\n";
                binHeader += $"user {_userName}\n";
                binHeader += "END\n";

                // objects {count*3+1}
                binaryWriter.Write(binHeader.ToCharArray());
                binaryWriter.Write(2);
                binaryWriter.Write(count*3+1);
                address += binHeader.Length + 4 + 4;
                
                int addressOfLastSignificantByteAddress = address;
                binaryWriter.Write(0xFFFFFFFF); // to be replaced later
                address += 4;
                
                // [% zCCSLib 0 0]
                char[] zCSSLib = "[% zCCSLib 0 0]".ToCharArray();
                binaryWriter.Write((Byte)1);
                binaryWriter.Write((Int16)zCSSLib.Length);
                binaryWriter.Write(zCSSLib);
                address += 1 + 2 + zCSSLib.Length;
                
                // NumOfItems=int:{count}
                binaryWriter.Write((Byte)18);
                binaryWriter.Write(0);
                binaryWriter.Write((Byte)2);
                binaryWriter.Write(count);
                address += 1 + 4 + 1 + 4;

                foreach (KeyValuePair<string, string> pair in _wavFileNameAndDialoguePairs)
                {
                    // [% zCCSBlock 0 {index++}]
                    char[] zCCSBlockHeader = $"[% zCCSBlock 0 {index++}]".ToCharArray();
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)zCCSBlockHeader.Length);
                    binaryWriter.Write(zCCSBlockHeader);
                    address += 1 + 2 + zCCSBlockHeader.Length;
                    
                    // blockName=string:{pair.Key}
                    binaryWriter.Write((Byte)18);
                    binaryWriter.Write(1);
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)pair.Key.Length);
                    binaryWriter.Write(pair.Key.ToCharArray());
                    address += 1 + 4 + 1 + 2 + pair.Key.Length;

                    // numOfBlocks=int:1
                    binaryWriter.Write((Byte)18);
                    binaryWriter.Write(2);
                    binaryWriter.Write((Byte)2);
                    binaryWriter.Write(1);
                    address += 1 + 4 + 1 + 4;
                    
                    // subBlock0=float:0
                    binaryWriter.Write((Byte)18);
                    binaryWriter.Write(3);
                    binaryWriter.Write((Byte)3);
                    binaryWriter.Write(0);
                    address += 1 + 4 + 1 + 4;
                    
                    // [% zCCSAtomicBlock 0 {index++}]
                    char[] zCCSAtomicBlock = $"[% zCCSAtomicBlock 0 {index++}]".ToCharArray();
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)zCCSAtomicBlock.Length);
                    binaryWriter.Write(zCCSAtomicBlock);
                    address += 1 + 2 + zCCSAtomicBlock.Length;
                    
                    // [% oCMsgConversation:oCNpcMessage:zCEventMessage 0 {index++}]
                    char[] zCEventMessage = $"[% oCMsgConversation:oCNpcMessage:zCEventMessage 0 {index++}]".ToCharArray();
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)zCEventMessage.Length);
                    binaryWriter.Write(zCEventMessage);
                    address += 1 + 2 + zCEventMessage.Length;
                    
                    // subType=enum:0
                    binaryWriter.Write((Byte)18);
                    binaryWriter.Write(4);
                    binaryWriter.Write((Byte)17);
                    binaryWriter.Write(0);
                    address += 1 + 4 + 1 + 4;
                    
                    // text=string:{pair.Value}
                    binaryWriter.Write((Byte)18);
                    binaryWriter.Write(5);
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)pair.Value.Length);
                    binaryWriter.Write(pair.Value.ToCharArray());
                    address += 1 + 4 + 1 + 2 + pair.Value.Length;
                    
                    // name=string:{pair.Key.ToUpper()}.WAV
                    char[] wavFileName = $"{pair.Key.ToUpper()}.WAV".ToCharArray();
                    binaryWriter.Write((Byte)18);
                    binaryWriter.Write(6);
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)wavFileName.Length);
                    binaryWriter.Write(wavFileName);
                    address += 1 + 4 + 1 + 2 + wavFileName.Length;
                    
                    // []
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)2);
                    binaryWriter.Write("[]".ToCharArray());
                    address += 1 + 2 + 2;
                    
                    // []
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)2);
                    binaryWriter.Write("[]".ToCharArray());
                    address += 1 + 2 + 2;
                    
                    // []
                    binaryWriter.Write((Byte)1);
                    binaryWriter.Write((Int16)2);
                    binaryWriter.Write("[]".ToCharArray());
                    address += 1 + 2 + 2;
                    
                    Console.WriteLine($"{(index - 1) / 3}/{count} bin elements written");
                }

                // []
                binaryWriter.Write((Byte)1);
                binaryWriter.Write((Int16)2);
                binaryWriter.Write("[]".ToCharArray());
                address += 1 + 2 + 2;
                
                byte[] binFooter = new[]
                {
                    0x07, 0x00, 0x00, 0x00,

                    0x04, 0x00,
                    0x05, 0x00,
                    0x0D, 0x00, 0x00, 0x00,
                    0x74, 0x65, 0x78, 0x74,
                
                    0x0B, 0x00,
                    0x02, 0x00,
                    0x27, 0x00, 0x00, 0x00,
                    0x6E, 0x75, 0x6D, 0x4F, 0x66, 0x42, 0x6C, 0x6F, 0x63, 0x6B, 0x73,
                
                    0x04, 0x00,
                    0x06, 0x00,
                    0x29, 0x00, 0x00, 0x00,
                    0x6E, 0x61, 0x6D, 0x65,
                
                    0x09, 0x00,
                    0x03, 0x00,
                    0x32, 0x00, 0x00, 0x00,
                    0x73, 0x75, 0x62, 0x42, 0x6C, 0x6F, 0x63, 0x6B, 0x30,
                
                    0x09, 0x00,
                    0x01, 0x00,
                    0x44, 0x00, 0x00, 0x00,
                    0x62, 0x6C, 0x6F, 0x63, 0x6B, 0x4E, 0x61, 0x6D, 0x65,
                
                    0x07, 0x00,
                    0x04, 0x00,
                    0x4A, 0x00, 0x00, 0x00,
                    0x73, 0x75, 0x62, 0x54, 0x79, 0x70, 0x65,
                
                    0x0A, 0x00,
                    0x00, 0x00,
                    0x5D, 0x00, 0x00, 0x00,
                    0x4E, 0x75, 0x6D, 0x4F, 0x66, 0x49, 0x74, 0x65, 0x6D, 0x73,
                }.Select(x => (byte)x).ToArray();
                binaryWriter.Write(binFooter);
                
                // update addressOfLastSignificantByteAddress
                binaryWriter.Seek(addressOfLastSignificantByteAddress, SeekOrigin.Begin);
                binaryWriter.Write(address);
            }
        }
    }
}