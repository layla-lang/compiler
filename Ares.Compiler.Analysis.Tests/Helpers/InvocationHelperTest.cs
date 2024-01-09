using Ares.Compiler.Analysis;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Helpers;
using Ares.Compiler.Checkpoints;
using Ares.Compiler.Parser;
using Ares.Compiler.Tokens;
using Xunit;

namespace Ares.Compiler.Tests.Analysis.Helpers;

public class InvocationHelperTest
{
    private readonly CodeCheckpoint _IsPalindrome = new ("IsPalindrome");
    private readonly CodeCheckpoint _GenericSquare = new ("Square");

    private readonly SyntaxUtilities.UtilityResult result;
    private readonly SourceContext context;

    public KnownMethod IsPalindrome => context.Methods.GetNearest(_IsPalindrome)!;
    public KnownMethod GenericSquare => context.Methods.GetNearest(_GenericSquare)!;

    public InvocationHelperTest()
    {
        this.result = SyntaxUtilities.InspectSource(
            $$"""
              context InvocationHelperTest;

              public Bool {{_IsPalindrome}}(Int x) {
                return x;
              }

              public 'T {{_GenericSquare}}<'T>('T x) {
                return x;
              }
              """);
        this.context = result.Context;
    }
    
    [Fact]
    public void SelectsMatchingNonGenericType()
    {

        var invocation = (InvocationExpressionToken)TokenParser.ParseToken<ExpressionToken>("IsPalindrome(3)");
        var req = invocation.ToResolveMethodRequest(context);
        var methodResponse = InvocationHelper.FindInvokedMethod(context, req);
        
        Assert.Equal(IsPalindrome, methodResponse.UnboundMethod);
    }
    
    [Fact]
    public void SelectsExactTypeArgMatch()
    {
        var invocation = (InvocationExpressionToken)TokenParser.ParseToken<ExpressionToken>("Square<Int>(3)");
        var req = invocation.ToResolveMethodRequest(context);
        var methodResponse = InvocationHelper.FindInvokedMethod(context, req);
        
        Assert.Equal(GenericSquare, methodResponse.UnboundMethod);
    }

    [Fact]
    public void InfersTypeArgMatch()
    {
        var invocation = (InvocationExpressionToken)TokenParser.ParseToken<ExpressionToken>("Square(3)");
        var req = invocation.ToResolveMethodRequest(context);
        var methodResponse = InvocationHelper.FindInvokedMethod(context, req);
        
        Assert.Equal(GenericSquare, methodResponse.UnboundMethod);
        Assert.Equal(TypeEntity.Int, methodResponse.BoundMethod.ParameterTypes[0]);
        Assert.Equal(TypeEntity.Int, methodResponse.BoundMethod.ReturnType);
    }

    [Fact]
    public void Infers1To1Args()
    {
        var te = new TypeArgEntity("'T", null);
        var r = InvocationHelper.InferTypeArgsFromArg(te, TypeEntity.String);
        Assert.Equal(TypeEntity.String, r["'T"]);
    }
    
    [Fact]
    public void InfersArrayArgs()
    {
        var te = new TypeArgEntity("'T", null);
        var declared = new ArrayTypeEntity(te, null);
        var provided = new ArrayTypeEntity(TypeEntity.Bool, null);
        var r = InvocationHelper.InferTypeArgsFromArg(declared, provided);
        Assert.Equal(TypeEntity.Bool, r["'T"]);
    }
    
    [Fact]
    public void InfersSingleTypeArgTupleArgs()
    {
        var te = new TypeArgEntity("'T", null);
        var declared = new TupleTypeEntity(
            new []{ TypeEntity.String, te, TypeEntity.Bool, }, null);
        
        var provided = new TupleTypeEntity(
            new []{ TypeEntity.String, TypeEntity.Float, TypeEntity.Bool, }, null);
        var r = InvocationHelper.InferTypeArgsFromArg(declared, provided);
        Assert.Equal(TypeEntity.Float, r["'T"]);
    }
    
    [Fact]
    public void InfersMultipleTypeArgTupleArgs()
    {
        var te = new TypeArgEntity("'T", null);
        var ue = new TypeArgEntity("`U", null);
        var declared = new TupleTypeEntity(
            new[] { ue, te }, null);
        var provided = new TupleTypeEntity(
            new []{ TypeEntity.String, TypeEntity.Float }, null);
        
        var r = InvocationHelper.InferTypeArgsFromArg(declared, provided);
        Assert.Equal(TypeEntity.Float, r["'T"]);
        Assert.Equal(TypeEntity.String, r["`U"]);
    }
    
    [Fact]
    public void InfersArgFuncArgs()
    {
        var te = new TypeArgEntity("'T", null);
        var declared = new FuncTypeEntity(
            new[] { te }, te, null);
        var provided = new FuncTypeEntity(
            new[] { TypeEntity.Bool }, TypeEntity.Bool, null);
        
        var r = InvocationHelper.InferTypeArgsFromArg(declared, provided);
        Assert.Equal(TypeEntity.Bool, r["'T"]);
    }
    
    [Fact]
    public void InfersAliasTypeArgs()
    {
        var _Stringify = new CodeCheckpoint("Stringify");
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context Scratchpad;

              public type ValFactory<'T> = () => 'T;

              public func {{_Stringify}}<'T>(ValFactory<'T> hi) {
                return hi();
              }

              scratch {
                var s = Stringify(() => "hi");
              }
              """);
        var Stringify = result.Context.Methods.GetNearest(_Stringify);
        var sVal = result.Context.Values["s"]!.Value.Result;
        Assert.Equal(TypeEntity.String, sVal.Type);
    }
}