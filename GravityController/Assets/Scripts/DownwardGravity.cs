using UnityEngine;

//Extending the GravityController abstract class
public class DownwardGravity : GravityController
{

    //Overriding the abstract method of the GravityController abstract class
    public override Vector3 GetGravityDirection(PlayerGravity playerGravity)
    {
        /*
         * Returns the downwards direction (-y) of the gravity collider object.
         * This can be used instead of the 'Gravity' class to calculate the direction of gravity on flat surfaces (Plane, Quad, Terrain, etc.) more efficiently.
         */
        return -transform.up;
    }
}
