using Game;
using UnityEngine;
using Utility;

namespace Game
{
    namespace World
    {
        /// <summary>
        /// Coin gameobject
        /// </summary>
        public class Coin : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// Called every frame
            /// Handles the coin rotation
            /// </summary>
            private void Update()
            {
                this.gameObject.transform.Rotate(0, 0, Time.deltaTime * 45);
            }

            /// <summary>
            /// Trigger for coin pick up
            /// </summary>
            /// <param name="other">What it collided with</param>
            private void OnTriggerEnter(Collider other)
            {
                this.gameObject.SetActive(false);
                other.gameObject.GetComponentInParent<PlayerCar>().PickedUpCoin();
            }
        }
    }
}