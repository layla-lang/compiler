using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Module.Inline;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = true)]
sealed class AresTypeAttribute : Attribute
{
    public AresTypeAttribute(TypeEntity typeEntity)
    {
        this.AresType = typeEntity;
    }
    
    public TypeEntity AresType { get; }
}