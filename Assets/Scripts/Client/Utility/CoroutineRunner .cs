using System.Collections;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Singleton class to run the coroutines so they are easier to track
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        //privatte instance
        private static CoroutineRunner _instance;

        /// <summary>
        /// Singleton instance getter
        /// </summary>
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Create a new GameObject to host the CoroutineRunner if it doesn't exist
                    GameObject obj = new GameObject("CoroutineRunner");
                    _instance = obj.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(obj); // Make sure it persists across scenes
                }
                return _instance;
            }
        }

        /// <summary>
        /// A helper method to start coroutines
        /// </summary>
        /// <param name="coroutine">The coroutine to start</param>
        /// <returns>The startted coroutine</returns>
        public static Coroutine RunCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        /// <summary>
        /// As the name implies stop every coroutine
        /// </summary>
        public static void StopAllCoroutinesGlobal()
        {
            Instance.StopAllCoroutines(); // Directly stop coroutines without recursion
        }
    }
}