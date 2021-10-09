using UnityEngine;

//Extending the GravityController abstract class
public class Gravity : GravityController
{
    public override Vector3 GetGravityDirection(PlayerGravity playerGravity)
    {
        //Get the closest point between the player and the gravity collider object
        Collider player = playerGravity.GetComponent<Collider>();
        Vector3 closestToGravity = player.ClosestPoint(transform.position);
        //Vector3 closestToGravityDirection = (player.transform.position - closestToGravity).normalized; /*Uncomment to draw debug lines*/
        //Debug.DrawRay(player.transform.position, -closestToGravityDirection, Color.green); /*Uncomment to draw debug lines*/


        //Get the collider of the child object (Ground) of the gravity object.
        GameObject groundObject = gameObject.transform.GetChild(0).gameObject;
        Collider groundObjectCollider = groundObject.GetComponent<Collider>();


        //Get the closest point between the surface of the ground object and closestToGravity
        Vector3 closestToGround = groundObjectCollider.ClosestPoint(closestToGravity);
        Vector3 closestToGroundDirection = (closestToGround - closestToGravity).normalized;
        //Debug.DrawRay(closestToGravity, closestToGroundDirection, Color.red); /*Uncomment to draw debug lines*/


        /* [IMPORTANT]
         * When the player is grounded on a spherical object (Spheres, some areas of capsules, etc.), 
         *      closestToGround and closestToGravity may be equal and closestToGroundDirection will return a zero Vector3.
         * This occurs on spherical objects only. 
         * Therefore, the closest direction from player to the gravity objects collider can be taken as the direction of gravity.
        */
        if (closestToGroundDirection != Vector3.zero)
            return closestToGroundDirection;

        //Calculate direction from the player to the gravity objects collider.
        Vector3 sphericalPoint = groundObjectCollider.ClosestPoint(player.transform.position);
        Vector3 sphericalDirection = (groundObject.transform.position - sphericalPoint).normalized;
        return sphericalDirection;

    }
}
