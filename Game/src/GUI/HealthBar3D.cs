using Godot;

namespace GUI;

// simple class that sets the fill percent of a health bar
public partial class HealthBar3D : Sprite3D {
    ProgressBar? bar;
    public override void _Ready() {
        bar = GetNode<ProgressBar>("SubViewport/HealthBar2D");
        Texture = GetNode<SubViewport>("SubViewport").GetTexture();
        SetHealthPercent(1);
    }

    public void SetHealthPercent(double percent) {
        if (bar != null) {
            bar.Value = percent * 100;
        }
    }
}
