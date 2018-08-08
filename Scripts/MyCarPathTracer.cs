using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Vehicles.Car;

 public class MyCarPathTracer : MonoBehaviour {

     public MyCommandServer_mpc CommandServer;
     public MyWaypointTracker_mpc wpt;
     public CarRemoteControl CarRemoteControl;
     private bool first = true;
     private int pt = 0; 
     public LineRenderer lineRenderer;
     public GameObject path_point;
     public CarController _carController;
     private Vector3 lastPoint;
    
     void Awake() {
	 Debug.Log("My CarPathTracer connector is waking up");
	 
     }
     
     void Start() {
	 
	 CommandServer = GameObject.Find("CommandServer").GetComponent<MyCommandServer_mpc>();
	
	 lineRenderer = path_point.GetComponentInParent<LineRenderer> ();
	 lineRenderer.SetWidth ((float).5, (float).5);
	 lineRenderer.useWorldSpace = true;

	 _carController = CarRemoteControl.GetComponent<CarController>();
	 lastPoint = _carController.Position();
	 lineRenderer.SetVertexCount(2);
     }
    
     void Update() {
	   
	 Vector3 here = _carController.Position();
	 if ((here-lastPoint).magnitude>2){
	     if (first){
		 lineRenderer.SetPosition(0,here);
		 lineRenderer.SetPosition(1,here);
		 first = false;
		 Debug.Log(string.Format("Adding the following point to our path trace: {0}",here));}
	     else{
		 wpt = CommandServer.wpt;
		 lineRenderer.SetVertexCount(lineRenderer.positionCount+1);
		 lineRenderer.SetPosition(lineRenderer.positionCount-1,here);
		 lastPoint = here;
		 Debug.Log(string.Format("Adding the following point to our path trace: {0}",here));
		 Debug.Log(string.Format("My code can tell we are between waypoints {0} and {1}",wpt.prev_wp, wpt.next_wp));
		 if (wpt.next_wp == 0){
		     lineRenderer.SetVertexCount(2);
		     first = true;
		 }
		
	     }
	    
	     	     
	 }
           
     }
        

 }
 
