using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private ConstantForce constantForceComponent;

    [SerializeField]
    private int speed = 2;

    public int Speed { get => speed; }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            constantForceComponent.relativeForce = new Vector3(0, 0, constantForceComponent.relativeForce.z + Speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S) && constantForceComponent.relativeForce.z > 0)
        {
            constantForceComponent.relativeForce = new Vector3(0, 0, constantForceComponent.relativeForce.z - Speed * Time.deltaTime * 2);
        }
        else if (constantForceComponent.relativeForce.z > 0)
        {
            constantForceComponent.relativeForce = new Vector3(0, 0, constantForceComponent.relativeForce.z - Time.deltaTime);
        }
    }
}