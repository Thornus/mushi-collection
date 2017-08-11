using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ItemsContainer : MonoBehaviour {
	public GameObject[] itemsPrefabs;

	public static Dictionary<string, GameObject> itemsDictionary = new Dictionary<string, GameObject>();

	void Start() {
		PopulateDictionary();
	}

	void PopulateDictionary() {
		for (int i = 0; i < itemsPrefabs.Length; i++) {
			string key = Regex.Replace(itemsPrefabs[i].name, "[A-Z]", " $0").Trim(); //divide up the name; e.g.: "OrangeScarab" -> "Orange Scarab"
			itemsDictionary.Add(key, itemsPrefabs[i]);
		}
	}
}
