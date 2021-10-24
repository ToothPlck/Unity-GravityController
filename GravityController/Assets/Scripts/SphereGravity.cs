using UnityEngine;

//Extending the GravityController abstract class
public class SphereGravity : GravityController
{
    //Overriding the abstract method of the GravityController abstract class
    public override Vector3 GetGravityDirection(PlayerGravity playerGravity)
    {
        //Get the collider of the child object (Ground) of the gravity object.
        GameObject groundObject = gameObject.transform.GetChild(0).gameObject;

        /*
         * Returns the direction from the ground objects position towards the player objects position (Closest distance between the two objects)
         * This is a more efficient alternative for the 'Gravity' class to calculate the direction of gravity of spherical surfaces.
         */
        return (groundObject.transform.position - playerGravity.transform.position).normalized;
    }
}
