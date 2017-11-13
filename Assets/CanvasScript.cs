using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLiter;

public class CanvasScript : MonoBehaviour
{

	SQLite sqlite;
	Text id;
	Text name;
	Text region;
	Text species;
	RawImage type1;
	RawImage type2;
	Text height;
	Text weight;

	// Use this for initialization
	void Start ()
	{
		//sqlite = GetComponent<SQLite> ();

		id = GameObject.Find ("NationalNumber").GetComponent<Text> ();
		name = GameObject.Find ("Name").GetComponent<Text> ();
		region = GameObject.Find ("Region").GetComponent<Text> ();
		species = GameObject.Find ("Species").GetComponent<Text> ();
		type1 = GameObject.Find ("Type1").GetComponent<RawImage> ();
		type2 = GameObject.Find ("Type2").GetComponent<RawImage> ();
		height = GameObject.Find ("Height").GetComponent<Text> ();
		weight = GameObject.Find ("Weight").GetComponent<Text> ();

	}

	public void UpdatePokedex (ArrayList pokemon)
	{
		setNationalNumber (pokemon[0].ToString());
		setName (pokemon[1].ToString ());
		setSpecies (pokemon[2].ToString ());
		setType1 (pokemon[3].ToString ());
		setType2 (pokemon[4].ToString ());
		setHeight (pokemon[5].ToString ());
		setWeight (pokemon[6].ToString ());
		setRegion (pokemon[7].ToString ());
	}


	void setNationalNumber (string id)
	{
		this.id.text = id;
	}

	void setName (string name)
	{
		//To captialize the first letter. Unknown reason why name is lower case.
		System.Text.StringBuilder sb = new System.Text.StringBuilder(name);
		sb.Replace (sb [0], char.ToUpper (sb [0]),0,1);

		this.name.text = sb.ToString();
	}

	void setRegion (string region)
	{
		this.region.text = region;
	}

	void setSpecies (string species)
	{
		this.species.text = species;
	}

	void setType1 (string type)
	{
		switch (type) {
		case "FIRE":
			type1.texture = (Texture)Resources.Load ("PokemonTypes/Type_Fire");
			break;
		case "WATER":
			type1.texture = (Texture)Resources.Load ("PokemonTypes/Type_Water");
			break;
		case "GRASS":
			type1.texture = (Texture)Resources.Load ("PokemonTypes/Type_Grass");
			break;
		case "ELECTRIC":
			type1.texture = (Texture)Resources.Load ("PokemonTypes/Type_Electric");
			break;
		case "POISON":
			type1.texture = (Texture)Resources.Load ("PokemonTypes/Type_Poison");
			break;
		default:
			Debug.Log ("Error should not get here!!!");
			break;
		}
	}

	void setType2 (string type)
	{
		type2.enabled = true;

		switch (type) {
		case "FIRE":
			type2.texture = (Texture)Resources.Load ("PokemonTypes/Type_Fire");
			break;
		case "WATER":
			type2.texture = (Texture)Resources.Load ("PokemonTypes/Type_Water");
			break;
		case "GRASS":
			type2.texture = (Texture)Resources.Load ("PokemonTypes/Type_Grass");
			break;
		case "ELECTRIC":
			type2.texture = (Texture)Resources.Load ("PokemonTypes/Type_Electric");
			break;
		case "POISON":
			type2.texture = (Texture)Resources.Load ("PokemonTypes/Type_Poison");
			break;
		default:
			type2.enabled = false;
			Debug.Log ("No second type");
			break;
		}
	}

	void setHeight (string height)
	{
		this.height.text = height;
	}

	void setWeight (string weight)
	{
		this.weight.text = weight;
	}


}
