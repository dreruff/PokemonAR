using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLiter;

public class CanvasScript : MonoBehaviour {

	SQLite sqlite;

	// Use this for initialization
	void Start () {
		sqlite = GetComponent<SQLite> ();
		Debug.Log (sqlite.GetPokemon ("001"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
