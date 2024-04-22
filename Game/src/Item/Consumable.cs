using GUI.DebugInspector.Display;

namespace Item;

// Consumable is a single-use item that a pawn can use to heal itself
// In the future Consumables will have many possible effects, 
// but for now it is just healing
public class Consumable : IItem {
    public double Healing { get; }
    public string Name { get; set; } = "Consumable";

    public int Value => (int)Healing;

    public IDisplay Display => ConstructDisplay();

    public Consumable(double _Healing, string name) {
        Name = name;
        Healing = _Healing;
    }

    public Consumable Copy() => new(Healing, Name);

    public void QueueFree() { }

    IDisplay ConstructDisplay() {
        // TODO: Item containers should have proper ID generation.... one day
        Display root = new(Name);
        root.AddDetail("Healing: " + Healing);
        return root;
    }
}
