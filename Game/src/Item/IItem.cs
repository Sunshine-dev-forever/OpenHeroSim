using GUI.DebugInspector.Display;
//  represents an item,or anything that will/could be put in a chest
namespace Item;

public interface IItem : IDisplayable {
    public string Name { get; }
    public int Value { get; }
    public void QueueFree();
}
