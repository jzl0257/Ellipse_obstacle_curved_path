using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

using System.IO;
using System;
using Newtonsoft.Json;
public class player : MonoBehaviour
{

    private bool isSelected = true;
    private bool isPlaying = true;
    private bool isDelayed = false;
    private double startTime;
    private double delayTime = 10;
    private double timeScale = 1;
    private Vector3 startPos;
    private Vector3 posOffet = Vector3.zero;
    private Quaternion startRot;
    private List<TrajectoryEntry> entries = new List<TrajectoryEntry>();


    public string trajectoryFilePath = @"C:\Users\liuj58\MRTK\Assets\TrajectoryRecording_Cube_59.json";
    public double targetDuration = 0;
    public GameObject targetEndPoint;
    public bool playOnStart;
    public bool loop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            TrajectoryEntry transposOffet = GetTrajectoryEntry((Time.time - startTime) * timeScale);
            this.gameObject.transform.position = startPos + transposOffet.position + posOffet * ((float)transposOffet.timeStamp);
            this.gameObject.transform.rotation = startRot * transposOffet.rotation;

            if (entries.Count == 1)
            {
                if (loop)
                {
                    this.gameObject.transform.position = startPos;
                    this.gameObject.transform.rotation = startRot;
                    StartPlaying();
                }
                else
                {
                    StopPlaying();
                }
            }
        }
    }

    public void StartPlaying()
    {
        if(isPlaying)
        {
            return;
        }
        else
        {
            isPlaying = true;
            if (File.Exists(trajectoryFilePath))
            {
                //Debug.Log("Loading trajectory file from: " + trajectoryFilePath);
                Console.WriteLine(File.Exists(trajectoryFilePath) ? "File exists." : "File does not exist.");
                try
                {
                    entries = JsonConvert.DeserializeObject<List<TrajectoryEntry>>(File.ReadAllText(trajectoryFilePath));
                    startTime = Time.time + delayTime;
                    startPos = this.gameObject.transform.position;
                    startRot = this.gameObject.transform.rotation;


                    if (targetEndPoint != null)
                    {

                        posOffet = targetEndPoint.transform.position - (entries[entries.Count - 1].position + startPos);
                        posOffet /= (float)entries[entries.Count - 1].timeStamp;


                    }

                    if (targetDuration != 0)
                    {
                        timeScale = entries[entries.Count - 1].timeStamp / targetDuration;
                    }

                    isDelayed = true;
                }
                catch (Exception exception)
                {
                    Debug.Log("An exception occurred while opening a trajectory file.");
                    Debug.LogException(exception);
                }

            }
            else
            {
                Debug.Log("Trajectory File does not exist");
            }
        }
    }
    public void StopPlaying()
    {
        
        isPlaying = false;
        posOffet = Vector3.zero;
        timeScale = 1;
        entries.Clear();
        
        

    }

    public TrajectoryEntry GetTrajectoryEntry(double timeposOffet)
    {
        TrajectoryEntry first = entries[0];
        while (timeposOffet > first.timeStamp && entries.Count > 1)
        {
            entries.RemoveAt(0);
            first = entries[0];
        }
        return first;
    }
}
