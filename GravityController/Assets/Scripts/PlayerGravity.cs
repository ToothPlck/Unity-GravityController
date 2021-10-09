using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PlayerGravity : MonoBehaviour
{
    public float gravityForce = 1500;
    public float rotationSpeedToGravity = 5f;
    private Rigidbody playerRigibody;
    private List<GravityController> gravityAreas;

    // Start is called before the first frame update
    void Start()
    {
        //Get the players rigidbody component
        playerRigibody = transform.GetComponent<Rigidbody>();
        gravityAreas = new List<GravityController>();
    }

    public Vector3 GravityDirection
    {
        get
        {
            /*
             * If the player is not inside (or entering) a collider of a gravity area, the player is not affected by any gravity. 
             * Therefore, an empty Vector3 is set as the direction of gravity.
             */
            if (gravityAreas.Count == 0)
                return Vector3.zero;


            /*
             * Sort the list of GravityControllers by comparing their priority. Gravity areas with higher priority are set at the end of the list.
             * Example of a sorted list by priority : [0, 0, 1, 2, 3, 3, 3, 5 ,8, 8]
             * If a GravityController is entered to the list with another GravityCollider with the same priority level, it will be added at the end of the list where the priority is equal
             * For example, if a GravityCollider with priority level 0 is entered to the above list, the new list will be : [0, 0, {0}, 1, 2, 3, ...]
             * -> Links to learn more about lambda expressions - [https://www.youtube.com/watch?v=dqheDZH_mNc] or [https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions]
             */
            gravityAreas.Sort((area1, area2) => area1.Priority.CompareTo(area2.Priority));
            //Get the GravityCollider at the end of the list (last entered GravityCollider with the highest priority) and get its gravity direction
            return gravityAreas.Last().GetGravityDirection(this).normalized;
        }
    }

    // FixedUpdate is called once per physics update
    private void FixedUpdate()
    {
        //Get the direction of gravity and add a force to the players rigidbody in that direction
        playerRigibody.AddForce(GravityDirection * (gravityForce * Time.fixedDeltaTime), ForceMode.Acceleration);


        //Rotate the player from its current rotation to the direction of gravity
        Quaternion playerUpRotation = Quaternion.FromToRotation(transform.up, -GravityDirection);
        Quaternion rotatePlayer = Quaternion.Slerp(playerRigibody.rotation, playerUpRotation * playerRigibody.rotation, Time.deltaTime * rotationSpeedToGravity);
        playerRigibody.MoveRotation(rotatePlayer);
    }

    public void AddGravityArea(GravityController gravity)
    {
        gravityAreas.Add(gravity); //Add the gravityController to the 'List<GravityController> gravityAreas'
    }

    public void RemoveGravityArea(GravityController gravity)
    {
        gravityAreas.Remove(gravity); //Remove the gravityController to the 'List<GravityController> gravityAreas'
    }
}
