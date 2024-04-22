using System.Collections.Generic;

namespace GUI.DebugInspector.Display;

public interface IDisplay {
    public List<IDisplay> GetChildDisplays();
    public List<string> GetDetails();
    public string Name { get; }
}
