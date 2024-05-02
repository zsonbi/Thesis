using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField]
    private int speed = 2;

    [SerializeField]
    WheelCollider[] wheelColliders;

    [SerializeField]
    private float motorPower;

    [SerializeField]
    private float steerPower;

    public int Speed { get => speed; }

    private Rigidbody rigidbody;
    private ConstantForce constantForceComponent;

    // Start is called before the first frame update
    private void Start()
    {
        //constantForceComponent = new ConstantForce();
        //this.rigidbody = GetComponent<Rigidbody>();
        //this.constantForceComponent = this.GetComponent<ConstantForce>();
        //Debug.Log(this.rigidbody.GetAccumulatedForce());
    }

    // Update is called once per frame
    private void Update()
    {
        //if (Input.GetKey(KeyCode.W))
        //{
        //    constantForceComponent.relativeForce = new Vector3(0, 0, constantForceComponent.relativeForce.z + Speed * Time.deltaTime);
        //}
        //else if (Input.GetKey(KeyCode.S) && constantForceComponent.relativeForce.z > 0)
        //{
        //    constantForceComponent.relativeForce = new Vector3(0, 0, constantForceComponent.relativeForce.z - Speed * Time.deltaTime * 5);
        //}
        //else
        //{
        //    constantForceComponent.relativeForce = new Vector3(0, 0, 0);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    constantForceComponent.relativeTorque = new Vector3(0, -10 * Time.deltaTime, 0);
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    constantForceComponent.relativeTorque = new Vector3(0, 10 * Time.deltaTime, 0);
        //}
        //else
        //{
        //    constantForceComponent.relativeTorque = new Vector3(0, 0, 0);
        //}

    }

    private void FixedUpdate()
    {
        foreach (var item in wheelColliders)
        {
            item.motorTorque = Input.GetAxis("Vertical")*motorPower;
        }

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (i < 2)
            {
                wheelColliders[i].steerAngle=Input.GetAxis("Horizontal")*steerPower;
            }
        }
    }

}