using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class unityEvent : MonoBehaviour {
	public UnityEvent e;

	void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Player") {
			e.Invoke();
		}
	}
}
