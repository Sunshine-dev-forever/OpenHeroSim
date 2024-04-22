using System;
using GUI.DebugInspector.Display;
//  represents an item,or anything that will/could be put in a chest
namespace Item;

public class StackItem : IStackable {
    public static string MONEY = "Money";
    public int Count { get; set; }
    public string Name { get; private set; }
    public bool IsMoney => Name == MONEY;
    public IDisplay Display => ConstructDisplay();
    public int Value => Count;
    public StackItem(int _count, string _name) {
        Count = _count;
        Name = _name;
    }
    IDisplay ConstructDisplay() {
        // TODO: Item containers should have proper ID generation.... one day
        Display root = new(Name + ": " + Count);
        return root;
    }
    public void QueueFree() {
        //no resources associated with this object
    }
}
