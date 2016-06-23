using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    GameObject player;
    GameObject target;
    GameObject inv;
    bool showInventory = true;
    bool paused = false;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        inv = GameObject.FindGameObjectWithTag("Inventory");
	}

    public bool giveMed(string[] med, float[] dosage)
    {
        target = player.GetComponent<PlayerControl>().getTarget();
        if (target == null)
            return false;
        else if (target.tag == "NPC")
            return(target.GetComponent<NPCV2>().giveMed(med, dosage));
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

    public void quitGame()
    {
        Application.Quit();
    }

    public void pauseGame()
    {
        paused = !paused;
        pause(paused);
    }

    public void pause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
        }
            
        else
            Time.timeScale = 1;
    }
}
