using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Tests
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

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

        // Optionally, you can provide a helper method to start coroutines
        public static Coroutine RunCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}