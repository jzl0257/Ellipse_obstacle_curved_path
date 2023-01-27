using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAngle : MonoBehaviour
{
    public int angle;
    
    public GameObject center;
    private float new_X;
    private float new_y;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var distance = Vector3.Distance(this.gameObject.transform.position, center.transform.position);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        var x = center.transform.position.x + distance * Mathf.Cos((angle * Mathf.PI) / 180);
        var z = center.transform.position.z + distance * Mathf.Sin((angle * Mathf.PI) / 180);
        transform.position = new Vector3(x, transform.position.y, z);
        
    }
}
