using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrate;

public class sparklesItem : MonoBehaviour {
	new public string name;
	public string description;
	[Tooltip("1 for Insect, 2 for Accessory")]
	public int type;
	public string amount;

	public Item getItem() {
		OnEnableNarrationTrigger narration = GetComponent<OnEnableNarrationTrigger>();
		narration.theNarration.phrases[0].text = "You got " + amount + " " + name + "!";
		narration.enabled = true;

		return new Item(name, description, type, amount, PlayerPrefs.GetString("username"));
	}
}
