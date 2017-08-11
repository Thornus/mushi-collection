using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class login : MonoBehaviour {
	string URL = "https://testthehost.000webhostapp.com/php/login.php"; //change for your URL

	public InputField usernameIF, passwordIF, regUsernameIF, regPasswordIF, regEmailIF;
	public GameObject responseGameObject;
	public GameObject game;

	public void fireAction(string whichAction) {
		StartCoroutine(Action(whichAction));
	}

	IEnumerator Action(string action) {
		WWWForm form = new WWWForm();
		form.AddField("Action", action);

		if (action == "Login") {
			form.AddField("User", usernameIF.text);
			form.AddField("Pass", passwordIF.text);
		} else if (action == "Register") {
			form.AddField("User", regUsernameIF.text);
			form.AddField("Pass", regPasswordIF.text);
			form.AddField("Email", regEmailIF.text);
		}
			
		WWW w = new WWW(URL, form); //create a www request
		responseGameObject.SetActive(true);

		yield return w; //wait for the form to check the PHP file

		if (w.error != null) {
			if(w.error.Contains("timed out")) {
				responseGameObject.transform.GetChild(1).GetComponent<Image>().enabled = false; //disable loading image
				responseGameObject.transform.GetChild(0).GetComponent<Text>().text = "Connection timed out."; //show response text

				yield return new WaitForSeconds(3);

				responseGameObject.SetActive(false);
				responseGameObject.transform.GetChild(1).GetComponent<Image>().enabled = true; //enable loading image
				responseGameObject.transform.GetChild(0).GetComponent<Text>().text = ""; //reset response text
			}
		} else {
			if(w.text.Contains("Logged in")){ //logged in successfully
				yield return new WaitForSeconds(1);
				GameObject.Find("Canvas/Login Panel").SetActive(false);
				Camera.main.GetComponent<SimpleCameraFollow>().enabled = true;
				game.SetActive(true); //enter game

				PlayerPrefs.SetString("username", usernameIF.text); //save latest username for later use
			}
			if(w.text == "    Wrong"){
				yield return new WaitForSeconds(3);
			}
			if(w.text == "    No User"){
				yield return new WaitForSeconds(3);
			}
			if(w.text == "    ILLEGAL REQUEST"){
				yield return new WaitForSeconds(3);
			}
			if(w.text == "    Registered"){
				print("Account Created. Logging In.");
			}
			if(w.text == "    ERROR"){
			}
		
			responseGameObject.transform.GetChild(1).GetComponent<Image>().enabled = false; //disable loading image
			responseGameObject.transform.GetChild(0).GetComponent<Text>().text = w.text; //show response text

			yield return new WaitForSeconds(3);

			responseGameObject.SetActive(false);
			responseGameObject.transform.GetChild(1).GetComponent<Image>().enabled = true; //enable loading image
			responseGameObject.transform.GetChild(0).GetComponent<Text>().text = ""; //reset response text

			w.Dispose(); //clear form
		}
	}

}
