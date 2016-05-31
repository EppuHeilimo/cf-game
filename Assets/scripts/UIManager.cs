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

    public bool giveMed(string med)
    {
        target = player.GetComponent<PlayerControl>().getTarget();
        if (target == null)
            return false;
        else if (target.tag == "NPC")
            return(target.GetComponent<NPC>().giveMed(med));
        return false;
    }
}
