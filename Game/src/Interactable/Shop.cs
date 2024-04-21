using Godot;
using GUI.DebugInspector.Display;
using Item;
using System;
using System.Collections.Generic;

namespace Interactable;

// represents shop in the world
public partial class Shop : Node3D, IInteractable
{
    // list of items the shop can sell
    public List<IItem> Items;
    // the container's mesh
    public Node3D Mesh;
    public IDisplay Display => ConstructDisplay();

    IDisplay ConstructDisplay()
    {
        // TODO: Shops should have proper ID generation.... one day
        Display root = new("Shop");
        root.AddDetail("number of contained Items: " + Items.Count);
        root.AddDetail("Mesh name: " + Mesh.Name);
        foreach (IItem item in Items)
        {
            root.AddChildDisplay(item.Display);
        }

        return root;
    }

    public Shop(List<IItem> _items, Node3D _mesh)
    {
        Items = _items;
        Mesh = _mesh;
        this.AddChild(Mesh);
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
