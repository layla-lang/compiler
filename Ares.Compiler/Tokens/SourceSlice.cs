using Ares.Compiler.Parser.Syntax;

namespace Ares.Compiler.Tokens;

public record SourcePosition(int Row, int Col, int Index);

public record SourceSlice
{
    private readonly Lazy<SourceCode> codeLazy;
    public SourceSlice(SourcePosition start, SourcePosition end, Func<SourceCode> codeSupplier)
    {
        this.Start = start;
        this.End = end;
        this.codeLazy = new Lazy<SourceCode>(() => codeSupplier());
    }
    
    public SourcePosition Start { get; private set; }
    public SourcePosition End { get; private set; }
    public int StartIndex => Start.Index;
    public int EndIndex => End.Index;
    public int Length => EndIndex - StartIndex;
    public string Value => codeLazy.Value.Code.Substring(StartIndex, Length);

    public static SourceSlice FromSyntaxElementAndCode(Common.SyntaxElement se, Func<SourceCode> codeSupplier)
    {
        var gps = se.CodeSpan;
        var spStart = new SourcePosition((int)gps.Start.Line, (int)gps.Start.Column, (int)gps.Start.Index);
        var spEnd = new SourcePosition((int)gps.End.Line, (int)gps.End.Column, (int)gps.End.Index);
        return new SourceSlice(spStart, spEnd, codeSupplier);
    }
    public static SourceSlice FromSourcePosition(SourcePosition start, SourcePosition end, Func<SourceCode> codeSupplier)
    {
        return new SourceSlice(start, end, codeSupplier);
    }
    public static SourceSlice FromSourcePosition(Common.SourceGps start, Common.SourceGps end, Func<SourceCode> codeSupplier)
    {
        var spStart = new SourcePosition((int)start.Line, (int)start.Column, (int)start.Index);
        var spEnd = new SourcePosition((int)end.Line, (int)end.Column, (int)end.Index);
        return FromSourcePosition(spStart, spEnd, codeSupplier);
    }

    public static SourceSlice FromSyntaxElementsAndCode(Common.SyntaxElement first, Common.SyntaxElement last,
        Func<SourceCode> codeSupplier) =>
        SourceSlice.FromSourcePosition(first.CodeSpan.Start, last.CodeSpan.End, codeSupplier);

}