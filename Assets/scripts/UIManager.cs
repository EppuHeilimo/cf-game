using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour {

    GameObject player;
    GameObject target;
    GameObject inv;
    bool showInventory = true;
    bool paused = false;
    GameObject gameOverCanvas;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        inv = GameObject.FindGameObjectWithTag("Inventory");
        gameOverCanvas = GameObject.Find("GameOver Canvas");
        gameOverCanvas.SetActive(false);
	}

    // gives item to NPC
    // called when player clicks item in inventory
    // medicines and dosages in the clicked item sent as parameters
    public bool giveMed(string[] med, float[] dosage)
    {
        // check if player has a target
        target = player.GetComponent<PlayerControl>().getTarget();
        if (target == null)
            return false;
        // target is NPC
        else if (target.tag == "NPC")
            // give the item to the NPC, returns true if giving item succeeds
            return(target.GetComponent<NPC>().giveMed(med, dosage));
        return false;
    }

    // pauses/unpauses all game logic
    public void pause(bool pause)
    {
        // pause
        if (pause && !paused)
        {
            // pause NPCs
            NPCManager npcmanager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManager>();          
            foreach (GameObject npc in npcmanager.npcList)
            {
                if (npc != null)
                    npc.GetComponent<NPC>().paused = true;
            }
            npcmanager.paused = true;
            // pause clock
            GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().paused = true;           
        }
        // unpause
        else
        {
            NPCManager npcmanager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManager>();
            npcmanager.paused = false;
            foreach (GameObject npc in npcmanager.npcList)
            {
                if (npc != null)
                    npc.GetComponent<NPC>().paused = false;
            }
            GameObject.FindGameObjectWithTag("Clock").GetComponent<ClockTime>().paused = false;
        }
    }

    // displays game over screen with the player's score
    public void gameOver(int score)
    {
        gameOverCanvas.SetActive(true);      
        GameObject.FindGameObjectWithTag("ScoreTxt").GetComponent<Text>().text = "Your score:\n" + score.ToString();
        Time.timeScale = 0;
    }

    // removes item from inventory
    // called when player clicks item in inventory
    public bool trashItem()
    {
        // check if player has a target
        target = player.GetComponent<PlayerControl>().getTarget();
        if (target == null)
            return false;
        // target is trash can
        else if (target.tag == "TrashCan")
        {
            // player is close enough to the trash can
            if(player.GetComponent<PlayerControl>().atTrashCan)
            {
                // play item trashing sound
                GetComponent<AudioSource>().Play();
                // returns true if removing item succeeds
                return true;
            }
        } 
        return false;
    }
}
