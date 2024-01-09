using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Module.Inline;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = true)]
public class AresTypeAttribute : Attribute
{
    public AresTypeAttribute(TypeEntity typeEntity)
    {
        this.AresType = typeEntity;
    }
    
    public TypeEntity AresType { get; }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = true)]
public sealed class AresTypeAttribute<T> : AresTypeAttribute
    where T : TypeEntity, new()
{
    public AresTypeAttribute() : base(new T())
    {
    }
}