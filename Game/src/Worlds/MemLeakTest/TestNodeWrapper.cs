using Godot;
using Serilog;

namespace Worlds.MemLeakTest;

// A small class created for testing memory leaks in C#
public partial class TestNodeWrapper : Node
{
    public TestNodeWrapper()
    {
        Log.Information("TestNodeWrapper created!");
    }

    public override void _Ready()
    {

    }

    ~TestNodeWrapper()
    {
        // I have never seen this line of code printed :(
        Log.Information("TestNodeWrapper finalizer called!");
    }
}
