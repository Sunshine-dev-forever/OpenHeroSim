using Godot;
using GUI.DebugInspector.Display;
using Item;
using System;
using System.Collections.Generic;

namespace Interactable;

//represents an item container in the world
public partial class ItemContainer : Node3D, IInteractable
{
    //list of items the container contains
    public List<IItem> Items;
    //the container's mesh
    public Node3D Mesh;
    private DateTime timeLastEmpty = DateTime.MaxValue;
    private static readonly int TIME_TO_LIVE_WHEN_EMPTY_SECONDS = 5;

    public IDisplay Display => ConstructDisplay();

    private IDisplay ConstructDisplay()
    {
        //TODO: Item containers should have proper ID generation.... one day
        Display root = new("Item Container");
        string timeLastEmptyInfo = "time last empty = ";
        if (timeLastEmpty == DateTime.MaxValue)
        {
            timeLastEmptyInfo += "never";
        }
        else
        {
            timeLastEmptyInfo += timeLastEmpty.ToString();
        }
        root.AddDetail(timeLastEmptyInfo);
        root.AddDetail("number of contained Items: " + Items.Count);
        root.AddDetail("Mesh name: " + Mesh.Name);
        foreach (IItem item in Items)
        {
            root.AddChildDisplay(item.Display);
        }
        return root;
    }

    public ItemContainer(List<IItem> _items, Node3D _mesh)
    {
        Items = _items;
        Mesh = _mesh;
        this.AddChild(Mesh);
    }

    public override void _Process(double delta)
    {
        //only update TimeLastEmpty if the container is empty and
        //the container used to be full (which is what TimeLastEmpty being DateTime.MaxValue means)
        if (Items.Count == 0 && timeLastEmpty == DateTime.MaxValue)
        {
            timeLastEmpty = DateTime.Now;
        }
        else if (Items.Count >= 1)
        {
            timeLastEmpty = DateTime.MaxValue;
        }

        //delete this object if it has been empty for more than TIME_TO_LIVE_WHEN_EMPTY_SECONDS
        if ((DateTime.Now - timeLastEmpty).TotalSeconds > TIME_TO_LIVE_WHEN_EMPTY_SECONDS)
        {
            this.QueueFree();
        }
    }

    public bool IsInstanceValid()
    {
        return IsInstanceValid(this);
    }
    public new void QueueFree()
    {
        foreach (IItem item in Items)
        {
            item.QueueFree();
        }
        base.QueueFree();
    }
}