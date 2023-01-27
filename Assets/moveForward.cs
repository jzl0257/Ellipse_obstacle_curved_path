using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveForward : MonoBehaviour
{
    public Transform start;
    public Transform end;
    Vector3 target1;
    Vector3 target2;
    Vector3 target3;

    private int speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            print("W");
            this.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        }

        if (Input.GetKey(KeyCode.A))
        {
            print("A");
            this.transform.Translate(Vector3.left * speed * Time.deltaTime, Space.Self);
        }

        if (Input.GetKey(KeyCode.S))
        {
            print("S");
            this.transform.Translate(Vector3.back * speed * Time.deltaTime, Space.Self);
        }

        if (Input.GetKey(KeyCode.D))
        {
            print("D");
            this.transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
        }
    }

}
