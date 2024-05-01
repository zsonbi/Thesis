using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CarProbeScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}