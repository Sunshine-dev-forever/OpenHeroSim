using Godot;
using System;
using Serilog;
using Serilog.Sinks.SystemConsole;

//TODO need to depreciate this entire class
public class LoggerUtil : Node
{

    public override void _Ready()
    {
        base.Name = "poop";
        Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
    }

    public LoggerUtil(){
        Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
    }
    public void Error(String message) {
        Log.Information("hello this worked!1");
        try{
        throw new Exception("failed");
        } catch (Exception e) {
            Log.Information($"oh no {e}");
        }
    }
}
