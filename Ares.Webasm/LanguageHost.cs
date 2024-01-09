using System;
using System.Runtime.InteropServices.JavaScript;
using Ares.Compiler.IO;
using Ares.Compiler.Parser.Syntax;
using Newtonsoft.Json;

namespace Ares.Webasm;

public static partial class LanguageHost
{
    [JSExport]
    public static string ParseAresSource(string sourceCode)
    {
        var stmt = Ares.Compiler.SyntaxUtilities.InspectSource(sourceCode);
        return stmt.ToJson();
    }
}