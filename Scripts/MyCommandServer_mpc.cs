using UnityEngine;
using System.Collections.Generic;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System;
using System.Timers;

public class MyCommandServer_mpc : MonoBehaviour
{
    public CarRemoteControl CarRemoteControl;
    public Camera FrontFacingCamera;
    private SocketIOComponent _socket;
    private CarController _carController;
    private MyPointTracker point_path;
    public MyWaypointTracker_mpc wpt;
    private bool startedYet = false;  
    private int polyOrder;
    private float startTime;

  

	// Use this for initialization
	void Start()
	{
		_socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
		_socket.On("manual", onManual);
		_carController = CarRemoteControl.GetComponent<CarController>();
		point_path = CarRemoteControl.GetComponent<MyPointTracker>();
		wpt = new MyWaypointTracker_mpc ();
		polyOrder = 5;

		//Data recording stuff
		
		
		
	}

     void RecordData()
    {
	Debug.Log("We are recording");
	float cte = wpt.CrossTrackError(_carController);
	float steeringAngle = _carController.CurrentSteerAngle  * Mathf.Deg2Rad;
        float accelInput = _carController.AccelInput;
	
	Debug.Log(string.Format("Writing Data, time: {0}, cte: {1}, acceleration: {2},steering angle: {3}", Time.time - startTime, cte,  accelInput, steeringAngle));
    }

	// Convert angle (degrees) from Unity orientation to 
	//            90
	//
	//  180                   0/360
	//
	//            270
	//
	// This is the standard format used in mathematical functions.
	float convertAngle(float psi) {
		if (psi >= 0 && psi <= 90) {
			return 90 - psi;
		}
		else if (psi > 90 && psi <= 180) {
			return 90 + 270 - (psi - 90);
		}
		else if (psi > 180 && psi <= 270) {
			return 180 + 90 - (psi - 180);
		}
		return 270 - 90 - (psi - 270);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		EmitTelemetry(obj);
	}

	// 
	void onManual(SocketIOEvent obj)
	{
        Debug.Log("Manual driving event ...");
		EmitTelemetry (obj);
	}

	void OnSteer(SocketIOEvent obj)
	{
	    if(!startedYet){
		InvokeRepeating("RecordData", 0.0f, 0.1f);
		startTime = Time.time;
		startedYet = true;
	    }

//	    Debug.Log(string.Format("Steering data event, startedYet set to {0}", startedYet));
		JSONObject jsonObject = obj.data;
		CarRemoteControl.SteeringAngle = float.Parse(jsonObject.GetField("steering_angle").ToString());
		CarRemoteControl.Acceleration = float.Parse(jsonObject.GetField("throttle").ToString());

		//string next_x = jsonObject.GetField ("next_x").ToString ();
		//string next_y = jsonObject.GetField ("next_y").ToString ();

		var next_x = jsonObject.GetField ("next_x");
		var next_y = jsonObject.GetField ("next_y");
		List<float> my_next_x = new List<float> ();
		List<float> my_next_y = new List<float> ();

		for (int i = 0; i < next_x.Count; i++) 
		{
			my_next_x.Add (float.Parse((next_x [i]).ToString()));
			my_next_y.Add (float.Parse((next_y [i]).ToString()));
		}
		point_path.setNextPoint( my_next_x, my_next_y ); 

		var mpc_x = jsonObject.GetField ("mpc_x");
		var mpc_y = jsonObject.GetField ("mpc_y");
		List<float> my_mpc_x = new List<float> ();
		List<float> my_mpc_y = new List<float> ();

		for (int i = 0; i < mpc_x.Count; i++) 
		{
			my_mpc_x.Add (float.Parse((mpc_x [i]).ToString()));
			my_mpc_y.Add (float.Parse((mpc_y [i]).ToString()));
		}

		point_path.setMpcPoint( my_mpc_x, my_mpc_y ); 

		EmitTelemetry(obj);
	}

	void EmitTelemetry(SocketIOEvent obj)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			print("Attempting to Send...");
			// send only if it's not being manually driven
			if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
				_socket.Emit("telemetry", new JSONObject());
			} else {
				// Collect Data from the Car
				Dictionary<string, JSONObject> data = new Dictionary<string, JSONObject>();
				var cte = wpt.CrossTrackError (_carController);
				//Debug.Log(string.Format("In between waypoint {0} and {1}", wpt.prev_wp, wpt.next_wp));
				var pos = _carController.Position();
				var psi = _carController.Orientation().eulerAngles.y;
				
				// Waypoints data
				var ptsx = new List<JSONObject>();
				var ptsy = new List<JSONObject>();
				for (int i = wpt.prev_wp; i < wpt.prev_wp+polyOrder+1; i++) {
					ptsx.Add(new JSONObject(wpt.waypoints[i%wpt.waypoints.Count].x));
					ptsy.Add(new JSONObject(wpt.waypoints[i%wpt.waypoints.Count].z));
				}
				data["ptsx"] = new JSONObject(ptsx.ToArray());
				data["ptsy"] = new JSONObject(ptsy.ToArray());

                // Orientations
                data["psi_unity"] = new JSONObject(psi * Mathf.Deg2Rad);
				data["psi"] = new JSONObject(convertAngle(psi) * Mathf.Deg2Rad);

                // Global position.
                data["x"] = new JSONObject(pos.x);
                data["y"] = new JSONObject(pos.z);
                // Steering angle
				data["steering_angle"] = new JSONObject(_carController.CurrentSteerAngle * Mathf.Deg2Rad);
                // Throttle
				data["throttle"] = new JSONObject(_carController.AccelInput);
                // Velocity
				data["speed"] = new JSONObject(_carController.CurrentSpeed);
				_socket.Emit("telemetry", new JSONObject(data));
			}
		});
				
	}
}
