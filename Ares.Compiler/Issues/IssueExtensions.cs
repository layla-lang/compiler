using System.Reflection;
using System.Text;
using Ares.Compiler.Parser;
using FParsec;

namespace Ares.Compiler.Issues;

public static class IssueExtensions
{
    private static Dictionary<string, SyntaxError.SyntaxErrorType> SyntaxErrorTypeDict = Enum
        .GetValues<SyntaxError.SyntaxErrorType>()!
        .ToDictionary(
            ev => Enum.GetName<SyntaxError.SyntaxErrorType>(ev)!,
            ev => ev)!;

    private static MethodInfo GetDebuggerDisplayMethod =
        typeof(ErrorMessage).GetMethod("GetDebuggerDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!;
    public static IssueLocation ToIssueLocation(this Position position) => new IssueLocation(
        position.StreamName,
        (int)position.Line,
        (int)position.Column,
        position.Index);

    public static SyntaxError.SyntaxErrorType ToSyntaxErrorType(this ErrorMessageType errMsgType)
    {
        var name = Enum.GetName<ErrorMessageType>(errMsgType);
        return SyntaxErrorTypeDict!.GetValueOrDefault(name, SyntaxError.SyntaxErrorType.Other);
    }

    public static List<SyntaxError> ToSyntaxErrors(this CodeParser.InternalParserException err)
    {
        var loc = err.error.Position.ToIssueLocation();
        var errs = err.error.Messages.ToErrorMessages();
        return errs.Select(err => new SyntaxError(
                err.ToDebuggerMessage(),
                err.Type.ToSyntaxErrorType(),
                loc))
            .ToList();
    }

    public static List<ErrorMessage> ToErrorMessages(this ErrorMessageList errLs)
    {
        var errs = new List<ErrorMessage>();
        ErrorMessageList ls = errLs;
        ErrorMessage msg = ls.Head;
        do
        {
            msg = ls.Head;
            errs.Add(msg);
            ls = ls.Tail;
        } while (ls != null);

        return errs;
    }

    public static string ToDebuggerMessage(this ErrorMessage errMsg) =>
        (string)GetDebuggerDisplayMethod.Invoke(errMsg, Array.Empty<object>())!;
}