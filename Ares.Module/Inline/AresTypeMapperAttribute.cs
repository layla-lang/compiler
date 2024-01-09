using Ares.Module.Mapping;

namespace Ares.Module.Inline;

[AttributeUsage(
    AttributeTargets.Assembly |
    AttributeTargets.Module |
    AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class AresTypeMapperAttribute : Attribute
{
    internal static readonly Type MapperType = typeof(IAresTypeMapper);
    
    public AresTypeMapperAttribute(Type mapperType)
    {
        if (!MapperType.IsAssignableFrom(mapperType))
        {
            throw new ArgumentException($"Type '{mapperType}' is not valid. Must be assignable to {MapperType}.");
        }

        try
        {
            this.Mapper = (IAresTypeMapper)Activator.CreateInstance(MapperType)!;
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Type '{mapperType}' is missing a parameterless constructor.");
        }
    }
    
    public IAresTypeMapper Mapper { get; }
}

[AttributeUsage(
    AttributeTargets.Assembly |
    AttributeTargets.Module |
    AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class AresTypeMapperAttribute<T> : Attribute where T : IAresTypeMapper, new()
{
    public AresTypeMapperAttribute()
    {
        this.Mapper = new T();
    }
    
    public IAresTypeMapper Mapper { get; }
}