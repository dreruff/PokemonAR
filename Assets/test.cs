using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLiter;

public class test : MonoBehaviour {

	SQLite sqliter;

	// Use this for initialization
	void Start () {
		sqliter = GetComponent<SQLite> ();
		Debug.Log(sqliter.GetPokemon ("001"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
