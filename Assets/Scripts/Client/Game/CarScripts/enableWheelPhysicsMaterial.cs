using UnityEngine;
using System.Collections;

namespace Game
{
    public class EnableWheelPhysicMaterial : ThreadSafeMonoBehaviour
    {
        private WheelCollider wheel;

        private float originalSidewaysStiffness;
        private float originalForwardStiffness;

        private void Start()
        {
            wheel = GetComponent<WheelCollider>();

            originalSidewaysStiffness = wheel.sidewaysFriction.stiffness;
            originalForwardStiffness = wheel.forwardFriction.stiffness;
        }

        // static friction of the ground material.
        private void FixedUpdate()
        {
            WheelHit hit;
            if (wheel.GetGroundHit(out hit))
            {
                WheelFrictionCurve fFriction = wheel.forwardFriction;
                fFriction.stiffness = hit.collider.material.staticFriction * originalForwardStiffness;
                wheel.forwardFriction = fFriction;

                WheelFrictionCurve sFriction = wheel.sidewaysFriction;
                sFriction.stiffness = hit.collider.material.staticFriction * originalSidewaysStiffness;
                wheel.sidewaysFriction = sFriction;
            }
        }
    }
}