using Godot;
using GUI.DebugInspector.Display;
using System.Collections.Generic;
using Util;

namespace GUI.DebugInspector;

public delegate void ItemSelected(List<string> details);

public partial class DebugInspectorTree : Control
{
    public event ItemSelected? ItemSelected;
    public Tree Tree { get; private set; } = null!;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Tree = GetNode<Tree>("Tree");
        Tree.CellSelected += HandleCellSelected;
    }

    private void HandleCellSelected()
    {
        TreeItem selection = Tree.GetSelected();
        GodotWrapper<List<string>> wrapper = (GodotWrapper<List<string>>)selection.GetMetadata(0);
        ItemSelected?.Invoke(wrapper.value);
    }

    public void CreateNewTree(IDisplay display)
    {
        //TODO: I can only hold this does not cause memory leaks
        Tree.Clear();
        TreeItem root = Tree.CreateItem();

        ConvertDisplayToTreeItem(root, display);

        Tree.SetSelected(root, 0);

    }

    private void ConvertDisplayToTreeItem(TreeItem item, IDisplay display)
    {
        item.SetText(0, display.Name);
        item.SetMetadata(0, new GodotWrapper<List<string>>(display.GetDetails()));
        foreach (IDisplay childDisplay in display.GetChildDisplays())
        {
            TreeItem childItem = item.CreateChild();
            ConvertDisplayToTreeItem(childItem, childDisplay);
        }
    }


}
