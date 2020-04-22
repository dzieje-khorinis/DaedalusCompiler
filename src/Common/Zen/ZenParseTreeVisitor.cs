using System.Linq;
using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;


namespace Common.Zen
{
    public class ZenParseTreeVisitor : ZenBaseVisitor<ZenNode>
    {
        const string VobTree = "VobTree";
        const string WayNet = "WayNet";
        const string EndMarker = "EndMarker";

        private static DateTime GetDateTime(string date, string time)
        {
            string[] d = date.Split('.');
            string[] t = time.Split(':');
            date = $"{d[0].PadLeft(2, '0')}.{d[1].PadLeft(2, '0')}.{d[2].PadLeft(4, '0')}";
            time = $"{t[0].PadLeft(2, '0')}:{t[1].PadLeft(2, '0')}:{t[2].PadLeft(2, '0')}";

            return DateTime.ParseExact($"{date} {time}", "dd.MM.yyyy HH:mm:ss", null);
        }

        public override ZenNode VisitMain(ZenParser.MainContext ctx)
        {
            ZenParser.HeadContext headCtx = ctx.head();

            ZenFileNode fileNode = new ZenFileNode
            {
                Version = int.Parse(headCtx.version.Text),
                Type = headCtx.zenType.Text,
                SaveGame = int.Parse(headCtx.saveGame.Text),
                DateTime = GetDateTime(headCtx.date.Text, headCtx.time.Text),
                User = headCtx.user.Text,
                ObjectsCount = int.Parse(headCtx.objectsCount.Text),
            };

            foreach (IParseTree childCtx in ctx.body.children) //ctx.body is oCWorld
            {
                if (childCtx is ZenParser.BlockContext blockCtx) // childCtx is VobTree | WayNet | EndMarker 
                {
                    switch (blockCtx.blockName().GetText())
                    {
                        case VobTree:
                            fileNode.VobTree = (ZenBlockNode) VisitBlock(blockCtx);
                            break;

                        case WayNet:
                            fileNode.WayNet = (ZenBlockNode) VisitBlock(blockCtx);
                            break;

                        case EndMarker:
                            break;

                        default:
                            throw new Exception($"Invalid section: {blockCtx.blockName().GetText()}");
                    }
                }
            }

            return fileNode;
        }


        public override ZenNode VisitBlock([NotNull] ZenParser.BlockContext ctx)
        {
            ZenBlockNode blockNode = new ZenBlockNode
            {
                BlockName = ctx.blockName().GetText(),
                ClassPath = ctx.classPath().GetText(),
                LeftIndex = int.Parse(ctx.leftIndex.Text),
                RightIndex = int.Parse(ctx.rightIndex.Text),
            };

            foreach (IParseTree childCtx in ctx.children)
            {
                switch (childCtx)
                {
                    case ZenParser.BlockContext blockContext:
                        blockNode.Children.Add(VisitBlock(blockContext));
                        break;
                    case ZenParser.AttrContext attrContext:
                        blockNode.Children.Add(VisitAttr(attrContext));
                        break;
                }
            }

            return blockNode;
        }

        public override ZenNode VisitAttr(ZenParser.AttrContext ctx)
        {
            string[] rside = ctx.Value().GetText().Split(':', 2);
            string type = rside[0];
            string textValue = rside[1];
            Object value = GetValue(type, textValue);

            return new ZenAttrNode
            {
                Name = ctx.Name().GetText(),
                Type = type,
                TextValue = textValue,
                Value = value,
            };
        }

        private Object GetValue(string type, string textValue)
        {
            switch (type)
            {
                case "bool":
                    return int.Parse(textValue) == 1;
                case "color":
                    return textValue.Split(' ').Select(int.Parse).ToList();
                case "enum":
                    return int.Parse(textValue);
                case "float":
                    return float.Parse(textValue);
                case "int":
                    return int.Parse(textValue);
                case "raw":
                    return textValue;
                case "rawFloat":
                    return textValue.Trim().Split(' ').Select(float.Parse).ToList();
                case "string":
                    return textValue;
                case "vec3":
                    return textValue.Trim().Split(' ').Select(float.Parse).ToList();

                default:
                    return textValue;
            }
        }
    }

    public abstract class ZenNode
    {
    }

    public class ZenFileNode : ZenNode
    {
        public int Version;
        public string Type;

        public int SaveGame;
        public DateTime DateTime;

        public string User;

        public int ObjectsCount;
        public ZenBlockNode VobTree;
        public ZenBlockNode WayNet;
    }

    public class ZenBlockNode : ZenNode
    {
        public string BlockName;
        public string ClassPath;

        public int LeftIndex;
        public int RightIndex;
        public readonly List<ZenNode> Children;

        public ZenBlockNode()
        {
            Children = new List<ZenNode>();
        }
    }

    public class ZenAttrNode : ZenNode
    {
        public string Name;
        public string Type;
        public string TextValue;
        public Object Value;
    }
}