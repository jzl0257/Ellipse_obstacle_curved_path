using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;

public class UserPosRecord : MonoBehaviour
{
    public GameObject worldAnchor;
    public GameObject obstacle;
    private bool isRecording = false;
    private double startTime;
    private Vector3 startPos;
    public GameObject startMark;
    public GameObject endMark;
    Collider start_Collider;
    Collider end_Collider;
    private Quaternion startRot;
    int count;
    float radius;
    private List<PositionEntry> entries = new List<PositionEntry>();
    // Start is called before the first frame update

    float[] xpositions = { 7.5f, 7.5f, 5.0f, 5.0f, -7.5f, -7.5f, -5.0f, -5.0f };
    float[] zpositions = { 7.5f, 2.5f, -2.5f, -7.5f, 7.5f, 2.5f, -2.5f, -7.5f };
    float[] obstaclescales = { 8.0f, 6.0f, 4.0f, 2.0f, 8.0f, 6.0f, 4.0f, 2.0f };
    //float[] endpositions = { 117.5f, 135.0f, 152.5f, 170.0f, 242.5f, 225.0f, 207.5f,190.0f };
    //float[] startpositions = { 10.0f, 10.0f, 10.0f, 10.0f, 350.0f, 350.0f, 350.0f, 350.0f };
    // test case for random start and end positions 
    float[] endpositions = { 125.0f, 104.0f, 316.0f, 284.0f, 28.0f, 348.0f, 164.0f,283.0f };
    float[] startpositions = { 42.0f, 347.0f, 213.0f, 179.0f, 265.0f, 204.0f, 37.0f, 10.0f };
    float[] remoteinitialaxes = {0.0f,20.0f,40.0f,60.0f,80.0f };
    float xposition;
    float zposition;
    float xposition_shift;
    float zposition_shift;
    float obstaclescale;
    float xstart;
    float zstart;
    float xend;
    float zend;
    float xstart_shift;
    float zstart_shift;
    float xend_shift;
    float zend_shift;
    public float remoteinitialaxis;
    public int initaxiscount;
    public bool instart;
    public bool outstart;



    void Start()
    {
        count = 0;
        radius = 13.0f;
        initaxiscount = 0;
        remoteinitialaxis = remoteinitialaxes[initaxiscount];
        start_Collider = startMark.GetComponent<Collider>();
        end_Collider = endMark.GetComponent<Collider>();
        instart = false;
        outstart = false;


        xposition = xpositions[count];
        zposition = zpositions[count];

        xstart = Mathf.Sin(startpositions[count] * Mathf.Deg2Rad) * radius;
        zstart = Mathf.Cos(startpositions[count] * Mathf.Deg2Rad) * radius;

        xend = Mathf.Sin(endpositions[count] * Mathf.Deg2Rad) * radius;
        zend = Mathf.Cos(endpositions[count] * Mathf.Deg2Rad) * radius;



        xposition_shift = xposition * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad) + zposition * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad);
        zposition_shift = -xposition * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad) + zposition * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad);

        xstart_shift = xstart * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad) + zstart * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad);
        zstart_shift = -xstart * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad) + zstart * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad);

        xend_shift = xend * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad) + zend * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad);
        zend_shift = -xend * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad) + zend * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad);




        // These are the positions when remoteinitialaxis = 0.0f
        obstaclescale = obstaclescales[count];
        obstacle.transform.position = new Vector3(xposition_shift, 0.0f, zposition_shift);
        obstacle.transform.rotation = Quaternion.Euler(0.0f, remoteinitialaxis, 0.0f);
        obstacle.transform.localScale = new Vector3(obstaclescale, 0.1f, 10.0f);


        //this.gameObject.transform.position = new Vector3(x, 0.0f, z);
        startMark.transform.position = new Vector3(xstart_shift, 0, zstart_shift);
        endMark.transform.position = new Vector3(xend_shift, 0, zend_shift);




    }

    // Update is called once per frame
    void Update()
    {
       
        //if (Input.GetKeyDown(KeyCode.C))
        //{
           // if (count >= 8)
            //{
              //  count = 0;
                //if (initaxiscount > 5)
                //{
                  //  Debug.Log("All trials complete !");
                //}
                //initaxiscount++;
            //}
           
            //count++;



        //}

        float startDistance = Vector3.Distance(this.gameObject.transform.position, startMark.transform.position);
        float endDistance = Vector3.Distance(this.gameObject.transform.position, endMark.transform.position);

        //if (startDistance <= 0.2)
        if (start_Collider.bounds.Contains(transform.position) && !instart)
        {
            Debug.Log("Start Recording count no : "+count+" intial angle : "+remoteinitialaxis);
            instart = true;
            outstart = false;
            if (isRecording)
            {
                return;
            }
            else
            {
                isRecording = true;

                startTime = Time.time;
                startPos = worldAnchor.transform.position;
                startRot = worldAnchor.transform.rotation;
            }
        }



        if (isRecording)
        {

            PositionEntry entry;
            //offset from start position
            entry.position = this.gameObject.transform.position - startPos;

            //quaternion diff such that startRot * diff = gameObject.transform.rotation
            entry.rotation = QuatDiff(startRot, this.gameObject.transform.rotation);

            //offset from start time
            entry.timeStamp = Time.time - startTime;

            entries.Add(entry);

        }
       
        if(end_Collider.bounds.Contains(transform.position) && !outstart)
        {
            
            outstart = true;
            instart = false;
            count++;
            // Debug.Log("Stop Recording count no : " + count + " intial angle : " + remoteinitialaxis);

            if (initaxiscount > 4)
            {
                Debug.Log("All trials complete !");
                initaxiscount = 0;
            }
            else
            {
                
                if(count > 7)
                {
                    initaxiscount++;
                    count = 0;
                }
                else
                {
                    
                    Debug.Log("Stop Recording count no : " + count + " intial angle : " + remoteinitialaxis);
                    remoteinitialaxis = remoteinitialaxes[initaxiscount];
                    xposition = xpositions[count];
                    zposition = zpositions[count];

                    xstart = Mathf.Sin(startpositions[count] * Mathf.Deg2Rad) * radius;
                    zstart = Mathf.Cos(startpositions[count] * Mathf.Deg2Rad) * radius;

                    xend = Mathf.Sin(endpositions[count] * Mathf.Deg2Rad) * radius;
                    zend = Mathf.Cos(endpositions[count] * Mathf.Deg2Rad) * radius;



                    xposition_shift = xposition * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad) + zposition * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad);
                    zposition_shift = -xposition * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad) + zposition * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad);

                    xstart_shift = xstart * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad) + zstart * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad);
                    zstart_shift = -xstart * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad) + zstart * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad);

                    xend_shift = xend * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad) + zend * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad);
                    zend_shift = -xend * Mathf.Sin(remoteinitialaxis * Mathf.Deg2Rad) + zend * Mathf.Cos(remoteinitialaxis * Mathf.Deg2Rad);




                    // These are the positions when remoteinitialaxis = 0.0f
                    obstaclescale = obstaclescales[count];
                    obstacle.transform.position = new Vector3(xposition_shift, 0.0f, zposition_shift);
                    obstacle.transform.rotation = Quaternion.Euler(0.0f, remoteinitialaxis, 0.0f);
                    obstacle.transform.localScale = new Vector3(obstaclescale, 0.1f, 10.0f);


                    //this.gameObject.transform.position = new Vector3(x, 0.0f, z);
                    startMark.transform.position = new Vector3(xstart_shift, 0, zstart_shift);
                    endMark.transform.position = new Vector3(xend_shift, 0, zend_shift);
                }
                

                if (isRecording)
                {
                    // stop recording
                    isRecording = false;

                    //save to file
                    string filename = "PositionRecording_entry_angle_" + startpositions[count].ToString() + "_exit_angle_" + endpositions[count].ToString() + "_initial_angle_" + remoteinitialaxis.ToString() +"_" +this.name + "_";
                    //string path = Application.persistentDataPath + "/pathOfDevice";
                    string path = @"C:\Users\ullala\Documents\GitHub\Ellipse_obstacle_curved_path\Assets\savedData";

                    Directory.CreateDirectory(path);

                    int inc = 1;

                    while (File.Exists(path + "/" + filename + inc.ToString() + ".json"))
                    {
                        inc++;
                        Debug.Log("exists");
                    }

                    string filepath = path + "/" + filename + inc.ToString() + ".json";

                    File.WriteAllText(filepath, JsonConvert.SerializeObject(entries, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));

                    Debug.Log(filepath);

                    //clear entries for next recording
                    entries.Clear();
                }
            }
        }
        
    }


    // provides a quaterion, diff, such that a * diff = b
    private Quaternion QuatDiff(Quaternion a, Quaternion b)
    {
        return Quaternion.Inverse(a) * b;
    }
}
