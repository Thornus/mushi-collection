using System.Collections;
using System.Collections.Generic;

public class Item {
	public string name { get; set; }
	public string description { get; set; }
	public int type { get; set; }
	public string amount { get; set; }
	public string username { get; set; }

	public Item(string name, string description, int type, string amount, string username) { 
		this.name = name;
		this.description = description;
		this.type = type;
		this.amount = amount;
		this.username = username;
	}

}
