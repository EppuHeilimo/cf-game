using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    GameObject player;
    GameObject target;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void giveMed(string med)
    {
        target = player.GetComponent<PlayerControl>().getTarget();
        if (target == null) return;
        else if (target.tag == "NPC")
            target.GetComponent<NPC>().giveMed(med);
    }
}
