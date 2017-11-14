using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using SQLiter;

public class TargetScript : MonoBehaviour, ITrackableEventHandler
{
	public string PokemonTargetId;
	public AudioClip cry;

	private TrackableBehaviour mTrackableBehaviour;

	Canvas canvas;

	// Use this for initialization
	void Start ()
	{

		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();

		mTrackableBehaviour = GetComponent<TrackableBehaviour> ();
		if (mTrackableBehaviour) {
			mTrackableBehaviour.RegisterTrackableEventHandler (this);
		}
	}

	public void OnTrackableStateChanged (
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
		    newStatus == TrackableBehaviour.Status.TRACKED ||
		    newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
			// Enable canvas when target is found
			canvas.enabled = true;


			LoomManager.Loom.QueueOnMainThread (() => {
				
			
				ArrayList pokemon = canvas.GetComponent<SQLite> ().GetPokemon (PokemonTargetId);
				pokemon.Add(cry);
				canvas.GetComponent<CanvasScript> ().UpdatePokedex (pokemon);
				canvas.GetComponent<CanvasScript> ().PlayCry();

			});
		} else {
			// Disable canv when target is lost
			if (canvas != null)
				canvas.enabled = false;
		}
	}
}
