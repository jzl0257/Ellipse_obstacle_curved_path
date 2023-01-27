using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;

public struct PositionEntry
{
    public double timeStamp;
    public Vector3 position;
    public Quaternion rotation;
}

public class PathRecorder : MonoBehaviour
{
    public GameObject worldAnchor;
    private bool isRecording = false;
    private double startTime;
    private Vector3 startPos;
    private Quaternion startRot;
    private List<PositionEntry> entries = new List<PositionEntry>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void StartRecording()
    {

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

    public void StopAndSaveRecording()
    {
        if (isRecording)
        {
            // stop recording
            isRecording = false;

            //save to file
            string filename = "PositionRecording_" + this.name + "_";
            string path = Application.persistentDataPath + "/pathOfDevice";
            //string path = @"C:\Users\liuj58\MRTK\Assets\savedData";

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



    // provides a quaterion, diff, such that a * diff = b
    private Quaternion QuatDiff(Quaternion a, Quaternion b)
    {
        return Quaternion.Inverse(a) * b;
    }
}

