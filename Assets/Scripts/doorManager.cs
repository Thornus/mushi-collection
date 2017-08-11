using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Map
{
	public int mapId;
	public Vector2 startPoint; //coordinates of player when going to this map

	public Map(int mapId, Vector2 startPoint) {
		this.mapId = mapId;
		this.startPoint = startPoint;
	}
}

public class doorManager : MonoBehaviour {
	public Transform playerTr;
	public Animator blackScreen;

	List<Map> maps = new List<Map>();

	void Start() {
		createMaps();
	}

	public void goToMap(int mapId) {
		StartCoroutine(fadeAnim(mapId));
	}

	IEnumerator fadeAnim(int mapId) {
		playerTr.GetComponent<playerScript>().enabled = false;

		if(!blackScreen.enabled) blackScreen.enabled = true;
		blackScreen.Play("fadeScreen", 0, 0f);

		yield return new WaitForSecondsRealtime(0.5f);
		playerTr.localPosition = maps[mapId].startPoint; //real teleport
		yield return new WaitForSecondsRealtime(0.5f);

		playerTr.GetComponent<playerScript>().enabled = true;
	}

	void createMaps() {
		maps.Add(new Map(0, new Vector2(-0.02f, -1.26f))); //larger map
		maps.Add(new Map(1, new Vector2(-3.724f, 10.34f))); //player's house
	}
}
