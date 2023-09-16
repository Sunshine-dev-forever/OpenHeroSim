using Godot;
using GUI.DebugInspector.Display;

namespace Item;

//this subcategory of items are for throwing weapons like djerds, javelins, etc
public class Throwable : IItem
{
    public double Damage { get; set; } = 40;
    //the number of times this throwable can be used before it is out of ammo
    public int Count = 4;
    public Node3D Mesh { get; }
    public string Name { get; set; } = "Throwable";
    public IDisplay Display => ConstructDisplay();

    public Throwable(Node3D mesh, double _damage, string name)
    {
        Name = name;
        Mesh = mesh;
        Damage = _damage;
    }
    public void QueueFree()
    {
        Mesh.QueueFree();
    }

    IDisplay ConstructDisplay()
    {
        //TODO: Item containers should have proper ID generation.... one day
        Display root = new(Name);
        root.AddDetail("remaining ammo: " + Count);
        root.AddDetail("Damage: " + Damage);
        root.AddDetail("Mesh name: " + Mesh.Name);
        return root;
    }
}
