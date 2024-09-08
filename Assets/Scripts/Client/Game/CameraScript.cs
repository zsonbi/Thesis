using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;

    private Vector3 CameraOffset;

    private void Start()
    {
        this.CameraOffset = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        this.transform.position = new Vector3(CameraOffset.x + gameController.PlayerPos.x, CameraOffset.y, CameraOffset.z + gameController.PlayerPos.z);
        Ray ray = this.gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Log the 3D object hit by the ray
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
        }

    }
}