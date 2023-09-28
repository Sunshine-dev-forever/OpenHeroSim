using GUI.DebugInspector.Display;
//  represents an item,or anything that will/could be put in a chest
namespace Item;

public interface IStackable : IItem
{
    public int Count { get; set; }
}
