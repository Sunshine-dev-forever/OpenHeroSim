

using Godot;
using GUI.DebugInspector.Display;
using Item;
using System;
using System.Collections.Generic;

namespace Interactable;

// represents a Building in the world
public partial class Building : Node3D, IInteractable
{
    //building type should perhaps be an enum?
    public static string BLACKSMITH_NAME = "Blacksmith";
    public string BuildingName { get; private set; }
    public Node3D Mesh;
    public IDisplay Display => ConstructDisplay();

    IDisplay ConstructDisplay()
    {
        Display root = new("Building: " + BuildingName);
        return root;
    }

    public Building(string _buildingName, Node3D _mesh)
    {
        BuildingName = _buildingName;
        Mesh = _mesh;
        this.AddChild(Mesh);
    }

    public override void _Process(double delta)
    {
        //unused for now
    }

    public bool IsInstanceValid()
    {
        return IsInstanceValid(this);
    }
    public new void QueueFree()
    {
        base.QueueFree();
    }
}
