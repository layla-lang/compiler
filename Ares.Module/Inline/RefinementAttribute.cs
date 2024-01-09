namespace Ares.Module.Inline;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = true)]
public class RefinementAttribute : Attribute
{
    public RefinementAttribute(string refinementId)
    {
        RefinementId = refinementId;
    }

    public string RefinementId { get; }
}