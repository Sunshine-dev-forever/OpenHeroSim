using Godot;
using System;

namespace Pawn;
public class PawnMovement
{
    private readonly NavigationAgent3D navigationAgent;
    private readonly RayCast3D downwardRayCast;
    private readonly RigidBody3D rigidBody;
    private readonly PawnVisuals pawnVisuals;

    private Vector3 originalLocationOfTarget = Vector3.Zero;


    private bool isNavigationServerReady;

    public PawnMovement(RigidBody3D _rigidBody, PawnVisuals _pawnVisuals, NavigationAgent3D _navigationAgent, RayCast3D _downwardRayCast)
    {
        rigidBody = _rigidBody;
        pawnVisuals = _pawnVisuals;
        navigationAgent = _navigationAgent;
        downwardRayCast = _downwardRayCast;
    }

    //ProcessMovement should be called in the _PhysicsProcess function
    public void ProcessMovement(Vector3 targetLocation, float speed)
    {
        //the Navigation Server can take some time to start up
        if (!isNavigationServerReady)
        {
            UpdateIsNavigationServerReady();
            return;
        }

        //TODO: dont update path for every minor change in position
        if (targetLocation != originalLocationOfTarget)
        {
            navigationAgent.TargetPosition = targetLocation;
            originalLocationOfTarget = targetLocation;
        }

        Vector3 floorNormal;
        if (downwardRayCast.IsColliding())
        {
            floorNormal = downwardRayCast.GetCollisionNormal();
        }
        else
        {
            //if in air then let physics take over and just fall
            return;
        }
        Vector3 nextLocation = navigationAgent.GetNextPathPosition();
        Vector3 currentLocation = rigidBody.GlobalTransform.Origin;
        Vector3 locationDiff = nextLocation - currentLocation;
        //Look in the direction of travel
        float newYRotation = (float) (Math.Atan2(locationDiff.X , locationDiff.Z));
        pawnVisuals.setPawnRotation(newYRotation);

        Vector3 velocity = (nextLocation - currentLocation).Slide(floorNormal).Normalized() * speed;
        rigidBody.LinearVelocity = velocity;
    }

    //Stops the pawn in place
    public void Stop()
    {
        //Cant stop if we are in the air
        if (downwardRayCast.IsColliding())
        {
            rigidBody.LinearVelocity = Vector3.Zero;
        }
    }

    /*
    *right now i am going of the navigation agents final destination to determine
    *if the final location has been reached
    *NOTE: the final location will always be locationed on the navigation mesh
    */
    public bool HasFinishedMovement(float targetDistance)
    {
        if (navigationAgent.GetFinalPosition().DistanceTo(rigidBody.GlobalTransform.Origin)
            < targetDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetNavigation(NavigationRegion3D navigation)
    {
        navigationAgent.SetNavigationMap(NavigationServer3D.RegionGetMap(navigation.GetRegionRid()));
    }

    //TODO: in godot 4 there may be a better way to do this
    private void UpdateIsNavigationServerReady()
    {
        Rid mapRid = NavigationServer3D.AgentGetMap(navigationAgent.GetRid());
        //an ID of 0 should always be an invalid ID
        //I use the ID of 0 to check if the Navigation server has started up
        //This method might not be fullproof, so this is a possible source of bugs
        if (mapRid.Id == 0)
        {
            isNavigationServerReady = false;
        }
        else
        {
            isNavigationServerReady = true;
        }
    }

    //returns the bearing to the point given 
    //assumes x is horizontal axis, y is vertical axis
    //gives bearing relative to the horizontal negative axis
    //such that quadrant 2 has the lowest bearings
    private double GetBearingTo(Vector2 point)
    {
        //Dont question it, it works :)
        return Math.PI - Math.Atan2(point.Y, point.X);
    }

}
