//using Core.ECS;

//namespace NotEngine.ECS.Components;

//public struct HierarchyComponent : IEntityComponent
//{
//    public Actor Self { get; }
//    public Actor? Parent { get; private set; }
//    public List<Actor> Children { get; }

//    public bool IsRoot() => Parent == null;
//    public HierarchyComponent(Actor self, Actor? parent = null)
//    {
//        Self = self;
//        Parent = null;
//        Children = [];
//    }

//    public void SetParent(Actor parent)
//    {
//        if (IsRoot()) throw new OperationCanceledException("Root actor can't set parent");
//        ArgumentNullException.ThrowIfNull(parent, "Parent actor can't be none");
//        if (parent != Parent)
//        {
//            ref HierarchyComponent previousParentHc = ref Parent!.TryGetComponent<HierarchyComponent>(out _);
//            previousParentHc.Children.Remove(Self);

//            ref HierarchyComponent currentParentHc = ref parent.TryGetComponent<HierarchyComponent>(out _);
//            currentParentHc.Children.Add(Self);
//            Parent = parent;
//        }
//    }


//}