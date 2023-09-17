using Godot;
using Serilog;

//Just sets up Serilog for now
public partial class Startup : Node
{
    public override void _Ready()
    {
        //Setting up Serilog
        Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
    }
}
