using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;

 public class MyWaypointConnector : MonoBehaviour {

     private float update = 0.0f; 
     public Vector3[] waypoints;
     public LineRenderer lineRenderer;
     public GameObject track_point;

     void Awake() {
	 Debug.Log("My connector is waking up");
	 waypoints = GameObject.Find("Waypoints").GetComponentInParent<WaypointCircuit>().points;

	 lineRenderer = track_point.GetComponentInParent<LineRenderer> ();
	 lineRenderer.SetVertexCount(waypoints.Length);
	 lineRenderer.SetPositions(waypoints);
	 lineRenderer.SetWidth ((float).5, (float).5);
	 lineRenderer.useWorldSpace = true;
     }
     
     void Start() {

	
	 /*GameObject waypointObject = GameObject.Find("Waypoints");

	 circuit = waypointObject.GetComponentInParent<WaypointCircuit>();
	 waypoints = circuit.points;	 */
	
	// lineRenderer.SetVertexCount(waypoints.Length);
	// lineRenderer.SetPositions(waypoints);
	// lineRenderer.SetWidth ((float).5, (float).5);
     }
    
     void Update() {
	   update += Time.deltaTime;
        if (update > 0.0f)
        {
            update = 0.0f;
          //  Debug.Log("Update");
        }
//	lineRenderer.SetVertexCount(waypoints.Length);
//	lineRenderer.SetPositions(waypoints);
//	lineRenderer.SetWidth ((float).5, (float).5);
     }
 }
