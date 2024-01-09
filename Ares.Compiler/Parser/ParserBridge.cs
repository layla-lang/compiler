using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;
using FParsec;

namespace Ares.Compiler.Parser;

public class ParserBridge
{
    public static Context.ContextSyntaxElement ParseContext(string str) => CodeParser.parseContext(str);
    public static Statement.StatementSyntaxElement ParseStatement(string str) => CodeParser.parseStatement(str);
}