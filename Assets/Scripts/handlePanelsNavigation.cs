using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handlePanelsNavigation : MonoBehaviour {
	public GameObject loginPanel, registrationPanel, responseGameObject;

	public void showLoginPanel() {
		registrationPanel.SetActive(false);
		loginPanel.SetActive(true);
		responseGameObject.transform.localPosition = new Vector2(-1.1f, -155f);
	}

	public void showRegistrationPanel() {
		loginPanel.SetActive(false);
		registrationPanel.SetActive(true);
		responseGameObject.transform.localPosition = new Vector2(-1.1f, -219f);
	}

}
