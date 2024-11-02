using System.Collections;
using Caliburn.Micro;

namespace NotEngine.Editor.Modules.ContentExplorer.Models.ContextMenu;

public class ContextMenuItemBase : PropertyChangedBase, IEnumerable<ContextMenuItemBase>
{

    public IObservableCollection<ContextMenuItemBase> Children { get; private set; }
        = new BindableCollection<ContextMenuItemBase>();


    protected ContextMenuItemBase()
    {
    }


    public void Add(params ContextMenuItemBase[] menuItems)
        => menuItems.Apply(Children.Add);

    public IEnumerator<ContextMenuItemBase> GetEnumerator()
        => Children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
