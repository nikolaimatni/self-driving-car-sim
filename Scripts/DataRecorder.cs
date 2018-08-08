using UnityEngine;
using System;
using System.Timers;
using UnityStandardAssets.Vehicles.Car;

public class DataRecorder : MonoBehaviour 
{
    public string outputFile;
    public float updateInterval;
    private Timer dataTimer;
    private MyWaypointTracker_mpc wpt;
    private CarRemoteControl CarRemoteControl;
    private CarController _carController;


    void Start () {

    }

    void Update () {

    }
}
