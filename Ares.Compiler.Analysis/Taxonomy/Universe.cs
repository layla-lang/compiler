using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Tables;

namespace Ares.Compiler.Analysis.Taxonomy;

public class Universe
{
    public TypeNode RootNode = new (TypeEntity.Any);
    
    public class TypeNode
    {
        public TypeNode(TypeEntity item)
        {
            Type = item;
        }
        public TypeNode? Parent { get; set; }
        public TypeEntity Type { get; }
        public List<TypeNode> Subtypes => new List<TypeNode>();

        public void ReParent(TypeNode newParent)
        {
            if (Parent != null)
            {
                Parent.Subtypes.Remove(this);
            }

            Parent = newParent;
            newParent.Subtypes.Add(this);
        }
    };
    
    private readonly Dictionary<TypeEntity, TypeNode> nodeLookup;
    private readonly Scope scope;

    public Universe(Scope scope)
    {
        this.scope = scope;
        this.nodeLookup = new Dictionary<TypeEntity, TypeNode>();
    }
    private Universe(Scope scope, IDictionary<TypeEntity, TypeNode> nodeLookup)
    {
        this.scope = scope;
        this.nodeLookup = new Dictionary<TypeEntity, TypeNode>(nodeLookup);
    }

    public TypeNode this[TypeEntity index] => FindOrCreateNodeForType(index);
    public void AddTypeExtendsFact(TypeEntity extendingType, TypeEntity extendedType)
    {
        var extendingNode = FindOrCreateNodeForType(extendingType);
        var extendedNode = FindOrCreateNodeForType(extendedType);
        extendingNode.ReParent(extendedNode);
    }

    public TypeEntity LowestCommonAncestor(TypeEntity t1, TypeEntity t2)
    {
        var n1 = FindOrCreateNodeForType(t1);
        var n2 = FindOrCreateNodeForType(t2);

        var path1 = BuildPathToRoot(n1);
        var path2 = BuildPathToRoot(n2);

        int offset;
        TypeEntity ancestor = TypeEntity.Any;
        for (offset = 0; offset < Math.Min(path1.Count, path2.Count); offset++)
        {
            var p1 = path1[path1.Count - 1 - offset];
            var p2 = path2[path2.Count - 1 - offset];

            if (p1 != p2)
            {
                break;
            }
            else
            {
                ancestor = p1;
            }
        }

        return ancestor;
    }

    public TypeEntity GetParent(TypeEntity entity)
    {
        var node = FindOrCreateNodeForType(entity);
        return node.Parent?.Type ?? TypeEntity.Any;
    }
    public List<TypeEntity> GetChildren(TypeEntity entity)
    {
        var node = FindOrCreateNodeForType(entity);
        return node.Subtypes.Select(st => st.Type).ToList();
    }

    public Universe NewScoped(string name) => new(new Scope(name), nodeLookup);
    public Universe NewScoped(Scope scope) => new(scope, nodeLookup);

    private TypeNode FindOrCreateNodeForType(TypeEntity entity)
    {
        if (!this.nodeLookup.ContainsKey(entity))
        {
            var node = new TypeNode(entity);
            node.Parent = RootNode;
            RootNode.Subtypes.Add(node);
            this.nodeLookup.Add(entity, node);
            return node;
        }

        return this.nodeLookup[entity];
    }

    private List<TypeEntity> BuildPathToRoot(TypeNode node)
    {
        var ls = new List<TypeEntity>();
        TypeNode current = node;
        while (current.Type != TypeEntity.Any)
        {
            current = current.Parent!;
            ls.Add(current.Type);
        }

        return ls;
    }
}