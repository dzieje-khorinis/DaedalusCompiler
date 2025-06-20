using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DaedalusCompiler.Resources;

public enum ResourceType
{
    MenuD,
    GothicD,
    HelpTxt
}

public abstract class ResourcesHelper
{
    private static readonly Dictionary<ResourceType, string> ResourceMap = new()
    {
        { ResourceType.MenuD, "DaedalusCompiler.Resources.menu.d" },
        { ResourceType.GothicD, "DaedalusCompiler.Resources.gothic.d" },
        { ResourceType.HelpTxt, "DaedalusCompiler.Resources.help.txt" },
    };

    public static string Get(ResourceType resourceType)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(ResourceMap[resourceType]);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }

    public static string Get(string resourcePath)
    {
        return ResourceMap.ContainsValue(resourcePath)
            ? Get(ResourceMap.First(kvp => kvp.Value == resourcePath).Key)
            : "";
    }

    public static string GetPath(ResourceType resourceType)
    {
        return ResourceMap[resourceType];
    }
}