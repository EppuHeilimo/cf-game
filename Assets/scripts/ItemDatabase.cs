using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour {

    public List<Item> items = new List<Item>();

	// Use this for initialization
	void Start () {
        items.Add(new Item("burana", 0, "särkylääke"));
        items.Add(new Item("kokaiini", 1, "auttaa vaivaan kuin vaivaan"));
	}

}
