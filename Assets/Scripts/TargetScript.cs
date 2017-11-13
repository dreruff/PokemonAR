using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using SQLiter;

public class TargetScript : MonoBehaviour, ITrackableEventHandler {

	public string PokemonTargetId;

	private TrackableBehaviour mTrackableBehaviour;

	Canvas canvas;

	// Use this for initialization
	void Start () {

		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();

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


			LoomManager.Loom.QueueOnMainThread (() => {
				
			
			ArrayList pokemon = canvas.GetComponent<SQLite> ().GetPokemon (PokemonTargetId);
			canvas.GetComponent<CanvasScript> ().UpdatePokedex (pokemon);

			});
			/*sb.Length = 0;
			sb.Append (arr[0]).Append (" ");
			sb.Append (arr[1]).Append (" ");
			sb.Append (arr[2]).Append (" ");
			sb.Append (arr[3]).Append (" ");
			sb.Append (arr[4]).Append (" ");
			sb.Append (arr[5]).Append (" ");
			sb.Append (arr[6]).Append (" ");
			sb.Append (arr[7]).Append (" ");
			sb.AppendLine ();

			Debug.Log(sb.ToString());*/
		}
		else
		{
			// Disable canv when target is lost
			if (canvas!=null)
				canvas.enabled = false;
		}
	}   
}
