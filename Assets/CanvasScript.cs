using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiter;

public class CanvasScript : MonoBehaviour
{

	SQLite sqlite;
	Text id;
	Text region;
	Text species;
	RawImage type1;
	RawImage type2;
	Text height;
	Text weight;

	// Use this for initialization
	void Start ()
	{
		sqlite = GetComponent<SQLite> ();

		id = GameObject.Find ("NationalNumber").GetComponent<Text> ();
		region = GameObject.Find ("Region").GetComponent<Text> ();
		species = GameObject.Find ("Species").GetComponent<Text> ();
		type1 = GameObject.Find ("Type1").GetComponent<RawImage> ();
		type2 = GameObject.Find ("Type2").GetComponent<RawImage> ();
		height = GameObject.Find ("Height").GetComponent<Text> ();
		weight = GameObject.Find ("Weight").GetComponent<Text> ();

	}

	void UpdatePokedex (ArrayList pokemon)
	{
		/*setNationalNumber(pokemon.IndexOf (0));
		setRegion(pokemon.IndexOf (1))
		setSpecies(pokemon.IndexOf (2))
		setType1(pokemon.IndexOf (3))
		setType2(pokemon.IndexOf (4))
		setHeight(pokemon.IndexOf (5))
		setWeight(pokemon.IndexOf (6))

		id.text = ;*/

	}


	void setNationalNumber(string id)
	{

	}

	void setRegion(string id)
	{

	}

	void setSpecies(string id)
	{

	}

	void setType1(string id)
	{

	}

	void setType2(string id)
	{

	}

	void setHeight(string id)
	{

	}

	void setWeight(string id)
	{

	}


}
