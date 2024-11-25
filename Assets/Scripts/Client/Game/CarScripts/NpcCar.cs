using UnityEngine;
using Utility;

namespace Game
{
    /// <summary>
    /// Object for the npc cars
    /// It is not yet used
    /// </summary>
    public class NpcCar : Car
    {
        /// <summary>
        /// Determine the way the player is
        /// </summary>
        /// <returns>The dir towards the player [-1,1]</returns>
        public float DetermineSteeringDirectionTowardsPlayer()
        {
            Vector3 difference = this.gameController.PlayerPos - this.gameObject.transform.position;
            Quaternion.RotateTowards(Quaternion.LookRotation(difference), this.gameObject.transform.rotation, Time.deltaTime);
            Vector3 crossProduct = Vector3.Cross(this.transform.forward, difference);
            //Debug.Log(crossProduct.y);
            return (crossProduct.y * 0.75f);
        }
    }
}