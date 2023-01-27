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
    private Quaternion startRot;
    int count;
    float radius;
    private List<PositionEntry> entries = new List<PositionEntry>();
    // Start is called before the first frame update

    float[] xpositions = { 7.5f, 7.5f, 5.0f, 5.0f, -7.5f, -7.5f, -5.0f, -5.0f };
    float[] zpositions = { 7.5f, 2.5f, -2.5f, -7.5f, 7.5f, 2.5f, -2.5f, -7.5f };
    float[] obstaclescales = { 8.0f, 6.0f, 4.0f, 2.0f, 8.0f, 6.0f, 4.0f, 2.0f };
    float[] startpositions = { 117.5f, 135.0f, 152.5f, 170.0f, 260.0f, 242.5f, 225.0f, 207.5f };
    float[] endpositions = { 10.0f, 10.0f, 10.0f, 10.0f, 350.0f, 350.0f, 350.0f, 350.0f };
    float xposition;
    float zposition;
    float obstaclescale;


    void Start()
    {
        count = 0;
        radius = 10.0f;
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (count >= 8)
            {
                count = 0;
            }
            xposition = xpositions[count];
            zposition = zpositions[count];
            obstaclescale = obstaclescales[count];
            obstacle.transform.position= new Vector3(xposition, 0.0f, zposition);
            obstacle.transform.localScale = new Vector3(obstaclescale,0.1f,10.0f);

            float xstart = Mathf.Sin(startpositions[count] * Mathf.Deg2Rad) * radius;
            float zstart = Mathf.Cos(startpositions[count] * Mathf.Deg2Rad) * radius;

            float xend = Mathf.Sin(endpositions[count] * Mathf.Deg2Rad) * radius;
            float zend = Mathf.Cos(endpositions[count] * Mathf.Deg2Rad) * radius;


            //this.gameObject.transform.position = new Vector3(x, 0.0f, z);
            startMark.transform.position = new Vector3(xstart,0,zstart);
            endMark.transform.position = new Vector3(xend, 0, zend);

            count++;



        }

        float startDistance = Vector3.Distance(this.gameObject.transform.position, startMark.transform.position);
        float endDistance = Vector3.Distance(this.gameObject.transform.position, endMark.transform.position);
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
        if (startDistance <= 0.2)
        {
            Debug.Log("Start Recording");
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
        if(endDistance <= 0.2)
        {
            Debug.Log("Stop Recording");
            if (isRecording)
            {
                // stop recording
                isRecording = false;

                //save to file
                string filename = "PositionRecording_" + this.name + "_";
                //string path = Application.persistentDataPath + "/pathOfDevice";
                string path = @"C:\Users\liuj58\MRTK\Assets\curveSavedData";

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


    // provides a quaterion, diff, such that a * diff = b
    private Quaternion QuatDiff(Quaternion a, Quaternion b)
    {
        return Quaternion.Inverse(a) * b;
    }
}
