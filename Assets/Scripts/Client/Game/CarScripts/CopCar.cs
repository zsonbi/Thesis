using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Game
{
    public class CopCar : NpcCar
    {
        protected override void Update()
        {
            if (!Alive)
            {
                carController.Move(0f, 0f, 0f, 0f);
                return;
            }

            base.Update();

            float steering = 0f;
            float v = 1f;

            int layerMask = (1 << 6) | (1 << 9);
            layerMask = ~layerMask;

            // Raycast distances and directions
            Vector3 forwardDirection = transform.TransformDirection(Vector3.forward);
            Vector3 leftDirection = Quaternion.Euler(0, -30, 0) * forwardDirection;
            Vector3 rightDirection = Quaternion.Euler(0, 30, 0) * forwardDirection;

            // Perform raycasts
            bool forwardHit = Physics.Raycast(transform.position, forwardDirection, out RaycastHit hitForward, GameConfig.POLICE_RAYCAST_FORWARD_DISTANCE, layerMask);
            bool leftHit = Physics.Raycast(transform.position, leftDirection, out RaycastHit hitLeft, GameConfig.POLICE_RAYCAST_SIDE_DISTANCE, layerMask);
            bool rightHit = Physics.Raycast(transform.position, rightDirection, out RaycastHit hitRight, GameConfig.POLICE_RAYCAST_SIDE_DISTANCE, layerMask);

            if (forwardHit)
            {
                if (forwardHit && hitForward.distance < GameConfig.POLICE_REVERSE_DISTANCE)
                {
                    // Reverse if too close to an obstacle
                    v = -2f;
            
                }

                    // Determine steering based on side raycasts
                    if (leftHit && rightHit)
                    {
                        steering = hitRight.distance - hitLeft.distance; // Steer based on the relative distances
                    }
                    else if (leftHit)
                    {
                        steering = 1f;  // Obstacle on the left, steer right
                    }
                    else if (rightHit)
                    {
                        steering = -1f; // Obstacle on the right, steer left
                    }
                
         
                if (Vector3.Distance(gameController.PlayerPos, this.gameObject.transform.position) < 4f)
                {
                    // Close to player, increase speed and steer towards player
                    v = 3f;
                    steering = DetermineSteeringDirectionTowardsPlayer();
                }
            }
            else
            {
                // No obstacle detected, steer towards player
                steering = DetermineSteeringDirectionTowardsPlayer();
            }

            // Move the car
            carController.Move(steering, v, 0f, 0f);

            // Optional: Debugging ray visualization
            Debug.DrawRay(transform.position, forwardDirection * GameConfig.POLICE_RAYCAST_FORWARD_DISTANCE, Color.red);
            Debug.DrawRay(transform.position, leftDirection * GameConfig.POLICE_RAYCAST_SIDE_DISTANCE, Color.yellow);
            Debug.DrawRay(transform.position, rightDirection * GameConfig.POLICE_RAYCAST_SIDE_DISTANCE, Color.green);
        }


    }
}