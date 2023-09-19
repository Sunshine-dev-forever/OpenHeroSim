using Godot;
using System.Collections.Generic;
using Util;

namespace GUI.DebugInspector;

public partial class DebugInspectorDetails : Panel
{
    const string DETAIL_LIST_ITEM_PATH = "res://scenes/gui/debug_inspector/detail_list_item.tscn";
    VBoxContainer ItemDetailsContainer = null!;
    //  Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ItemDetailsContainer = GetNode<VBoxContainer>("ScrollContainer/Items");
    }

    //  Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void AddDetail(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        Control detailListItem = CustomResourceLoader.LoadUI(DETAIL_LIST_ITEM_PATH);
        Label label = detailListItem.GetNode<Label>("Label");
        label.Text = input;
        ItemDetailsContainer.AddChild(detailListItem);
    }

    public void AddDisplay(IEnumerable<string> input)
    {
        IEnumerator<string> enumerator = input.GetEnumerator();
        while (enumerator.MoveNext())
        {
            AddDetail(enumerator.Current);
        }
    }

    public void ResetDetails()
    {
        SceneTreeUtil.RemoveAndFreeAllChildren(ItemDetailsContainer);
    }
}
