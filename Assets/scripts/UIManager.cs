using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    GameObject player;
    GameObject target;
    GameObject inv;
    bool showInventory;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        inv = GameObject.FindGameObjectWithTag("Inventory");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Inventory"))
            toggleInventory();
    }

    public bool giveMed(string med)
    {
        target = player.GetComponent<PlayerControl>().getTarget();
        if (target == null)
            return false;
        else if (target.tag == "NPC")
            return(target.GetComponent<NPC>().giveMed(med));
        return false;
    }

    public void toggleInventory()
    {
        showInventory = !showInventory;
        drawInventory(showInventory);
    }

    public void drawInventory(bool show)
    {
        inv.SetActive(show);
    }
}
