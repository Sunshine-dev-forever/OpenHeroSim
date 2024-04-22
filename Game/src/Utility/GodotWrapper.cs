using Godot;

namespace Util;

public partial class GodotWrapper<T> : GodotObject {
    public T value;
    public GodotWrapper(T _value) {
        value = _value;
    }
}
