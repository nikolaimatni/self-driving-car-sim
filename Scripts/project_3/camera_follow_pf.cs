﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_follow_pf : MonoBehaviour {

	public GameObject target;
	public float zoom = 5;

	// Use this for initialization
	void Start () {
		
		this.GetComponentInParent<Camera> ().orthographicSize = zoom;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		float new_x = target.transform.position.x;
		float new_y = target.transform.position.y;

		particle_filter_v2 pf = (particle_filter_v2) target.GetComponent(typeof(particle_filter_v2));

		if (pf.isRunning ()) 
		{
			transform.position = new Vector3 (new_x, new_y, -1);
		}
		else 
		{
			if (Input.GetKey (KeyCode.RightArrow)) {
				transform.Translate (new Vector3 (0.2f * (zoom / 5), 0, 0));
			}
			if (Input.GetKey (KeyCode.LeftArrow)) {
				transform.Translate (new Vector3 (-0.2f * (zoom / 5), 0, 0));
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				transform.Translate (new Vector3 (0, -0.2f * (zoom / 5), 0));
			}
			if (Input.GetKey (KeyCode.UpArrow)) {
				transform.Translate (new Vector3 (0, 0.2f * (zoom / 5), 0));
			}
		}
			

	} 
	public void Restart()
	{
		float new_x = target.transform.position.x;
		float new_y = target.transform.position.y;
		transform.position = new Vector3 (new_x, new_y, -1);
	}

	public void ZoomIn()
	{
		if (zoom > 0.5f) 
		{
			zoom-=0.5f;
		}
		this.GetComponentInParent<Camera> ().orthographicSize = zoom;
	}
	public void ZoomOut()
	{

		if (zoom < 20.0f) 
		{
			zoom+=0.5f;
		}
		this.GetComponentInParent<Camera> ().orthographicSize = zoom;
	}
		

		
		

}
