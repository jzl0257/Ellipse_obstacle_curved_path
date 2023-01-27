using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using System.IO;
using System;
using Newtonsoft.Json;

public class TrajectoryPlayer : MonoBehaviour
{

    private bool isPlaying = true;
    private bool isDelayed = false;
    private double startTime;
    public double timeScale = 1;
    private Vector3 startPos;
    private Vector3 posOffet = Vector3.zero;
    private Quaternion startRot;
    private List<PositionEntry> entries = new List<PositionEntry>();

    public string trajectoryFilePath;
    [Tooltip("Leave empty for the trajectory's default movement")]
    public GameObject targetEndPoint;
    
    public bool loop = false;


    // Start is called before the first frame update
    void Start()
    {

        StartPlaying();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isDelayed)
        {
            if(Time.time > startTime)
            {
                isDelayed = false;
                isPlaying = true;
                //OnPlaybackStart.Invoke();
            }
        }
        if(isPlaying)
        {
            PositionEntry transposOffet = GetTrajectoryEntry((Time.time - startTime)*timeScale);
            this.gameObject.transform.position = startPos + transposOffet.position + posOffet*((float) transposOffet.timeStamp);
            this.gameObject.transform.rotation = startRot * transposOffet.rotation;

            if(entries.Count == 1)
            {
                if(loop)
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
        
        if(File.Exists(trajectoryFilePath))
        {
            Debug.Log("Loading trajectory file from: " + trajectoryFilePath);
            try
            {
                /**
                entries = JsonConvert.DeserializeObject<List<PositionEntry>>(File.ReadAllText(trajectoryFilePath), 
                    new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                **/
                //String json ="{ 'timeStamp':0.017732620239257813,'position':{ 'x':0.0,'y':0.0,'z':0.0,'magnitude':0.0,'sqrMagnitude':0.0},'rotation':{ 'x':0.0,'y':0.0,'z':0.0,'eulerAngles':{ 'x':0.0,'y':0.0,'z':0.0,'magnitude':0.0,'sqrMagnitude':0.0} } }";
                //String json = "time:012321321";
                //entries = JsonConvert.DeserializeObject<List<TrajectoryEntry>>(json);
                entries = JsonConvert.DeserializeObject<List<PositionEntry>>(File.ReadAllText(trajectoryFilePath));
                startTime = Time.time;
                startPos = this.gameObject.transform.position;
                startRot = this.gameObject.transform.rotation;

                    
                if(targetEndPoint != null)
                {
                        
                    posOffet = targetEndPoint.transform.position - ( entries[entries.Count-1].position + startPos);
                    posOffet /=  (float)entries[entries.Count-1].timeStamp;
                        
                        
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

    public void StopPlaying()
    {
        
        isPlaying = false;
        posOffet = Vector3.zero;
        timeScale = 1;
        entries.Clear();
        
        
        
    }

    public PositionEntry GetTrajectoryEntry(double timeposOffet)
    {
        PositionEntry first = entries[0];
        while(timeposOffet > first.timeStamp && entries.Count > 1)
        {
            entries.RemoveAt(0);
            first = entries[0];
        }
        return first;
    }
}
