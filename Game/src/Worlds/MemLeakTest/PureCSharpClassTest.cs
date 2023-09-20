using Serilog;

namespace Worlds.MemLeakTest;

// A small class created for testing memory leaks in C#
public class PureCSharpClassTest
{
    public PureCSharpClassTest()
    {
        Log.Information("PureCSharpClassTest created!");
    }
    ~PureCSharpClassTest()
    {
        Log.Information("PureCSharpClassTest finalizer called!");
    }
}
