namespace Ares.Compiler.Analysis.Entities.Types.TypeArgs;

public record ClosedTypeArg(string Identifier, TypeEntity ClosingType) : TypeArg(Identifier);
