

using System;
namespace Worlds.BattleRoyale;
public class FogController
{
    //Fog Controller just has the magically know the sidelength is 500
    //its ok for testing
    static readonly int SIDE_LENGTH = 500;
    static FogController? instance;

    //The arena fight will last 3 minutes by default
    static readonly int TIME_TILL_ALL_FOG = 180;

    DateTime timeFogStarted = DateTime.MinValue;

    FogController() { }

    public static FogController GetFogController()
    {
        if (instance == null)
        {
            instance = new FogController();
        }
        return instance;
    }

    public void StartFog()
    {
        timeFogStarted = DateTime.Now;
    }

    //Gives fog position away from the origin as a scalar,
    //assuming you dropped a perpendicular to the side of a square
    public double GetFogPosition()
    {
        if (DateTime.MinValue == timeFogStarted)
        {
            return SIDE_LENGTH / 2;
        }
        //We are using center from origin so we want to use half of side length
        double timeDiffSeconds = (DateTime.Now - timeFogStarted).TotalSeconds;
        return (1 - (timeDiffSeconds / TIME_TILL_ALL_FOG)) * (SIDE_LENGTH / 2);
    }

    public Boolean IsInbounds(Godot.Vector3 point)
    {
        double x = point.X;
        double z = point.Z;
        double fogPosition = GetFogPosition();
        return (Math.Abs(x) < fogPosition) && (Math.Abs(z) < fogPosition);
    }

}