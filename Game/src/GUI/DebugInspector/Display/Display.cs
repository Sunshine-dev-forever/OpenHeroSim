using System.Collections.Generic;

namespace GUI.DebugInspector.Display;

public class Display : IDisplay
{
    readonly List<IDisplay> children = new();
    readonly List<string> details = new();

    public string Name { get; set; }

    Display()
    {
        Name = "";
    }

    public Display(string _name)
    {
        Name = _name;
    }

    public static Display GenerateTest()
    {
        // Holy crap I need real unit tests
        Display root = new();
        Display child1 = new();
        Display child2 = new();
        root.AddDetail("name: rock");
        root.AddDetail("another detail");
        root.Name = "TesyMcTesterson";
        child1.Name = "stats";
        child1.AddDetail("attack: 32");
        child1.AddDetail("defense: 23213");
        child2.AddDetail("crappy wooden sword");
        child2.AddDetail("better wooden sheild");
        child2.Name = "equipment";
        root.AddChildDisplay(child1);
        root.AddChildDisplay(child2);
        return root;
    }

    public void AddChildDisplay(IDisplay displayable)
    {
        children.Add(displayable);
    }
    public void AddDetail(string detail)
    {
        details.Add(detail);
    }

    public List<IDisplay> GetChildDisplays()
    {
        return children;
    }

    public List<string> GetDetails()
    {
        return details;
    }
}
