using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Narrate;

public struct canMove {
	public static bool up = true, down = true, left = true, right = true;
}

public enum Dir { Right, Left, Up, Down }

public class playerScript : MonoBehaviour {
	public NarrationManager narrationManager;
	public GameObject inventory, loading;

	Animator anim;
	SpriteRenderer spriteRenderer;

	//public GameObject orangeScarabPrefab, inventorySlot0; //<---- DELETE

	public int rowsN, columnsN; //default number of rows and columns in inventory
	Transform[,] inventorySlotsTr; //stores the transforms of each inventory slot. used to instantiate GOs in and to check if a slot is empty or not
	Vector2 emptySlotCoords = new Vector2(0, 0); //stores the coords of the first empty slot available in the inventory


	/// <summary>
	/// Dictionary used to retrieve item's GameObject given a string (name of the item).
	/// </summary>
	Dictionary<string, GameObject> inventoryItems = new Dictionary<string, GameObject>(); //stores items with name as key

	/// <summary>
	/// The items that are currently in the player's inventory.
	/// </summary>
	public static List<Item> itemsInInventory = new List<Item>();

	/// <summary>
	/// The items that will be added in the database on log out.
	/// </summary>
	public List<Item> itemsToAdd = new List<Item>();

	Dir lastDir;
	bool quitting = false;

	void Start() {
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		getInventorySlots();
		StartCoroutine(getItemsFromDB(PlayerPrefs.GetString("username")));
	}

	void Update() {
		HandlePlayerMovement();

		if(Input.GetKeyDown(KeyCode.I)) { //open or close inventory
			if(inventory.activeInHierarchy)
				inventory.SetActive(false);
			else
				inventory.SetActive(true);
		}

		if(Input.GetKeyDown(KeyCode.Z)) //DEBUG (save game)
			StartCoroutine(addItemsToDB());

		checkCollisions();
	}

	void HandlePlayerMovement() {
		if (Input.GetAxisRaw("Horizontal") == 1) { //RIGHT
			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("walkRightwardAnim")) anim.Play("walkRightwardAnim", 0, 0f);

			if(canMove.right) transform.Translate(Vector2.right * Time.deltaTime * 3);
			spriteRenderer.flipX = false;
			lastDir = Dir.Right;
		} else if (Input.GetAxisRaw("Horizontal") == -1) { //LEFT
			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("walkLeftwardAnim")) anim.Play("walkLeftwardAnim", 0, 0f);

			if(canMove.left) transform.Translate(Vector2.left * Time.deltaTime * 3);
			spriteRenderer.flipX = true;
			lastDir = Dir.Left;
		} else if (Input.GetAxisRaw("Vertical") == 1) { //UP
			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("walkUpwardAnim")) anim.Play("walkUpwardAnim", 0, 0f);

			if(canMove.up) transform.Translate(Vector2.up * Time.deltaTime * 3);
			lastDir = Dir.Up;
		} else if (Input.GetAxisRaw("Vertical") == -1) { //DOWN
			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("walkDownwardAnim")) anim.Play("walkDownwardAnim", 0, 0f);

			if(canMove.down) transform.Translate(Vector2.down * Time.deltaTime * 3);
			lastDir = Dir.Down;
		} else if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0) { //IDLE
			if(lastDir == Dir.Right) {
				if(!anim.GetCurrentAnimatorStateInfo(0).IsName("idleRightwardAnim")) anim.Play("idleRightwardAnim", 0, 0f);
			} else if(lastDir == Dir.Left) {
				if(!anim.GetCurrentAnimatorStateInfo(0).IsName("idleLeftwardAnim")) anim.Play("idleLeftwardAnim", 0, 0f);
			} else if(lastDir == Dir.Up) {
				if(!anim.GetCurrentAnimatorStateInfo(0).IsName("idleUpwardAnim")) anim.Play("idleUpwardAnim", 0, 0f);
			} else if(lastDir == Dir.Down) {
				if(!anim.GetCurrentAnimatorStateInfo(0).IsName("idleDownwardAnim")) anim.Play("idleDownwardAnim", 0, 0f);
			}
		}
	}

	void checkCollisions() {
		Debug.DrawRay(transform.position, Vector2.up, Color.red);
		Debug.DrawRay(transform.position, Vector2.down, Color.red);
		Debug.DrawRay(transform.position, Vector2.right, Color.red);
		Debug.DrawRay(transform.position, Vector2.left, Color.red);

		RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 0.3f, 1 << LayerMask.NameToLayer("Object"));
		RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.45f, 1 << LayerMask.NameToLayer("Object"));
		RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.25f, 1 << LayerMask.NameToLayer("Object"));
		RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.25f, 1 << LayerMask.NameToLayer("Object"));
		if (hitUp) {
			canMove.up = false;
		} else {
			canMove.up = true;
		}

		if (hitDown) {
			canMove.down = false;
		} else {
			canMove.down = true;
		}

		if (hitLeft) {
			canMove.left = false;
		} else {
			canMove.left = true;
		}

		if (hitRight) {
			canMove.right = false;
		} else {
			canMove.right = true;
		}
	}

	void OnTriggerStay2D(Collider2D col) {
		if (Input.GetKeyDown(KeyCode.X) && !narrationManager.isPlaying) {
			if (col.gameObject.tag == "Sparkles") {
				Item item = col.gameObject.GetComponent<sparklesItem>().getItem();

				itemsToAdd.Add(item); //add item hidden by the sparkles

				if (isItemInInventory(item)) {
					Item itemInInv = itemsInInventory.Find(x => x.name == item.name);
					itemInInv.amount = (int.Parse(itemInInv.amount) + int.Parse(item.amount)).ToString(); //set amount to new amount
					setItemAmount(itemName: item.name, finalAmount: int.Parse(itemInInv.amount));
				} else {
					addItemToInventory(itemPrefab: ItemsContainer.itemsDictionary[item.name], slotCoords: getEmptySlotCoords(), itemAmount: int.Parse(item.amount));
				}

				col.gameObject.SetActive(false);
				print("You got "+item.amount+" "+item.name+"!");
			} else if(col.gameObject.name == "mysteriousMan") {
				OnEnableNarrationTrigger[] triggers = col.gameObject.GetComponents<OnEnableNarrationTrigger>();

				int reqAmt0 = 3, reqAmt1 = 1;
				Item requiredItem0 = itemsInInventory.Find(x => x.name == "Orange Scarab");
				Item requiredItem1 = itemsInInventory.Find(x => x.name == "Blue Scarab");

				if (requiredItem0 != null && requiredItem1 != null) {
					if (int.Parse(requiredItem0.amount) >= reqAmt0 && int.Parse(requiredItem1.amount) >= reqAmt1) {
						triggers[1].enabled = true;

						requiredItem0.amount = (int.Parse(requiredItem0.amount) - reqAmt0).ToString();
						setItemAmount(itemName: requiredItem0.name, finalAmount: int.Parse(requiredItem0.amount));
						requiredItem1.amount = (int.Parse(requiredItem1.amount) - reqAmt1).ToString();
						setItemAmount(itemName: requiredItem1.name, finalAmount: int.Parse(requiredItem1.amount));

						Item item = new Item("Fairy Necklace", "A mysterious necklace with a fairy.", 2, "1", PlayerPrefs.GetString("username"));
						itemsToAdd.Add(item);

						if (isItemInInventory(item)) {
							Item itemInInv = itemsInInventory.Find(x => x.name == item.name);
							itemInInv.amount = (int.Parse(itemInInv.amount) + int.Parse(item.amount)).ToString(); //set amount to new amount
							setItemAmount(itemName: item.name, finalAmount: int.Parse(itemInInv.amount));
						} else {
							addItemToInventory(itemPrefab: ItemsContainer.itemsDictionary[item.name], slotCoords: getEmptySlotCoords(), itemAmount: int.Parse(item.amount));
						}

						print("You got "+item.amount+" "+item.name+"!");
						return;
					}
				}

				triggers[0].enabled = true;
			}
		}
	}

	IEnumerator addItemsToDB() {
		WWWForm form = new WWWForm();
		form.AddField("numberOfItemsToAdd", itemsToAdd.Count.ToString());

		for (int i = 0; i < itemsToAdd.Count; i++) {
			form.AddField("itemName"+i.ToString(), itemsToAdd[i].name);
			form.AddField("itemDescription"+i.ToString(), itemsToAdd[i].description);
			form.AddField("typeId"+i.ToString(), itemsToAdd[i].type);

			Item alreadyExistingItem = itemsInInventory.Find(x => x.name.Contains(itemsToAdd[i].name));

			if (alreadyExistingItem == null) {
				form.AddField("itemAmount"+i.ToString(), itemsToAdd[i].amount);
			} else {
				int newAmount = int.Parse(itemsToAdd[i].amount) + int.Parse(alreadyExistingItem.amount);
				form.AddField("itemAmount"+i.ToString(), newAmount.ToString());
			}
		}

		form.AddField("username".ToString(), PlayerPrefs.GetString("username"));

		WWW w = new WWW("https://testthehost.000webhostapp.com/php/addItems.php", form);

		StartCoroutine(showLoading(w));
		yield return w; //wait for the form to check the PHP file, so our game doesn't just hang

		if (w.error != null) {
			print(w.error); //if there is an error, tell us
			if(w.error.Contains("timed out")) {
				print("Adding item: connection timed out :(");
			}
		} else {
			print(w.text);
			w.Dispose(); //clear form in game
		}
	}

	IEnumerator showLoading(WWW request) {
		loading.SetActive(true);

		yield return request;

		loading.SetActive(false);
	}

	void getInventorySlots() {
		inventorySlotsTr = new Transform[rowsN, columnsN];

		Transform[] inventoryRows = new Transform[inventory.transform.childCount];
		for (int i = 0; i < inventory.transform.childCount; i++) {
			inventoryRows[i] = inventory.transform.GetChild(i);
		}

		int slotsOnRow = columnsN;
		for (int i = 0; i < inventoryRows.Length; i++) {
			for (int j = 0; j < slotsOnRow; j++) {
				inventorySlotsTr[i, j] = inventoryRows[i].GetChild(j);
			}
		}
	}

	Vector2 getEmptySlotCoords() { //gets the more immediate empty slot coordinates
		for (int column = 0; column < inventorySlotsTr.GetLength(0); column++) {
			for (int row = 0; row < inventorySlotsTr.GetLength(1); row++) {
				GameObject itemGO;
				try {
					itemGO = inventorySlotsTr[column, row].GetChild(0).gameObject;
					if(!itemGO.activeSelf) { //if slot child is not active then the slot is empty
						return new Vector2(row, column);
					}
				} catch {
					return new Vector2(row, column); //if slot child is null then the slot is empty (it must be null if the code in catch is executed)
				}
				}
			}

		return Vector2.zero;
	}

	IEnumerator getItemsFromDB(string username) {
		WWWForm form = new WWWForm(); //here you create a new form connection
		form.AddField("username", username);

		WWW w = new WWW("https://testthehost.000webhostapp.com/php/getItems.php", form);

		StartCoroutine(showLoading(w));
		yield return w; //wait for the form to check the PHP file, so our game doesn't just hang

		if (w.error != null) {
			print(w.error); //if there is an error, tell us
			if(w.error.Contains("timed out")) {
				print("Getting items: connection timed out :(");
			}
		} else {
			print(w.text);

			//get each item data
			string[] itemsStrings = w.text.Split('|');
			for (int i = 0; i < itemsStrings.Length - 1; i++) {
				string[] itemData = itemsStrings[i].Split(';');
				Item item = new Item(name: itemData[1], description: itemData[2], type: int.Parse(itemData[3]), amount: itemData[4], username: PlayerPrefs.GetString("username"));

				if(int.Parse(item.amount) > 0) itemsInInventory.Add(item);
			}

			w.Dispose(); //clear form in game

			for (int i = 0; i < itemsInInventory.Count; i++) {
				addItemToInventory(ItemsContainer.itemsDictionary[itemsInInventory[i].name], getEmptySlotCoords(), int.Parse(itemsInInventory[i].amount));
			}
		}
	}

	bool isItemInInventory(Item item) {
		if(itemsInInventory.Exists(x => x.name == item.name)) {
			return true;
		}

		return false;
	}

	void addItemToInventory(GameObject itemPrefab, Vector2 slotCoords, int itemAmount) {
		GameObject itemGO = GameObject.Instantiate(itemPrefab, inventorySlotsTr[(int)slotCoords.y, (int)slotCoords.x]); //instantiate and position as child of a slot gameobject
		//item.transform.localPosition = new Vector2(0, 0);
		//item.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
		itemGO.transform.GetChild(0).GetComponent<Text>().text = "x" + itemAmount; //GET AND SET ITEM AMOUNT TEXT

		string key = System.Text.RegularExpressions.Regex.Replace(itemPrefab.name, "[A-Z]", " $0").Trim(); //divide up the name; e.g.: "OrangeScarab" -> "Orange Scarab"
		inventoryItems.Add(key, itemGO);
	}

	void removeItemFromInventory(GameObject itemGO, string itemName) {
		itemsInInventory.RemoveAll(x => x.name == itemName);
		inventoryItems.Remove(itemName);

		itemsToAdd.RemoveAll(x => x.name == itemName); //this line is needed in case the user got an item and then it was removed in the same session without saving on db

		itemGO.SetActive(false); //deactivating the GO is needed because it won't be destroyed immediately and getEmptySlotCoords() would therefore fail
		GameObject.Destroy(itemGO);
	}

	void setItemAmount(string itemName, int finalAmount) {
		GameObject itemGO = inventoryItems[itemName];

		if (finalAmount > 0) {
			itemGO.transform.GetChild(0).GetComponent<Text>().text = "x" + finalAmount; //GET AND SET ITEM AMOUNT TEXT
		} else {
			removeItemFromInventory(itemGO, itemName);
		}
	}

	void OnApplicationQuit() {
		if (!quitting) {
			Application.CancelQuit();
			StartCoroutine(realQuit());
		} else {
			Application.Quit();
		}

		quitting = true;
	}

	IEnumerator realQuit() {
		print("SAVING DATA...");
		yield return StartCoroutine(addItemsToDB());

		Application.Quit();
	}
}
