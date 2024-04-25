using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    ConstantForce2D constantForceComponent;

    [SerializeField]
    private int speed=2;

    public int Speed {  get=>speed; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            constantForceComponent.force=new Vector2 (0,constantForceComponent.force.y+Speed*Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S) && constantForceComponent.force.y>0)
        {
            constantForceComponent.force = new Vector2(0, constantForceComponent.force.y - Speed * Time.deltaTime);
        }
        else if(constantForceComponent.force.y > 0)
        {
            constantForceComponent.force = new Vector2(0, constantForceComponent.force.y - Time.deltaTime);
        }


    }
}
