using Godot;

namespace Worlds;

public interface IRunner
{
    void Process(double delta);
    void Input(InputEvent input);
}
