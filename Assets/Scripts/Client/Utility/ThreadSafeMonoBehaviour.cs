using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Safely stores if the gameobject been destroyed in a bool
    /// </summary>
    public class ThreadSafeMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// True if the gameobject been destroyed
        /// </summary>
        public bool Destroyed { get; private set; }

        /// <summary>
        /// Called when the gameobject has been destroyed
        /// </summary>
        protected void OnDestroy()
        {
            this.Destroyed = true;
            StopAllCoroutines();
        }
    }
}