using Ares.Compiler.Parser.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ares.Compiler.Transformer;

public class CompilationUnitTransformer
{
    /*
    public static SyntaxTree CreateTreeFromStmt(AresProject proj, Statement.StmtStx stmt)
    {
        var statements = StatementTransformer.TransformStatement(stmt);
        var blk = statements is BlockSyntax bs ? bs : SyntaxFactory.Block(new[] { statements });  
        var cu = SyntaxFactory.CompilationUnit()
            .AddMembers(
                SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(proj.Name))
                    .AddMembers(
                        SyntaxFactory.ClassDeclaration("Program")
                            .AddMembers(
                                SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "Main")
                                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                    .WithBody(blk)
                            )
                    )
            );
        return SyntaxFactory.SyntaxTree(cu);
    }
    */
}