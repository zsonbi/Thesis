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
            float steering = 0;
            float v = 10;

            int layerMask = (1 << 6) | (1 << 9);
            layerMask = ~layerMask;
            RaycastHit hitForward;
            RaycastHit hitLeft;
            RaycastHit hitRight;
            bool collided = false;
            //Forward raycast
            collided = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitForward, GameConfig.POLICE_RAYCAST_FORWARD_DISTANCE, layerMask);
            //Left raycast
            Vector3 leftDirection = Quaternion.Euler(0, -10, 0) * transform.TransformDirection(Vector3.forward);
            collided = Physics.Raycast(transform.position, leftDirection, out hitLeft, GameConfig.POLICE_RAYCAST_SIDE_DISTANCE, layerMask) || collided;
            //Right raycast
            Vector3 rightDirection = Quaternion.Euler(0, 10, 0) * transform.TransformDirection(Vector3.forward);
            collided = Physics.Raycast(transform.position, rightDirection, out hitRight, GameConfig.POLICE_RAYCAST_SIDE_DISTANCE, layerMask) || collided;

            if (collided)
            {
                if (hitForward.collider is not null)
                {
                    if (hitForward.collider.gameObject.layer == 1 << 7)
                    {
                        v = 20;
                    }
                    else
                    {
                        if (hitForward.distance < GameConfig.POLICE_REVERSE_DISTANCE)
                        {
                            v = -40;
                            Debug.Log("Hit forward " + hitForward.collider.gameObject.name);
                        }
                        else if (Vector3.Distance(gameController.PlayerPos, this.gameObject.transform.position) > 4f)
                        {
                            //Determine the steering
                            if (hitLeft.collider is not null)
                            {
                                if (hitRight.collider is not null)
                                {
                                    steering = hitRight.distance - hitLeft.distance;
                                }
                                else
                                {
                                    steering = 1;
                                }
                            }
                            else if (hitRight.collider is not null)
                            {
                                steering = -1;
                            }
                        }
                    }
                }
            }
            else
            {
                steering = DetermineSteeringDirectionTowardsPlayer();
            }

            carController.Move(steering, v, 0f, 0f);
        }
    }
}