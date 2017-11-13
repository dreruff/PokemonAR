using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TargetScript : MonoBehaviour, ITrackableEventHandler {

	private TrackableBehaviour mTrackableBehaviour;

	public Canvas canvas;

	// Use this for initialization
	void Start () {

		//canvas = GetComponent<Canvas> ();

		mTrackableBehaviour = GetComponent<TrackableBehaviour> ();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
	}
	
	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			// Enable canvas when target is found
			canvas.enabled = true;
		}
		else
		{
			// Disable canv when target is lost
			canvas.enabled = false;
		}
	}   
}
